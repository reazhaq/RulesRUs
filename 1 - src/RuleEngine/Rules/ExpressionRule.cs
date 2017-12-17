using System;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces;

namespace RuleEngine.Rules
{
    public class ExpressionRule<T> : Rule, IExpressionRule<T>
    {
        private readonly Expression<Func<T>> _ruleExpression;
        private Func<T> CompiledDelegate { get; set; }

        public ExpressionRule(Expression<Func<T>> ruleExpression) => _ruleExpression = ruleExpression;
        public override Expression BuildExpression(ParameterExpression parameter) => _ruleExpression;
        public override bool Compile()
        {
            CompiledDelegate = _ruleExpression.Compile();
            return CompiledDelegate != null;
        }

        public T Execute()
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            return CompiledDelegate();
        }
    }

    public class ExpressionRule<T1, T2> : Rule, IExpressionRule<T1, T2>
    {
        private Func<T1, T2> CompiledDelegate { get; set; }
        private readonly Expression<Func<T1, T2>> _ruleExpression;

        public ExpressionRule(Expression<Func<T1, T2>> ruleExpression) => _ruleExpression = ruleExpression;
        public override Expression BuildExpression(ParameterExpression parameter) => _ruleExpression;
        public override bool Compile()
        {
            CompiledDelegate = _ruleExpression.Compile();
            return CompiledDelegate != null;
        }

        public T2 Execute(T1 parameter)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            return CompiledDelegate(parameter);
        }
    }

    public class ExpressionRule<T1, T2, T3> : Rule, IExpressionRule<T1,T2,T3>
    {
        private Func<T1, T2, T3> CompiledDelegate { get; set; }
        private readonly Expression<Func<T1, T2, T3>> _ruleExpression;

        public ExpressionRule(Expression<Func<T1, T2, T3>> ruleExpression) => _ruleExpression = ruleExpression;
        public override Expression BuildExpression(ParameterExpression parameter) => _ruleExpression;
        public override bool Compile()
        {
            CompiledDelegate = _ruleExpression.Compile();
            return CompiledDelegate != null;
        }

        public T3 Execute(T1 parameter1, T2 parameter2)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            return CompiledDelegate(parameter1, parameter2);
        }
    }
}