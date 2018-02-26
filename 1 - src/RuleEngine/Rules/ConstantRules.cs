using System;
using System.Diagnostics;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    public class ConstantRuleBase : Rule
    {
        public virtual string Value { get; set; }

        protected ConstantExpression GetUnderlyingTypedValue(Type tType)
        {
            tType = Nullable.GetUnderlyingType(tType) ?? tType;
            var valueToConvert = tType.IsEnum ? Enum.Parse(tType, Value) : Value;
            return Expression.Constant(Convert.ChangeType(valueToConvert, tType));
        }

        protected ConstantExpression GetNullValueExpression<T>(Type tType)
        {
            if (!tType.IsValueType || Nullable.GetUnderlyingType(tType) != null)
                return Expression.Constant(default(T), tType);

            throw new RuleEngineException($"{typeof(T)} is not nullable and [null and/or empty string] can't be assigned");
        }

        public override Expression BuildExpression(params ParameterExpression[] parameters) => throw new NotImplementedException();
        public override bool Compile() => throw new NotImplementedException();
    }
    // creates a typed lambda that takes no paramter and returns a fixed value
    public class ConstantRule<T> : ConstantRuleBase, IConstantRule<T>
    {
        private Func<T> CompiledDelegate { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] _)
        {
            var tType = typeof(T);
            if (string.IsNullOrEmpty(Value) || Value.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                return GetNullValueExpression<T>(tType);

            return Expression.Convert(GetUnderlyingTypedValue(tType), typeof(T));
        }

        public override bool Compile()
        {
            var expressionBoday = BuildExpression(null);
#if DEBUG
            Debug.WriteLine($"constantExpressionBody for Func<{typeof(T)}>: " +
                            $"with Value: {Value} is{Environment.NewLine}{expressionBoday}");
            expressionBoday.TraceNode();
#endif
            CompiledDelegate = Expression.Lambda<Func<T>>(expressionBoday).Compile();
            return CompiledDelegate != null;
        }

        public T Get()
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            return CompiledDelegate();
        }
    }

    public class ConstantRule<T1, T2> : ConstantRuleBase, IConstantRule<T1, T2>
    {
        private Func<T1, T2> CompiledDelegate { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T1))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T1)}");

            var tType = typeof(T2);
            if (string.IsNullOrEmpty(Value) || Value.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                return GetNullValueExpression<T2>(tType);

            return Expression.Convert(GetUnderlyingTypedValue(tType), typeof(T2));
        }

        public override bool Compile()
        {
            var parameter = Expression.Parameter(typeof(T1));
            var expressionBody = BuildExpression(parameter);
#if DEBUG
            Debug.WriteLine($"constantExpressionBody for Func<{typeof(T1)},{typeof(T2)}>: " +
                            $"with Value: {Value} is{Environment.NewLine}{expressionBody}");
            expressionBody.TraceNode();
#endif
            CompiledDelegate = Expression.Lambda<Func<T1, T2>>(expressionBody, parameter).Compile();
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