using System;
using RuleEngine.Rules;
using RuleEngine.Utils;
//using RuleFactory.Factory;

namespace RuleFactory
{
    public static class RuleFactory
    {
        //public static Rule CreateRuleFromDictionary<T>(IDictionary<string, object> propValueDictionary)
        //{
        //    if (propValueDictionary == null) return null;
        //    if (propValueDictionary.ContainsKey("RuleType"))
        //    {
        //        var ruleType = propValueDictionary["RuleType"].ToString();
        //        var boundingTypes = (propValueDictionary["BoundingTypes"] as List<string>);

        //        switch (ruleType)
        //        {
        //            case "ConstantRule" when boundingTypes?.Count == 1:
        //                var type = boundingTypes[0].ToString();
        //                return ConstantRuleFactories.CreateConstantRuleFromPrimitiveTypeAndString(type,
        //                    propValueDictionary["Value"].ToString());
        //            case "ConditionalIfThActionRule" when boundingTypes?.Count == 1:
        //                return ConditionalRuleFactories.CreateConditionalIfThActionRule<T>(propValueDictionary);
        //            case "ValidationRule" when boundingTypes?.Count == 1:
        //                return ValidationRuleFactories.CreateValidationRule<T>(propValueDictionary);
        //            case "MethodCallRule" when boundingTypes?.Count == 2:
        //                return MethodCallRuleFactories.CreateMethodCallRule<T, bool>(propValueDictionary);
        //            case "UpdateValueRule" when boundingTypes?.Count == 1:
        //                return UpdateValueRuleFactories.CreateUpdateValueRule<T>(propValueDictionary);
        //        }
        //    }

        //    return null;
        //}

        public static Rule CreateRule(string ruleType, string[] boundingTypes)
        {
            switch (ruleType)
            {
                case "ConstantRule`1":
                case "ConstantRule`2":
                    return CreateConstantRule(boundingTypes);
                case "ValidationRule`1":
                case "ValidationRule`2":
                    return CreateValidationRule(boundingTypes);
            }

            return null;
        }

        public static Rule CreateConstantRule(string[] boundingTypes)
        {
            if (boundingTypes == null || boundingTypes.Length < 1 || boundingTypes.Length > 2) return null;

            return (boundingTypes.Length == 1
                ? CreateRule(typeof(ConstantRule<>), new[] {ReflectionExtensions.GetTypeFor(boundingTypes[0])})
                : CreateRule(typeof(ConstantRule<,>),
                    new[] {ReflectionExtensions.GetTypeFor(boundingTypes[0]), ReflectionExtensions.GetTypeFor(boundingTypes[1])}));
        }

        public static Rule CreateValidationRule(string[] boundingTypes)
        {
            if (boundingTypes == null || boundingTypes.Length < 1 || boundingTypes.Length > 2) return null;

            return (boundingTypes.Length == 1
                    ? CreateRule(typeof(ValidationRule<>), new[] {ReflectionExtensions.GetTypeFor(boundingTypes[0])})
                    : CreateRule(typeof(ValidationRule<,>),
                        new[] {ReflectionExtensions.GetTypeFor(boundingTypes[0]), ReflectionExtensions.GetTypeFor(boundingTypes[1])}));
        }

        private static Rule CreateRule(Type genericType, Type[] typesToBoundTo)
        {
            if (genericType == null || typesToBoundTo == null) return null;
            var boundedGenericType = genericType.MakeGenericType(typesToBoundTo);
            var instance = Activator.CreateInstance(boundedGenericType);
            return (Rule) instance;
        }
    }
}

