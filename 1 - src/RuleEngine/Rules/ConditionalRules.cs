using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    public class ConditionalRuleBase : Rule
    {
        public Rule ConditionRule;
        public Rule TrueRule;
        public Rule FalseRule;

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            throw new NotImplementedException();
        }

        public override bool Compile()
        {
            throw new NotImplementedException();
        }
    }

    // executes a rule if true or executes another rule if false
    public class ConditionalActionRule<T> : ConditionalRuleBase, IConditionalActionRule<T>
    {
        private Action<T> CompiledDelegate { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T)}");

            var conditionalExpression = ConditionRule.BuildExpression(parameters);
            var trueExpression = TrueRule.BuildExpression(parameters);
            var falseExpression = FalseRule.BuildExpression(parameters);
#if DEBUG
            Debug.WriteLine($"trueExpression: {trueExpression}");
            trueExpression.TraceNode();
            Debug.WriteLine($"falseExpression: {falseExpression}");
            falseExpression.TraceNode();
            Debug.WriteLine($"conditionalExpression: {conditionalExpression}");
            conditionalExpression.TraceNode();
#endif

            return Expression.Condition(Expression.Invoke(conditionalExpression, parameters.Cast<Expression>()),
                                        Expression.Invoke(trueExpression, parameters.Cast<Expression>()),
                                        Expression.Invoke(falseExpression, parameters.Cast<Expression>()));
        }

        public override bool Compile()
        {
            var parameter = Expression.Parameter(typeof(T));
            var expression = BuildExpression(parameter);
#if DEBUG
            Debug.WriteLine($"Expression for ConditionalRule: {expression}");
            expression.TraceNode();
#endif
            CompiledDelegate = Expression.Lambda<Action<T>>(expression, parameter).Compile();
            return CompiledDelegate != null;
        }

        public void Execute(T param)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            CompiledDelegate(param);
        }
    }

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
            var falseExpression = FalseRule.BuildExpression(parameters);
            var ifThenElseExpression = Expression.IfThenElse(
                                        Expression.Invoke(conditionalExpression, parameters.Cast<Expression>()),
                                        Expression.Return(returnLabel, Expression.Invoke(trueExpression, parameters.Cast<Expression>())),
                                        Expression.Return(returnLabel, Expression.Invoke(falseExpression, parameters.Cast<Expression>()))
            );
#if DEBUG
            Debug.WriteLine($"trueExpression: {trueExpression}");
            trueExpression.TraceNode();
            Debug.WriteLine($"falseExpression: {falseExpression}");
            falseExpression.TraceNode();
            Debug.WriteLine($"conditionalExpression: {conditionalExpression}");
            conditionalExpression.TraceNode();
            Debug.WriteLine($"ifThenElseExpression: {ifThenElseExpression}");
            ifThenElseExpression.TraceNode();
#endif

            return Expression.Block(ifThenElseExpression, Expression.Label(returnLabel, Expression.Constant(string.Empty)));
        }

        public override bool Compile()
        {
            var parameter = Expression.Parameter(typeof(T1));
            var expression = BuildExpression(parameter);
#if DEBUG
            Debug.WriteLine($"Expression for ConditionalRule: {expression}");
            expression.TraceNode();
#endif
            CompiledDelegate = Expression.Lambda<Func<T1, T2>>(expression, parameter).Compile();
            return CompiledDelegate != null;
        }

        public T2 Execute(T1 param1)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            return CompiledDelegate(param1);
        }
    }
}