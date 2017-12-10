using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RuleEngine.Interfaces;
using RuleEngine.RuleCompilers;

namespace RuleEngine.Rules
{
    public class ValidationRule<TTarget, TTargetValue> : Rule, IValidationRule<TTarget>
    {
        private Func<TTarget, bool> CompiledDelegate { get; set; }
        private static readonly IValidationRuleCompiler<TTarget, TTargetValue> ValidationRuleCompiler = new ValidationRuleCompiler<TTarget, TTargetValue>();

        public string RelationBetweenChildrenRules { get; set; }
        public IList<Rule> ChildrenRules { get; } = new List<Rule>();
        public IList<object> Inputs { get; } = new List<object>();

        public ConstantRule<TTargetValue> ValueToValidateAgainst;
        public string OperatorToUse;

        public override Expression BuildExpression(ParameterExpression funcParameter)
        {
            return ValidationRuleCompiler.BuildExpression(funcParameter);
        }

        public override bool Compile()
        {
            CompiledDelegate = ValidationRuleCompiler.CompileRule(this);
            return CompiledDelegate != null;
        }

        public bool Execute(TTarget targetObject)
        {
            if (CompiledDelegate == null)
                throw new Exception("A Rule must be compiled first");

            return CompiledDelegate(targetObject);
        }
    }
}