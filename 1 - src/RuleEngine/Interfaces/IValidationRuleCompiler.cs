using System;
using System.Linq.Expressions;
using RuleEngine.Rules;

namespace RuleEngine.Interfaces
{
    public interface IValidationRuleCompiler<TTarget, TTargetValue>
    {
        Expression BuildExpression(ParameterExpression funcParameter);
        Func<TTarget, bool> CompileRule(ValidationRule<TTarget, TTargetValue> validationRule);
    }
}