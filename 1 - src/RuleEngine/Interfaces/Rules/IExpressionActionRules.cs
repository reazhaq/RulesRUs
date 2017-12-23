namespace RuleEngine.Interfaces.Rules
{
    public interface IExpressionActionRule<in T>
    {
        void Execute(T parameter);
    }
}