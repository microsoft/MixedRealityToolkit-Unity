// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

#if WMR_ENABLED
using UnityEngine.XR.WindowsMR;
#endif // WMR_ENABLED

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsUniversal,
        "XR SDK Windows Mixed Reality Spatial Mesh Observer",
        "Profiles/DefaultMixedRealitySpatialAwarenessMeshObserverProfile.asset",
        "MixedRealityToolkit.SDK")]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.html")]
    public class WindowsMixedRealitySpatialMeshObserver :
        GenericXRSDKSpatialMeshObserver,
        IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsMixedRealitySpatialMeshObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        { }

        /// <inheritdoc/>
        protected override void ConfigureObserverVolume()
        {
            if (SpatialAwarenessSystem == null || XRSDKSubsystemHelpers.MeshSubsystem == null)
            {
                return;
            }

            // Update the observer
            switch (ObserverVolumeType)
            {
                case VolumeType.AxisAlignedCube:
                    XRSDKSubsystemHelpers.MeshSubsystem.SetBoundingVolume(ObserverOrigin, ObservationExtents);
                    break;
#if WMR_ENABLED
                case VolumeType.Sphere:
                    // We use the x value of the extents as the sphere radius
                    XRSDKSubsystemHelpers.MeshSubsystem.SetBoundingVolumeSphere(ObserverOrigin, ObservationExtents.x);
                    break;

                case VolumeType.UserAlignedCube:
                    XRSDKSubsystemHelpers.MeshSubsystem.SetBoundingVolumeOrientedBox(ObserverOrigin, ObservationExtents, ObserverRotation);
                    break;
#endif // WMR_ENABLED
                default:
                    Debug.LogError($"Unsupported ObserverVolumeType value {ObserverVolumeType}");
                    break;
            }
        }
    }
}
