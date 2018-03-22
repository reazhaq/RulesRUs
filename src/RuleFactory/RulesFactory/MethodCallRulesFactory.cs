using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RuleEngine.Rules;
using RuleEngine.Utils;

namespace RuleFactory.RulesFactory
{
    public class MethodCallRulesFactory
    {
        public static MethodCallRule<T1, T2> CreateMethodCallRule<T1, T2>(string methodToCall, string methodClassName,
            Expression<Func<T1, object>> objectToCallMethodOn, IList<Rule> methodParams)
        {
            var rule = new MethodCallRule<T1, T2>
            {
                MethodToCall = methodToCall,
                MethodClassName = methodClassName,
                ObjectToCallMethodOn = objectToCallMethodOn.GetObjectToValidateFromExpression()
            };
            rule.MethodParameters.AddRange(methodParams);
            return rule;
        }
    }
}