// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    public class LogStructure : MonoBehaviour
    {
        public virtual string[] GetHeaderColumns()
        {
            return new string[0];
        }

        public virtual object[] GetData(string inputType, string inputStatus, EyeTrackingTarget intTarget)
        {
            return new object[0];
        }
    }
}