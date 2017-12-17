namespace RuleEngine.Interfaces
{
    public interface IExpressionRule<out T>
    {
        T Execute();
    }

    public interface IExpressionRule<in T1, out T2>
    {
        T2 Execute(T1 parameter);
    }

    public interface IExpressionRule<in T1, in T2, out T3>
    {
        T3 Execute(T1 parameter1, T2 parameter2);
    }
}