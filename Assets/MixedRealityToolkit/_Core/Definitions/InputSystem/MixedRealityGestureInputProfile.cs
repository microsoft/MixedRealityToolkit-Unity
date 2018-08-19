// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up input source actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Gesture Input Profile", fileName = "MixedRealityGestureInputProfile", order = (int)CreateProfileMenuItemIndices.Gestures)]
    public class MixedRealityGestureInputProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Action to raise when pointer event is raised.")]
        private MixedRealityInputAction pointerAction = MixedRealityInputAction.None;

        /// <summary>
        /// Action to raise when pointer event is raised.
        /// </summary>
        public MixedRealityInputAction PointerAction => pointerAction;

        [SerializeField]
        [Tooltip("Action to raise when hold event is raised.")]
        private MixedRealityInputAction holdAction = MixedRealityInputAction.None;

        /// <summary>
        /// Action to raise when hold event is raised.
        /// </summary>
        public MixedRealityInputAction HoldAction => holdAction;

        [SerializeField]
        [Tooltip("Action to raise when navigation event is raised.")]
        private MixedRealityInputAction navigationAction = MixedRealityInputAction.None;

        /// <summary>
        /// Action to raise when navigation event is raised.
        /// </summary>
        public MixedRealityInputAction NavigationAction => navigationAction;

        [SerializeField]
        [Tooltip(" Action to raise when manipulation event is raised.")]
        private MixedRealityInputAction manipulationAction = MixedRealityInputAction.None;

        /// <summary>
        /// Action to raise when manipulation event is raised.
        /// </summary>
        public MixedRealityInputAction ManipulationAction => manipulationAction;

        [SerializeField]
        [Tooltip("Should the gesture input source use rails navigation?")]
        private bool useRailsNavigation = false;

        /// <summary>
        /// Should the gesture input source use rails navigation?
        /// </summary>
        public bool UseRailsNavigation => useRailsNavigation;
    }
}