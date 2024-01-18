namespace RuleEngine.Interfaces.Rules;

public interface IValidationRule<in T>
{
    bool IsValid(T targetObject);
}

public interface IValidationRule<in T1, in T2>
{
    bool IsValid(T1 param1, T2 param2);
}