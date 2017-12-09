using System;
using System.Diagnostics;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces;

namespace RuleEngine.Rules
{
    public class ConstantRule<T> : Rule, IConstantRule<T>
    {
        private Func<T> CompiledDelegate { get; set; }
        public string Value { get; set; }

        public override Expression BuildExpression(ParameterExpression parameter)
        {
            if (string.IsNullOrEmpty(Value) || Value.Equals("null", StringComparison.InvariantCultureIgnoreCase))
            {
                if (Nullable.GetUnderlyingType(typeof(T)) != null)
                    return Expression.Constant(null, typeof(T));

                throw new RuleEngineException($"{typeof(T)} is not nullable and null and/or empty string can't be assigned");
            }

            return Expression.Constant(Convert.ChangeType(Value, typeof(T)));
        }

        public override bool Compile()
        {
            var parameter = Expression.Parameter(typeof(T));
            var expression = BuildExpression(parameter);
            Debug.WriteLine($"Expressiong for ConstantRule with value:{Value} is {expression}");

            CompiledDelegate = Expression.Lambda<Func<T>>(expression).Compile();
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