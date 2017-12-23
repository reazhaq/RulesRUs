namespace RuleEngine.Interfaces.Rules
{
    public interface IValidationRule<in T>
    {
        bool Execute(T targetObject);
    }

    public interface IValidationRule<in T1, in T2>
    {
        bool Execute(T1 param1, T2 param2);
    }
}