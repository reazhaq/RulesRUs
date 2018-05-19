using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Rules;

namespace RuleEngine.Rules
{
    public class RegExRule<T> : Rule, IRegExRule<T>
    {
        private Func<T, bool> CompiledDelegate { get; set; }

        public string RegExToUse;
        public string ObjectToValidate { get; set; }

        public override Expression BuildExpression(params ParameterExpression[] parameters)
        {
            if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T))
                throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T)}");

            var param = parameters[0];
            ExpressionForThisRule = GetExpressionWithSubPropertyForIsMatch(param);
            return ExpressionForThisRule;
        }

        public override bool Compile()
        {
            var funcParameter = Expression.Parameter(typeof(T));
            ExpressionForThisRule = BuildExpression(funcParameter);
            if (ExpressionForThisRule == null) return false;

            Debug.WriteLine($"{nameof(ExpressionForThisRule)}:" +
                            $"{Environment.NewLine}{ExpressionDebugView()}");

            CompiledDelegate = Expression.Lambda<Func<T, bool>>(ExpressionForThisRule, funcParameter).Compile();
            return CompiledDelegate != null;
        }

        public bool IsMatch(T targetObject)
        {
            if (CompiledDelegate == null)
                throw new RuleEngineException("A Rule must be compiled first");

            return CompiledDelegate(targetObject);
        }

        private Expression GetExpressionWithSubPropertyForIsMatch(ParameterExpression parameterExpression)
        {
            var fieldOrProperty = GetExpressionWithSubProperty(parameterExpression, ObjectToValidate);
            var isMatchParameters = new[] {typeof(string), typeof(string), typeof(RegexOptions)};
            var isMatchMethod = typeof(Regex).GetMethod("IsMatch", isMatchParameters);

            return Expression.Call(isMatchMethod, fieldOrProperty,
                Expression.Constant(RegExToUse, typeof(string)),
                Expression.Constant(RegexOptions.IgnoreCase, typeof(RegexOptions)));
        }
    }
}