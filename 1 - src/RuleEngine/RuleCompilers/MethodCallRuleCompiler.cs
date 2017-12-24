using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using RuleEngine.Interfaces.Compilers;
using RuleEngine.Rules;
using RuleEngine.Utils;

namespace RuleEngine.RuleCompilers
{
    public class MethodCallRuleCompiler<TTarget, TResult> : RuleCompilerBase, IMethodCallRuleCompiler<TTarget, TResult>
    {
        public Expression BuildExpression(ParameterExpression param, MethodCallRule<TTarget, TResult> methodCallRuleToBuildExpression)
        {
            var expression = GetExpressionWithSubProperty(param, methodCallRuleToBuildExpression.ObjectToValidate);

            var inputTypes = methodCallRuleToBuildExpression.Inputs?.Select(i => i.GetType()).ToArray();
            var expressionType = expression.Type.GetMethod(methodCallRuleToBuildExpression.OperatorToUse, inputTypes);
            if (!expressionType.IsGenericMethod)
                inputTypes = null;
            var expressions = methodCallRuleToBuildExpression.Inputs?.Select(Expression.Constant).ToArray();

            return Expression.Call(expression, methodCallRuleToBuildExpression.OperatorToUse, inputTypes, expressions);
        }

        public Func<TTarget, TResult> CompileRule(MethodCallRule<TTarget, TResult> methodCallRuleToCompile)
        {
            var funcParameter = Expression.Parameter(typeof(TTarget));
            var methodCallExpression = BuildExpression(funcParameter, methodCallRuleToCompile);
#if DEBUG
            Debug.WriteLine($"{nameof(methodCallExpression)} ready to compile: {methodCallExpression}");
            methodCallExpression.TraceNode();
#endif
            return Expression.Lambda<Func<TTarget, TResult>>(methodCallExpression, funcParameter).Compile();
        }
    }
}