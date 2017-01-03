// http://stackoverflow.com/a/40364078/492
// https://github.com/mgravell/fast-member/issues/21

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FastMember
{
    /// <summary> for nested property access in FastMember. </summary>
    public static class FastMemberNestedPropertiesExtensions
    {
        /// <summary> Used internally to return the deepest member in a property chain. </summary>
        private class DeepestObjectAccessor
        {
            internal string DeepestPropertyName  { get; set; }
            internal ObjectAccessor ObjectAccessor  { get; set; }
        }

        /// <summary> An ObjectAccessor extension method that gets deepest nested object accessor in a Property chain. </summary>
        /// <param name="accessor">     This accessor to act on. </param>
        /// <param name="propertyName"> Name of the Property chain. </param>
        /// <returns> The object accessor for the deepest nested Property. </returns>
        private static DeepestObjectAccessor GetDeepestNestedObjectAccessor(this ObjectAccessor accessor, string propertyName)
        {
            DeepestObjectAccessor deepestObjectAccessor = new DeepestObjectAccessor {
                                                              DeepestPropertyName = propertyName,
                                                              ObjectAccessor = accessor
                                                          };

            if (!propertyName.Contains("."))
                return deepestObjectAccessor;

            List<string> nestedProperties = propertyName.Split('.').ToList();
            deepestObjectAccessor.DeepestPropertyName = nestedProperties?.Last() ?? "null";

            nestedProperties.RemoveAt(nestedProperties.Count-1);
            ObjectAccessor nestedAccessor = accessor;

            foreach (string nestedProperty in nestedProperties)
            {
                nestedAccessor = ObjectAccessor.Create(nestedAccessor[nestedProperty]);
            }

            
            deepestObjectAccessor.ObjectAccessor = nestedAccessor;
            return deepestObjectAccessor;
        }

        /// <summary> An ObjectAccessor extension method that gets value of deepest nested Property. </summary>
        /// <param name="accessor">     This accessor to act on. </param>
        /// <param name="propertyName"> Name of the Property chain. </param>
        /// <returns> The value of nested property. </returns>
        public static object GetValueOfDeepestNestedProperty(this ObjectAccessor accessor, string propertyName)
        {
            DeepestObjectAccessor nestedAccessor = GetDeepestNestedObjectAccessor(accessor, propertyName);
            var ret = nestedAccessor.ObjectAccessor[nestedAccessor.DeepestPropertyName];
            return ret;
        }

        /// <summary> An ObjectAccessor extension method that sets value of deepest nested property. </summary>
        /// <param name="accessor">     This accessor to act on. </param>
        /// <param name="propertyName"> Name of the Property chain. </param>
        /// <param name="value">        The value. </param>
        public static void SetValueOfDeepestNestedProperty(this ObjectAccessor accessor, string propertyName , object value)
        {
            DeepestObjectAccessor nestedAccessor = GetDeepestNestedObjectAccessor(accessor, propertyName);
            nestedAccessor.ObjectAccessor[nestedAccessor.DeepestPropertyName] = value;
        }



        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}