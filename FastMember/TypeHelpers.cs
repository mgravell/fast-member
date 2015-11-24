using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FastMember
{
    internal static class TypeHelpers
    {
        public static bool _IsValueType(this Type type)
        {
#if DNXCORE50
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }
        public static bool _IsPublic(this Type type)
        {
#if DNXCORE50
            return type.GetTypeInfo().IsPublic;
#else
            return type.IsPublic;
#endif
        }

        public static bool _IsNestedPublic(this Type type)
        {
#if DNXCORE50
            return type.GetTypeInfo().IsNestedPublic;
#else
            return type.IsNestedPublic;
#endif
        }
        public static bool _IsClass(this Type type)
        {
#if DNXCORE50
            return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

        public static bool _IsAbstract(this Type type)
        {
#if DNXCORE50
            return type.GetTypeInfo().IsAbstract;
#else
            return type.IsAbstract;
#endif
        }
        public static Type _CreateType(this TypeBuilder type)
        {
#if DNXCORE50
            return type.CreateTypeInfo().AsType();
#else
            return type.CreateType();
#endif
        }
    }
}
