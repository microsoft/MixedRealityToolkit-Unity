// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl
{
    /// <summary>
    /// Configuration for <see cref="RotationHandles"/> used in <see cref="BoundsControl"/>
    /// This class provides all data members needed to create rotation handles for <see cref="BoundsControl"/>
    /// </summary>
    [CreateAssetMenu(fileName = "RotationHandlesConfiguration", menuName = "Mixed Reality Toolkit/Experimental/Bounds Control/Rotation Handles Configuration")]
    public class RotationHandlesConfiguration : PerAxisHandlesConfiguration
    {
        /// <summary>
        /// Fabricates an instance of RotationHandles, applying
        /// this config to it whilst creating it.
        /// </summary>
        /// <returns>New RotationHandles</returns>
        internal virtual RotationHandles ConstructInstance()
        {
            // Return a new RotationHandles, using this config as the active config.
            return new RotationHandles(this);
        }
    }
}
