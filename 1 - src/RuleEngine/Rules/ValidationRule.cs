using System;
using System.Data;
using RuleEngine.Interfaces;
using RuleEngine.RuleCompilers;

namespace RuleEngine.Rules
{
    public class ValidationRule<TTarget, TResult> : Rule, IValidationRule<TTarget, TResult>
    {
        private Func<TTarget, TResult> CompiledDelegate { get; set; }
        private static readonly IValidationRuleCompiler<TTarget, TResult> ValidationRuleCompiler = new ValidationRuleCompiler<TTarget, TResult>();

        public override bool Compile()
        {
            CompiledDelegate = ValidationRuleCompiler.CompileRule(this);
            return CompiledDelegate != null;
        }

        public TResult Execute(TTarget targetObject)
        {
            if (CompiledDelegate == null)
                throw new Exception("A Rule must be compiled first");

            return CompiledDelegate(targetObject);
        }
    }
}