using System.Collections.Generic;
using System.Linq;

namespace RuleEngineTests.Utils
{
    public static class ListExtensions
    {
        public static bool ContainsValue<T>(this IList<T> listToCheck, T valueToCheck, IEqualityComparer<T> equalityComparerToUse)
        {
            if (listToCheck == null) return false;

            if (equalityComparerToUse == null)
                equalityComparerToUse = EqualityComparer<T>.Default;

            return listToCheck.Any(element => equalityComparerToUse.Equals(valueToCheck, element));
        }
    }
}