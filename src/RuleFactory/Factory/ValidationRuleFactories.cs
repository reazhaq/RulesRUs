//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using RuleEngine.Rules;

//namespace RuleFactory.Factory
//{
//    public static class ValidationRuleFactories
//    {
//        public static ValidationRule<T> CreateValidationRule<T>(IDictionary<string, object> propValueDictionary)
//        {
//            if (propValueDictionary == null) return null;
//            var instance = new ValidationRule<T>();
//            RuleFactories.ReadRuleValues(instance, propValueDictionary);

//            if (propValueDictionary.ContainsKey("OperatorToUse"))
//                instance.OperatorToUse = propValueDictionary["OperatorToUse"].ToString();
//            if (propValueDictionary.ContainsKey("ObjectToValidate"))
//                instance.ObjectToValidate = propValueDictionary["ObjectToValidate"].ToString();
//            if (propValueDictionary.ContainsKey("ValueToValidateAgainst"))
//            {
//                var valueRule = (IDictionary<string, object>) propValueDictionary["ValueToValidateAgainst"];
//                instance.ValueToValidateAgainst = RuleFactory.CreateRuleFromDictionary<T>(valueRule);
//            }

//            if (propValueDictionary.ContainsKey("ChildrenRules"))
//            {
//                var childrenRules = (List<IDictionary<string, object>>) propValueDictionary["ChildrenRules"];
//                foreach (var childrenRule in childrenRules)
//                    instance.ChildrenRules.Add(RuleFactory.CreateRuleFromDictionary<T>(childrenRule));
//            }

//            return instance;
//        }
//    }
//}