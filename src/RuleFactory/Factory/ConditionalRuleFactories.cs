using System.Collections.Generic;
using RuleEngine.Rules;

namespace RuleFactory.Factory
{
    public static class ConditionalRuleFactories
    {
        public static ConditionalIfThActionRule<T> CreateConditionalIfThActionRule<T>(
            IDictionary<string, object> propValueDictionary)
        {
            if (propValueDictionary == null) return null;
            var instance = new ConditionalIfThActionRule<T>();
            if (propValueDictionary.ContainsKey("ConditionRule"))
                instance.ConditionRule = (Rule) propValueDictionary["ConditionRule"];
            if (propValueDictionary.ContainsKey("TrueRule"))
                instance.TrueRule = (Rule) propValueDictionary["TrueRule"];

            return instance;
        }
    }
}