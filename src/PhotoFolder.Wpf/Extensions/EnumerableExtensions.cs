using System.Collections.Generic;

namespace PhotoFolder.Wpf.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}