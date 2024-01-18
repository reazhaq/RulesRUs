namespace RuleEngine.Rules;

public class SelfReturnRule<T> : Rule, ISelfReturnRule<T>
{
    private Func<T, T> CompiledDelegate { get; set; }

    public override Expression BuildExpression(params ParameterExpression[] parameters)
    {
        if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T))
            throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T)}");

        ExpressionForThisRule = parameters[0];
        return ExpressionForThisRule;
    }

    public override bool Compile()
    {
        var parameter = Expression.Parameter(typeof(T));
        ExpressionForThisRule = BuildExpression(parameter);

        Debug.WriteLine($"selfReturnExpressionBody for Func<{typeof(T)},{typeof(T)}>: " +
                        $"{ExpressionDebugView()}");

        CompiledDelegate = Expression.Lambda<Func<T, T>>(ExpressionForThisRule, parameter).Compile();
        return CompiledDelegate != null;
    }

    public T Get(T param)
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("Rule has to be compiled before it can be executed");

        return CompiledDelegate(param);
    }
}