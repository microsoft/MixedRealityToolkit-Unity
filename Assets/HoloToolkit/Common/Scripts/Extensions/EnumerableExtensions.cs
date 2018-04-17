using System;
using System.Collections.Generic;

namespace HoloToolkit.Unity
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the max element based on the provided comparer or the default value when the list is empty
        /// </summary>
        /// <returns>Max or default value of T</returns>
        public static T MaxOrDefault<T>(this IEnumerable<T> items, IComparer<T> comparer = null)
        {
            if (items == null) { throw new ArgumentNullException("items"); }
            comparer = comparer ?? Comparer<T>.Default;

            using (var enumerator = items.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return default(T);
                }

                var max = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    if (comparer.Compare(max, enumerator.Current) < 0)
                    {
                        max = enumerator.Current;
                    }
                }
                return max;
            }
        }
    }
}
