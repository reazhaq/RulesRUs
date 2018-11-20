namespace RuleEngine.Interfaces.Rules
{
    public interface IActionBlockRule<in T>
    {
        void Execute(T param);
    }

    public interface IFuncBlockRule<in TIn, out TOut>
    {
        TOut Execute(TIn param);
    }
}