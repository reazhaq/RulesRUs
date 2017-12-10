using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using RuleEngine.Interfaces;
using RuleEngine.Rules;

namespace RuleEngine.RuleCompilers
{
    public class ValidationRuleCompiler<TTarget, TTargetValue> : RuleCompilerBase, IValidationRuleCompiler<TTarget, TTargetValue>
    {
        public Expression BuildExpression(ParameterExpression funcParameter)
        {
            return funcParameter;
        }

        public Func<TTarget, bool> CompileRule(ValidationRule<TTarget, TTargetValue> validationRule)
        {
            var funcParameter = Expression.Parameter(typeof(TTarget));

            var targetValueParam = Expression.Parameter(typeof(TTargetValue));
            var targetValueExpression = validationRule.ValueToValidateAgainst.BuildExpression(targetValueParam);

            if (Enum.TryParse(validationRule.OperatorToUse, out ExpressionType operatorToUse) &&
                LogicalOperatorsToUseAtTheRuleLevel.Contains(operatorToUse))
            {
                var leftExpression = funcParameter;
                var thisExpression = Expression.MakeBinary(operatorToUse, leftExpression, targetValueExpression);
                Debug.WriteLine($"validation expression = {thisExpression}");

                return Expression.Lambda<Func<TTarget, bool>>(thisExpression, funcParameter).Compile();
            }

            throw new NotImplementedException();
        }
    }
}