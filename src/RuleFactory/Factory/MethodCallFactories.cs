using System;
using System.Collections;
using System.Collections.Generic;
using RuleEngine.Rules;

namespace RuleFactory.Factory
{
    public static class MethodCallFactories
    {
        public static MethodCallRule<TTarget,TResult> CreateMethodCallRule<TTarget, TResult>(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return null;

            var instance = new MethodCallRule<TTarget, TResult>();

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
    }
}