namespace RuleFactory.RulesFactory;

public static class ConstantRulesFactory
{
    public static ConstantRule<T> CreateConstantRule<T>(string value)
    {
        return new ConstantRule<T> {Value = value};
    }

    public static ConstantRule<T1, T2> CreateConstantRule<T1, T2>(string value)
    {
        return new ConstantRule<T1, T2> {Value = value};
    }
}