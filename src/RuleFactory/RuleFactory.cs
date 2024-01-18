namespace RuleFactory;

public static class RuleFactory
{
    public static Rule CreateRule(string ruleType, string[] boundingTypes)
    {
        switch (ruleType)
        {
            case "ConstantRule`1":
            case "ConstantRule`2":
                return CreateConstantRule(boundingTypes);
            case "ValidationRule`1":
            case "ValidationRule`2":
                return CreateValidationRule(boundingTypes);
            case "UpdateValueRule`1":
            case "UpdateValueRule`2":
                return CreateUpdateValueRule(boundingTypes);
            case "UpdateRefValueRule`1":
                return CreateUpdateRefValueRule(boundingTypes);
            case "MethodVoidCallRule`1":
            case "MethodCallRule`2":
                return CreateMethodVoidCallRule(boundingTypes);
            case "StaticMethodCallRule`1":
                return CreateStaticMethodCallRule(boundingTypes);
            case "StaticVoidMethodCallRule":
                return CreateStaticVoidMethodCallRule();
            case "ConditionalIfThActionRule`1":
                return CreateConditionalIfThActionRule(boundingTypes);
            case "ConditionalIfThElActionRule`1":
                return CreateConditionalIfThElActionRule(boundingTypes);
            case "ConditionalFuncRule`2":
                return CreateConditionalFunctionRule(boundingTypes);
            case "ContainsValueRule`1":
                return CreateContainsValueRule(boundingTypes);
            case "RegExRule`1":
                return CreateRegExRule(boundingTypes);
            case "SelfReturnRule`1":
                return CreateSelfReturnRule(boundingTypes);
            case "ActionBlockRule`1":
                return CreateActionBlockRule(boundingTypes);
            case "FuncBlockRule`2":
                return CreateFuncBlockRule(boundingTypes);
        }

        return null;
    }

    private static Rule CreateStaticVoidMethodCallRule()
    {
        return CreateRule(typeof(StaticVoidMethodCallRule));
    }

    private static Rule CreateStaticMethodCallRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length != 1) return null;
        return CreateRule(typeof(StaticMethodCallRule<>),
            new[] { ReflectionExtensions.GetTypeFor(boundingTypes[0]) });
    }

    private static Rule CreateUpdateRefValueRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length != 1) return null;
        return CreateRule(typeof(UpdateRefValueRule<>),
            new[] { ReflectionExtensions.GetTypeFor(boundingTypes[0]) });
    }

    private static Rule CreateSelfReturnRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length != 1) return null;
        return CreateRule(typeof(SelfReturnRule<>),
            new[] { ReflectionExtensions.GetTypeFor(boundingTypes[0]) });
    }

    private static Rule CreateRegExRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length != 1) return null;
        return CreateRule(typeof(RegExRule<>),
            new[] { ReflectionExtensions.GetTypeFor(boundingTypes[0]) });
    }

    private static Rule CreateContainsValueRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length != 1) return null;
        return CreateRule(typeof(ContainsValueRule<>),
            new[] { ReflectionExtensions.GetTypeFor(boundingTypes[0]) });
    }

    private static Rule CreateConditionalFunctionRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length != 2) return null;
        return CreateRule(typeof(ConditionalFuncRule<,>),
            new[]
            {
                ReflectionExtensions.GetTypeFor(boundingTypes[0]),
                ReflectionExtensions.GetTypeFor(boundingTypes[1])
            });
    }

    private static Rule CreateConditionalIfThElActionRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length != 1) return null;
        return CreateRule(typeof(ConditionalIfThElActionRule<>),
            new[] { ReflectionExtensions.GetTypeFor(boundingTypes[0]) });
    }

    private static Rule CreateConditionalIfThActionRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length != 1) return null;

        return CreateRule(typeof(ConditionalIfThActionRule<>),
                new[] { ReflectionExtensions.GetTypeFor(boundingTypes[0]) });
    }

    private static Rule CreateMethodVoidCallRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length < 1 || boundingTypes.Length > 2) return null;

        return (boundingTypes.Length == 1
            ? CreateRule(typeof(MethodVoidCallRule<>), new[] { ReflectionExtensions.GetTypeFor(boundingTypes[0]) })
            : CreateRule(typeof(MethodCallRule<,>),
                new[]
                {
                    ReflectionExtensions.GetTypeFor(boundingTypes[0]),
                    ReflectionExtensions.GetTypeFor(boundingTypes[1])
                }));
    }

    public static Rule CreateConstantRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length < 1 || boundingTypes.Length > 2) return null;

        return (boundingTypes.Length == 1
            ? CreateRule(typeof(ConstantRule<>), new[] {ReflectionExtensions.GetTypeFor(boundingTypes[0])})
            : CreateRule(typeof(ConstantRule<,>),
                new[]
                {
                    ReflectionExtensions.GetTypeFor(boundingTypes[0]),
                    ReflectionExtensions.GetTypeFor(boundingTypes[1])
                }));
    }

    public static Rule CreateValidationRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length < 1 || boundingTypes.Length > 2) return null;

        return (boundingTypes.Length == 1
            ? CreateRule(typeof(ValidationRule<>), new[] {ReflectionExtensions.GetTypeFor(boundingTypes[0])})
            : CreateRule(typeof(ValidationRule<,>),
                new[]
                {
                    ReflectionExtensions.GetTypeFor(boundingTypes[0]),
                    ReflectionExtensions.GetTypeFor(boundingTypes[1])
                }));
    }

    public static Rule CreateUpdateValueRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length < 1 || boundingTypes.Length > 2) return null;

        return (boundingTypes.Length == 1
            ? CreateRule(typeof(UpdateValueRule<>), new[] {ReflectionExtensions.GetTypeFor(boundingTypes[0])})
            : CreateRule(typeof(UpdateValueRule<,>),
                new[]
                {
                    ReflectionExtensions.GetTypeFor(boundingTypes[0]),
                    ReflectionExtensions.GetTypeFor(boundingTypes[1])
                }));

    }

    public static Rule CreateActionBlockRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length != 1) return null;

        return CreateRule(typeof(ActionBlockRule<>),
            new[] { ReflectionExtensions.GetTypeFor(boundingTypes[0]) });
    }

    public static Rule CreateFuncBlockRule(string[] boundingTypes)
    {
        if (boundingTypes == null || boundingTypes.Length != 2) return null;
        return CreateRule(typeof(FuncBlockRule<,>),
            new[]
            {
                ReflectionExtensions.GetTypeFor(boundingTypes[0]),
                ReflectionExtensions.GetTypeFor(boundingTypes[1])
            });
    }

    private static Rule CreateRule(Type ruleType, Type[] typesToBindTo)
    {
        // make sure we are trying to create a bounded generic object inherited from Rule
        if (ruleType == null || typesToBindTo == null || !ruleType.IsSubclassOf(typeof(Rule))) return null;

        var boundedGenericType = ruleType.MakeGenericType(typesToBindTo);
        var instance = Activator.CreateInstance(boundedGenericType);
        return (Rule)instance;
    }

    private static Rule CreateRule(Type ruleType)
    {
        if (ruleType == null || !ruleType.IsSubclassOf(typeof(Rule))) return null;

        var instance = Activator.CreateInstance(ruleType);
        return (Rule) instance;
    }
}

