using System.Linq.Expressions;

namespace RuleEngine.RuleCompilers
{
    public class RuleCompilerBase
    {
        protected readonly ExpressionType[] LogicalOperatorsToBindChildrenRules =
        {
            ExpressionType.Not,
            //ExpressionType.And, ExpressionType.Or,
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

        protected virtual Expression GetExpressionWithSubProperty(ParameterExpression param, string objectToValidate)
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