// http://stackoverflow.com/a/40364078/492
// https://github.com/mgravell/fast-member/issues/21

using System;
using System.Linq.Expressions;

namespace FastMember
{
    /// <summary> for nested property access in FastMember. </summary>
    public static class FastMemberExtensions
    {
        public static void AssignValueToNestedProperty(this ObjectAccessor accessor, string propertyName, object value)
        {
            var index = propertyName.IndexOf('.');

            if (index == -1)
            {
                var targetType = Expression.Parameter(accessor.Target.GetType());
                var property = Expression.Property(targetType, propertyName);

                var type = property.Type;
                type = Nullable.GetUnderlyingType(type) ?? type;
                value = value == null ? GetDefault(type) : Convert.ChangeType(value, type);
                accessor[propertyName] = value;
            }
            else
            {
                accessor = ObjectAccessor.Create(accessor[propertyName.Substring(0, index)]);
                AssignValueToNestedProperty(accessor, propertyName.Substring(index + 1), value);
            }
        }

        /// <summary> An ObjectAccessor extension method that gets value of nested property. Maybe. </summary>
        /// <param name="accessor">     The accessor to act on. </param>
        /// <param name="propertyName"> Name of the property. </param>
        /// <returns> The value of nested property. </returns>
        public static object GetValueOfNestedProperty (this ObjectAccessor accessor, string propertyName)
        {
            var index = propertyName.IndexOf('.');

            if (index == -1)
            {
                var targetType = Expression.Parameter(accessor.Target.GetType());
                var property = Expression.Property(targetType, propertyName);

                return accessor[propertyName];
            }
            else
            {
                accessor = ObjectAccessor.Create(accessor[propertyName.Substring(0, index)]);
                return GetValueOfNestedProperty(accessor, propertyName.Substring(index + 1));
            }
        }

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}