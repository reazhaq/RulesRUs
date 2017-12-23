using System;
using System.Linq.Expressions;
using RuleEngine.Rules;

namespace RuleEngine.Interfaces.Compilers
{
    public interface IRegExRuleCompiler<T>
    {
        Expression BuildExpression(ParameterExpression param, RegExRule<T> regExRuleToBuildExpression);
        Func<T, bool> CompileRule(RegExRule<T> regExRuleToCompile);
    }
}