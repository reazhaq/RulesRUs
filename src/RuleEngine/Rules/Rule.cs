using System;
using System.Collections.Generic;
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
        // rule expresion
        protected Expression ExpressionForThisRule { get; set; }

        public virtual string ExpressionDebugView()
        {
            var sb = new StringBuilder();
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

        protected readonly string[] RegularExpressionOperator =
        {
            "IsMatch"
        };

        public static Expression[] GetArgumentsExpressions(ParameterExpression param, List<Rule> paramList, Type[] paramTypes)
        {
            var argumentsExpressions = new Expression[paramList.Count];
            for (var index = 0; index < paramList.Count; index++)
            {
                var paramRule = paramList[index];
                argumentsExpressions[index] = paramRule.BuildExpression(param);
                paramTypes[index] = argumentsExpressions[index].Type;
            }
            return argumentsExpressions;
        }

        public static Expression GetExpressionWithSubProperty(ParameterExpression param, string objectToValidate)
        {
            if (string.IsNullOrEmpty(objectToValidate))
                return param;

            var partsAndPieces = objectToValidate.Split('.');
            Expression bodyWithSubProperty = param;
            foreach (var partsAndPiece in partsAndPieces)
                bodyWithSubProperty = Expression.PropertyOrField(bodyWithSubProperty, partsAndPiece);

            return bodyWithSubProperty;
        }
    }
}