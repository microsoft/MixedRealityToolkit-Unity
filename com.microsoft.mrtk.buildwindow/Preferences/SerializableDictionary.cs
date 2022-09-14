// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Generic Dictionary helper class that handles serialization of keys and values into lists before/after serialization time since Dictionary by itself is not Serializable.
    /// Extends C# Dictionary class to support typical API access methods
    /// </summary>
    /// <typeparam name="TKey">Key type for Dictionary</typeparam>
    /// <typeparam name="TValue">Value type for Dictionary</typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.Clear();

            if (keys.Count != values.Count)
            {
                throw new System.Exception(string.Format($"Error after deserialization in SerializableDictionary class. There are {keys.Count} keys and {values.Count} values after deserialization. Could not load SerializableDictionary"));
            }

            for (int i = 0; i < keys.Count; i++)
            {
                this.Add(keys[i], values[i]);
            }
        }
    }
}
