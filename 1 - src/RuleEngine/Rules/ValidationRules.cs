using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using RuleEngine.Interfaces;
using RuleEngine.RuleCompilers;

namespace RuleEngine.Rules
{
    public class ValidationRule<T> : Rule, IValidationRule<T>
    {
        private Func<T, bool> CompiledDelegate { get; set; }
        private static readonly IValidationRuleCompiler<T> ValidationRuleCompiler = new ValidationRuleCompiler<T>();

        public Rule ValueToValidateAgainst;
        public string OperatorToUse;
        public string ObjectToValidate { get; set; }

        public string RelationBetweenChildrenRules { get; set; }
        public IList<Rule> ChildrenRules { get; } = new List<Rule>();
        public IList<object> Inputs { get; } = new List<object>();

        public override Expression BuildExpression(ParameterExpression funcParameter)
        {
            var expression = ValidationRuleCompiler.BuildExpression(funcParameter, this);
            Debug.WriteLine(expression);
            return expression;
        }

        public override bool Compile()
        {
            CompiledDelegate = ValidationRuleCompiler.CompileRule(this);
            return CompiledDelegate != null;
        }

        public bool Execute(T targetObject)
        {
            if (CompiledDelegate == null)
                throw new Exception("A Rule must be compiled first");

            return CompiledDelegate(targetObject);
        }
    }

    public class ValidationRule<T1, T2> : Rule, IValidationRule<T1, T2>
    {
        public override Expression BuildExpression(ParameterExpression parameter)
        {
            throw new NotImplementedException();
        }

        public override bool Compile()
        {
            throw new NotImplementedException();
        }

        public bool Execute(T1 param1, ITypeInfo2 param2)
        {
            throw new NotImplementedException();
        }
    }
}