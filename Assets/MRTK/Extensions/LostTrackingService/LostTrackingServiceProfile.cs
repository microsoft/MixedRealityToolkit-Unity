// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking
{
    /// <summary>
    /// The profile definition for an <see cref="ILostTrackingService"/>.
    /// </summary>
    [MixedRealityServiceProfile(typeof(ILostTrackingService))]
    [CreateAssetMenu(fileName = "LostTrackingServiceProfile", menuName = "Mixed Reality/Toolkit/Extensions/Lost Tracking Service/Mixed Reality Lost Tracking Service Profile")]
    public class LostTrackingServiceProfile : BaseMixedRealityProfile
    {
        /// <summary>
        /// Prefab for the lost tracking visual. Must include a component that implements ILostTrackingVisual.
        /// A radial solver or tagalong script is recommended as well.
        /// </summary>
        public GameObject TrackingLostVisualPrefab => trackingLostVisualPrefab;

        /// <summary>
        /// The layer used to display the lost tracking visual.
        /// </summary>
        public int TrackingLostVisualLayer => trackingLostVisualLayer;

        /// <summary>
        /// The culling mask to use when tracking is lost. The tracking lost visual layer is automatically included.
        /// </summary>
        public LayerMask TrackingLostCullingMask => trackingLostCullingMask;

        /// <summary>
        /// If true, the service will set timescale to 0 while tracking is lost.
        /// </summary>
        public bool HaltTimeWhileTrackingLost => haltTimeWhileTrackingLost;

        /// <summary>
        /// If true, the service will pause audio while tracking is lost.
        /// </summary>
        public bool HaltAudioOnTrackingLost => haltAudioWhileTrackingLost;

        [SerializeField]
        [Tooltip("Prefab for the lost tracking visual. Must include a component that implements ILostTrackingVisual. A radial solver or tagalong script is recommended as well.")]
        private GameObject trackingLostVisualPrefab = null;

        [SerializeField]
        [PhysicsLayer]
        [Tooltip("The layer used to display the lost tracking visual.")]
        private int trackingLostVisualLayer = 31;

        [SerializeField]
        [Tooltip("The culling mask to use when tracking is lost. The tracking lost visual layer is automatically included.")]
        private LayerMask trackingLostCullingMask = new LayerMask();

        [SerializeField]
        [Tooltip("If true, the service will set timescale to 0 while tracking is lost.")]
        private bool haltTimeWhileTrackingLost = true;

        [SerializeField]
        [Tooltip("If true, the service will pause audio while tracking is lost.")]
        private bool haltAudioWhileTrackingLost = true;

        private void OnValidate()
        {
            // Ensure that the tracking lost culling mask contains the visual's layer
            // Otherwise it won't be visible when tracking is lost
            trackingLostCullingMask.value |= 1 << trackingLostVisualLayer;
        }
    }
}