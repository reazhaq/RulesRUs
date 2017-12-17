namespace RuleEngine.Interfaces
{
    public interface IExpressionFuncRule<out T>
    {
        T Execute();
    }

    public interface IExpressionFuncRule<in T1, out T2>
    {
        T2 Execute(T1 parameter);
    }

    public interface IExpressionFuncRule<in T1, in T2, out T3>
    {
        T3 Execute(T1 parameter1, T2 parameter2);
    }
}