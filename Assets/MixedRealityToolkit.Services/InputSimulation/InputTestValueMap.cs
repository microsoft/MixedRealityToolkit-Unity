// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Dictionary of values to test for.
    /// </summary>
    [System.Serializable]
    public class InputTestValueMap : ISerializationCallbackReceiver
    {
        [System.Serializable]
        internal class Key : Tuple<int, string>
        {
            public Key(int componentID, string name) : base(componentID, name)
            {}
        }

        Dictionary<Key, InputTestCurve<int>> intValues;
        Dictionary<Key, InputTestCurve<bool>> boolValues;

        [SerializeField]
        [HideInInspector]
        internal List<int> serializedKeyComponentIDs;
        [SerializeField]
        [HideInInspector]
        internal List<string> serializedKeyNames;

        [System.Serializable]
        internal class IntCurve : InputTestCurve<int>
        {}
        [System.Serializable]
        internal class BoolCurve : InputTestCurve<bool>
        {}

        [SerializeField]
        [HideInInspector]
        internal IntCurve serializedIntValues;
        [SerializeField]
        [HideInInspector]
        internal BoolCurve serializedBoolValues;

        public void OnBeforeSerialize()
        {
            int totalCount = intValues.Count + boolValues.Count;
            serializedKeyComponentIDs = new List<int>(totalCount);
            serializedKeyNames = new List<string>(totalCount);
            serializedIntValues = new IntCurve();
            serializedBoolValues = new BoolCurve();
            foreach (var item in intValues)
            {
                serializedKeyComponentIDs.Add(item.Key.Item1);
                serializedKeyNames.Add(item.Key.Item2);
                serializedIntValues.Add(item.Value)
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < serializedKeyComponentID.Count; ++i)
            {
                // Add(new InputTestPropertyKey(serializedKeyComponentID[i], serializedKeyName[i]), serializedValue[i]);
            }
            serializedKeyComponentID = null;
            serializedKeyName = null;
            serializedValue = null;
        }
    }
}