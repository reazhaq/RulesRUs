namespace RuleEngine.Interfaces.Rules;

public interface IContainsValueRule<in T>
{
    bool ContainsValue(T valueToSearch);
}