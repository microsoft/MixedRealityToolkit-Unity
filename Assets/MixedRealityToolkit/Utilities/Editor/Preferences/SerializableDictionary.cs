// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{

    public class SerializationCallbackScript<K> : MonoBehaviour, ISerializationCallbackReceiver
    {
        public List<int> _keys = new List<int>();
        public List<string> _values = new List<string>();

        //Unity doesn't know how to serialize a Dictionary
        public Dictionary<int, string> _myDictionary = new Dictionary<int, string>()

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in _myDictionary)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            _myDictionary = new Dictionary<int, string>();

            for (var i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
                _myDictionary.Add(_keys[i], _values[i]);
        }
    }
    /*
    [System.Serializable]
    public class SerializableDictionary<K, V> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private K[] keys;
        [SerializeField]
        private V[] values;

        public Dictionary<K, V> Data = new Dictionary<K, V>();

        public V this[K flag]
        {
            get
            {
                return Data[flag];
            }
            set
            {
                Data[flag] = value;
            }
        }

        public void Add(K key, V item)
        {
            Data.Add(key, item);
        }

        public bool ContainsKey(K key)
        {
            return Data.ContainsKey(key);
        }

        static public T New<T>() where T : SerializableDictionary<K, V>, new()
        {
            var result = new T();
            result.Data = new Dictionary<K, V>();
            return result;
        }

        public void OnAfterDeserialize()
        {
            var c = keys.Length;
            Data = new Dictionary<K, V>(c);
            for (int i = 0; i < c; i++)
            {
                Data[keys[i]] = values[i];
            }
            keys = null;
            values = null;
        }

        public void OnBeforeSerialize()
        {
            var c = Data.Count;
            keys = new K[c];
            values = new V[c];
            int i = 0;
            using (var e = Data.GetEnumerator())
                while (e.MoveNext())
                {
                    var kvp = e.Current;
                    keys[i] = kvp.Key;
                    values[i] = kvp.Value;
                    i++;
                }
        }
    }
    */
}
