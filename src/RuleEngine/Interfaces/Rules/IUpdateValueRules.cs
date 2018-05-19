namespace RuleEngine.Interfaces.Rules
{
    public interface IUpdateValueRule<in T1, in T2>
    {
        void UpdateFieldOrPropertyValue(T1 targetObject, T2 source);
    }

    public interface IUpdateValueRule<in T>
    {
        void UpdateFieldOrPropertyValue(T targetObject);
    }

    public interface IUpdateRefValueRule<T>
    {
        void RefUpdate(ref T target);
        void RefUpdate(ref T target, T source);
    }
}