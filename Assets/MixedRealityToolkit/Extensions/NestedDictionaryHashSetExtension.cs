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
#if ENABLE_DOTNET
                collection = default(Dictionary<T, HashSet<U>>);
                collection.Add(t1, default(HashSet<U>));
#else
                collection = default;
                collection.Add(t1, default);
#endif
                collection[t1].Add(t2);
                return true;
            }

            if (!collection.ContainsKey(t1))
            {
#if ENABLE_DOTNET
                collection.Add(t1, default(HashSet<U>));
#else
                collection.Add(t1, default);
#endif
            }

            if (collection[t1].Contains(t2))
                return false;

            collection[t1].Add(t2);
            return true;
        }

        public static bool RemoveUnique<T, U>(this Dictionary<T, HashSet<U>> collection, T t1, U t2)
        {
            if (t1 == null || t2 == null || collection == null || !collection.ContainsKey(t1) || !collection[t1].Contains(t2))
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
