namespace RuleEngine.Interfaces
{
    public interface IExpressionActionRules<in T>
    {
        void Execute(T parameter);
    }
}