using System;
using System.Linq.Expressions;
using RuleEngine.Rules;

namespace RuleEngine.Interfaces
{
    public interface IValidationRuleCompiler<TTarget>
    {
        Expression BuildExpression(ParameterExpression funcParameter, ValidationRule<TTarget> validationRuleToBuildExpression);
        Func<TTarget, bool> CompileRule(ValidationRule<TTarget> validationRuleToCompile);
    }
}