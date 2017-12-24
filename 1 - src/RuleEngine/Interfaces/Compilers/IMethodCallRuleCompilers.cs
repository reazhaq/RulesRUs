using System;
using System.Linq.Expressions;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Rules;

namespace RuleEngine.Interfaces.Compilers
{
    public interface IMethodVoidCallRuleCompiler<T>
    {
        Expression BuildExpression(ParameterExpression param, MethodVoidCallRule<T> methodVoidCallRuleToBuildExpression);
        Action<T> CompileRule(MethodVoidCallRule<T> methodVoidCallRuleToCompile);
    }

    public interface IMethodCallRuleCompiler<TTarget, TResult>
    {
        Expression BuildExpression(ParameterExpression param, MethodCallRule<TTarget, TResult> methodCallRuleToBuildExpression);
        Func<TTarget, TResult> CompileRule(MethodCallRule<TTarget, TResult> methodCallRuleToCompile);
    }
}