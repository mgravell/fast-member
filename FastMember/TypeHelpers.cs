using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FastMember
{

    internal static class TypeHelpers
    {
#if COREFX
        public static readonly Type[] EmptyTypes = new Type[0];
#else
        public static readonly Type[] EmptyTypes = Type.EmptyTypes;
#endif

        public static bool _IsValueType(Type type)
        {
#if COREFX
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }
        public static bool _IsPublic(Type type)
        {
#if COREFX
            return type.GetTypeInfo().IsPublic;
#else
            return type.IsPublic;
#endif
        }

        public static bool _IsNestedPublic(Type type)
        {
#if COREFX
            return type.GetTypeInfo().IsNestedPublic;
#else
            return type.IsNestedPublic;
#endif
        }
        public static bool _IsClass(Type type)
        {
#if COREFX
            return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

        public static bool _IsAbstract(Type type)
        {
#if COREFX
            return type.GetTypeInfo().IsAbstract;
#else
            return type.IsAbstract;
#endif
        }
        public static Type _CreateType(TypeBuilder type)
        {
#if COREFX
            return type.CreateTypeInfo().AsType();
#else
            return type.CreateType();
#endif
        }

        public static int Min(int x, int y)
        {
            return x < y ? x : y;
        }
    }
}
