using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RuleEngine.Interfaces;
using RuleEngine.RuleCompilers;

namespace RuleEngine.Rules
{
    public class ValidationRule<TTarget> : Rule, IValidationRule<TTarget>
    {
        private Func<TTarget, bool> CompiledDelegate { get; set; }
        private static readonly IValidationRuleCompiler<TTarget> ValidationRuleCompiler = new ValidationRuleCompiler<TTarget>();

        public string RelationBetweenChildrenRules { get; set; }
        public IList<ValidationRule<TTarget>> ChildrenRules { get; } = new List<ValidationRule<TTarget>>();
        public IList<object> Inputs { get; } = new List<object>();

        public override Expression BuildExpression(ParameterExpression funcParameter)
        {
            throw new NotImplementedException();
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