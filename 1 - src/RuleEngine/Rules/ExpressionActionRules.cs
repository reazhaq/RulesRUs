using System;
using System.Diagnostics;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    public class ExpressionActionRule<T> : Rule, IExpressionActionRule<T>
    {
        private Action<T> CompiledDelegate { get; set; }
        public Expression<Action<T>> RuleExpression { get; }

        public ExpressionActionRule(Expression<Action<T>> ruleExpression) => RuleExpression = ruleExpression;
        public override Expression BuildExpression(params ParameterExpression[] _) => RuleExpression;

        public override bool Compile()
        {
#if DEBUG
            Debug.WriteLine($"ExpressionActionRules<{typeof(T)}> RuleExpression: {RuleExpression}");
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

    public class ExpressionActionRule<T1, T2> : Rule, IExpressionActionRule<T1, T2>
    {
        private Action<T1, T2> CompiledDelegate { get; set; }
        public Expression<Action<T1, T2>> RuleExpression { get; }

        public ExpressionActionRule(Expression<Action<T1, T2>> ruleExpression) => RuleExpression = ruleExpression;
        public override Expression BuildExpression(params ParameterExpression[] _) => RuleExpression;

        public override bool Compile()
        {
#if DEBUG
            Debug.WriteLine($"ExpressionActionRules<{typeof(T1)}, {typeof(T2)}> RuleExpression: {RuleExpression}");
            RuleExpression.TraceNode();
#endif
            CompiledDelegate = RuleExpression.Compile();
            return CompiledDelegate != null;
        }

        public void Execute(T1 param1, T2 param2)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            CompiledDelegate(param1, param2);
        }
    }

    public class ExpressionActionRule<T1, T2, T3> : Rule, IExpressionActionRule<T1, T2, T3>
    {
        private Action<T1, T2, T3> CompiledDelegate { get; set; }
        public Expression<Action<T1, T2, T3>> RuleExpression { get; }

        public ExpressionActionRule(Expression<Action<T1, T2, T3>> ruleExpression) => RuleExpression = ruleExpression;
        public override Expression BuildExpression(params ParameterExpression[] _) => RuleExpression;

        public override bool Compile()
        {
#if DEBUG
            Debug.WriteLine($"ExpressionActionRules<{typeof(T1)}, {typeof(T2)}, {typeof(T3)}> RuleExpression: {RuleExpression}");
            RuleExpression.TraceNode();
#endif
            CompiledDelegate = RuleExpression.Compile();
            return CompiledDelegate != null;
        }

        public void Execute(T1 param1, T2 param2, T3 param3)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("Rule has to be compiled before it can be executed");

            CompiledDelegate(param1, param2, param3);
        }
    }
}