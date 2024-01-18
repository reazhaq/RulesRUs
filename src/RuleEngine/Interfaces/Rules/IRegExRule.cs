namespace RuleEngine.Interfaces.Rules;

public interface IRegExRule<in T>
{
    bool IsMatch(T targetObject);
}