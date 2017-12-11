using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces;
using RuleEngine.Rules;

namespace RuleEngine.RuleCompilers
{
    public class ValidationRuleCompiler<TTarget> : RuleCompilerBase, IValidationRuleCompiler<TTarget>
    {
        public Expression BuildExpression(ParameterExpression rootParameterExpression, ValidationRule<TTarget> validationRuleToBuildExpression)
        {
            if (!validationRuleToBuildExpression.ChildrenRules.Any())
            {
                if (validationRuleToBuildExpression.ObjectToValidate == null)
                    return rootParameterExpression;

                if (validationRuleToBuildExpression.ObjectToValidate.Contains("."))
                {
                    var partsAndPieces = validationRuleToBuildExpression.ObjectToValidate.Split('.');
                    Expression body = rootParameterExpression;
                    foreach (var partsAndPiece in partsAndPieces)
                    {
                        body = Expression.PropertyOrField(body, partsAndPiece);
                    }
                    return body;
                }

                return Expression.PropertyOrField(rootParameterExpression, validationRuleToBuildExpression.ObjectToValidate);
            }

            IList<Expression> childrenExpressions = new List<Expression>();
            foreach (var childrenRule in validationRuleToBuildExpression.ChildrenRules)
            {
                childrenExpressions.Add(childrenRule.BuildExpression(rootParameterExpression));
            }

            throw new NotImplementedException();
        }

        public Func<TTarget, bool> CompileRule(ValidationRule<TTarget> validationRuleToCompile)
        {
            var funcParameter = Expression.Parameter(typeof(TTarget));

            var targetValueParam = Expression.Parameter(typeof(Rule));
            var targetValueExpression = validationRuleToCompile.ValueToValidateAgainst?.BuildExpression(targetValueParam);

            if (Enum.TryParse(validationRuleToCompile.OperatorToUse, out ExpressionType operatorToUse))
            {
                //Expression leftExpression = null;
                //if (LogicalOperatorsToUseAtTheRuleLevel.Contains(operatorToUse))
                //{
                //    leftExpression = BuildExpression(funcParameter, validationRuleToCompile);
                //}
                //else if (LogicalOperatorsToBindChildrenRules.Contains(operatorToUse) && validationRuleToCompile.ChildrenRules.Any())
                //{
                //    leftExpression = BuildExpression(funcParameter, validationRuleToCompile);
                //}

                //if(leftExpression == null) throw new RuleEngineException("validation rule with non-supported opreator to use");

                var leftExpression = BuildExpression(funcParameter, validationRuleToCompile);
                var binaryExpressionBody = Expression.MakeBinary(operatorToUse, leftExpression, targetValueExpression);
                Debug.WriteLine($"validation expression = {binaryExpressionBody}");

                return Expression.Lambda<Func<TTarget, bool>>(binaryExpressionBody, funcParameter).Compile();
            }

            throw new NotImplementedException();
        }
    }
}