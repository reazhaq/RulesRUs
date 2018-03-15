//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using RuleEngine.Rules;

//namespace RuleFactory.Factory
//{
//    public static class MethodCallRuleFactories
//    {
//        public static MethodCallRule<TTarget,TResult> CreateMethodCallRule<TTarget, TResult>(IDictionary<string, object> propValueDictionary)
//        {
//            if (propValueDictionary == null) return null;

//            var instance = new MethodCallRule<TTarget, TResult>();
//            RuleFactories.ReadRuleValues(instance, propValueDictionary);

//            if (propValueDictionary.ContainsKey("MethodToCall"))
//                instance.MethodToCall = propValueDictionary["MethodToCall"].ToString();
//            if (propValueDictionary.ContainsKey("MethodClassName"))
//                instance.MethodClassName = propValueDictionary["MethodClassName"].ToString();
//            if (propValueDictionary.ContainsKey("ObjectToCallMethodOn"))
//                instance.ObjectToCallMethodOn = propValueDictionary["ObjectToCallMethodOn"].ToString();
//            if (propValueDictionary.ContainsKey("Inputs"))
//            {
//                var inputs = (List<object>) propValueDictionary["Inputs"];
//                foreach (var input in inputs)
//                {
//                    if (input is IDictionary<string, object> objects)
//                    {
//                        instance.Inputs.Add(RuleFactory.CreateRuleFromDictionary<TTarget>(objects));
//                    }
//                    else
//                    {
//                        instance.Inputs.Add(input);
//                    }
//                }
//            }

//            return instance;
//        }
//    }
//}