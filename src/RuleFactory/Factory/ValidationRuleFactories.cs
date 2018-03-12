using System;
using System.Collections;
using System.Collections.Generic;
using RuleEngine.Rules;

namespace RuleFactory.Factory
{
    public static class ValidationRuleFactories
    {
        public static ValidationRule<T> CreateValidationRule<T>(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return null;
            var instance = new ValidationRule<T>();

            if (propValueDictionary.ContainsKey("OperatorToUse"))
                instance.OperatorToUse = propValueDictionary["OperatorToUse"].ToString();
            if (propValueDictionary.ContainsKey("ObjectToValidate"))
                instance.ObjectToValidate = propValueDictionary["ObjectToValidate"].ToString();
            if (propValueDictionary.ContainsKey("ValueToValidateAgainst"))
                instance.ValueToValidateAgainst = (Rule) propValueDictionary["ValueToValidateAgainst"];
            if (propValueDictionary.ContainsKey("ChildrenRules"))
                instance.ChildrenRules.AddRange((IEnumerable<Rule>) propValueDictionary["ChildrenRules"]);

            return instance;
        }
    }
}