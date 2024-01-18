namespace RuleFactory.RulesFactory;

public static class MethodCallRulesFactory
{
    public static MethodVoidCallRule<T> CreateMethodVoidCallRule<T>(string methodToCall, string methodClassName,
        Expression<Func<T, object>> objectToCallMethodOn, IList<Rule> methodParams)
    {
        var rule = new MethodVoidCallRule<T>
        {
            MethodToCall = methodToCall,
            MethodClassName = methodClassName,
            ObjectToCallMethodOn = objectToCallMethodOn?.GetObjectToWorkOnFromExpression()
        };
        rule.MethodParameters.AddRange(methodParams);
        return rule;
    }

    public static MethodCallRule<T1, T2> CreateMethodCallRule<T1, T2>(string methodToCall, string methodClassName,
        Expression<Func<T1, object>> objectToCallMethodOn, IList<Rule> methodParams)
    {
        var rule = new MethodCallRule<T1, T2>
        {
            MethodToCall = methodToCall,
            MethodClassName = methodClassName,
            ObjectToCallMethodOn = objectToCallMethodOn?.GetObjectToWorkOnFromExpression()
        };
        rule.MethodParameters.AddRange(methodParams);
        return rule;
    }

    public static StaticMethodCallRule<T> CreateStaticMethodCallRule<T>(string methodToCall, string methodClassName, IList<Rule> methodParams)
    {
        var rule = new StaticMethodCallRule<T>
        {
            MethodToCall = methodToCall,
            MethodClassName = methodClassName
        };
        rule.MethodParameters.AddRange(methodParams);

        return rule;
    }

    public static StaticVoidMethodCallRule CreateStaticVoidMethodCallRule(string methodToCall, string methodClassName, IList<Rule> methodParams)
    {
        var rule = new StaticVoidMethodCallRule
        {
            MethodToCall = methodToCall,
            MethodClassName = methodClassName
        };
        rule.MethodParameters.AddRange(methodParams);

        return rule;
    }
}