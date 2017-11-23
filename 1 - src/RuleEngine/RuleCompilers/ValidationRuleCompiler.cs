using System;
using RuleEngine.Interfaces;
using RuleEngine.Rules;

namespace RuleEngine.RuleCompilers
{
    public class ValidationRuleCompiler<TTarget, TResult> : IValidationRuleCompiler<TTarget, TResult>
    {
        public Func<TTarget, TResult> CompileRule(ValidationRule<TTarget, TResult> validationRule)
        {
            throw new NotImplementedException();
        }
    }
}