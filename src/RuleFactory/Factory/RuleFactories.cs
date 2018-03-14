using System;
using System.Collections.Generic;
using RuleEngine.Rules;

namespace RuleFactory.Factory
{
    public static class RuleFactories
    {
        public static void ReadRuleValues(Rule rule, IDictionary<string, object> propValueDictionary)
        {
            if (rule == null) return;

            if (propValueDictionary.ContainsKey("Id") && int.TryParse(propValueDictionary["Id"].ToString(), out var idValue))
                rule.Id = idValue;
            if (propValueDictionary.ContainsKey("Name"))
                rule.Name = propValueDictionary["Name"].ToString();
            if (propValueDictionary.ContainsKey("Description"))
                rule.Description = propValueDictionary["Description"].ToString();
            if (propValueDictionary.ContainsKey("RuleError"))
            {
                rule.RuleError = new RuleError
                {
                    Code = ((IDictionary<string, string>) propValueDictionary["RuleError"])["Code"],
                    Message = ((IDictionary<string,string>)propValueDictionary["RuleError"])["Message"]
                };
            }
        }
    }
}