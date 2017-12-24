namespace RuleEngine.Interfaces.Rules
{
    public interface IMethodCallRule<in TTarget, out TResult>
    {
        TResult Execute(TTarget target);
    }
}