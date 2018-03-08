using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using RuleEngine.Common;
using RuleEngine.Interfaces.Compilers;
using RuleEngine.Rules;
using RuleEngine.Utils;

namespace RuleEngine.RuleCompilers
{
    public class RegExRuleCompiler<T>:RuleCompilerBase, IRegExRuleCompiler<T>
    {
        public Expression BuildExpression(ParameterExpression param, RegExRule<T> regExRuleToBuildExpression)
        {
            if(!RegularExpressionOperator.Contains(regExRuleToBuildExpression.OperatorToUse))
                throw new RuleEngineException($"Bad {regExRuleToBuildExpression.OperatorToUse} for RegExRule"); //todo: update message

            switch (regExRuleToBuildExpression.OperatorToUse)
            {
                case "IsMatch":
                    return GetExpressionWithSubPropertyForIsMatch(param, regExRuleToBuildExpression);
            }

            throw new NotImplementedException();
        }

        private Expression GetExpressionWithSubPropertyForIsMatch(ParameterExpression parameterExpression, RegExRule<T> regExRuleToBuildExpression)
        {
            var fieldOrProperty = GetExpressionWithSubProperty(parameterExpression, regExRuleToBuildExpression.ObjectToValidate);
            var isMatchMethod = typeof(Regex).GetMethod("IsMatch", new[] {typeof(string), typeof(string), typeof(RegexOptions)});

            return Expression.Call(isMatchMethod, fieldOrProperty,
                Expression.Constant(regExRuleToBuildExpression.RegExToUse, typeof(string)),
                Expression.Constant(RegexOptions.IgnoreCase, typeof(RegexOptions)));
        }

        public Func<T, bool> CompileRule(RegExRule<T> regExRuleToCompile)
        {
            var funcParameter = Expression.Parameter(typeof(T));
            var binaryExpressionBody = BuildExpression(funcParameter, regExRuleToCompile);
            if (binaryExpressionBody == null) return null;
#if DEBUG
            Debug.WriteLine($"{nameof(binaryExpressionBody)}: {binaryExpressionBody}");
            var sb = new StringBuilder();
            binaryExpressionBody.TraceNode(sb);
            Debug.WriteLine(sb);
#endif
            return Expression.Lambda<Func<T, bool>>(binaryExpressionBody, funcParameter).Compile();
        }
    }
}