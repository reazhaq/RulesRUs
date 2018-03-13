using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    public class MethodCallBase : Rule
    {
        public string MethodToCall;
        // MethodClassName needed for extension methods...
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

        public override void WriteRuleValuesToDictionary(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return;
            base.WriteRuleValuesToDictionary(propValueDictionary);

            if (!string.IsNullOrEmpty(MethodToCall))
                propValueDictionary.Add(nameof(MethodToCall), MethodToCall);
            if (!string.IsNullOrEmpty(MethodClassName))
                propValueDictionary.Add(nameof(MethodClassName), MethodClassName);
            if (!string.IsNullOrEmpty(ObjectToCallMethodOn))
                propValueDictionary.Add(nameof(ObjectToCallMethodOn), ObjectToCallMethodOn);

            var inputs = new List<object>(Inputs.Capacity);
            foreach (var input in Inputs)
            {
                if (input is Rule rule)
                {
                    var ruleDictionary = new Dictionary<string, object>();
                    rule.WriteRuleValuesToDictionary(ruleDictionary);
                    inputs.Add(ruleDictionary);
                }
                else
                {
                    inputs.Add(input);
                }
            }
            propValueDictionary.Add(nameof(Inputs), inputs);
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

        public override void WriteRuleValuesToDictionary(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return;
            base.WriteRuleValuesToDictionary(propValueDictionary);
            propValueDictionary.Add("RuleType", "MethodVoidCallRule");
            propValueDictionary.Add("BoundingTypes", new List<string> { typeof(T).ToString() });
        }
    }

    public class MethodCallRule<T1, T2> : MethodCallBase, IMethodCallRule<T1, T2>
    {
        private Func<T1, T2> CompiledDelegate { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T1))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T1)}");

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
            var funcParameter = Expression.Parameter(typeof(T1));
            ExpressionForThisRule = BuildExpression(funcParameter);
            if (ExpressionForThisRule == null) return false;

            Debug.WriteLine($"{nameof(ExpressionForThisRule)} ready to compile:" +
                            $"{Environment.NewLine}{ExpressionDebugView()}");

            CompiledDelegate = Expression.Lambda<Func<T1, T2>>(ExpressionForThisRule, funcParameter).Compile();
            return CompiledDelegate != null;
        }

        public T2 Execute(T1 target)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            return CompiledDelegate(target);
        }

        public override void WriteRuleValuesToDictionary(IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return;
            base.WriteRuleValuesToDictionary(propValueDictionary);
            propValueDictionary.Add("RuleType", "MethodCallRule");
            propValueDictionary.Add("BoundingTypes", new List<string> { typeof(T1).ToString(), typeof(T2).ToString() });
        }
    }
}