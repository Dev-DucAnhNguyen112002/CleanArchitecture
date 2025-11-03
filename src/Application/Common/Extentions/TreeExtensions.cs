using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitectureTest.Application.Common.Extentions;
public static class TreeExtensions
{
    public static List<T> BuildTree<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey?> parentSelector,
        Func<T, TKey> keySelector,
        Action<T, List<T>> addChildrenAction)
        where TKey : struct
    {
        var items = source.ToList();
        var lookup = items.ToLookup(parentSelector);

        foreach (var item in items)
        {
            var key = keySelector(item);
            var children = lookup[key]?.ToList() ?? new List<T>();
            addChildrenAction(item, children);
        }
        return lookup[default].ToList();
    }
}
