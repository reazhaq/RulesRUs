using System.Collections.Generic;

namespace RuleEngine.Interfaces.Rules
{
    public interface IContainsValueRule<T>
    {
        bool ContainsValue(T valueToSearch);
    }
}