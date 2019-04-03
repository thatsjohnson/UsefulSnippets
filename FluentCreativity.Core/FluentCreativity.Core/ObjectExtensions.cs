using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FluentCreativity.Core
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns object as specified type.
        /// </summary>
        public static T As<T>(this object Value)
        {
            if (Value is T)
                return (T)Value;
            return default(T);
        }

        /// <summary>
        /// Check if object is equal to all given objects.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public static bool EqualsAll(this object Value, params object[] Values)
        {
            foreach (var i in Values)
            {
                if (Value != i)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Check if object is equal to any given object.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public static bool EqualsAny(this object Value, params object[] Values)
        {
            foreach (var i in Values)
            {
                if (Value == i)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if object is equal to no given object.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public static bool EqualsNone(this object Value, params object[] Values)
        {
            foreach (var i in Values)
            {
                if (Value == i)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Get attribute for member of specified object and of specified type.
        /// </summary>
        /// <typeparam name="T">The type of attribute to get.</typeparam>
        /// <param name="ToEvaluate">The object containing the target attribute.</param>
        /// <param name="Member">The name of the member to get the attribute from; if object is enum, field attribute is obtained.</param>
        /// <param name="Inherits">Whether or not to check inherited attributes.</param>
        /// <returns>Attribute for member of object and specified type.</returns>
        public static T GetAttribute<T>(this object ToEvaluate, string Member = "", bool Inherits = false) where T : Attribute
        {
            if (ToEvaluate != null)
            {
                var Items = ToEvaluate.GetAttributes<T>(Member, Inherits);
                return Items.Count() > 0 ? Items.First() : default(T);
            }
            return default(T);
        }

        /// <summary>
        /// Get all attributes for member of specified object and of specified type.
        /// </summary>
        /// <typeparam name="T">The type of attributes to get.</typeparam>
        /// <param name="ToEvaluate">The object containing the target attributes.</param>
        /// <param name="Member">The name of the member to get attributes from; if object is enum, field attributes are obtained.</param>
        /// <param name="Inherits">Whether or not to check inherited attributes.</param>
        /// <returns>Attributes for member of object and specified type.</returns>
        public static IEnumerable<T> GetAttributes<T>(this object ToEvaluate, string Member = "", bool Inherits = false) where T : Attribute
        {
            if (ToEvaluate != null)
                return ToEvaluate.GetAttributes(Member, Inherits).Where(x => x is T).Cast<T>();
            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// Get all attributes for member of specified object.
        /// </summary>
        /// <param name="ToEvaluate">The object containing the target attributes.</param>
        /// <param name="Member">The name of the member to get attributes from; if object is enum, field attributes are obtained.</param>
        /// <param name="Inherits">Whether or not to check inherited attributes.</param>
        /// <returns>Attributes for member of object.</returns>
        public static IEnumerable<object> GetAttributes(this object ToEvaluate, string Member = "", bool Inherits = false)
        {
            if (ToEvaluate != null)
            {
                var Type = ToEvaluate.GetType();
                var Info = Type.GetMember(ToEvaluate.GetType().IsEnum ? ToEvaluate.ToString() : Member);
                if (Info != null && Info.Length >= 1)
                    return Info[0].GetCustomAttributes(Inherits);
            }
            return Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get value for object from given property name.
        /// </summary>
        /// <param name="Value">The object to get the value for.</param>
        /// <param name="PropertyName">The name of the property to get a value for.</param>
        /// <returns>The value of the property for an object.</returns>
        public static object GetValue(this object Value, string PropertyName)
        {
            var properties = PropertyName.Split('.');
            object parentObj = null;
            object propObj = Value;
            foreach (var property in properties)
            {
                parentObj = propObj;
                propObj = parentObj.GetPropertyValue(property);
            }
            return parentObj.GetType().GetProperty(properties[properties.Length - 1]).GetValue(parentObj, null);
            //return Value.GetType().GetProperty(PropertyName).GetValue(Value, null);
        }

        private static object GetPropertyValue(this object T, string PropName)
        {
            return T.GetType().GetProperty(PropName) == null ? null : T.GetType().GetProperty(PropName).GetValue(T, null);
        }

        /// <summary>
        /// Check if member of specified object has attribute of specified type.
        /// </summary>
        /// <typeparam name="T">The type of attribute to check exists.</typeparam>
        /// <param name="ToEvaluate">The object containing the target attribute.</param>
        /// <param name="Member"></param>
        /// <param name="Inherits">Whether or not to check inherited attributes.</param>
        /// <returns>Whether or not the member of the object (or the object itself) has the attribute.</returns>
        public static bool HasAttribute<T>(this object ToEvaluate, string Member = "", bool Inherits = false) where T : Attribute
        {
            if (ToEvaluate != null)
            {
                var Type = ToEvaluate.GetType();
                var Info = Type.GetMember(ToEvaluate.GetType().IsEnum ? ToEvaluate.ToString() : Member);
                if (Info != null && Info.Length >= 1)
                    return Info[0].GetCustomAttributes(typeof(T), Inherits).Any(x => (Inherits ? x.Is<T>() : x.GetType().Equals<T>()));
            }
            return false;
        }

        /// <summary>
        /// Check if specified object has property with specified name.
        /// </summary>
        /// <param name="ToEvaluate">The object to evaluate.</param>
        /// <param name="PropertyName">The name of the property to check exists.</param>
        /// <returns>Whether or not the specified object has a property with the specified name.</returns>
        public static bool HasProperty(this object ToEvaluate, string PropertyName)
        {
            return ToEvaluate.GetType().GetProperty(PropertyName) != null;
        }

        /// <summary>
        /// Checks if given object's type implements interface (T).
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool Implements<TType>(this object Value) where TType : class
        {
            return Value.GetType().Implements<TType>();
        }

        /// <summary>
        /// Checks if specified object is of specified type.
        /// </summary>
        public static bool Is<T>(this object ToEvaluate)
        {
            return ToEvaluate is T;
        }

        /// <summary>
        /// Checks if specified object is of specified type.
        /// </summary>
        public static bool IsAny(this object ToEvaluate, params Type[] Types)
        {
            foreach (var i in Types)
            {
                var Type = ToEvaluate.GetType();
                if (Type == i || Type.IsSubclassOf(i))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if specified object is NOT of specified type.
        /// </summary>
        public static bool IsNot<T>(this object Value)
        {
            return !Value.Is<T>();
        }

        /// <summary>
        /// Checks if specified object is null.
        /// </summary>
        public static bool IsNull(this object Value)
        {
            return Value == null;
        }

        /// <summary>
        /// Return object if it is null or given object.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Or"></param>
        /// <returns></returns>
        public static object NullOr(this object Value, object Or)
        {
            return Value == null ? null : Or;
        }

        /// <summary>
        /// Return object if it is null or given object.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Or"></param>
        /// <returns></returns>
        public static T NullOr<T>(this object Value, object Or)
        {
            return Value == null ? default(T) : (T)Or;
        }

        /// <summary>
        /// Cast object to given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static T To<T>(this object Value)
        {
            return (T)Value;
        }

        /// <summary>
        /// Casts object to dynamic type.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static dynamic ToDynamic(this object Value)
        {
            return (dynamic)Value;
        }

        #region ComponentModel Extensions

        public static T GetAttribute<T>(this MemberInfo member, bool isRequired) where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0} attribute must be defined on member {1}",
                        typeof(T).Name,
                        member.Name));
            }

            return (T)attribute;
        }

        public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            var attr = memberInfo.GetAttribute<DisplayNameAttribute>(false);
            if (attr == null)
            {
                return memberInfo.Name;
            }

            return attr.DisplayName;
        }

        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            //Debug.Assert(propertyExpression != null, "propertyExpression != null");
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }

        #endregion ComponentModel Extensions
    }
}
