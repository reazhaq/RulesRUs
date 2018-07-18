using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace RuleEngine.Rules.Block
{
    public class ActionBlockRule<T> : BlockRules, IActionBlockRule<T>
    {
        private Action<T> CompiledDelegate { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T)}");

            var param = parameters[0];
            foreach (var rule in Rules)
                Expressions.Add(rule.BuildExpression(param));

            ExpressionForThisRule = Expression.Block(new ParameterExpression[]{}, Expressions);
            return ExpressionForThisRule;
        }

        public override bool Compile()
        {
            var param = Expression.Parameter(typeof(T));
            ExpressionForThisRule = BuildExpression(param);
            if (ExpressionForThisRule == null) return false;

            Debug.WriteLine($"{ExpressionForThisRule} ready to compile:" +
                            $"{Environment.NewLine}{ExpressionDebugView()}");

            CompiledDelegate = Expression.Lambda<Action<T>>(ExpressionForThisRule, param).Compile();
            return CompiledDelegate != null;
        }

        public void Exectue(T param)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            CompiledDelegate(param);
        }
    }
}
