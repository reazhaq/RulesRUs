namespace RuleEngine.Interfaces
{
    public interface IConstantRule<out T>
    {
        T Execute();
    }
}