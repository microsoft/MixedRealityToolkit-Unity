// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;

#if WMR_ENABLED
using UnityEngine.XR.WindowsMR;
#endif // WMR_ENABLED

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsUniversal,
        "XR SDK Windows Mixed Reality Spatial Mesh Observer",
        "Profiles/DefaultMixedRealitySpatialAwarenessMeshObserverProfile.asset",
        "MixedRealityToolkit.SDK",
        true,
        SupportedUnityXRPipelines.XRSDK)]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/spatial-awareness-getting-started")]
    public class WindowsMixedRealitySpatialMeshObserver :
        GenericXRSDKSpatialMeshObserver
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

        protected override bool? IsActiveLoader =>
#if WMR_ENABLED
            LoaderHelpers.IsLoaderActive("Windows MR Loader");
#else
            false;
#endif // WMR_ENABLED

        private static readonly ProfilerMarker ConfigureObserverVolumePerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver.ConfigureObserverVolume");

        private Vector3 oldObserverOrigin = Vector3.zero;
        private Vector3 oldObservationExtents = Vector3.zero;
        private VolumeType oldObserverVolumeType = VolumeType.None;

        /// <inheritdoc/>
        protected override void ConfigureObserverVolume()
        {
            if (XRSubsystemHelpers.MeshSubsystem == null
                || (oldObserverOrigin == ObserverOrigin
                && oldObservationExtents == ObservationExtents
                && oldObserverVolumeType == ObserverVolumeType))
            {
                return;
            }

            using (ConfigureObserverVolumePerfMarker.Auto())
            {
                Vector3 observerOriginPlayspace = MixedRealityPlayspace.InverseTransformPoint(ObserverOrigin);

                // Update the observer
                switch (ObserverVolumeType)
                {
                    case VolumeType.AxisAlignedCube:
                        XRSubsystemHelpers.MeshSubsystem.SetBoundingVolume(observerOriginPlayspace, ObservationExtents);
                        break;
#if WMR_ENABLED
                    case VolumeType.Sphere:
                        // We use the x value of the extents as the sphere radius
                        XRSubsystemHelpers.MeshSubsystem.SetBoundingVolumeSphere(observerOriginPlayspace, ObservationExtents.x);
                        break;

                    case VolumeType.UserAlignedCube:
                        Quaternion observerRotationPlayspace = Quaternion.Inverse(MixedRealityPlayspace.Rotation) * ObserverRotation;
                        XRSubsystemHelpers.MeshSubsystem.SetBoundingVolumeOrientedBox(observerOriginPlayspace, ObservationExtents, observerRotationPlayspace);
                        break;
#endif // WMR_ENABLED
                    default:
                        Debug.LogError($"Unsupported ObserverVolumeType value {ObserverVolumeType}");
                        break;
                }


                oldObserverOrigin = ObserverOrigin;
                oldObservationExtents = ObservationExtents;
                oldObserverVolumeType = ObserverVolumeType;
            }
        }
    }
}
