using System;
using System.Linq.Expressions;
using RuleEngine.Rules;

namespace RuleEngine.Interfaces
{
    public interface IValidationRuleCompiler<T>
    {
        Expression BuildExpression(ParameterExpression param, ValidationRule<T> validationRuleToBuildExpression);
        Func<T, bool> CompileRule(ValidationRule<T> validationRuleToCompile);
    }

    public interface IValidationRuleCompiler<T1, T2>
    {
        Expression BuildExpression(ParameterExpression param1, ParameterExpression param2, ValidationRule<T1, T2> validationRuleToBuildExpression);
        Func<T1, T2, bool> CompileRule(ValidationRule<T1, T2> validationRuleToCompile);
    }
}