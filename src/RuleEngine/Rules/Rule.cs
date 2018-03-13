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

        public virtual void WriteRuleValuesToDictionary(IDictionary<string, object> propValueDictionary)
        {
            if(propValueDictionary==null) return;
            propValueDictionary.Add("Id", Id);
            if (!string.IsNullOrEmpty(Name))
                propValueDictionary.Add("Name", Name);
            if (!string.IsNullOrEmpty(Description))
                propValueDictionary.Add("Description", Description);
            if (RuleError != null)
            {
                propValueDictionary.Add("RuleError",
                    new Dictionary<string, string>
                    {
                        {"Code", RuleError.Code},
                        {"Message", RuleError.Message}
                    });
            }

        }

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