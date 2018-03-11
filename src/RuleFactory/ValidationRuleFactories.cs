using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using RuleEngine.Rules;

namespace RuleFactory
{
    public class ValidationRuleFactories
    {
        public static ValidationRule<T> CreateValidationRule<T>(IDictionary<string, string> propValueDictionary)
        {
            if (propValueDictionary == null) return null;

            var constantRuleGenericType = typeof(ValidationRule<>);
            var typesToUse = new[] { typeof(T) };
            var validationRuleOfT = constantRuleGenericType.MakeGenericType(typesToUse);
            var instance = (ValidationRule<T>)Activator.CreateInstance(validationRuleOfT);

            if (propValueDictionary.ContainsKey("OperatorToUse"))
                instance.OperatorToUse = propValueDictionary["OperatorToUse"];
            if (propValueDictionary.ContainsKey("ObjectToValidate"))
                instance.ObjectToValidate = propValueDictionary["ObjectToValidate"];

            return instance;
        }
    }
}