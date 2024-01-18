namespace RuleEngine.Interfaces.Rules;

public interface IConditionalActionRule<in T>
{
    void Execute(T param);
}

public interface IConditionalFuncRule<in T1, out T2>
{
    T2 Execute(T1 param1);
}