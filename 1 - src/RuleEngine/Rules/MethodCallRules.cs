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
    public class MethodCallBase : Rule
    {
        public string MethodToCall;
        public string MethodClassName { get; set; }
        public string ObjectToCallMethodOn { get; set; }
        public List<object> Inputs { get; } = new List<object>();

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            throw new NotImplementedException();
        }

        public override bool Compile()
        {
            throw new NotImplementedException();
        }
    }

    public class MethodVoidCallRule<T> : MethodCallBase, IMethodVoidCallRule<T>
    {
        private Action<T> CompiledDelegate { get; set; }
        private static readonly IMethodVoidCallRuleCompiler<T> MethodVoidCallRuleCompiler = new MethodVoidCallRuleCompiler<T>();

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T)}");

            var expression = MethodVoidCallRuleCompiler.BuildExpression(parameters[0], this);
            Debug.WriteLine($"  {nameof(expression)}: {expression}");
            return expression;
        }

        public override bool Compile()
        {
            CompiledDelegate = MethodVoidCallRuleCompiler.CompileRule(this);
            return CompiledDelegate != null;
        }

        public void Execute(T param)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            CompiledDelegate(param);
        }
    }

    public class MethodCallRule<TTarget, TResult> : MethodCallBase, IMethodCallRule<TTarget, TResult>
    {
        private Func<TTarget, TResult> CompiledDelegate { get; set; }
        private static readonly IMethodCallRuleCompiler<TTarget, TResult> MethodCallRuleCompiler = new MethodCallRuleCompiler<TTarget, TResult>();

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(TTarget))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(TTarget)}");

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
                throw new RuleEngineException("A Rule must be compiled first");

            return CompiledDelegate(target);
        }
    }
}