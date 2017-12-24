using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Compilers;
using RuleEngine.Interfaces.Rules;
using RuleEngine.RuleCompilers;

namespace RuleEngine.Rules
{
    public class MethodCallRule<TTarget, TResult> : Rule, IMethodCallRule<TTarget, TResult>
    {
        private Func<TTarget,TResult> CompiledDelegate { get; set; }
        private static readonly IMethodCallRuleCompiler<TTarget,TResult> MethodCallRuleCompiler = new MethodCallRuleCompiler<TTarget, TResult>();

        public string OperatorToUse;
        public string ObjectToValidate { get; set; }
        public List<object> Inputs { get; } = new List<object>();

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1)
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter");

            var expression = MethodCallRuleCompiler.BuildExpression(parameters[0], this);
            Debug.WriteLine($"  {nameof(expression)}: {expression}");
            return expression;
        }

        public override bool Compile()
        {
            CompiledDelegate = MethodCallRuleCompiler.CompileRule(this);
            return CompiledDelegate != null;
        }

        public TResult Execute(TTarget target)
        {
            if (CompiledDelegate == null)
                throw new Exception("A Rule must be compiled first");

            return CompiledDelegate(target);
        }
    }
}