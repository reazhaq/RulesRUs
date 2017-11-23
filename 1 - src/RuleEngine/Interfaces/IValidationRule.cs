namespace RuleEngine.Interfaces
{
    public interface IValidationRule<in TTarget, out TResult>
    {
        TResult Execute(TTarget targetObject);
    }
}