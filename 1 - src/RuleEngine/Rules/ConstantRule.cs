using System;
using System.Diagnostics;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    // creates a typed lambda that takes no paramter and returns a fixed value
    public class ConstantRule<T> : Rule, IConstantRule<T>
    {
        private Func<T> CompiledDelegate { get; set; }
        public string Value { get; set; }

        public override Expression BuildExpression(ParameterExpression parameter)
        {
            var tType = typeof(T);
            if (string.IsNullOrEmpty(Value) || Value.Equals("null", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!tType.IsValueType || Nullable.GetUnderlyingType(tType) != null)
                    return Expression.Constant(default(T), tType);

                throw new RuleEngineException($"{typeof(T)} is not nullable and [null and/or empty string] can't be assigned");
            }

            tType = Nullable.GetUnderlyingType(tType) ?? tType;
            return Expression.Constant(Convert.ChangeType(Value, tType));
        }

        public override bool Compile()
        {
            var parameter = Expression.Parameter(typeof(T));
            var expression = BuildExpression(parameter);
#if DEBUG
            Debug.WriteLine($"Expression for ConstantRule with value: {Value} is {expression}");
            expression.TraceNode();
#endif
            CompiledDelegate = Expression.Lambda<Func<T>>(Expression.Convert(expression, typeof(T))).Compile();
            return CompiledDelegate != null;
        }

        public T Execute()
        {
            if(CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            return CompiledDelegate();
        }
    }
}