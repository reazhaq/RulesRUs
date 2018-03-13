using System;
using System.Collections.Generic;
using RuleEngine.Rules;
using RuleFactory.Factory;

namespace RuleFactory
{
    public static class RuleFactory
    {
        public static Rule CreateRuleFromDictionary<T>(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return null;
            if (propValueDictionary.ContainsKey("RuleType"))
            {
                var ruleType = propValueDictionary["RuleType"].ToString();
                var boundingTypes = (propValueDictionary["BoundingTypes"] as List<string>);

                switch (ruleType)
                {
                    case "ConstantRule" when boundingTypes?.Count==1:
                        var type = boundingTypes[0].ToString();
                        return ConstantRuleFactories.CreateConstantRuleFromPrimitiveTypeAndString(type,
                            propValueDictionary["Value"].ToString());
                    case "ConditionalIfThActionRule" when boundingTypes?.Count==1:
                        return ConditionalRuleFactories.CreateConditionalIfThActionRule<T>(propValueDictionary);
                    case "ValidationRule" when boundingTypes?.Count==1:
                        return ValidationRuleFactories.CreateValidationRule<T>(propValueDictionary);
                    case "MethodCallRule" when boundingTypes?.Count==2:
                        return MethodCallRuleFactories.CreateMethodCallRule<T, bool>(propValueDictionary);
                    case "UpdateValueRule" when boundingTypes?.Count==1:
                        return UpdateValueRuleFactories.CreateUpdateValueRule<T>(propValueDictionary);
                }

                if (ruleType.Equals("ValidationRule"))
                    return ValidationRuleFactories.CreateValidationRule<T>(propValueDictionary);


            }

            return null;
        }
    }
}

