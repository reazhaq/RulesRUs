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
            RuleFactories.ReadRuleValues(instance, propValueDictionary);

            if (propValueDictionary.ContainsKey("ConditionRule"))
            {
                var conditionRuleDic = (IDictionary<string, object>) propValueDictionary["ConditionRule"];
                instance.ConditionRule = RuleFactory.CreateRuleFromDictionary<T>(conditionRuleDic);
            }
            if (propValueDictionary.ContainsKey("TrueRule"))
            {
                var trueRuleDic = (IDictionary<string, object>) propValueDictionary["TrueRule"];
                instance.TrueRule = RuleFactory.CreateRuleFromDictionary<T>(trueRuleDic);
            }
            return instance;
        }
    }
}