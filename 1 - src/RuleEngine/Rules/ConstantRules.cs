using System;
using System.Diagnostics;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    // creates a typed lambda that takes no paramter and returns a fixed value
    public class ConstantRule<T> : Rule, IConstantRule<T>
    {
        private Func<T> CompiledDelegate { get; set; }
        public string Value { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] _)
        {
            var tType = typeof(T);
            if (string.IsNullOrEmpty(Value) || Value.Equals("null", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!tType.IsValueType || Nullable.GetUnderlyingType(tType) != null)
                    return Expression.Constant(default(T), tType);

                throw new RuleEngineException($"{typeof(T)} is not nullable and [null and/or empty string] can't be assigned");
            }

            tType = Nullable.GetUnderlyingType(tType) ?? tType;
            var valueToConvert = tType.IsEnum ? Enum.Parse(tType, Value) : Value;
            return Expression.Constant(Convert.ChangeType(valueToConvert, tType));
        }

        public override bool Compile()
        {
            var expression = BuildExpression(null);
#if DEBUG
            Debug.WriteLine($"Expression for ConstantRule with value: {Value} is {expression}");
            expression.TraceNode();
#endif
            CompiledDelegate = Expression.Lambda<Func<T>>(Expression.Convert(expression, typeof(T))).Compile();
            return CompiledDelegate != null;
        }

        public T Get()
        {
            if(CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            return CompiledDelegate();
        }
    }
    
    public class ConstantRule<T1, T2> : Rule, IConstantRule<T1, T2>
    {
        private Func<T1, T2> CompiledDelegate { get; set; }
        public string Value { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T1))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T1)}");

            var tType = typeof(T2);
            if (string.IsNullOrEmpty(Value) || Value.Equals("null", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!tType.IsValueType || Nullable.GetUnderlyingType(tType) != null)
                    return Expression.Constant(default(T2), tType);

                throw new RuleEngineException($"{typeof(T2)} is not nullable and [null and/or empty string] can't be assigned");
            }

            tType = Nullable.GetUnderlyingType(tType) ?? tType;
            var valueToConvert = tType.IsEnum ? Enum.Parse(tType, Value) : Value;
            return Expression.Constant(Convert.ChangeType(valueToConvert, tType));
        }

        public override bool Compile()
        {
            var parameter = Expression.Parameter(typeof(T1));
            var expression = BuildExpression(parameter);
#if DEBUG
            Debug.WriteLine($"Expression for ConstantRule with value: {Value} is {expression}");
            expression.TraceNode();
#endif
            CompiledDelegate = Expression.Lambda<Func<T1,T2>>(expression, parameter).Compile();
            return CompiledDelegate != null;
        }

        public T2 Get(T1 param)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            return CompiledDelegate(param);
        }
    }
}