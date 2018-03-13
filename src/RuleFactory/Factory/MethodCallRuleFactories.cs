using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RuleEngine.Rules;

namespace RuleFactory.Factory
{
    public static class MethodCallRuleFactories
    {
        public static MethodCallRule<TTarget,TResult> CreateMethodCallRule<TTarget, TResult>(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return null;

            var instance = new MethodCallRule<TTarget, TResult>();
            RuleFactories.ReadRuleValues(instance, propValueDictionary);

            if (propValueDictionary.ContainsKey("MethodToCall"))
                instance.MethodToCall = propValueDictionary["MethodToCall"].ToString();
            if (propValueDictionary.ContainsKey("MethodClassName"))
                instance.MethodClassName = propValueDictionary["MethodClassName"].ToString();
            if (propValueDictionary.ContainsKey("ObjectToCallMethodOn"))
                instance.ObjectToCallMethodOn = propValueDictionary["ObjectToCallMethodOn"].ToString();
            if (propValueDictionary.ContainsKey("Inputs"))
                instance.Inputs.AddRange((IEnumerable<object>) propValueDictionary["Inputs"]);

            return instance;
        }

        //public static void WriteRuleValues<T>(MethodCallRule<T, bool> methodCallRule, IDictionary<string, object> propValueDictionary)
        //{
        //    if (methodCallRule == null || propValueDictionary == null) return;
        //    RuleFactories.WriteRuleValues(methodCallRule, propValueDictionary);

        //    if (!string.IsNullOrEmpty(methodCallRule.MethodToCall))
        //        propValueDictionary.Add("MethodToCall", methodCallRule.MethodToCall);
        //    if (!string.IsNullOrEmpty(methodCallRule.MethodClassName))
        //        propValueDictionary.Add("MethodClassName", methodCallRule.MethodClassName);
        //    if(!string.IsNullOrEmpty(methodCallRule.ObjectToCallMethodOn))
        //        propValueDictionary.Add("ObjectToCallMethodOn", methodCallRule.ObjectToCallMethodOn);
        //    if (methodCallRule.Inputs != null)
        //    {
        //        var inputDictionary = new Dictionary<string,object>();
        //        propValueDictionary.Add("Inputs", inputDictionary);
        //        foreach (var item in methodCallRule.Inputs.Select((value, i)=>new {i,value}))
        //        {
        //            inputDictionary.Add(item.i.ToString(),
        //                item.value is Rule
        //                    ? RuleFactory.ConvertRuleToDictionary<object>(item.value as Rule)
        //                    : item.value);
        //        }
        //    }
        //}
    }
}