namespace RuleEngine.Interfaces.Rules
{
    public interface IActionBlockRule<in T>
    {
        void Exectue(T param);
    }

    public interface IFuncBlockRule<out T>
    {
        T Execute();
    }

    //public interface IVoidBlockRule<in T>
    //{
    //    void Execute();
    //}

    //public interface IActionBlockRule<in T1, in T2>
    //{
    //    void Execute(T1 param1, T2 param2);
    //}
}