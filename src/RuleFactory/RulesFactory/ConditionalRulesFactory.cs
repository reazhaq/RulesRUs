namespace RuleFactory.RulesFactory;

public static class ConditionalRulesFactory
{
    public static ConditionalIfThActionRule<T> CreateConditionalIfThActionRule<T>(Rule conditionRule, Rule trueRule)
    {
        return new ConditionalIfThActionRule<T>
        {
            ConditionRule = conditionRule,
            TrueRule = trueRule
        };
    }

    public static ConditionalIfThElActionRule<T> CreateConditionalIfThElActionRule<T>(Rule conditionRule, Rule trueRule,
                                                                                        Rule falseRule)
    {
        return new ConditionalIfThElActionRule<T>
        {
            ConditionRule = conditionRule,
            TrueRule = trueRule,
            FalseRule = falseRule
        };
    }

    public static ConditionalFuncRule<T1, T2> CreateConditionalFuncRule<T1, T2>(Rule conditionRule, Rule trueRule,
                                                                                Rule falseRule)
    {
        return new ConditionalFuncRule<T1,T2>
        {
            ConditionRule = conditionRule,
            TrueRule = trueRule,
            FalseRule = falseRule
        };
    }
}