namespace RuleEngine.Interfaces
{
    public interface IRegExRule<T>
    {
        bool IsMatch(T targetObject);
    }
}