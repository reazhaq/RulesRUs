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

            var inputTypes = new Type[methodCallRuleToBuildExpression.Inputs.Count];
            var argumentsExpressions = new Expression[methodCallRuleToBuildExpression.Inputs.Count];
            for (var index = 0; index < methodCallRuleToBuildExpression.Inputs.Count; index++)
            {
                var input = methodCallRuleToBuildExpression.Inputs[index];
                if (input is Rule)
                {
                    argumentsExpressions[index] = (input as Rule).BuildExpression(param);
                    inputTypes[index] = argumentsExpressions[index].Type;
                }
                else
                {
                    argumentsExpressions[index] = Expression.Constant(input);
                    inputTypes[index] = input.GetType();
                }
            }

            var expressionType = expression.Type.GetMethod(methodCallRuleToBuildExpression.OperatorToUse, inputTypes);
            if (!expressionType.IsGenericMethod)
                inputTypes = null;
            
            return Expression.Call(expression, methodCallRuleToBuildExpression.OperatorToUse, inputTypes, argumentsExpressions);
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