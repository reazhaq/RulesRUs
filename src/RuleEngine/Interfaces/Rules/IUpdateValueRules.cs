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

    public interface IRefUpdateValueRule<T1, in T2>
    {
        void RefUpdate(ref T1 target, T2 source);
    }

    public interface IRefUpdateValueRule<T>
    {
        void RefUpdate(ref T target);
    }
}