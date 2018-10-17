using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.SpatialAwareness
{
    /// <summary>
    /// Moves the spatial observer's origin to match the camera's position.
    /// </summary>
    public class SpatialObserverMover : MonoBehaviour
    {
        private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;

        /// <summary>
        /// The active <see cref="IMixedRealitySpatialAwarenessSystem"/> implementation.
        /// </summary>
        private IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem => spatialAwarenessSystem ?? (spatialAwarenessSystem = MixedRealityManager.SpatialAwarenessSystem);

        void Update()
        {
            if (SpatialAwarenessSystem == null) { return; }

            // Set the origin of the observer to the current camera position.
            SpatialAwarenessSystem.ObserverOrigin = CameraCache.Main.transform.position;
        }
    }
}