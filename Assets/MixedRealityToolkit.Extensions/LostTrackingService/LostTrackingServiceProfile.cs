// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking
{
	[MixedRealityServiceProfile(typeof(ILostTrackingService))]
	[CreateAssetMenu(fileName = "LostTrackingServiceProfile", menuName = "MixedRealityToolkit/LostTrackingService Configuration Profile")]
	public class LostTrackingServiceProfile : BaseMixedRealityProfile
	{
        public GameObject TrackingLostVisualPrefab => trackingLostVisualPrefab;

        public int TrackingLostVisualLayer => trackingLostVisualLayer;

        public LayerMask TrackingLostCullingMask => trackingLostCullingMask;

        public bool HaltTimeWhileTrackingLost => haltTimeWhileTrackingLost;

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