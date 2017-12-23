using RuleEngine.Common;
using RuleEngine.Interfaces;
using RuleEngine.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using RuleEngine.Interfaces.Compilers;
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
                throw new RuleEngineException($"Bad {nameof(operatorToUse)} value {operatorToUse}"); //todo: update message 

            if (!validationRuleToBuildExpression.ChildrenRules.Any())
            {
                var targetValueParam = Expression.Parameter(typeof(Rule));
                var targetValueExpression = validationRuleToBuildExpression.ValueToValidateAgainst?.BuildExpression(targetValueParam);

                var leftExpression = GetExpressionWithSubProperty(rootParameterExpression, validationRuleToBuildExpression.ObjectToValidate);
                var binaryExpressionBody = Expression.MakeBinary(operatorToUse, leftExpression, targetValueExpression);
                Debug.WriteLine($"{nameof(binaryExpressionBody)}: {binaryExpressionBody}");
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
            binaryExpressionBody.TraceNode();
#endif
            return Expression.Lambda<Func<T, bool>>(binaryExpressionBody, funcParameter).Compile();
        }
    }

    public class ValidationRuleCompiler<T1, T2> : RuleCompilerBase, IValidationRuleCompiler<T1, T2>
    {
        public Expression BuildExpression(ParameterExpression param1, ParameterExpression param2, ValidationRule<T1, T2> validationRuleToBuildExpression)
        {
            if (!Enum.TryParse(validationRuleToBuildExpression.OperatorToUse, out ExpressionType operatorToUse) ||
                !LogicalOperatorsToUseAtTheRuleLevel.Contains(operatorToUse)
            )
                throw new RuleEngineException($"Bad {nameof(operatorToUse)} value {operatorToUse}"); //todo: update message 

            var expression1 = GetExpressionWithSubProperty(param1, validationRuleToBuildExpression.ObjectToValidate1);
            var expression2 = GetExpressionWithSubProperty(param2, validationRuleToBuildExpression.ObjectToValidate2);

            var binaryExpression = Expression.MakeBinary(operatorToUse, expression1, expression2);
            Debug.WriteLine($"{nameof(binaryExpression)}: {binaryExpression}");
            return binaryExpression;
        }

        public Func<T1, T2, bool> CompileRule(ValidationRule<T1, T2> validationRuleToCompile)
        {
            var param1 = Expression.Parameter(typeof(T1));
            var param2 = Expression.Parameter(typeof(T2));
            var expressionBody = BuildExpression(param1, param2, validationRuleToCompile);
#if DEBUG
            expressionBody.TraceNode();
#endif
            return Expression.Lambda<Func<T1, T2, bool>>(expressionBody, param1, param2).Compile();
        }
    }
}