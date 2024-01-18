namespace RuleFactory.RulesFactory;

public enum LogicalOperatorAtTheRootLevel
{
    NotEqual,
    Equal,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual
}

public enum ChildrenBindingOperator
{
    Not,
    AndAlso,
    OrElse
}

public static class ValidationRulesFactory
{
    public static ValidationRule<T> CreateValidationRule<T>(LogicalOperatorAtTheRootLevel operatorToUse,
                                        Rule valueToValidateAgainst)
    {
        return new ValidationRule<T>
        {
            OperatorToUse = operatorToUse.ToString(),
            ValueToValidateAgainst = valueToValidateAgainst
        };
    }

    public static ValidationRule<T> CreateValidationRule<T>(Expression<Func<T, object>> objectToValidate,
                                        LogicalOperatorAtTheRootLevel operatorToUse, Rule valueToValidateAgainst)
    {
        return new ValidationRule<T>
        {
            ObjectToValidate = objectToValidate?.GetObjectToWorkOnFromExpression(),
            OperatorToUse = operatorToUse.ToString(),
            ValueToValidateAgainst = valueToValidateAgainst
        };
    }

    public static ValidationRule<T> CreateValidationRule<T>(ChildrenBindingOperator operatorToUse,
                                        IList<Rule> childrenRules)
    {
        var rule = new ValidationRule<T>
        {
            OperatorToUse = operatorToUse.ToString()
        };
        if (childrenRules != null)
            rule.ChildrenRules.AddRange(childrenRules);
        return rule;
    }

    public static ValidationRule<T1, T2> CreateValidationRule<T1, T2>(LogicalOperatorAtTheRootLevel operatorToUse,
        Expression<Func<T1, object>> objectToValidate1, Expression<Func<T2, object>> objectToValidate2)
    {
        return new ValidationRule<T1, T2>
        {
            OperatorToUse = operatorToUse.ToString(),
            ObjectToValidate1 = objectToValidate1?.GetObjectToWorkOnFromExpression(),
            ObjectToValidate2 = objectToValidate2?.GetObjectToWorkOnFromExpression()
        };
    }
}