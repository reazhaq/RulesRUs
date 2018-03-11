using System;
using System.Collections.Generic;
using RuleEngine.Rules;
using RuleFactory.Factory;

namespace RuleFactory
{
    public static class RuleFactory
    {
        public static Rule CreateRule(string ruleType, IDictionary<string, string> propValueDictionary)
        {
            if (string.IsNullOrEmpty(ruleType) || propValueDictionary == null) return null;

            switch (ruleType.ToLower())
            {
                case "constantrule":
                    return ConstantRuleFactories.CreateConstantRule(propValueDictionary);
            }

            return null;
        }
    }
}

