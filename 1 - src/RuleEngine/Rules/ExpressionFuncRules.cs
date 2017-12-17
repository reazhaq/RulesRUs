using System;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    public class ExpressionFuncRules<T> : Rule, IExpressionFuncRule<T>
    {
        private Func<T> CompiledDelegate { get; set; }
        public Expression<Func<T>> RuleExpression { get; }

        public ExpressionFuncRules(Expression<Func<T>> ruleExpression) => RuleExpression = ruleExpression;
        public override Expression BuildExpression(params ParameterExpression[] _) => RuleExpression;

        public override bool Compile()
        {
#if DEBUG
            RuleExpression.TraceNode();
#endif
            CompiledDelegate = RuleExpression.Compile();
            return CompiledDelegate != null;
        }

        public T Execute()
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            return CompiledDelegate();
        }
    }

    public class ExpressionFuncRules<T1, T2> : Rule, IExpressionFuncRule<T1, T2>
    {
        private Func<T1, T2> CompiledDelegate { get; set; }
        public Expression<Func<T1, T2>> RuleExpression { get; }

        public ExpressionFuncRules(Expression<Func<T1, T2>> ruleExpression) => RuleExpression = ruleExpression;
        public override Expression BuildExpression(params ParameterExpression[] _) => RuleExpression;

        public override bool Compile()
        {
            CompiledDelegate = RuleExpression.Compile();
            return CompiledDelegate != null;
        }

        public T2 Execute(T1 parameter)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            return CompiledDelegate(parameter);
        }
    }

    public class ExpressionFuncRules<T1, T2, T3> : Rule, IExpressionFuncRule<T1,T2,T3>
    {
        private Func<T1, T2, T3> CompiledDelegate { get; set; }
        public Expression<Func<T1, T2, T3>> RuleExpression { get; }

        public ExpressionFuncRules(Expression<Func<T1, T2, T3>> ruleExpression) => RuleExpression = ruleExpression;
        public override Expression BuildExpression(params ParameterExpression[] _) => RuleExpression;

        public override bool Compile()
        {
            CompiledDelegate = RuleExpression.Compile();
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