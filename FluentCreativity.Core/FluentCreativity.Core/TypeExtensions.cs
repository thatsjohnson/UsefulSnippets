using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FluentCreativity.Core
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets whether or not the type is equal to type, <see cref="{TType}"/>.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool Equals<TType>(this Type Value)
        {
            return Value == typeof(TType);
        }

        /// <summary>
        /// Gets whether or not the type implements interface, <see cref="{TType}"/> (or whether <see cref="{TType}"/> is assignable from the type).
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool Implements<TType>(this Type Value) where TType : class
        {
            if (!typeof(TType).IsInterface)
                throw new InvalidCastException("Type is not an interface.");

            return typeof(TType).IsAssignableFrom(Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type Value)
        {
            if (!Value.IsGenericType)
                return false;

            return Value.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Attempts to create a new instance of given type using <see cref="Activator.CreateInstance()"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static T TryCreate<T>(this Type Value)
        {
            return (T)GetDefault(typeof(T));
        }

        public static object GetDefault(this Type type)
        {
            // If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
            if (type == null || !type.IsValueType || type == typeof(void))
                return null;

            // If the supplied Type has generic parameters, its default value cannot be determined
            if (type.ContainsGenericParameters)
                throw new ArgumentException(
                    "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                    "> contains generic parameters, so the default value cannot be retrieved");

            // If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct/enum), return a 
            //  default instance of the value type
            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(
                        "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe Activator.CreateInstance method could not " +
                        "create a default instance of the supplied value type <" + type +
                        "> (Inner Exception message: \"" + e.Message + "\")", e);
                }
            }

            // Fail with exception
            throw new ArgumentException("{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                "> is not a publicly-visible type, so the default value cannot be retrieved");
        }

        public static bool IsObjectSetToDefault(this Type ObjectType, object ObjectValue)
        {
            // If no ObjectType was supplied, attempt to determine from ObjectValue
            if (ObjectType == null)
            {
                // If no ObjectValue was supplied, abort
                if (ObjectValue == null)
                {
                    MethodBase currmethod = MethodInfo.GetCurrentMethod();
                    string ExceptionMsgPrefix = currmethod.DeclaringType + " {" + currmethod + "} Error:\n\n";
                    throw new ArgumentNullException(ExceptionMsgPrefix + "Cannot determine the ObjectType from a null Value");
                }

                // Determine ObjectType from ObjectValue
                ObjectType = ObjectValue.GetType();
            }

            // Get the default value of type ObjectType
            object Default = ObjectType.GetDefault();

            // If a non-null ObjectValue was supplied, compare Value with its default value and return the result
            if (ObjectValue != null)
                return ObjectValue.Equals(Default);

            // Since a null ObjectValue was supplied, report whether its default value is null
            return Default == null;
        }
    }
}
