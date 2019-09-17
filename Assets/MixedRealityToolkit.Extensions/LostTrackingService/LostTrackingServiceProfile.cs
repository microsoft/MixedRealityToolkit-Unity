using Microsoft.MixedReality.Toolkit.Physics;
using System;
using UnityEditor.Experimental.UIElements;
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
        private GameObject trackingLostVisualPrefab = null;

        [SerializeField]
        [PhysicsLayer]
        private int trackingLostVisualLayer = 31;

        [SerializeField]
        private LayerMask trackingLostCullingMask = new LayerMask();

        [SerializeField]
        private bool haltTimeWhileTrackingLost = true;

        [SerializeField]
        private bool haltAudioWhileTrackingLost = true;

        private void OnValidate()
        {
            // Ensure that the tracking lost culling mask contains the visual's layer
            // Otherwise it won't be visible when tracking is lost
            trackingLostCullingMask.value |= 1 << trackingLostVisualLayer;
        }
    }
}