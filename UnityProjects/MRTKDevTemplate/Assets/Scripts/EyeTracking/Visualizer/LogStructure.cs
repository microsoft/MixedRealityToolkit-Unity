// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    [AddComponentMenu("Scripts/MRTK/Examples/LogStructure")]
    public abstract class LogStructure : MonoBehaviour
    {
        public virtual string[] GetHeaderColumns()
        {
            return Array.Empty<string>();
        }

        public virtual object[] GetData(string inputType, string inputStatus, EyeTrackingTarget intTarget)
        {
            return Array.Empty<object>();
        }
    }
}
