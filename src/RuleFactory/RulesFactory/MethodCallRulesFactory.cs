using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RuleEngine.Rules;
using RuleEngine.Utils;

namespace RuleFactory.RulesFactory
{
    public class MethodCallRulesFactory
    {
        public static MethodVoidCallRule<T> CreateMethodVoidCallRule<T>(string methodToCall, string methodClassName,
            Expression<Func<T, object>> objectToCallMethodOn, IList<Rule> methodParams)
        {
            var rule = new MethodVoidCallRule<T>
            {
                MethodToCall = methodToCall,
                MethodClassName = methodClassName,
                ObjectToCallMethodOn = objectToCallMethodOn?.GetObjectToValidateFromExpression()
            };
            if (methodParams != null)
                rule.MethodParameters.AddRange(methodParams);
            return rule;
        }

        public static MethodCallRule<T1, T2> CreateMethodCallRule<T1, T2>(string methodToCall, string methodClassName,
            Expression<Func<T1, object>> objectToCallMethodOn, IList<Rule> methodParams)
        {
            var rule = new MethodCallRule<T1, T2>
            {
                MethodToCall = methodToCall,
                MethodClassName = methodClassName,
                ObjectToCallMethodOn = objectToCallMethodOn?.GetObjectToValidateFromExpression()
            };
            if (methodParams != null)
                rule.MethodParameters.AddRange(methodParams);
            return rule;
        }
    }
}