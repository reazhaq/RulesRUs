namespace RuleEngine.Interfaces
{
    public interface IValidationRule<in T>
    {
        bool Execute(T targetObject);
    }
}