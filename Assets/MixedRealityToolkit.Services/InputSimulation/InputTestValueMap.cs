// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Devices.Hands;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Services.InputSimulation
{
    [System.Serializable]
    public class InputTestPropertyKey : Tuple<int, string>
    {
        public InputTestPropertyKey(int componentID, string name) : base(componentID, name)
        {}
    }

    /// <summary>
    /// Dictionary of values to test for.
    /// </summary>
    [System.Serializable]
    public class InputTestExpectedValueMap : Dictionary<InputTestPropertyKey, InputTestCurve<object>>, ISerializationCallbackReceiver
    {
        [SerializeField]
        [HideInInspector]
        internal List<int> serializedKeyComponentID;
        [SerializeField]
        [HideInInspector]
        internal List<string> serializedKeyName;

        [System.Serializable]
        internal class SerializableValueCurve : ISerializationCallbackReceiver
        {
            public SerializableValueCurve(InputTestCurve<object> curve)
            {

            }

            public void OnBeforeSerialize()
            {
            }

            public void OnAfterDeserialize()
            {
            }
        }
        [SerializeField]
        [HideInInspector]
        internal object serializedOopsie = 4;
        [SerializeField]
        [HideInInspector]
        internal InputTestCurve<object> serializedTets = new InputTestCurve<object>();
        [SerializeField]
        [HideInInspector]
        internal List<SerializableValueCurve> serializedValue;

        public void OnBeforeSerialize()
        {
            serializedKeyComponentID = new List<int>(Count);
            serializedKeyName = new List<string>(Count);
            serializedValue = new List<SerializableValueCurve>(Count);
            foreach (var item in this)
            {
                serializedKeyComponentID.Add(item.Key.Item1);
                serializedKeyName.Add(item.Key.Item2);
                // var curve = new SerializableValueCurve();
                // curve.``
                // serializedValue.Add();
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < serializedKeyComponentID.Count; ++i)
            {
                Add(new InputTestPropertyKey(serializedKeyComponentID[i], serializedKeyName[i]), serializedValue[i]);
            }
            serializedKeyComponentID = null;
            serializedKeyName = null;
            serializedValue = null;
        }
    }
}