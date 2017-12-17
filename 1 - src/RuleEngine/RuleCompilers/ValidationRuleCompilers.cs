using RuleEngine.Common;
using RuleEngine.Interfaces;
using RuleEngine.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using RuleEngine.Utils;

namespace RuleEngine.RuleCompilers
{
    public class ValidationRuleCompiler<T> : RuleCompilerBase, IValidationRuleCompiler<T>
    {
        public Expression BuildExpression(ParameterExpression rootParameterExpression, ValidationRule<T> validationRuleToBuildExpression)
        {
            if (!Enum.TryParse(validationRuleToBuildExpression.OperatorToUse, out ExpressionType operatorToUse) ||
                (!LogicalOperatorsToUseAtTheRuleLevel.Contains(operatorToUse) && !LogicalOperatorsToBindChildrenRules.Contains(operatorToUse))
            )
                throw new RuleEngineException("bad operator to use"); //todo: update message 

            var targetValueParam = Expression.Parameter(typeof(Rule));
            var targetValueExpression = validationRuleToBuildExpression.ValueToValidateAgainst?.BuildExpression(targetValueParam);

            if (!validationRuleToBuildExpression.ChildrenRules.Any())
            {
                // ObjectToValidate is where a rule defines what need to be validated
                // if nothing; means check the object against something like not null
                // if it has dots; means rule is checking against a sub-property or field
                // if it has some value but no dots; meaning we are looking for a property or field

                Expression leftExpression;
                if (validationRuleToBuildExpression.ObjectToValidate == null)
                    leftExpression = rootParameterExpression;

                else if (validationRuleToBuildExpression.ObjectToValidate.Contains("."))
                {
                    var partsAndPieces = validationRuleToBuildExpression.ObjectToValidate.Split('.');
                    Expression bodyWithSubProperty = rootParameterExpression;
                    foreach (var partsAndPiece in partsAndPieces)
                    {
                        bodyWithSubProperty = Expression.PropertyOrField(bodyWithSubProperty, partsAndPiece);
                    }
                    leftExpression = bodyWithSubProperty;
                }
                else
                    leftExpression = Expression.PropertyOrField(rootParameterExpression, validationRuleToBuildExpression.ObjectToValidate);

                var binaryExpressionBody = Expression.MakeBinary(operatorToUse, leftExpression, targetValueExpression);
                Debug.WriteLine($"validation expression = {binaryExpressionBody}");
                return binaryExpressionBody;
            }

            IList<Expression> childrenExpressions = new List<Expression>();
            foreach (var childrenRule in validationRuleToBuildExpression.ChildrenRules)
            {
                childrenExpressions.Add(childrenRule.BuildExpression(rootParameterExpression));
            }

            Expression bodyExpression = null;
            if (childrenExpressions.Any())
            {
                switch (operatorToUse)
                {
                    case ExpressionType.Not:
                        bodyExpression = Expression.Not(childrenExpressions[0]);
                        break;
                    case ExpressionType.AndAlso:
                        bodyExpression = Expression.AndAlso(childrenExpressions[0], childrenExpressions[1]);
                        for (var index = 2; index < childrenExpressions.Count; index++)
                            bodyExpression = Expression.AndAlso(bodyExpression, childrenExpressions[index]);
                        break;
                    default:
                        bodyExpression = Expression.OrElse(childrenExpressions[0], childrenExpressions[1]);
                        for (var index = 2; index < childrenExpressions.Count; index++)
                            bodyExpression = Expression.OrElse(bodyExpression, childrenExpressions[index]);
                        break;
                }
            }

            Debug.WriteLine($"validation expression = {bodyExpression}");
            return bodyExpression;
        }

        public Func<T, bool> CompileRule(ValidationRule<T> validationRuleToCompile)
        {
            var funcParameter = Expression.Parameter(typeof(T));
            var binaryExpressionBody = BuildExpression(funcParameter, validationRuleToCompile);
#if DEBUG
            (binaryExpressionBody as BinaryExpression)?.TraceNode();
#endif
            return Expression.Lambda<Func<T, bool>>(binaryExpressionBody, funcParameter).Compile();
        }
    }

    public class ValidationRuleCompiler<T1, T2> : RuleCompilerBase, IValidationRuleCompiler<T1, T2>
    {
        public Expression BuildExpression(ParameterExpression param1, ParameterExpression param2, ValidationRule<T1, T2> validationRuleToBuildExpression)
        {
            throw new NotImplementedException();
        }

        public Func<T1, T2, bool> CompileRule(ValidationRule<T1, T2> validationRuleToCompile)
        {
            throw new NotImplementedException();
        }
    }
}