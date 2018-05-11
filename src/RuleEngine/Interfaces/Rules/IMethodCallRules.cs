namespace RuleEngine.Interfaces.Rules
{
    public interface IMethodVoidCallRule<in T>
    {
        void Execute(T param);
    }

    public interface IMethodCallRule<in TTarget, out TResult>
    {
        TResult Execute(TTarget param);
    }

    public interface IStaticMethodCallRule<out T>
    {
        T Execute();
    }

    public interface IStaticVoidMethodCallRule
    {
        void Execute();
    }
}