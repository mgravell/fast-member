using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Collections.Generic;

namespace FastMember
{
    public interface IObject
    {
        object this[string name] {get; set;}
    }
    public abstract class MemberAccess
    {
        class ObjectWrapper : IObject
        {
            private readonly object target;
            private readonly MemberAccess accessor;
            public ObjectWrapper(object target, MemberAccess accessor)
            {
                this.target = target;
                this.accessor = accessor;
            }
            object IObject.this[string name]
            {
                get { return accessor[target, name]; }
                set { accessor[target, name] = value; }
            }
        }
        // hash-table has better read-without-locking semantics than dictionary
        private static readonly Hashtable typeLookyp = new Hashtable();

        public static IObject Wrap(object target)
        {
            if (target == null) throw new ArgumentNullException("target");
            return new ObjectWrapper(target, Create(target.GetType()));
        }
        public static MemberAccess GetAccessor(Type type)
        {
            if(type == null) throw new ArgumentNullException("type");
            MemberAccess obj = (MemberAccess)typeLookyp[type];
            if (obj != null) return obj;

            lock(typeLookyp)
            {
                // double-check
                obj = (MemberAccess)typeLookyp[type];
                if (obj != null) return obj;

                obj = Create(type);

                typeLookyp[type] = obj;
                return obj;
            }
        }

        private static AssemblyBuilder assembly;
        private static ModuleBuilder module;
        private static int counter;
        static MemberAccess Create(Type type)
        {
            // note this region is synchronized; only one is being created at a time so we don't need to stress about the builders
            if(assembly == null)
            {
                AssemblyName name = new AssemblyName("FastMember_dymamic");
                assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
                module = assembly.DefineDynamicModule(name.Name);
            }
            TypeBuilder tb = module.DefineType("FastMember_dymamic." + type.Name + "_" + Interlocked.Increment(ref counter),
                (typeof(MemberAccess).Attributes | TypeAttributes.Sealed) & ~TypeAttributes.Abstract, typeof(MemberAccess) );

            tb.DefineDefaultConstructor(MethodAttributes.Public);
            PropertyInfo indexer = typeof (MemberAccess).GetProperty("Item");
            MethodInfo baseGetter = indexer.GetGetMethod(), baseSetter = indexer.GetSetMethod();

            MethodBuilder body = tb.DefineMethod(baseGetter.Name, baseGetter.Attributes & ~MethodAttributes.Abstract, typeof(object), new Type[] {typeof(object), typeof(string)});
            ILGenerator il = body.GetILGenerator();

            MethodInfo eq = typeof (string).GetMethod("op_Equality", new Type[] {typeof (string), typeof (string)});
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            LocalBuilder loc = type.IsValueType ? il.DeclareLocal(type) : null;
            foreach(PropertyInfo prop in props)
            {
                if (prop.GetIndexParameters().Length != 0 || !prop.CanRead) continue;

                Label next = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldstr, prop.Name);
                il.EmitCall(OpCodes.Call, eq, null);
                il.Emit(OpCodes.Brfalse_S, next);
                // match:
                il.Emit(OpCodes.Ldarg_1);
                Cast(il, type, loc);
                il.EmitCall(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, prop.GetGetMethod(), null);
                if(prop.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Box, prop.PropertyType);
                }
                il.Emit(OpCodes.Ret);
                // not match:
                il.MarkLabel(next);
            }
            foreach (FieldInfo field in fields)
            {
                Label next = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldstr, field.Name);
                il.EmitCall(OpCodes.Call, eq, null);
                il.Emit(OpCodes.Brfalse_S, next);
                // match:
                il.Emit(OpCodes.Ldarg_1);
                Cast(il, type, loc);
                il.Emit(OpCodes.Ldfld, field);
                if (field.FieldType.IsValueType)
                {
                    il.Emit(OpCodes.Box, field.FieldType);
                }
                il.Emit(OpCodes.Ret);
                // not match:
                il.MarkLabel(next);
            }
            il.Emit(OpCodes.Ldstr, "name");
            il.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Throw);
            tb.DefineMethodOverride(body, baseGetter);

            body = tb.DefineMethod(baseSetter.Name, baseSetter.Attributes & ~MethodAttributes.Abstract, null, new Type[] { typeof(object), typeof(string), typeof(object) });
            il = body.GetILGenerator();
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldstr, "Write is not supported for structs");
                il.Emit(OpCodes.Newobj, typeof (NotSupportedException).GetConstructor(new Type[] {typeof (string)}));
                il.Emit(OpCodes.Throw);
            }
            else
            {
                loc = type.IsValueType ? il.DeclareLocal(type) : null;
                foreach (PropertyInfo prop in props)
                {
                    if (prop.GetIndexParameters().Length != 0 || !prop.CanWrite) continue;

                    Label next = il.DefineLabel();
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Ldstr, prop.Name);
                    il.EmitCall(OpCodes.Call, eq, null);
                    il.Emit(OpCodes.Brfalse_S, next);
                    // match:
                    il.Emit(OpCodes.Ldarg_1);
                    Cast(il, type, loc);
                    il.Emit(OpCodes.Ldarg_3);
                    Cast(il, prop.PropertyType, null);
                    il.EmitCall(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, prop.GetSetMethod(), null);
                    il.Emit(OpCodes.Ret);
                    // not match:
                    il.MarkLabel(next);
                }
                foreach (FieldInfo field in fields)
                {
                    Label next = il.DefineLabel();
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Ldstr, field.Name);
                    il.EmitCall(OpCodes.Call, eq, null);
                    il.Emit(OpCodes.Brfalse_S, next);
                    // match:
                    il.Emit(OpCodes.Ldarg_1);
                    Cast(il, type, loc);
                    il.Emit(OpCodes.Ldarg_3);
                    Cast(il, field.FieldType, null);
                    il.Emit(OpCodes.Stfld, field);
                    il.Emit(OpCodes.Ret);
                    // not match:
                    il.MarkLabel(next);
                }
                il.Emit(OpCodes.Ldstr, "name");
                il.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string) }));
                il.Emit(OpCodes.Throw);
            }
            tb.DefineMethodOverride(body, baseSetter);

            return (MemberAccess)Activator.CreateInstance(tb.CreateType());
        }

        private static void Cast(ILGenerator il, Type type, LocalBuilder addr)
        {
            if(type == typeof(object)) {}
            else if(type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
                if (addr != null)
                {
                    il.Emit(OpCodes.Stloc, addr);
                    il.Emit(OpCodes.Ldloca_S, addr);
                }
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }


        int Fetch(string name)
        {
            switch(name)
            {
                case "abc":
                    return 123;
                case "def":
                    return 456;
                case "ghi":
                    return 789;
                default:
                    return -1;
            }
        }

        public abstract object this[object target, string name]
        {
            get; set;
        }
    }
}
