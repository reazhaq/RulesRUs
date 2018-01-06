namespace RuleEngine.Interfaces.Rules
{
    public interface IConditionalRule<in T>
    {
        void Execute(T param);
    }

    public interface IConditionalRule<in T1, out T2>
    {
        T2 Execute(T1 param1);
    }
}