using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RuleEngine.Rules;

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

        public static Expression[] GetArgumentsExpressions(ParameterExpression param, List<object> inputs, Type[] inputTypes)
        {
            var argumentsExpressions = new Expression[inputs.Count];
            for (var index = 0; index < inputs.Count; index++)
            {
                var input = inputs[index];
                if (input is Rule)
                {
                    argumentsExpressions[index] = (input as Rule).BuildExpression(param);
                    inputTypes[index] = argumentsExpressions[index].Type;
                }
                else
                {
                    argumentsExpressions[index] = Expression.Constant(input);
                    inputTypes[index] = input.GetType();
                }
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