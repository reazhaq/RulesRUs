namespace RuleEngine.Interfaces.Rules
{
    public interface IExpressionActionRule<in T>
    {
        void Execute(T parameter);
    }

    public interface IExpressionActionRule<in T1, in T2>
    {
        void Execute(T1 param1, T2 param2);
    }

    public interface IExpressionActionRule<in T1, in T2, in T3>
    {
        void Execute(T1 param1, T2 param2, T3 param3);
    }
}