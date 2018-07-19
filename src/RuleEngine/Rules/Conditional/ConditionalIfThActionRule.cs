using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace RuleEngine.Rules.Conditional
{
    // creates an if-then block with no-return value
    // executes the true rule - if conditional rule passes
    public class ConditionalIfThActionRule<T> : ConditionalRuleBase, IConditionalActionRule<T>
    {
        private Action<T> CompiledDelegate { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T)}");

            var conditionalExpression = ConditionRule.BuildExpression(parameters);
            if (!(conditionalExpression is LambdaExpression))
                conditionalExpression = Expression.Lambda(conditionalExpression, parameters);

            var trueExpression = TrueRule.BuildExpression(parameters);
            if (!(trueExpression is LambdaExpression))
                trueExpression = Expression.Lambda(trueExpression, parameters);

            ExpressionForThisRule = Expression.IfThen(
                                    Expression.Invoke(conditionalExpression, parameters.Cast<Expression>()),
                                    Expression.Invoke(trueExpression, parameters.Cast<Expression>()));
            return ExpressionForThisRule;
        }

        public override bool Compile()
        {
            var parameter = Expression.Parameter(typeof(T));
            ExpressionForThisRule = BuildExpression(parameter);

            Debug.WriteLine($"Expression for ConditionalIfThActionRule: {Environment.NewLine}" +
                            $"{ExpressionDebugView()}");

            CompiledDelegate = Expression.Lambda<Action<T>>(ExpressionForThisRule, parameter).Compile();
            return CompiledDelegate != null;
        }

        public void Execute(T param)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            CompiledDelegate(param);
        }
    }
}
