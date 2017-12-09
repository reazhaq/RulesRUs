using System;
using RuleEngine.Rules;

namespace RuleEngine.Interfaces
{
    public interface IValidationRuleCompiler<TTarget>
    {
        Func<TTarget, bool> CompileRule(ValidationRule<TTarget> validationRule);
    }
}