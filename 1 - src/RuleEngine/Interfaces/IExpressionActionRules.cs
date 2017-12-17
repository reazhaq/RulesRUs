namespace RuleEngine.Interfaces
{
    public interface IExpressionActionRule<in T>
    {
        void Execute(T parameter);
    }
}