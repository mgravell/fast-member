using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FastMember
{

    internal static class TypeHelpers
    {

        public static int Min(int x, int y)
        {
            return x < y ? x : y;
        }
    }
}
