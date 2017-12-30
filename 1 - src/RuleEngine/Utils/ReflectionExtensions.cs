using System;
using System.Collections.Generic;

namespace RuleEngine.Utils
{
    internal static class ReflectionExtensions
    {
        //public static IEqualityComparer<T> GetEqualityComparer<T>(Type type, string fieldOrPropertyName)
        //{
        //    if (type == null) throw new ArgumentNullException($"{nameof(type)} cannot be null");
        //    if (string.IsNullOrEmpty(fieldOrPropertyName)) throw new ArgumentNullException($"{nameof(fieldOrPropertyName)} cannot be null");

        //    if (TryGetFieldValue(type, fieldOrPropertyName, out var fieldValue))
        //        return (IEqualityComparer<T>)fieldValue;

        //    if (TryGetPropertyValue(type, fieldOrPropertyName, out var propValue))
        //        return (IEqualityComparer<T>)propValue;

        //    return null;
        //}

        //private static bool TryGetPropertyValue(Type type, string fieldOrPropertyName, out object propValue)
        //{
        //    propValue = null;
        //    var property = type.GetProperty(fieldOrPropertyName);
        //    if (property == null) return false;

        //    var isStatic = property.GetAccessors()?[0].IsStatic;


        //    return false;
        //}

        //private static bool TryGetFieldValue(Type type, string fieldOrPropertyName, out object fieldValue)
        //{
        //    fieldValue = null;
        //    throw new NotImplementedException();
        //}
    }
}