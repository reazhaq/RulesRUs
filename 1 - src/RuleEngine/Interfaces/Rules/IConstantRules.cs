namespace RuleEngine.Interfaces.Rules
{
    public interface IConstantRule<out T>
    {
        T Get();
    }

    public interface IConstantRule<in T1, out T2>
    {
        T2 Get(T1 param);
    }
}