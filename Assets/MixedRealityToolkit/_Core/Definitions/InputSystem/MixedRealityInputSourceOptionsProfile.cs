// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up input source actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Input Source Options Profile", fileName = "MixedRealitySpeechCommandsProfile", order = (int)CreateProfileMenuItemIndices.GenericInputSourceOptions)]
    public class MixedRealityInputSourceOptionsProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Action to raise when hypothesis event is raised by Dictation Input Source.")]
        private MixedRealityInputAction hypothesisAction;

        /// <summary>
        /// Action to raise when hypothesis event is raised by Dictation Input Source.
        /// </summary>
        public MixedRealityInputAction HypothesisAction => hypothesisAction;

        [SerializeField]
        [Tooltip("Action to raise when result event is raised by Dictation Input Source.")]
        private MixedRealityInputAction resultAction;

        /// <summary>
        /// Action to raise when result event is raised by Dictation Input Source.
        /// </summary>
        public MixedRealityInputAction ResultAction => resultAction;

        [SerializeField]
        [Tooltip("Action to raise when complete event is raised by Dictation Input Source.")]
        private MixedRealityInputAction completeAction;

        /// <summary>
        /// Action to raise when complete event is raised by Dictation Input Source.
        /// </summary>
        public MixedRealityInputAction CompleteAction => completeAction;

        [SerializeField]
        [Tooltip("Action to raise when error event is raised by Dictation Input Source.")]
        private MixedRealityInputAction errorAction;

        /// <summary>
        /// Action to raise when error event is raised by Dictation Input Source.
        /// </summary>
        public MixedRealityInputAction ErrorAction => errorAction;

        [SerializeField]
        [Tooltip("Action to raise when pointer event is raised.")]
        private MixedRealityInputAction pointerAction;

        /// <summary>
        /// Action to raise when pointer event is raised.
        /// </summary>
        public MixedRealityInputAction PointerAction => pointerAction;

        [SerializeField]
        [Tooltip("Action to raise when hold event is raised.")]
        private MixedRealityInputAction holdAction;

        /// <summary>
        /// Action to raise when hold event is raised.
        /// </summary>
        public MixedRealityInputAction HoldAction => holdAction;

        [SerializeField]
        [Tooltip("Action to raise when navigation event is raised.")]
        private MixedRealityInputAction navigationAction;

        /// <summary>
        /// Action to raise when navigation event is raised.
        /// </summary>
        public MixedRealityInputAction NavigationAction => navigationAction;

        [SerializeField]
        [Tooltip(" Action to raise when manipulation event is raised.")]
        private MixedRealityInputAction manipulationAction;

        /// <summary>
        /// Action to raise when manipulation event is raised.
        /// </summary>
        public MixedRealityInputAction ManipulationAction => manipulationAction;

        [SerializeField]
        [Tooltip("Should the gesture input source use rails navigation?")]
        private bool useRailsNavigation;

        /// <summary>
        /// Should the gesture input source use rails navigation?
        /// </summary>
        public bool UseRailsNavigation => useRailsNavigation;
    }
}