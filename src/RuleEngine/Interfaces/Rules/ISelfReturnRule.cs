namespace RuleEngine.Interfaces.Rules;

public interface ISelfReturnRule<T>
{
    T Get(T param);
}