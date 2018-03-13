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



        //public static Rule CreateRule(string ruleType, IDictionary<string, string> propValueDictionary)
        //{
        //    if (string.IsNullOrEmpty(ruleType) || propValueDictionary == null) return null;

        //    switch (ruleType.ToLower())
        //    {
        //        case "ConstantRule_1":
        //            return ConstantRuleFactories.CreateConstantRule(propValueDictionary);
        //    }

        //    return null;
        //}

        //public static IDictionary<string, object> ConvertRuleToDictionary<T>(Rule rule)
        //{
        //    if (rule == null) return null;

        //    var propValueDictionary = new Dictionary<string, object>();
        //    switch (rule)
        //    {
        //        case ValidationRule<T> validationRule:
        //            propValueDictionary.Add("rule_type", "ValidationRule_1");
        //            ValidationRuleFactories.WriteRuleValues<T>(validationRule, propValueDictionary);
        //            break;
        //        case ConditionalIfThActionRule<T> conditionalIfThActionRule:
        //            propValueDictionary.Add("rule_type", "ConditionalIfThActionRule_1");
        //            ConditionalRuleFactories.WriteRuleValues<T>(conditionalIfThActionRule, propValueDictionary);
        //            break;
        //        case MethodCallRule<T, bool> methodCallRule:
        //            propValueDictionary.Add("rule_type", "MethodCallRule_1Bool");
        //            MethodCallRuleFactories.WriteRuleValues<T>(methodCallRule, propValueDictionary);
        //            break;
        //        case ConstantRule<T> constantRule:
        //            propValueDictionary.Add("rule_type", "ConstantRule_1");
        //            ConstantRuleFactories.WriteRuleValues<T>(constantRule, propValueDictionary);
        //            break;
        //        case UpdateValueRule<T> updateValueRule:
        //            propValueDictionary.Add("rule_type", "UpdateValueRule_1");
        //            UpdateValueRuleFactories.WriteRuleValues<T>(updateValueRule, propValueDictionary);
        //            break;
        //        case Rule someRule when someRule.GetType() == typeof(ConstantRule<>):
        //            propValueDictionary.Add("rule_type", "ConstantRule_1");
        //            ConstantRuleFactories.WriteRuleValues<object>((ConstantRule<object>)someRule, propValueDictionary);
        //            break;
        //            break;
        //    }

        //    return propValueDictionary;
        //}
    }
}

