// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [AddComponentMenu("Scripts/MRTK/SDK/WindowsMixedRealityControllerVisualizer")]
    public class WindowsMixedRealityControllerVisualizer : MixedRealityControllerVisualizer
    {
        protected override Quaternion RotationOffset => rotationOffset * Quaternion.Euler(0, 180.0f, 0);
    }
}