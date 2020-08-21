// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK.Input
{
    /// <summary>
    /// Additional usages to Unity's CommonUsages.
    /// </summary>
    /// <remarks>These might be plugin-specific, shared across several plug-ins, or custom-defined for new input sources.</remarks>
    public static class CustomUsages
    {
        /// <summary>
        /// Represents the origin of the pointing ray of an input source.
        /// </summary>
        public static readonly InputFeatureUsage<Vector3> PointerPosition = new InputFeatureUsage<Vector3>("PointerPosition");

        /// <summary>
        /// Represents the orientation of the pointing ray of an input source.
        /// </summary>
        public static readonly InputFeatureUsage<Quaternion> PointerRotation = new InputFeatureUsage<Quaternion>("PointerRotation");
    }
}
