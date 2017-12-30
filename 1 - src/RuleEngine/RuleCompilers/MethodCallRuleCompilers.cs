using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RuleEngine.Common;
using RuleEngine.Interfaces.Compilers;
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

        protected MethodInfo GetMethodInfo(string methodClassName, string methodToCall, Type[] inputTypes,
            Expression expression)
        {
            if (string.IsNullOrEmpty(methodClassName))
                return expression.Type.GetMethodInfo(methodToCall, Array.AsReadOnly(inputTypes));

            var type = Type.GetType(methodClassName);
            if (type == null) throw new RuleEngineException($"can't find class named: {methodClassName}");

            return type.GetMethodInfo(methodToCall, Array.AsReadOnly(inputTypes));
        }
    }

    public class MethodVoidCallRuleCompiler<T> : MethodCallBase, IMethodVoidCallRuleCompiler<T>
    {
        public Expression BuildExpression(ParameterExpression param, MethodVoidCallRule<T> methodVoidCallRuleToBuildExpression)
        {
            var expression = GetExpressionWithSubProperty(param, methodVoidCallRuleToBuildExpression.ObjectToCallMethodOn);

            var inputTypes = new Type[methodVoidCallRuleToBuildExpression.Inputs.Count];
            var argumentsExpressions = GetArgumentsExpressions(param, methodVoidCallRuleToBuildExpression.Inputs, inputTypes);

            var methodInfo = GetMethodInfo(methodVoidCallRuleToBuildExpression.MethodClassName, methodVoidCallRuleToBuildExpression.MethodToCall, inputTypes, expression);
            if (methodInfo == null) return null;
            if (!methodInfo.IsGenericMethod)
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

            var methodInfo = GetMethodInfo(methodCallRuleToBuildExpression.MethodClassName, methodCallRuleToBuildExpression.MethodToCall, inputTypes, expression);
            if (methodInfo == null) return null;
            if (!methodInfo.IsGenericMethod)
                inputTypes = null;

            return Expression.Call(expression, methodCallRuleToBuildExpression.MethodToCall, inputTypes, argumentsExpressions);
        }


        public Func<TTarget, TResult> CompileRule(MethodCallRule<TTarget, TResult> methodCallRuleToCompile)
        {
            var funcParameter = Expression.Parameter(typeof(TTarget));
            var methodCallExpression = BuildExpression(funcParameter, methodCallRuleToCompile);
            if (methodCallExpression == null)
                return null;
#if DEBUG
            Debug.WriteLine($"{nameof(methodCallExpression)} ready to compile: {methodCallExpression}");
            methodCallExpression.TraceNode();
#endif
            return Expression.Lambda<Func<TTarget, TResult>>(methodCallExpression, funcParameter).Compile();
        }
    }
}