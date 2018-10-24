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

    //public interface IActionBlockRule<in T1, in T2>
    //{
    //    void Execute(T1 param1, T2 param2);
    //}
}