using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using RuleEngine.Common;
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

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if(parameters == null || parameters.Length!=1)
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter");

            var expression = ValidationRuleCompiler.BuildExpression(parameters[0], this);
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
        private Func<T1, T2, bool> CompiledDelegate { get; set; }
        private static readonly IValidationRuleCompiler<T1, T2> ValidationRuleCompiler = new ValidationRuleCompiler<T1, T2>();

        public string OperatorToUse;
        public string ObjectToValidate1;
        public string ObjectToValidate2;

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 2)
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with two parameter");

            var expression = ValidationRuleCompiler.BuildExpression(parameters[0], parameters[1], this);
            Debug.WriteLine(expression);
            return expression;
        }

        public override bool Compile()
        {
            CompiledDelegate = ValidationRuleCompiler.CompileRule(this);
            return CompiledDelegate != null;
        }

        public bool Execute(T1 param1, T2 param2)
        {
            if (CompiledDelegate == null)
                throw new Exception("A Rule must be compiled first");

            return CompiledDelegate(param1, param2);
        }
    }
}