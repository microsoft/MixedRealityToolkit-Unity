// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities
{
    /// <summary>
    /// Sets global shader variables relating to calibration space transforms
    /// </summary>
    public class CalibrationSpace : MonoBehaviour
    {
        private void Update()
        {
            Shader.SetGlobalMatrix("CalibrationSpaceWorldToLocal", transform.worldToLocalMatrix);
            Shader.SetGlobalMatrix("CalibrationSpaceLocalToWorld", transform.localToWorldMatrix);
        }
    }
}
