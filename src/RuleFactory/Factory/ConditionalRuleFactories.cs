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
                instance.ConditionRule = (Rule) propValueDictionary["ConditionRule"];
            if (propValueDictionary.ContainsKey("TrueRule"))
                instance.TrueRule = (Rule) propValueDictionary["TrueRule"];

            return instance;
        }

        //public static void WriteRuleValues<T>(ConditionalIfThActionRule<T> conditionalIfThActionRule, Dictionary<string, object> propValueDictionary)
        //{
        //    if (conditionalIfThActionRule == null || propValueDictionary == null) return;

        //    propValueDictionary.Add("ConditionRule", RuleFactory.ConvertRuleToDictionary<T>(conditionalIfThActionRule.ConditionRule));
        //    propValueDictionary.Add("TrueRule", RuleFactory.ConvertRuleToDictionary<T>(conditionalIfThActionRule.TrueRule));
        //}
    }
}