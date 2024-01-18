namespace RuleEngine.Utils;

public static class CollectionExtensions
{
    public static void AddRange<T>(this IList<T> targetList, IEnumerable<T> sourceItems)
    {
        if (targetList == null || sourceItems == null)
            return;

        if (targetList is List<T> list1)
            list1.AddRange(sourceItems);
        else
        {
            foreach (var item in sourceItems)
                targetList.Add(item);
        }
    }
}