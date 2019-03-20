using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DequeUtility
{
    ///
    /// Based on https://unitylist.com/p/cxl/Web-Rtc-Unity-Plugin-Sample
    internal class Utility
    {
        public static int ClosestPowerOfTwoGreaterThan(int x)
        {
            x--;
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);
            return (x+1);
        }

        /// <summary>
        /// Jon Skeet's excellent reimplementation of LINQ Count.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source">The source IEnumerable.</param>
        /// <returns>The number of items in the source.</returns>
        public static int Count<TSource>(IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // Optimization for ICollection<T> 
            ICollection<TSource> genericCollection = source as ICollection<TSource>;
            if (genericCollection != null)
            {
                return genericCollection.Count;
            }

            // Optimization for ICollection 
            ICollection nonGenericCollection = source as ICollection;
            if (nonGenericCollection != null)
            {
                return nonGenericCollection.Count;
            }

            // Do it the slow way - and make sure we overflow appropriately 
            checked
            {
                int count = 0;
                using (var iterator = source.GetEnumerator())
                {
                    while (iterator.MoveNext())
                    {
                        count++;
                    }
                }
                return count;
            }
        }
    }
}
