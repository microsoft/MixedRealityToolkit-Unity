// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Configuration profile settings for setting up controller pointers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Pointer Profile", fileName = "MixedRealityInputPointerProfile", order = (int)CreateProfileMenuItemIndices.Pointer)]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Input/Pointers.html")]
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
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.")]
        private LayerMask[] pointingRaycastLayerMasks = { UnityEngine.Physics.DefaultRaycastLayers };

        /// <summary>
        /// The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.
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
        [Tooltip("The Pointer options for this profile.")]
        private PointerOption[] pointerOptions = new PointerOption[0];

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
