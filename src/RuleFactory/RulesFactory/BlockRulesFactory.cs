namespace RuleFactory.RulesFactory;

public static class BlockRulesFactory
{
    public static ActionBlockRule<T> CreateActionBlockRule<T>() => new ActionBlockRule<T>();

    public static ActionBlockRule<T> CreateActionBlockRule<T>(IList<Rule> rules)
    {
        var actionBlockRule = CreateActionBlockRule<T>();
        actionBlockRule.Rules.AddRange(rules);
        return actionBlockRule;
    }

    public static FuncBlockRule<TIn, TOut> CreateFuncBlockRule<TIn, TOut>() => new FuncBlockRule<TIn, TOut>();

    public static FuncBlockRule<TIn, TOut> CreateFuncBlockRule<TIn, TOut>(IList<Rule> rules)
    {
        var funcBlockRule = CreateFuncBlockRule<TIn, TOut>();
        funcBlockRule.Rules.AddRange(rules);
        return funcBlockRule;
    }
}