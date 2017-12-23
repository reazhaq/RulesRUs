namespace RuleEngine.Interfaces.Rules
{
    public interface IConstantRule<out T>
    {
        T Execute();
    }
}