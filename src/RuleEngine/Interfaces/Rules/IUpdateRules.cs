namespace RuleEngine.Interfaces.Rules
{
    public interface IUpdateRule<in T1, in T2>
    {
        void UpdateFieldOrPropertyValue(T1 targetObject, T2 source);
    }
}