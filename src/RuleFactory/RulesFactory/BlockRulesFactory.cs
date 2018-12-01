using System.Collections.Generic;
using System.Globalization;
using RuleEngine.Rules;

namespace RuleFactory.RulesFactory
{
    public static class BlockRulesFactory
    {
        public static ActionBlockRule<T> CreateActionBlockRule<T>() => new ActionBlockRule<T>();

        public static ActionBlockRule<T> CreateActionBlockRule<T>(IList<Rule> rules)
        {
            var actionBlockRule = CreateActionBlockRule<T>();
            foreach (var rule in rules)
            {
                actionBlockRule.Rules.Add(rule);
            }
            return actionBlockRule;
        }

        public static FuncBlockRule<TIn, TOut> CreateFuncBlockRule<TIn, TOut>() => new FuncBlockRule<TIn, TOut>();

        public static FuncBlockRule<TIn, TOut> CreateFuncBlockRule<TIn, TOut>(IList<Rule> rules)
        {
            var funcBlockRule = CreateFuncBlockRule<TIn, TOut>();
            foreach (var rule in rules)
            {
                funcBlockRule.Rules.Add(rule);
            }
            return funcBlockRule;
        }
    }
}