using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static void Add<T,U>(this Dictionary<T,HashSet<U>> dictionary, T key, U value)
    {
        if (key == null || value == null)
            return;

        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, new HashSet<U>());
        }

        dictionary[key].Add(value);
    }

    public static void Remove<T, U>(this Dictionary<T, HashSet<U>> dictionary, T key, U value)
    {
        if (key == default || value == default)
            return;

        dictionary[key].Remove(value);

        if (dictionary[key].Count == 0)
        {
            dictionary.Remove(key);
        }
    }
}
