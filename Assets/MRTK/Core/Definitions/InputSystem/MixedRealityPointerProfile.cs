﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Configuration profile settings for setting up controller pointers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality/Toolkit/Profiles/Mixed Reality Pointer Profile", fileName = "MixedRealityInputPointerProfile", order = (int)CreateProfileMenuItemIndices.Pointer)]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/input/pointers")]
    public class MixedRealityPointerProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Maximum distance at which all pointers can collide with a GameObject, unless it has an override extent.")]
        private float pointingExtent = 10f;

        /// <summary>
        /// Maximum distance at which all pointers can collide with a GameObject, unless it has an override extent.
        /// </summary>
        public float PointingExtent => pointingExtent;

        [SerializeField]
        [Tooltip("The default LayerMasks, in prioritized order, that are used to determine pointer's target. These layer masks are used if " +
            "the pointer doesn't specify its own override")]
        private LayerMask[] pointingRaycastLayerMasks = { UnityEngine.Physics.DefaultRaycastLayers };

        /// <summary>
        /// The default layerMasks, in prioritized order, that are used to determine the target when raycasting.
        /// </summary>
        public LayerMask[] PointingRaycastLayerMasks => pointingRaycastLayerMasks;

        [SerializeField]
        private bool debugDrawPointingRays = false;

        /// <summary>
        /// Toggle to enable or disable debug pointing rays.
        /// </summary>
        public bool DebugDrawPointingRays => debugDrawPointingRays;

        [SerializeField]
        private Color[] debugDrawPointingRayColors = null;

        /// <summary>
        /// The colors to use when debugging pointer rays.
        /// </summary>
        public Color[] DebugDrawPointingRayColors => debugDrawPointingRayColors;

        [Prefab]
        [SerializeField]
        [Tooltip("The gaze cursor prefab to use on the Gaze pointer.")]
        private GameObject gazeCursorPrefab = null;

        /// <summary>
        /// The gaze cursor prefab to use on the Gaze pointer.
        /// </summary>
        public GameObject GazeCursorPrefab => gazeCursorPrefab;

        [SerializeField]
        [Tooltip("The concrete type of IMixedRealityGazeProvider to use.")]
        [Implements(typeof(IMixedRealityGazeProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType gazeProviderType;

        /// <summary>
        /// The concrete type of <see cref="IMixedRealityGazeProvider"/> to use.
        /// </summary>
        public SystemType GazeProviderType
        {
            get { return gazeProviderType; }
            internal set { gazeProviderType = value; }
        }

        [SerializeField]
        [Tooltip("If true, platform-specific head gaze override is used, when available. Otherwise, the center of the camera frame is used by default.")]
        private bool useHeadGazeOverride = false;

        /// <summary>
        /// If true, platform-specific head gaze override is used, when available. Otherwise, the center of the camera frame is used by default.
        /// </summary>
        public bool UseHeadGazeOverride => useHeadGazeOverride;

        [SerializeField]
        [Tooltip("If true, eye-based tracking will be used as gaze input when available. This field does not control whether eye tracking data is provided.")]
        private bool isEyeTrackingEnabled = false;

        /// <summary>
        /// If true, eye-based tracking will be used as gaze input when available. This field does not control whether eye tracking data is provided.
        /// </summary>
        public bool IsEyeTrackingEnabled
        {
            get { return isEyeTrackingEnabled; }
            internal set { isEyeTrackingEnabled = value; }
        }

        [SerializeField]
        [Tooltip("The Pointer options for this profile.")]
        private PointerOption[] pointerOptions = System.Array.Empty<PointerOption>();

        /// <summary>
        /// The Pointer options for this profile.
        /// </summary>
        public PointerOption[] PointerOptions => pointerOptions;

        [SerializeField]
        [Implements(typeof(IMixedRealityPointerMediator), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("The concrete Pointer Mediator component to use. This is a component that mediates all pointers in system, disabling / enabling them based on the state of other pointers.")]
        private SystemType pointerMediator = null;

        /// <summary>
        /// The concrete Pointer Mediator component to use.
        /// This is a component that mediates all pointers in system, disabling / enabling them based on the state of other pointers.
        /// </summary>
        public SystemType PointerMediator
        {
            get { return pointerMediator; }
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityPrimaryPointerSelector), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("Primary pointer selector implementation to use. This is used by the focus provider to choose the primary pointer.")]
        private SystemType primaryPointerSelector = null;

        /// <summary>
        /// Primary pointer selector implementation to use. This is used by the focus provider to choose the primary pointer.
        /// </summary>
        public SystemType PrimaryPointerSelector
        {
            get { return primaryPointerSelector; }
        }
    }
}
