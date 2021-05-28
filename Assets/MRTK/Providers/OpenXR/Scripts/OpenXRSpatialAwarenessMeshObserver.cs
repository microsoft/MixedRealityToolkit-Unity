// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

#if MSFT_OPENXR_0_9_4_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)
using Microsoft.MixedReality.OpenXR;
using Unity.Profiling;
using UnityEngine.XR.OpenXR;
#endif // MSFT_OPENXR_0_9_4_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsUniversal,
        "OpenXR Spatial Mesh Observer",
        "Profiles/DefaultMixedRealitySpatialAwarenessMeshObserverProfile.asset",
        "MixedRealityToolkit.SDK",
        true,
        SupportedUnityXRPipelines.XRSDK)]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/spatial-awareness-getting-started")]
    public class OpenXRSpatialAwarenessMeshObserver :
        GenericXRSDKSpatialMeshObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OpenXRSpatialAwarenessMeshObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        { }

        protected override bool? IsActiveLoader =>
#if MSFT_OPENXR_0_9_4_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)
            LoaderHelpers.IsLoaderActive<OpenXRLoaderBase>();
#else
            false;
#endif // MSFT_OPENXR_0_9_4_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)

#if MSFT_OPENXR_0_9_4_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)
        private static readonly ProfilerMarker ApplyUpdatedMeshDisplayOptionPerfMarker = new ProfilerMarker($"[MRTK] {nameof(OpenXRSpatialAwarenessMeshObserver)}.ApplyUpdatedMeshDisplayOption");

        /// <inheritdoc/>
        protected override void ApplyUpdatedMeshDisplayOption(SpatialAwarenessMeshDisplayOptions option)
        {
            using (ApplyUpdatedMeshDisplayOptionPerfMarker.Auto())
            {
                SetMeshComputeSettings(option, LevelOfDetail);
                base.ApplyUpdatedMeshDisplayOption(option);
            }
        }

        private static readonly ProfilerMarker LookupTriangleDensityPerfMarker = new ProfilerMarker($"[MRTK] {nameof(OpenXRSpatialAwarenessMeshObserver)}.LookupTriangleDensity");

        /// <inheritdoc/>
        protected override int LookupTriangleDensity(SpatialAwarenessMeshLevelOfDetail levelOfDetail)
        {
            using (LookupTriangleDensityPerfMarker.Auto())
            {
                if (Application.isPlaying && SetMeshComputeSettings(DisplayOption, levelOfDetail))
                {
                    return (int)levelOfDetail;
                }
                else
                {
                    return base.LookupTriangleDensity(levelOfDetail);
                }
            }
        }

        private bool SetMeshComputeSettings(SpatialAwarenessMeshDisplayOptions option, SpatialAwarenessMeshLevelOfDetail levelOfDetail)
        {
            MeshComputeSettings settings = new MeshComputeSettings
            {
                MeshType = (option == SpatialAwarenessMeshDisplayOptions.Visible) ? MeshType.Visual : MeshType.Collider,
                VisualMeshLevelOfDetail = MapMRTKLevelOfDetailToOpenXR(levelOfDetail),
                MeshComputeConsistency = MeshComputeConsistency.OcclusionOptimized,
            };

            return MeshSettings.TrySetMeshComputeSettings(settings);
        }

        private VisualMeshLevelOfDetail MapMRTKLevelOfDetailToOpenXR(SpatialAwarenessMeshLevelOfDetail levelOfDetail)
        {
            switch (levelOfDetail)
            {
                case SpatialAwarenessMeshLevelOfDetail.Coarse:
                    return VisualMeshLevelOfDetail.Coarse;
                case SpatialAwarenessMeshLevelOfDetail.Medium:
                    return VisualMeshLevelOfDetail.Medium;
                case SpatialAwarenessMeshLevelOfDetail.Fine:
                    return VisualMeshLevelOfDetail.Fine;
                case SpatialAwarenessMeshLevelOfDetail.Unlimited:
                    return VisualMeshLevelOfDetail.Unlimited;
                case SpatialAwarenessMeshLevelOfDetail.Custom:
                default:
                    Debug.LogError($"Unsupported LevelOfDetail value {levelOfDetail}. Defaulting to {VisualMeshLevelOfDetail.Coarse}");
                    return VisualMeshLevelOfDetail.Coarse;
            }
        }
#endif // MSFT_OPENXR_0_9_4_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)
    }
}
