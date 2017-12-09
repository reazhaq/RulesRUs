namespace RuleEngine.Interfaces
{
    public interface IValidationRule<in TTarget>
    {
        bool Execute(TTarget targetObject);
    }
}