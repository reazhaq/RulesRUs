using System;
using System.Collections.Generic;
using RuleEngine.Rules;

namespace RuleFactory.Factory
{
    public static class MethodCallFactories
    {
        public static MethodCallRule<TTarget,TResult> CreateMethodCallRule<TTarget, TResult>(IDictionary<string, string> propValueDictionary, IList<object> inputs)
        {
            if (propValueDictionary == null) return null;

            var constantRuleGenericType = typeof(MethodCallRule<,>);
            var typesToUse = new[] { typeof(TTarget),typeof(TResult) };
            var validationRuleOfT = constantRuleGenericType.MakeGenericType(typesToUse);
            var instance = (MethodCallRule<TTarget,TResult>)Activator.CreateInstance(validationRuleOfT);

            if (propValueDictionary.ContainsKey("MethodToCall"))
                instance.MethodToCall = propValueDictionary["MethodToCall"];
            if (propValueDictionary.ContainsKey("MethodClassName"))
                instance.MethodClassName = propValueDictionary["MethodClassName"];
            if (propValueDictionary.ContainsKey("ObjectToCallMethodOn"))
                instance.ObjectToCallMethodOn = propValueDictionary["ObjectToCallMethodOn"];
            instance.Inputs.AddRange(inputs);

            return instance;
        }
    }
}