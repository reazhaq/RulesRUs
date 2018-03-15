using System;
using RuleEngine.Rules;
using RuleEngine.Utils;
//using RuleFactory.Factory;

namespace RuleFactory
{
    public static class RuleFactory
    {
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
                case "UpdateValueRule`1":
                case "UpdateValueRule`2":
                    return CreateUpdateValueRule(boundingTypes);
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

        public static Rule CreateUpdateValueRule(string[] boundingTypes)
        {
            if (boundingTypes == null || boundingTypes.Length < 1 || boundingTypes.Length > 2) return null;

            return (boundingTypes.Length == 1
                ? CreateRule(typeof(UpdateValueRule<>), new[] {ReflectionExtensions.GetTypeFor(boundingTypes[0])})
                : CreateRule(typeof(UpdateValueRule<,>),
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

