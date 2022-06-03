// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
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
        private List<SerializableDictionaryEntry> entries = new List<SerializableDictionaryEntry>();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            entries.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                entries.Add(new SerializableDictionaryEntry(pair.Key, pair.Value));
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.Clear();

            foreach (SerializableDictionaryEntry entry in entries)
            {
                this.Add(entry.Key, entry.Value);
            }
        }

        [Serializable]
        private struct SerializableDictionaryEntry
        {
            [SerializeField]
            private TKey key;

            public TKey Key => key;

            [SerializeField]
            private TValue value;

            public TValue Value => value;

            public SerializableDictionaryEntry(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
        }
    }
}
