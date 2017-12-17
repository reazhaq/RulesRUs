using System.Runtime.InteropServices.ComTypes;

namespace RuleEngine.Interfaces
{
    public interface IValidationRule<in T>
    {
        bool Execute(T targetObject);
    }

    public interface IValidationRule<in T1, in T2>
    {
        bool Execute(T1 param1, ITypeInfo2 param2);
    }
}