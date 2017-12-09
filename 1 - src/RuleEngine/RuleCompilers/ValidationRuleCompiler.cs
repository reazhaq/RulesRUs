using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RuleEngine.Interfaces;
using RuleEngine.Rules;

namespace RuleEngine.RuleCompilers
{
    public class ValidationRuleCompiler<TTarget> : IValidationRuleCompiler<TTarget>
    {
        public Func<TTarget, bool> CompileRule(ValidationRule<TTarget> validationRule)
        {
            var funcParameter = Expression.Parameter(typeof(TTarget), "rootArgument");
            if (validationRule.ChildrenRules.Any() && validationRule.ChildrenRules.Count >= 2)
            {
                var leftExpression = validationRule.ChildrenRules[0].BuildExpression(funcParameter);
                var rightExpression = validationRule.ChildrenRules[1].BuildExpression(funcParameter);
                var binaryExpression = Expression.AndAlso(leftExpression, rightExpression);
                return Expression.Lambda<Func<TTarget, bool>>(binaryExpression, funcParameter).Compile();

                //var newFuncMethod = new Func<Expression, Expression, Expression>(Expression.And);

                //var expression = newFuncMethod(leftExpression, rightExpression);
                //for (int index = 2; index < validationRule.ChildrenRules.Count; index++)
                //{
                //    expression = newFuncMethod(expression,
                //        validationRule.ChildrenRules[index].BuildExpression(funcParameter));
                //}
            }

            throw new NotImplementedException();
        }
    }
}