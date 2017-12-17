using System;
using System.Linq.Expressions;
using RuleEngine.Rules;

namespace RuleEngine.Interfaces
{
    public interface IValidationRuleCompiler<T>
    {
        Expression BuildExpression(ParameterExpression funcParameter, ValidationRule<T> validationRuleToBuildExpression);
        Func<T, bool> CompileRule(ValidationRule<T> validationRuleToCompile);
    }
}