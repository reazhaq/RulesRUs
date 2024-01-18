namespace RuleFactory.RulesFactory;

public static class RegExRuleFactory
{
    public static RegExRule<T> CreateRegExRule<T>(Expression<Func<T, object>> objectToValidate, string regExToUse)
    {
        return new RegExRule<T>
        {
            ObjectToValidate = objectToValidate?.GetObjectToWorkOnFromExpression(),
            RegExToUse = regExToUse
        };
    }
}