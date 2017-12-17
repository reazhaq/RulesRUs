using System;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces;

namespace RuleEngine.Rules
{
    public class ExpressionActionRules<T> : Rule, IExpressionActionRules<T>
    {
        private Action<T> CompiledDelegate { get; set; }
        private readonly Expression<Action<T>> _ruleExpression;

        public ExpressionActionRules(Expression<Action<T>> ruleExpression) => _ruleExpression = ruleExpression;
        public override Expression BuildExpression(ParameterExpression parameter) => _ruleExpression;

        public override bool Compile()
        {
            CompiledDelegate = _ruleExpression.Compile();
            return CompiledDelegate != null;
        }

        public void Execute(T parameter)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            CompiledDelegate(parameter);
        }
    }
}