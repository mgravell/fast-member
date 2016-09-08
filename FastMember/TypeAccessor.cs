using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Collections.Generic;

using static FastMember.TypeHelpers;

#if !NO_DYNAMIC
using System.Dynamic;
#endif

namespace FastMember
{
#if NET20
    public delegate TResult Func<TResult>();
    public delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
    public delegate void Action<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
#endif

    /// <summary>
    /// Provides by-name member-access to objects of a given type
    /// </summary>
    public abstract class TypeAccessor
    {
        // hash-table has better read-without-locking semantics than dictionary
        private static readonly Hashtable publicAccessorsOnly = new Hashtable(), nonPublicAccessors = new Hashtable();

        /// <summary>
        /// Does this type support new instances via a parameterless constructor?
        /// </summary>
        public virtual bool CreateNewSupported { get { return false; } }
        /// <summary>
        /// Create a new instance of this type
        /// </summary>
        public virtual object CreateNew() { throw new NotSupportedException(); }

        /// <summary>
        /// Can this type be queried for member availability?
        /// </summary>
        public virtual bool GetMembersSupported { get { return false; } }
        /// <summary>
        /// Query the members available for this type
        /// </summary>
        public virtual MemberSet GetMembers() { throw new NotSupportedException(); }

        /// <summary>
        /// Provides a type-specific accessor, allowing by-name access for all objects of that type
        /// </summary>
        /// <remarks>The accessor is cached internally; a pre-existing accessor may be returned</remarks>
        public static TypeAccessor Create(Type type)
        {
            return Create(type, false);
        }

        /// <summary>
        /// Provides a type-specific accessor, allowing by-name access for all objects of that type
        /// </summary>
        /// <remarks>The accessor is cached internally; a pre-existing accessor may be returned</remarks>
        public static TypeAccessor Create(Type type, bool allowNonPublicAccessors)
        {
            if (type == null) throw new ArgumentNullException("type");
            var lookup = allowNonPublicAccessors ? nonPublicAccessors : publicAccessorsOnly;
            TypeAccessor obj = (TypeAccessor)lookup[type];
            if (obj != null) return obj;

            lock (lookup)
            {
                // double-check
                obj = (TypeAccessor)lookup[type];
                if (obj != null) return obj;

                obj = CreateNew(type, allowNonPublicAccessors);

                lookup[type] = obj;
                return obj;
            }
        }
#if !NO_DYNAMIC
        sealed class DynamicAccessor : TypeAccessor
        {
            public static readonly DynamicAccessor Singleton = new DynamicAccessor();
            private DynamicAccessor() { }
            public override object this[object target, string name]
            {
                get { return CallSiteCache.GetValue(name, target); }
                set { CallSiteCache.SetValue(name, target, value); }
            }
        }
#endif

        private static AssemblyBuilder assembly;
        private static ModuleBuilder module;
        private static int counter;

#if COREFX
        private static readonly object counterLock = new object();
#endif
        private static int GetNextCounterValue()
        {
#if COREFX
            lock(counterLock)
            {
                return counter++;
            }
#else
            return Interlocked.Increment(ref counter);
#endif
        }

        static readonly MethodInfo tryGetValue = typeof(Dictionary<string, int>).GetMethod("TryGetValue");
        private static void WriteMapImpl(ILGenerator il, Type type, List<MemberInfo> members, FieldBuilder mapField, bool allowNonPublicAccessors, bool isGet)
        {
            OpCode obj, index, value;

            Label fail = il.DefineLabel();
            if (mapField == null)
            {
                index = OpCodes.Ldarg_0;
                obj = OpCodes.Ldarg_1;
                value = OpCodes.Ldarg_2;
            }
            else
            {
                il.DeclareLocal(typeof(int));
                index = OpCodes.Ldloc_0;
                obj = OpCodes.Ldarg_1;
                value = OpCodes.Ldarg_3;

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, mapField);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldloca_S, (byte)0);
                il.EmitCall(OpCodes.Callvirt, tryGetValue, null);
                il.Emit(OpCodes.Brfalse, fail);
            }            
            Label[] labels = new Label[members.Count];
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = il.DefineLabel();
            }
            il.Emit(index);
            il.Emit(OpCodes.Switch, labels);
            il.MarkLabel(fail);
            il.Emit(OpCodes.Ldstr, "name");
            il.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Throw);
            for (int i = 0; i < labels.Length; i++)
            {
                il.MarkLabel(labels[i]);
                var member = members[i];
                bool isFail = true;
                FieldInfo field;
                PropertyInfo prop;
                if((field = member as FieldInfo) != null)
                {
                    il.Emit(obj);
                    Cast(il, type, true);
                    if (isGet)
                    {
                        il.Emit(OpCodes.Ldfld, field);
                        if (_IsValueType(field.FieldType)) il.Emit(OpCodes.Box, field.FieldType);
                    }
                    else
                    {
                        il.Emit(value);
                        Cast(il, field.FieldType, false);
                        il.Emit(OpCodes.Stfld, field);
                    }
                    il.Emit(OpCodes.Ret);
                    isFail = false;
                }
                else if ((prop = member as PropertyInfo) != null)
                {
                    MethodInfo accessor;
                    if (prop.CanRead && (accessor = isGet ? prop.GetGetMethod(allowNonPublicAccessors) : prop.GetSetMethod(allowNonPublicAccessors)) != null)
                    {
                        il.Emit(obj);
                        Cast(il, type, true);
                        if (isGet)
                        {
                            il.EmitCall(_IsValueType(type) ? OpCodes.Call : OpCodes.Callvirt, accessor, null);
                            if (_IsValueType(prop.PropertyType)) il.Emit(OpCodes.Box, prop.PropertyType);
                        }
                        else
                        {
                            il.Emit(value);
                            Cast(il, prop.PropertyType, false);
                            il.EmitCall(_IsValueType(type) ? OpCodes.Call : OpCodes.Callvirt, accessor, null);
                        }
                        il.Emit(OpCodes.Ret);
                        isFail = false;
                    }
                }
                if (isFail) il.Emit(OpCodes.Br, fail);
            }
        }

        private static readonly MethodInfo strinqEquals = typeof(string).GetMethod("op_Equality", new Type[] { typeof(string), typeof(string) });

        /// <summary>
        /// A TypeAccessor based on a Type implementation, with available member metadata
        /// </summary>
        protected abstract class RuntimeTypeAccessor : TypeAccessor
        {
            /// <summary>
            /// Returns the Type represented by this accessor
            /// </summary>
            protected abstract Type Type { get; }

            /// <summary>
            /// Can this type be queried for member availability?
            /// </summary>
            public override bool GetMembersSupported { get { return true; } }
            private MemberSet members;
            /// <summary>
            /// Query the members available for this type
            /// </summary>
            public override MemberSet GetMembers()
            {
                return members ?? (members = new MemberSet(Type));
            }
        }
        sealed class DelegateAccessor : RuntimeTypeAccessor
        {
            private readonly Dictionary<string, int> map;
            private readonly Func<int, object, object> getter;
            private readonly Action<int, object, object> setter;
            private readonly Func<object> ctor;
            private readonly Type type;
            protected override Type Type
            {
                get { return type; }
            }
            public DelegateAccessor(Dictionary<string, int> map, Func<int, object, object> getter, Action<int, object, object> setter, Func<object> ctor, Type type)
            {
                this.map = map;
                this.getter = getter;
                this.setter = setter;
                this.ctor = ctor;
                this.type = type;
            }
            public override bool CreateNewSupported { get { return ctor != null; } }
            public override object CreateNew()
            {
                return ctor != null ? ctor() : base.CreateNew();
            }
            public override object this[object target, string name]
            {
                get
                {
                    int index;
                    if (map.TryGetValue(name, out index)) return getter(index, target);
                    else throw new ArgumentOutOfRangeException("name");
                }
                set
                {
                    int index;
                    if (map.TryGetValue(name, out index)) setter(index, target, value);
                    else throw new ArgumentOutOfRangeException("name");
                }
            }
        }
        private static bool IsFullyPublic(Type type, PropertyInfo[] props, bool allowNonPublicAccessors)
        {
            while (_IsNestedPublic(type)) type = type.DeclaringType;
            if (!_IsPublic(type)) return false;

            if (allowNonPublicAccessors)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    if (props[i].GetGetMethod(true) != null && props[i].GetGetMethod(false) == null) return false; // non-public getter
                    if (props[i].GetSetMethod(true) != null && props[i].GetSetMethod(false) == null) return false; // non-public setter
                }
            }

            return true;
        }
        static TypeAccessor CreateNew(Type type, bool allowNonPublicAccessors)
        {
#if !NO_DYNAMIC
            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type))
            {
                return DynamicAccessor.Singleton;
            }
#endif

            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, int> map = new Dictionary<string, int>();
            List<MemberInfo> members = new List<MemberInfo>(props.Length + fields.Length);
            int i = 0;
            foreach (var prop in props)
            {
                if (!map.ContainsKey(prop.Name) && prop.GetIndexParameters().Length == 0)
                {
                    map.Add(prop.Name, i++);
                    members.Add(prop);
                }
            }
            foreach (var field in fields) if (!map.ContainsKey(field.Name)) { map.Add(field.Name, i++); members.Add(field); }

            ConstructorInfo ctor = null;
            if (_IsClass(type) && !_IsAbstract(type))
            {
                ctor = type.GetConstructor(TypeHelpers.EmptyTypes);
            }
            ILGenerator il;
            if (!IsFullyPublic(type, props, allowNonPublicAccessors))
            {
                DynamicMethod dynGetter = new DynamicMethod(type.FullName + "_get", typeof(object), new Type[] { typeof(int), typeof(object) }, type, true),
                              dynSetter = new DynamicMethod(type.FullName + "_set", null, new Type[] { typeof(int), typeof(object), typeof(object) }, type, true);
                WriteMapImpl(dynGetter.GetILGenerator(), type, members, null, allowNonPublicAccessors, true);
                WriteMapImpl(dynSetter.GetILGenerator(), type, members, null, allowNonPublicAccessors, false);
                DynamicMethod dynCtor = null;
                if (ctor != null)
                {
                    dynCtor = new DynamicMethod(type.FullName + "_ctor", typeof(object), TypeHelpers.EmptyTypes, type, true);
                    il = dynCtor.GetILGenerator();
                    il.Emit(OpCodes.Newobj, ctor);
                    il.Emit(OpCodes.Ret);
                }
                return new DelegateAccessor(
                    map,
                    (Func<int, object, object>)dynGetter.CreateDelegate(typeof(Func<int, object, object>)),
                    (Action<int, object, object>)dynSetter.CreateDelegate(typeof(Action<int, object, object>)),
                    dynCtor == null ? null : (Func<object>)dynCtor.CreateDelegate(typeof(Func<object>)), type);
            }

            // note this region is synchronized; only one is being created at a time so we don't need to stress about the builders
            if (assembly == null)
            {
                AssemblyName name = new AssemblyName("FastMember_dynamic");
#if COREFX
                assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
#else
                assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
#endif
                module = assembly.DefineDynamicModule(name.Name);
            }
#if COREFX
            TypeAttributes attribs = typeof(TypeAccessor).GetTypeInfo().Attributes;
#else
            TypeAttributes attribs = typeof(TypeAccessor).Attributes;
#endif
            TypeBuilder tb = module.DefineType("FastMember_dynamic." + type.Name + "_" + GetNextCounterValue(),
                (attribs | TypeAttributes.Sealed | TypeAttributes.Public) & ~(TypeAttributes.Abstract | TypeAttributes.NotPublic), typeof(RuntimeTypeAccessor));

            il = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] {
                typeof(Dictionary<string,int>)
            }).GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            FieldBuilder mapField = tb.DefineField("_map", typeof(Dictionary<string, int>), FieldAttributes.InitOnly | FieldAttributes.Private);
            il.Emit(OpCodes.Stfld, mapField);
            il.Emit(OpCodes.Ret);


            PropertyInfo indexer = typeof(TypeAccessor).GetProperty("Item");
            MethodInfo baseGetter = indexer.GetGetMethod(), baseSetter = indexer.GetSetMethod();
            MethodBuilder body = tb.DefineMethod(baseGetter.Name, baseGetter.Attributes & ~MethodAttributes.Abstract, typeof(object), new Type[] { typeof(object), typeof(string) });
            il = body.GetILGenerator();
            WriteMapImpl(il, type, members, mapField, allowNonPublicAccessors, true);
            tb.DefineMethodOverride(body, baseGetter);

            body = tb.DefineMethod(baseSetter.Name, baseSetter.Attributes & ~MethodAttributes.Abstract, null, new Type[] { typeof(object), typeof(string), typeof(object) });
            il = body.GetILGenerator();
            WriteMapImpl(il, type, members, mapField, allowNonPublicAccessors, false);
            tb.DefineMethodOverride(body, baseSetter);

            MethodInfo baseMethod;
            if (ctor != null)
            {
                baseMethod = typeof(TypeAccessor).GetProperty("CreateNewSupported").GetGetMethod();
                body = tb.DefineMethod(baseMethod.Name, baseMethod.Attributes, baseMethod.ReturnType, TypeHelpers.EmptyTypes);
                il = body.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Ret);
                tb.DefineMethodOverride(body, baseMethod);

                baseMethod = typeof(TypeAccessor).GetMethod("CreateNew");
                body = tb.DefineMethod(baseMethod.Name, baseMethod.Attributes, baseMethod.ReturnType, TypeHelpers.EmptyTypes);
                il = body.GetILGenerator();
                il.Emit(OpCodes.Newobj, ctor);
                il.Emit(OpCodes.Ret);
                tb.DefineMethodOverride(body, baseMethod);
            }

            baseMethod = typeof(RuntimeTypeAccessor).GetProperty("Type", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            body = tb.DefineMethod(baseMethod.Name, baseMethod.Attributes & ~MethodAttributes.Abstract, baseMethod.ReturnType, TypeHelpers.EmptyTypes);
            il = body.GetILGenerator();
            il.Emit(OpCodes.Ldtoken, type);
            il.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            il.Emit(OpCodes.Ret);
            tb.DefineMethodOverride(body, baseMethod);

            var accessor = (TypeAccessor)Activator.CreateInstance(_CreateType(tb), map);
            return accessor;
        }

        private static void Cast(ILGenerator il, Type type, bool valueAsPointer)
        {
            if (type == typeof(object)) { }
            else if (_IsValueType(type))
            {
                if (valueAsPointer)
                {
                    il.Emit(OpCodes.Unbox, type);
                }
                else
                {
                    il.Emit(OpCodes.Unbox_Any, type);
                }
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>
        /// Get or set the value of a named member on the target instance
        /// </summary>
        public abstract object this[object target, string name]
        {
            get;
            set;
        }
    }
}
