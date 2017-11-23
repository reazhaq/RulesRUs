using System;
using RuleEngine.Rules;

namespace RuleEngine.Interfaces
{
    public interface IValidationRuleCompiler<TTarget, TResult>
    {
        Func<TTarget, TResult> CompileRule(ValidationRule<TTarget, TResult> validationRule);
    }
}