// http://stackoverflow.com/a/40364078/492
// https://github.com/mgravell/fast-member/issues/21

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace FastMember
{
    /// <summary> for nested property access in FastMember. </summary>
    public static class FastMemberExtensions
    {
        public static void SetValueOfNestedPropertyzzzzz(this ObjectAccessor accessor, string propertyName, object value)
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


        private class DeepestObjectAccessor
        {
            internal string DeepestPropertyName  { get; set; }
            internal ObjectAccessor ObjectAccessor  { get; set; }
        }


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
                Console.WriteLine("nested property: " + nestedProperty);
                nestedAccessor = ObjectAccessor.Create(nestedAccessor[nestedProperty]);
            }

            
            deepestObjectAccessor.ObjectAccessor = nestedAccessor;
            return deepestObjectAccessor;
        }


        public static object GetValueOfNestedProperty(this ObjectAccessor accessor, string propertyName)
        {
            DeepestObjectAccessor nestedAccessor = GetDeepestNestedObjectAccessor(accessor, propertyName);

            Console.WriteLine("Whole property name: " + propertyName);
            Console.WriteLine("Deepest property name: " + nestedAccessor.DeepestPropertyName);
            var ret = nestedAccessor.ObjectAccessor[nestedAccessor.DeepestPropertyName];

            Console.WriteLine("property value read was: " + (ret?.ToString() ?? "null"));
            return ret;

        }

        public static void SetValueOfNestedProperty(this ObjectAccessor accessor, string propertyName , object value)
        {
            DeepestObjectAccessor nestedAccessor = GetDeepestNestedObjectAccessor(accessor, propertyName);

            Console.WriteLine("Whole property name: " + propertyName);
            Console.WriteLine("Deepest property name: " + nestedAccessor.DeepestPropertyName);

            ParameterExpression targetType = Expression.Parameter(nestedAccessor.ObjectAccessor.Target.GetType());
            MemberExpression property = Expression.Property(targetType, nestedAccessor.DeepestPropertyName);

            Type type = property.Type;
            type = Nullable.GetUnderlyingType(type) ?? type;
            value = value == null ? GetDefault(type) : Convert.ChangeType(value, type);
            nestedAccessor.ObjectAccessor[nestedAccessor.DeepestPropertyName] = value;
        }



        /// <summary> An ObjectAccessor extension method that gets value of nested property. Maybe. </summary>
        /// <param name="accessor">     The accessor to act on. </param>
        /// <param name="propertyName"> Name of the property. </param>
        /// <returns> The value of nested property. </returns>
        public static object GetValueOfNestedPropertyold (this ObjectAccessor accessor, string propertyName)
        {
            int dot = propertyName.IndexOf('.');
            ObjectAccessor accessorToReturn = accessor;

Console.WriteLine(); Console.WriteLine(dot); Console.WriteLine("whole property name: "+ propertyName);

            if (dot == -1)
                return accessorToReturn[propertyName];

            while (dot < propertyName.Length-1)
            {
                int indexNextDot = propertyName.IndexOf('.', dot );
                if (indexNextDot == -1)
                    indexNextDot = propertyName.Length;

Console.WriteLine("start next Prop: " + (dot + 1)); Console.WriteLine("Next dot: " + indexNextDot); Console.WriteLine("Length dot: " + (indexNextDot - dot));

                string thisLevelPropertyName       = propertyName.Substring(0, dot);
                string nextLevelNestedPropertyName = propertyName.Substring(dot + 1, indexNextDot - dot );

Console.WriteLine("this level property name: **" + thisLevelPropertyName + "**"); Console.WriteLine("next level nested property name: **" + nextLevelNestedPropertyName + "**");

                object nestedProperty = accessor[thisLevelPropertyName];

Console.WriteLine("nested property type: " + (nestedProperty?.GetType().Name ?? "null"));

                accessorToReturn = ObjectAccessor.Create(nestedProperty);

                dot = indexNextDot;
            }

            // if (dot == -1)
                return accessorToReturn[propertyName];


           

            // return GetValueOfNestedProperty(nestedAccessor, nextLevelNestedPropertyName);
        }



        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}