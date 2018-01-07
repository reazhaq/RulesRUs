using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    // executes a rule if true or executes another rule if false
    public class ConditionalRule<T> :Rule, IConditionalRule<T>
    {
        private Action<T> CompiledDelegate { get; set; }

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

        public void Execute(T param)
        {
            throw new NotImplementedException();
        }
    }

    // returns a value if true or returns another value if false
    public class ConditionalRule<T1, T2> : Rule, IConditionalRule<T1, T2>
    {
        private Func<T1, T2> CompiledDelegate { get; set; }

        public Rule ConditionRule;
        public Rule TrueRule;
        public Rule FalseRule;

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
                                        Expression.Return(returnLabel, trueExpression),
                                        Expression.Return(returnLabel, falseExpression)
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