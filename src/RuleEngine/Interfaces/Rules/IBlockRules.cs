namespace RuleEngine.Interfaces.Rules
{
    public interface IVoidBlockRule<in T>
    {
        void Exectue(T param);
    }

    //public interface IActionBlockRule<in T>
    //{
    //    void Execute(T param);
    //}

    //public interface IActionBlockRule<in T1, in T2>
    //{
    //    void Execute(T1 param1, T2 param2);
    //}

    //public interface IFuncBlockRule<out T>
    //{
    //    T Execute();
    //}
}