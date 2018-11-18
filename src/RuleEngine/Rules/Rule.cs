using System;
using System.Linq.Expressions;
using System.Text;
using RuleEngine.Utils;

namespace RuleEngine.Rules
{
    public abstract class Rule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // what does this rule do
        public string Description { get; set; }
        // in the event of a rule logic failure - send this error
        public RuleError RuleError { get; set; }
        // rule expression
        protected Expression ExpressionForThisRule { get; set; }

        public virtual string ExpressionDebugView()
        {
            var sb = new StringBuilder($"Expression: {ExpressionForThisRule}{Environment.NewLine}");
            sb.Append($"Expression Tree:{Environment.NewLine}");
            ExpressionForThisRule.TraceNode(sb);
            return sb.ToString();
        }

        public abstract Expression BuildExpression(params ParameterExpression[] parameters);
        public abstract bool Compile();

        protected readonly ExpressionType[] LogicalOperatorsToBindChildrenRules =
        {
            ExpressionType.Not,
            ExpressionType.AndAlso,ExpressionType.OrElse
        };

        protected readonly ExpressionType[] LogicalOperatorsToUseAtTheRuleLevel =
        {
            ExpressionType.NotEqual, ExpressionType.Equal,
            ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual,
            ExpressionType.LessThan, ExpressionType.LessThanOrEqual
        };

        public static Expression GetExpressionWithSubProperty(ParameterExpression param, string objectToValidate)
        {
            if (string.IsNullOrEmpty(objectToValidate))
                return param;

            var fieldsOrProperties = objectToValidate.Split('.');
            Expression bodyWithSubProperty = param;
            foreach (var fieldOrProperty in fieldsOrProperties)
                bodyWithSubProperty = Expression.PropertyOrField(bodyWithSubProperty, fieldOrProperty);

            return bodyWithSubProperty;
        }

        public bool RuleReturnsValueOfTOut<TIn, TOut>()
        {
            switch (this)
            {
                case FuncBlockRule<TIn, TOut> funcBlockRule:
                case ConditionalFuncRule<TIn, TOut> conditionalFuncRule:
                case ConstantRule<TIn, TOut> constantRule:
                case MethodCallRule<TIn, TOut> methodCallRule:
                case ExpressionFuncRule<TIn, TOut> expressionFuncRule:
                    return true;
           }

            return RuleReturnsValueOfTOut<TOut>() || RuleReturnsBool<TIn, TOut>();
        }

        public bool RuleReturnsValueOfTOut<TOut>()
        {
            switch(this)
            {
                case ConstantRule<TOut> constantRule:
                case StaticMethodCallRule<TOut> staticMethodCallRule:
                    return true;
            }

            return false;
        }

        public bool RuleReturnsBool<TIn, TOut>()
        {
            switch (this)
            {
                case ContainsValueRule<TIn> containsValueRule when (typeof(TOut) == typeof(bool)):
                case RegExRule<TIn> regExRule when (typeof(TOut) == typeof(bool)):
                case SelfReturnRule<TIn> selfReturnRule when (typeof(TIn) == typeof(TOut)):
                case ValidationRule<TIn> validationRule when (typeof(TOut) == typeof(bool)):
                    return true;
            }

            return false;
        }
    }
}
