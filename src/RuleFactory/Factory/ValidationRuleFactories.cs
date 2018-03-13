﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RuleEngine.Rules;

namespace RuleFactory.Factory
{
    public static class ValidationRuleFactories
    {
        public static ValidationRule<T> CreateValidationRule<T>(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return null;
            var instance = new ValidationRule<T>();
            RuleFactories.ReadRuleValues(instance, propValueDictionary);

            if (propValueDictionary.ContainsKey("OperatorToUse"))
                instance.OperatorToUse = propValueDictionary["OperatorToUse"].ToString();
            if (propValueDictionary.ContainsKey("ObjectToValidate"))
                instance.ObjectToValidate = propValueDictionary["ObjectToValidate"].ToString();
            if (propValueDictionary.ContainsKey("ValueToValidateAgainst"))
                instance.ValueToValidateAgainst = (Rule)propValueDictionary["ValueToValidateAgainst"];

            //instance.ChildrenRules.AddRange((IEnumerable<Rule>)propValueDictionary["ChildrenRules"]);
            //if (propValueDictionary.ContainsKey("ChildrenRules"))
            //{
            //    var childrenPropValueDictionary = (IDictionary<string, object>) propValueDictionary["ChildrenRules"];
            //    // todo......
            //}

            return instance;
        }

        //public static void WriteRuleValues<T>(ValidationRule<T> rule, IDictionary<string, object> propValueDictionary)
        //{
        //    if (rule == null || propValueDictionary == null) return;

        //    RuleFactories.WriteRuleValues(rule, propValueDictionary);
        //    if (!string.IsNullOrEmpty("OperatorToUse"))
        //        propValueDictionary.Add("OperatorToUse", rule.OperatorToUse);
        //    if (!string.IsNullOrEmpty("ObjectToValidate"))
        //        propValueDictionary.Add("ObjectToValidate", rule.ObjectToValidate);
        //    if (rule.ValueToValidateAgainst != null)
        //        RuleFactories.WriteRuleValues(rule.ValueToValidateAgainst, propValueDictionary);
        //    if (rule.ChildrenRules != null && rule.ChildrenRules.Any())
        //    {
        //        var childrenPropValueDictionary = new Dictionary<string, object>();
        //        propValueDictionary.Add("ChildrenRules", childrenPropValueDictionary);
        //        foreach (var item in rule.ChildrenRules.Select((r,i)=>new {i,r}))
        //        {
        //            childrenPropValueDictionary.Add(item.i.ToString(), RuleFactory.ConvertRuleToDictionary<T>(item.r));
        //        }
        //    }
        //}
    }
}