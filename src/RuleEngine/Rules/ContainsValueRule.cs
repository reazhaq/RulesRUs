namespace RuleEngine.Rules;

public class ContainsValueRule<T> : Rule, IContainsValueRule<T>
{
    private Func<T, bool> CompiledDelegate { get; set; }

    public List<T> CollectionToSearch = new List<T>();

    public string EqualityComparerClassName { get; set; }
    public string EqualityComparerPropertyName { get;set; }
    public IEqualityComparer<T> EqualityComparer { get; set; } = EqualityComparer<T>.Default;

    public override Expression BuildExpression(params ParameterExpression[] parameters)
    {
        if (!string.IsNullOrEmpty(EqualityComparerClassName) && !string.IsNullOrEmpty(EqualityComparerPropertyName))
            EqualityComparer =
                ReflectionExtensions.GetEqualityComparerProperty<T>(EqualityComparerClassName,
                    EqualityComparerPropertyName);

        Expression<Func<T, bool>> expression = s => CollectionToSearch.Contains(s, EqualityComparer);
        ExpressionForThisRule = expression;
        return expression;
    }

    public override bool Compile()
    {
        var parameter = Expression.Parameter(typeof(T));
        ExpressionForThisRule = BuildExpression(parameter);
        if (!(ExpressionForThisRule is Expression<Func<T, bool>>)) return false;

        Debug.WriteLine($"expression ={Environment.NewLine}{ExpressionDebugView()}");

        CompiledDelegate = (ExpressionForThisRule as Expression<Func<T, bool>>)?.Compile();
        return CompiledDelegate != null;
    }

    public bool ContainsValue(T valueToSearch)
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("A Rule must be compiled first");

        return CompiledDelegate(valueToSearch);
    }
}