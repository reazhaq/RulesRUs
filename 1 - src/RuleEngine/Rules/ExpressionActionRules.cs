using System;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    public class ExpressionActionRules<T> : Rule, IExpressionActionRule<T>
    {
        private Action<T> CompiledDelegate { get; set; }
        public Expression<Action<T>> RuleExpression { get; }

        public ExpressionActionRules(Expression<Action<T>> ruleExpression) => RuleExpression = ruleExpression;
        public override Expression BuildExpression(params ParameterExpression[] _) => RuleExpression;

        public override bool Compile()
        {
#if DEBUG
            RuleExpression.TraceNode();
#endif
            CompiledDelegate = RuleExpression.Compile();
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