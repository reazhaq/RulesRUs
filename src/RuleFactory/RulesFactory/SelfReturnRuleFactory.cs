using RuleEngine.Rules;

namespace RuleFactory.RulesFactory
{
    public static class SelfReturnRuleFactory
    {
        public static SelfReturnRule<T> CreateSelfReturnRule<T>() => new SelfReturnRule<T>();
    }
}