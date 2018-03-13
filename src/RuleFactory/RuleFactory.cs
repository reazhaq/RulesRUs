using System;
using System.Collections.Generic;
using RuleEngine.Rules;
using RuleFactory.Factory;

namespace RuleFactory
{
    public static class RuleFactory
    {
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

