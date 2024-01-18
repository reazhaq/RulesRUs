﻿namespace RuleEngine.Rules;

public abstract class MethodCallBase : Rule
{
    public string MethodToCall;
    // MethodClassName needed for extension methods...
    public string MethodClassName { get; set; }
    public IList<Rule> MethodParameters { get; } = new List<Rule>();

    protected MethodInfo GetMethodInfo(string methodClassName, string methodToCall,
                                    Type[] methodArgumentTypes, Expression expression)
    {
        if (string.IsNullOrEmpty(methodClassName))
            return expression?.Type.GetMethodInfo(methodToCall, methodArgumentTypes);

        var type = ReflectionExtensions.GetTypeFor(methodClassName);
        if (type == null) throw new RuleEngineException($"can't find class named: {methodClassName}");

        return type.GetMethodInfo(methodToCall, methodArgumentTypes);
    }

    protected Expression[] GetArgumentsExpressions(ParameterExpression rootParam, out Type[] methodArgumentTypes)
    {
        methodArgumentTypes = new Type[MethodParameters.Count];
        var argumentsExpressions = new Expression[MethodParameters.Count];
        for (var index = 0; index < MethodParameters.Count; index++)
        {
            var paramRule = MethodParameters[index];
            argumentsExpressions[index] = paramRule.BuildExpression(rootParam);
            methodArgumentTypes[index] = argumentsExpressions[index].Type;
        }
        return argumentsExpressions;
    }
}

public class MethodVoidCallRule<T> : MethodCallBase, IMethodVoidCallRule<T>
{
    private Action<T> CompiledDelegate { get; set; }
    public string ObjectToCallMethodOn { get; set; }

    public override Expression BuildExpression(params ParameterExpression[] parameters)
    {
        if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T))
            throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T)}");

        var param = parameters[0];
        var expression = GetExpressionWithSubProperty(param, ObjectToCallMethodOn);

        var argumentsExpressions = GetArgumentsExpressions(param, out var methodArgumentTypes);

        var methodInfo = GetMethodInfo(MethodClassName, MethodToCall, methodArgumentTypes, expression);
        if (methodInfo == null) return null;
        if (!methodInfo.IsGenericMethod)
            methodArgumentTypes = null;

        ExpressionForThisRule = Expression.Call(expression, MethodToCall, methodArgumentTypes, argumentsExpressions);
        return ExpressionForThisRule;
    }

    public override bool Compile()
    {
        var param = Expression.Parameter(typeof(T));
        ExpressionForThisRule = BuildExpression(param);
        if (ExpressionForThisRule == null) return false;

        Debug.WriteLine($"{ExpressionForThisRule} ready to compile:" +
                        $"{Environment.NewLine}{ExpressionDebugView()}");

        CompiledDelegate = Expression.Lambda<Action<T>>(ExpressionForThisRule, param).Compile();
        return CompiledDelegate != null;
    }

    public void Execute(T param)
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("A Rule must be compiled first");

        CompiledDelegate(param);
    }
}

public class MethodCallRule<T1, T2> : MethodCallBase, IMethodCallRule<T1, T2>
{
    private Func<T1, T2> CompiledDelegate { get; set; }
    public string ObjectToCallMethodOn { get; set; }

    public override Expression BuildExpression(params ParameterExpression[] parameters)
    {
        if (parameters == null || parameters.Length != 1 || parameters[0].Type != typeof(T1))
            throw new RuleEngineException($"{nameof(BuildExpression)} must call with one parameter of {typeof(T1)}");

        var param = parameters[0];
        var expression = GetExpressionWithSubProperty(param, ObjectToCallMethodOn);

        var argumentsExpressions = GetArgumentsExpressions(param, out var methodArgumentTypes);

        var methodInfo = GetMethodInfo(MethodClassName, MethodToCall, methodArgumentTypes, expression);
        if (methodInfo == null) return null;
        if (!methodInfo.IsGenericMethod)
            methodArgumentTypes = null;

        ExpressionForThisRule = Expression.Call(expression, MethodToCall, methodArgumentTypes, argumentsExpressions);
        return ExpressionForThisRule;
    }

    public override bool Compile()
    {
        var funcParameter = Expression.Parameter(typeof(T1));
        ExpressionForThisRule = BuildExpression(funcParameter);
        if (ExpressionForThisRule == null) return false;

        Debug.WriteLine($"{ExpressionForThisRule} ready to compile:" +
                        $"{Environment.NewLine}{ExpressionDebugView()}");

        CompiledDelegate = Expression.Lambda<Func<T1, T2>>(ExpressionForThisRule, funcParameter).Compile();
        return CompiledDelegate != null;
    }

    public T2 Execute(T1 target)
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("A Rule must be compiled first");

        return CompiledDelegate(target);
    }
}

public abstract class StaticCallBase : MethodCallBase
{
    public override Expression BuildExpression(params ParameterExpression[] _)
    {
        var argumentsExpressions = GetArgumentsExpressions(null, out var methodArgumentTypes);

        var methodInfo = GetMethodInfo(MethodClassName, MethodToCall, methodArgumentTypes, null);
        if (methodInfo == null) return null;

        ExpressionForThisRule = Expression.Call(methodInfo, argumentsExpressions);
        return ExpressionForThisRule;
    }

    public override bool Compile()
    {
        ExpressionForThisRule = BuildExpression();
        if (ExpressionForThisRule == null) return false;

        Debug.WriteLine($"{ExpressionForThisRule} ready to compile:" +
                        $"{Environment.NewLine}{ExpressionDebugView()}");
        return true;
    }
}

public class StaticMethodCallRule<T> : StaticCallBase, IStaticMethodCallRule<T>
{
    private Func<T> CompiledDelegate { get; set; }

    public override bool Compile()
    {
        if(base.Compile())
            CompiledDelegate = Expression.Lambda<Func<T>>(ExpressionForThisRule).Compile();

        return CompiledDelegate != null;
    }

    public T Execute()
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("A Rule must be compiled first");

        return CompiledDelegate();
    }
}

public class StaticVoidMethodCallRule : StaticCallBase, IStaticVoidMethodCallRule
{
    private delegate void VoidAction();
    private VoidAction CompiledDelegate { get; set; }

    public override bool Compile()
    {
        if (base.Compile())
            CompiledDelegate = Expression.Lambda<VoidAction>(ExpressionForThisRule).Compile();

        return CompiledDelegate != null;
    }

    public void Execute()
    {
        if (CompiledDelegate == null)
            throw new RuleEngineException("A Rule must be compiled first");

        CompiledDelegate();
    }
}