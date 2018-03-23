using System.Collections.Generic;
using RuleEngine.Rules;

namespace RuleFactory.RulesFactory
{
    public static class ContainsValueRuleFactory
    {
        public static ContainsValueRule<T> CreateContainsValueRule<T>(IList<T> collectionToSearch,
            string equalityComparerClassName, string equalityComparerPropertyName)
        {
            var rule = new ContainsValueRule<T>
            {
                EqualityComparerClassName = equalityComparerClassName,
                EqualityComparerPropertyName = equalityComparerPropertyName
            };
            if (collectionToSearch != null)
                rule.CollectionToSearch.AddRange(collectionToSearch);

            return rule;
        }
    }
}