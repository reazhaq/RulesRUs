using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace RuleEngine.Rules
{
    public abstract class ConditionalRuleBase : Rule
    {
        public Rule ConditionRule;
        public Rule TrueRule;
        public Rule FalseRule;
    }

    // creates a if-then-else block that returns a value
    // take a param of type T1 and returns a value type of T2
    // returns a value if true or returns another value if false
    public class ConditionalFuncRule<T1, T2> : ConditionalRuleBase, IConditionalFuncRule<T1, T2>
    {
        private Func<T1, T2> CompiledDelegate { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T1))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T1)}");

            var returnLabel = Expression.Label(typeof(T2), "returnLable");

            var conditionalExpression = ConditionRule.BuildExpression(parameters);

            var trueExpression = TrueRule.BuildExpression(parameters);
            if (!(trueExpression is LambdaExpression))
                trueExpression = Expression.Lambda(trueExpression, parameters);

            var falseExpression = FalseRule.BuildExpression(parameters);
            if (!(falseExpression is LambdaExpression))
                falseExpression = Expression.Lambda(falseExpression, parameters);

            var ifThenElseExpression = Expression.IfThenElse(
                                        Expression.Invoke(conditionalExpression, parameters.Cast<Expression>()),
                                        Expression.Return(returnLabel, Expression.Invoke(trueExpression, parameters.Cast<Expression>())),
                                        Expression.Return(returnLabel, Expression.Invoke(falseExpression, parameters.Cast<Expression>()))
            );

            Debug.WriteLine($"ConditionRule:{Environment.NewLine}{ConditionRule.ExpressionDebugView()}");
            Debug.WriteLine($"TrueRule:{Environment.NewLine}{TrueRule.ExpressionDebugView()}");
            Debug.WriteLine($"FalseRule:{Environment.NewLine}{FalseRule.ExpressionDebugView()}");

            var defaultForReturnLabel = Expression.Convert(Expression.Constant(default(T2)), typeof(T2));
            ExpressionForThisRule = Expression.Block(ifThenElseExpression,
                                                    Expression.Label(returnLabel, defaultForReturnLabel));
            return ExpressionForThisRule;
        }

        public override bool Compile()
        {
            var parameter = Expression.Parameter(typeof(T1));
            ExpressionForThisRule = BuildExpression(parameter);

            Debug.WriteLine($"Expression for ConditionalIfThElFuncRule:" +
                            $"{Environment.NewLine}{ExpressionDebugView()}");

            CompiledDelegate = Expression.Lambda<Func<T1, T2>>(ExpressionForThisRule, parameter).Compile();
            return CompiledDelegate != null;
        }

        public T2 Execute(T1 param1)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            return CompiledDelegate(param1);
        }
    }

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
    // creates a if-then-else block with no-return
    // executes a rule if true or executes another rule if false
    public class ConditionalIfThElActionRule<T> : ConditionalRuleBase, IConditionalActionRule<T>
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

            var falseExpression = FalseRule.BuildExpression(parameters);
            if (!(falseExpression is LambdaExpression))
                falseExpression = Expression.Lambda(falseExpression, parameters);

            ExpressionForThisRule = Expression.Condition(Expression.Invoke(conditionalExpression, parameters.Cast<Expression>()),
                                    Expression.Invoke(trueExpression, parameters.Cast<Expression>()),
                                    Expression.Invoke(falseExpression, parameters.Cast<Expression>()));
            return ExpressionForThisRule;
        }

        public override bool Compile()
        {
            var parameter = Expression.Parameter(typeof(T));
            ExpressionForThisRule = BuildExpression(parameter);

            Debug.WriteLine($"Expression for ConditionalIfThElActionRule:{Environment.NewLine}" +
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