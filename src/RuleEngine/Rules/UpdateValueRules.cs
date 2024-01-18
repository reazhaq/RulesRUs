namespace RuleEngine.Rules;

public abstract class UpdateValueRuleBase : Rule
{
    public string ObjectToUpdate;
}

public class UpdateValueRule<T> : UpdateValueRuleBase, IUpdateValueRule<T>
{
    private Action<T> CompiledDelegate { get; set; }
    public Rule SourceDataRule;

    public override Expression BuildExpression(params ParameterExpression[] parameters)
    {
        if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T))
            throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T)}");

        var targetObject = parameters[0];
        var targetExpression = GetExpressionWithSubProperty(targetObject, ObjectToUpdate);
        var sourceExpression = SourceDataRule.BuildExpression(targetObject);
        ExpressionForThisRule = Expression.Assign(targetExpression, sourceExpression);
        return ExpressionForThisRule;
    }

    public override bool Compile()
    {
        var paramObjectToUpdate = Expression.Parameter(typeof(T));
        ExpressionForThisRule = BuildExpression(paramObjectToUpdate);
        if (ExpressionForThisRule == null) return false;

        Debug.WriteLine($"Expression for UpdateRule<{typeof(T)}>:" +
                        $"{Environment.NewLine}{ExpressionDebugView()}`");

        CompiledDelegate = Expression.Lambda<Action<T>>(ExpressionForThisRule, paramObjectToUpdate).Compile();
        return CompiledDelegate != null;
    }

    public void UpdateFieldOrPropertyValue(T targetObject)
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("A Rule must be compiled first");

        CompiledDelegate(targetObject);
    }
}

public class UpdateValueRule<T1, T2> : UpdateValueRuleBase, IUpdateValueRule<T1, T2>
{
    private Action<T1, T2> CompiledDelegate { get; set; }

    public override Expression BuildExpression(params ParameterExpression[] parameters)
    {
        if (parameters == null || parameters.Length != 2 || parameters[0].Type != typeof(T1) || parameters[1].Type != typeof(T2))
            throw new RuleEngineException($"{nameof(BuildExpression)} must call with two parameters of {typeof(T1)} and {typeof(T2)}");

        var targetObject = parameters[0];
        var sourceParam = parameters[1];

        var targetExpression = GetExpressionWithSubProperty(targetObject, ObjectToUpdate);
        ExpressionForThisRule = Expression.Assign(targetExpression, sourceParam);
        return ExpressionForThisRule;
    }

    public override bool Compile()
    {
        var paramObjectToUpdate = Expression.Parameter(typeof(T1));
        var paramSourceValue = Expression.Parameter(typeof(T2));
        ExpressionForThisRule = BuildExpression(paramObjectToUpdate, paramSourceValue);
        if (ExpressionForThisRule == null) return false;

        Debug.WriteLine($"Expression for UpdateRule<{typeof(T1)},{typeof(T2)}>:" +
                        $"{Environment.NewLine}{ExpressionDebugView()}");

        CompiledDelegate = Expression.Lambda<Action<T1, T2>>(ExpressionForThisRule, paramObjectToUpdate, paramSourceValue).Compile();
        return CompiledDelegate != null;
    }

    public void UpdateFieldOrPropertyValue(T1 targetObject, T2 source)
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("A Rule must be compiled first");

        CompiledDelegate(targetObject, source);
    }
}

public class UpdateRefValueRule<T> : Rule, IUpdateRefValueRule<T>
{
    private delegate void UpdateAction2(ref T target, T source);
    private UpdateAction2 CompiledDelegate2 { get; set; }

    private delegate void UpdateAction(ref T target);
    private UpdateAction CompiledDelegate { get; set; }
    public Rule SourceDataRule;

    public override Expression BuildExpression(params ParameterExpression[] parameters)
    {
        if (parameters == null || (parameters.Length != 1 && parameters.Length != 2) ||
            (parameters[0].Type != typeof(T) && parameters[1].Type != typeof(T)))
            throw new RuleEngineException($"{nameof(BuildExpression)} must call with one or two parameter of {typeof(T)}");

        var target = parameters[0];
        var sourceExpression = SourceDataRule != null ? SourceDataRule.BuildExpression(target) : parameters[1];
        ExpressionForThisRule = Expression.Assign(target, sourceExpression);
        return ExpressionForThisRule;
    }

    public override bool Compile()
    {
        var param = Expression.Parameter(typeof(T).MakeByRefType());
        var paramSourceValue = Expression.Parameter(typeof(T));
        ExpressionForThisRule = SourceDataRule != null ?
                                    BuildExpression(param) :
                                    BuildExpression(param, paramSourceValue);
        if (ExpressionForThisRule == null) return false;

        Debug.WriteLine($"Expression for RefUpdateRule<{typeof(T)}>:" +
                        $"{Environment.NewLine}{ExpressionDebugView()}`");

        if (SourceDataRule != null)
            CompiledDelegate = Expression.Lambda<UpdateAction>(ExpressionForThisRule, param).Compile();
        else
            CompiledDelegate2 = Expression.Lambda<UpdateAction2>(ExpressionForThisRule, param, paramSourceValue).Compile();

        return (CompiledDelegate != null || CompiledDelegate2 != null);
    }

    public void RefUpdate(ref T target)
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("A Rule must be compiled first");

        CompiledDelegate(ref target);
    }

    public void RefUpdate(ref T target, T source)
    {
        if (CompiledDelegate2 == null)
            throw new RuleEngineException("A Rule must be compiled first");

        CompiledDelegate2(ref target, source);
    }
}