// http://stackoverflow.com/a/40364078/492
// https://github.com/mgravell/fast-member/issues/21

using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace FastMember
{
    /// <summary> for nested property access in FastMember. </summary>
    public static class FastMemberExtensions
    {
        public static void SetValueOfNestedProperty(this ObjectAccessor accessor, string propertyName, object value)
        {
            int index = propertyName.IndexOf('.');

            if (index == -1)
            {
                ParameterExpression targetType = Expression.Parameter(accessor.Target.GetType());
                MemberExpression property      = Expression.Property(targetType, propertyName);

                Type type = property.Type;
                type = Nullable.GetUnderlyingType(type) ?? type;
                value = value == null ? GetDefault(type) : Convert.ChangeType(value, type);
                accessor[propertyName] = value;
            }
            else
            {
                accessor = ObjectAccessor.Create(accessor[propertyName.Substring(0, index)]);
                SetValueOfNestedProperty(accessor, propertyName.Substring(index + 1), value);
            }
        }

        /// <summary> An ObjectAccessor extension method that gets value of nested property. Maybe. </summary>
        /// <param name="accessor">     The accessor to act on. </param>
        /// <param name="propertyName"> Name of the property. </param>
        /// <returns> The value of nested property. </returns>
        public static object GetValueOfNestedProperty (this ObjectAccessor accessor, string propertyName)
        {
            int index = propertyName.IndexOf('.');

            Console.WriteLine(index);

            if (index == -1)
                return accessor[propertyName];


            int indexNextDot = propertyName.IndexOf('.', index + 1);
            if (indexNextDot == -1)
                indexNextDot = propertyName.Length;

            string newPropertyName = propertyName.Substring(index + 1, indexNextDot - index -1);

            Console.WriteLine();
            Console.WriteLine(propertyName);
            Console.Write(propertyName.Substring(0, index) +"    ");
            Console.WriteLine(newPropertyName);

            var newAccessor = ObjectAccessor.Create(accessor[propertyName.Substring(0, index)]);
            // 
            return GetValueOfNestedProperty(newAccessor, newPropertyName);
        }

        

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}