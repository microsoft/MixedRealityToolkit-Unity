using System.Collections.Generic;

namespace MixedRealityToolkit
{
    public static class NestedDictionaryHashSetExtension
    {
        public static bool AddUnique<T,U>(this Dictionary<T, HashSet<U>> collection, T t1, U t2)
        {
            if (t1 == null || t2 == null)
            {
                return false;
            }

            if (collection == null)
            {
                collection = new Dictionary<T, HashSet<U>>();
                collection.Add(t1, new HashSet<U>());
                collection[t1].Add(t2);
                return true;
            }

            if (!collection.ContainsKey(t1))
            {
                collection.Add(t1, new HashSet<U>());
                collection[t1].Add(t2);
                return true;
            }

            if (collection[t1].Contains(t2))
                return false;

            collection[t1].Add(t2);
            return true;
        }

        public static bool RemoveUnique<T, U>(this Dictionary<T, HashSet<U>> collection, T t1, U t2)
        {
            if (collection == null || t1 == null || t2 == null || !collection.ContainsKey(t1) || !collection[t1].Contains(t2))
                return false;

            collection[t1].Remove(t2);
            if(collection[t1].Count == 0)
            {
                collection.Remove(t1);
            }

            return true;
        }
    }
}
