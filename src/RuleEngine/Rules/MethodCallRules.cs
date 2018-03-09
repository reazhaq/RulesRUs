using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    public class MethodCallBase : Rule
    {
        public string MethodToCall;
        public string MethodClassName { get; set; }
        public string ObjectToCallMethodOn { get; set; }
        public List<object> Inputs { get; } = new List<object>();

        public override Expression BuildExpression(params ParameterExpression[] parameters) => throw new NotImplementedException();
        public override bool Compile() => throw new NotImplementedException();

        protected MethodInfo GetMethodInfo(string methodClassName, string methodToCall, Type[] inputTypes,
            Expression expression)
        {
            if (string.IsNullOrEmpty(methodClassName))
                return expression.Type.GetMethodInfo(methodToCall, inputTypes);

            var type = Type.GetType(methodClassName);
            if (type == null) throw new RuleEngineException($"can't find class named: {methodClassName}");

            return type.GetMethodInfo(methodToCall, inputTypes);
        }
    }

    public class MethodVoidCallRule<T> : MethodCallBase, IMethodVoidCallRule<T>
    {
        private Action<T> CompiledDelegate { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T)}");

            var param = parameters[0];
            var expression = GetExpressionWithSubProperty(param, ObjectToCallMethodOn);

            var inputTypes = new Type[Inputs.Count];
            var argumentsExpressions = GetArgumentsExpressions(param, Inputs, inputTypes);

            var methodInfo = GetMethodInfo(MethodClassName, MethodToCall, inputTypes, expression);
            if (methodInfo == null) return null;
            if (!methodInfo.IsGenericMethod)
                inputTypes = null;

            ExpressionForThisRule = Expression.Call(expression, MethodToCall, inputTypes, argumentsExpressions);
            return ExpressionForThisRule;
        }

        public override bool Compile()
        {
            var param = Expression.Parameter(typeof(T));
            ExpressionForThisRule = BuildExpression(param);
            if (ExpressionForThisRule == null) return false;

            Debug.WriteLine($"{nameof(ExpressionForThisRule)} ready to compile:" +
                            $"{Environment.NewLine}{ExpressionDebugView()}");

            CompiledDelegate = Expression.Lambda<Action<T>>(ExpressionForThisRule, param).Compile();
            return CompiledDelegate != null;
        }

        public void Execute(T param)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            CompiledDelegate(param);
        }
    }

    public class MethodCallRule<TTarget, TResult> : MethodCallBase, IMethodCallRule<TTarget, TResult>
    {
        private Func<TTarget, TResult> CompiledDelegate { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(TTarget))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(TTarget)}");

            var param = parameters[0];
            var expression = GetExpressionWithSubProperty(param, ObjectToCallMethodOn);

            var inputTypes = new Type[Inputs.Count];
            var argumentsExpressions = GetArgumentsExpressions(param, Inputs, inputTypes);

            var methodInfo = GetMethodInfo(MethodClassName, MethodToCall, inputTypes, expression);
            if (methodInfo == null) return null;
            if (!methodInfo.IsGenericMethod)
                inputTypes = null;

            ExpressionForThisRule = Expression.Call(expression, MethodToCall, inputTypes, argumentsExpressions);
            return ExpressionForThisRule;
        }

        public override bool Compile()
        {
            var funcParameter = Expression.Parameter(typeof(TTarget));
            ExpressionForThisRule = BuildExpression(funcParameter);
            if (ExpressionForThisRule == null) return false;

            Debug.WriteLine($"{nameof(ExpressionForThisRule)} ready to compile:" +
                            $"{Environment.NewLine}{ExpressionDebugView()}");

            CompiledDelegate = Expression.Lambda<Func<TTarget, TResult>>(ExpressionForThisRule, funcParameter).Compile();
            return CompiledDelegate != null;
        }

        public TResult Execute(TTarget target)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            return CompiledDelegate(target);
        }
    }
}