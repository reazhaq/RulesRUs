using System.Collections;
using System.Collections.Generic;
using RuleEngine.Rules;

namespace RuleFactory.RulesFactory
{
    public static class BlockRulesFactory
    {
        public static ActionBlockRule<T> CreateActionBlockRule<T>() => new ActionBlockRule<T>();

        public static ActionBlockRule<T> CreateActionBlockRule<T>(IList<Rule> rules)
        {
            var actionBlockRule = new ActionBlockRule<T>();
            foreach (var rule in rules)
            {
                actionBlockRule.Rules.Add(rule);
            }
            return actionBlockRule;
        }
    }
}