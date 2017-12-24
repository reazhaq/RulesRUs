using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using RuleEngine.Interfaces.Compilers;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Rules;
using RuleEngine.Utils;

namespace RuleEngine.RuleCompilers
{
    public class MethodCallBase : RuleCompilerBase
    {
        protected virtual Expression[] GetArgumentsExpressions(ParameterExpression param, List<object> inputs, Type[] inputTypes)
        {
            var argumentsExpressions = new Expression[inputs.Count];
            for (var index = 0; index < inputs.Count; index++)
            {
                var input = inputs[index];
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
            return argumentsExpressions;
        }
    }

    public class MethodVoidCallRuleCompiler<T> : MethodCallBase, IMethodVoidCallRuleCompiler<T>
    {
        public Expression BuildExpression(ParameterExpression param, MethodVoidCallRule<T> methodVoidCallRuleToBuildExpression)
        {
            var expression = GetExpressionWithSubProperty(param, methodVoidCallRuleToBuildExpression.ObjectToCallMethodOn);

            var inputTypes = new Type[methodVoidCallRuleToBuildExpression.Inputs.Count];
            var argumentsExpressions = GetArgumentsExpressions(param, methodVoidCallRuleToBuildExpression.Inputs, inputTypes);

            var expressionType = expression.Type.GetMethod(methodVoidCallRuleToBuildExpression.MethodToCall, inputTypes);
            if (!expressionType.IsGenericMethod)
                inputTypes = null;

            return Expression.Call(expression, methodVoidCallRuleToBuildExpression.MethodToCall, inputTypes, argumentsExpressions);
        }

        public Action<T> CompileRule(MethodVoidCallRule<T> methodVoidCallRuleToCompile)
        {
            var param = Expression.Parameter(typeof(T));
            var methodExpression = BuildExpression(param, methodVoidCallRuleToCompile);
#if DEBUG
            Debug.WriteLine($"{nameof(methodVoidCallRuleToCompile)} ready to compile: {methodExpression}");
            methodExpression.TraceNode();
#endif
            return Expression.Lambda<Action<T>>(methodExpression, param).Compile();
        }
    }

    public class MethodCallRuleCompiler<TTarget, TResult> : MethodCallBase, IMethodCallRuleCompiler<TTarget, TResult>
    {
        public Expression BuildExpression(ParameterExpression param, MethodCallRule<TTarget, TResult> methodCallRuleToBuildExpression)
        {
            var expression = GetExpressionWithSubProperty(param, methodCallRuleToBuildExpression.ObjectToCallMethodOn);

            var inputTypes = new Type[methodCallRuleToBuildExpression.Inputs.Count];
            var argumentsExpressions = GetArgumentsExpressions(param, methodCallRuleToBuildExpression.Inputs, inputTypes);

            var expressionType = expression.Type.GetMethod(methodCallRuleToBuildExpression.MethodToCall, inputTypes);
            if (!expressionType.IsGenericMethod)
                inputTypes = null;
            
            return Expression.Call(expression, methodCallRuleToBuildExpression.MethodToCall, inputTypes, argumentsExpressions);
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