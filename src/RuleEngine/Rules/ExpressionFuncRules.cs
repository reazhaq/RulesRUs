﻿namespace RuleEngine.Rules;

public class ExpressionFuncRule<T> : Rule, IExpressionFuncRule<T>
{
    private Func<T> CompiledDelegate { get; set; }
    public Expression<Func<T>> RuleExpression { get; }

    public ExpressionFuncRule(Expression<Func<T>> ruleExpression) => ExpressionForThisRule = RuleExpression = ruleExpression;
    public override Expression BuildExpression(params ParameterExpression[] _) => RuleExpression;

    public override bool Compile()
    {
        Debug.WriteLine($"ExpressionFuncRules<{typeof(T)}> RuleExpression:" +
                        $"{Environment.NewLine}{ExpressionDebugView()}");

        CompiledDelegate = RuleExpression.Compile();
        return CompiledDelegate != null;
    }

    public T Execute()
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("Rule has to be compiled before it can be executed");

        return CompiledDelegate();
    }
}

public class ExpressionFuncRule<T1, T2> : Rule, IExpressionFuncRule<T1, T2>
{
    private Func<T1, T2> CompiledDelegate { get; set; }
    public Expression<Func<T1, T2>> RuleExpression { get; }

    public ExpressionFuncRule(Expression<Func<T1, T2>> ruleExpression) => ExpressionForThisRule = RuleExpression = ruleExpression;
    public override Expression BuildExpression(params ParameterExpression[] _) => RuleExpression;

    public override bool Compile()
    {
        Debug.WriteLine($"ExpressionFuncRules<{typeof(T1)},{typeof(T2)}> RuleExpression:" +
                        $"{Environment.NewLine}{ExpressionDebugView()}");

        CompiledDelegate = RuleExpression.Compile();
        return CompiledDelegate != null;
    }

    public T2 Execute(T1 parameter)
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("Rule has to be compiled before it can be executed");

        return CompiledDelegate(parameter);
    }
}

public class ExpressionFuncRule<T1, T2, T3> : Rule, IExpressionFuncRule<T1, T2, T3>
{
    private Func<T1, T2, T3> CompiledDelegate { get; set; }
    public Expression<Func<T1, T2, T3>> RuleExpression { get; }

    public ExpressionFuncRule(Expression<Func<T1, T2, T3>> ruleExpression) => ExpressionForThisRule = RuleExpression = ruleExpression;
    public override Expression BuildExpression(params ParameterExpression[] _) => RuleExpression;

    public override bool Compile()
    {
        Debug.WriteLine($"ExpressionFuncRules<{typeof(T1)},{typeof(T2)},{typeof(T3)}> RuleExpression:" +
                        $"{Environment.NewLine}{ExpressionDebugView()}");

        CompiledDelegate = RuleExpression.Compile();
        return CompiledDelegate != null;
    }

    public T3 Execute(T1 parameter1, T2 parameter2)
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("Rule has to be compiled before it can be executed");

        return CompiledDelegate(parameter1, parameter2);
    }
}