namespace RuleEngine.Interfaces.Rules
{
    public interface IRegExRule<T>
    {
        bool IsMatch(T targetObject);
    }
}