// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Teleport;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Utility class to easily access references to runtime Mixed Reality Toolkit Services
    /// </summary>
    public static class MRTKServicesUtils
    {
        private static IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system
        /// </summary>
        public static IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        private static IMixedRealityBoundarySystem boundarySystem = null;

        /// <summary>
        /// The active instance of the boundary system
        /// </summary>
        public static IMixedRealityBoundarySystem BoundarySystem
        {
            get
            {
                if (boundarySystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityBoundarySystem>(out boundarySystem);
                }
                return boundarySystem;
            }
        }

        private static IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;

        /// <summary>
        /// The active instance of the spatial awareness system
        /// </summary>
        public static IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem
        {
            get
            {
                if (spatialAwarenessSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out spatialAwarenessSystem);
                }
                return spatialAwarenessSystem;
            }
        }

        private static IMixedRealityTeleportSystem teleportSystem = null;
        /// <summary>
        /// The active instance of the teleport system
        /// </summary>
        public static IMixedRealityTeleportSystem TeleportSystem
        {
            get
            {
                if (teleportSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityTeleportSystem>(out teleportSystem);
                }

                return teleportSystem;
            }
        }

        private static IMixedRealityDiagnosticsSystem diagnosticsSystem = null;
        /// <summary>
        /// The active instance of the diagnostics system
        /// </summary>
        public static IMixedRealityDiagnosticsSystem DiagnosticsSystem
        {
            get
            {
                if (diagnosticsSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityDiagnosticsSystem>(out diagnosticsSystem);
                }
                return diagnosticsSystem;
            }
        }

        public static void InvalidateReferences()
        {
            diagnosticsSystem = null;
            teleportSystem = null;
            spatialAwarenessSystem = null;
            boundarySystem = null;
            inputSystem = null;
        }
    }
}
