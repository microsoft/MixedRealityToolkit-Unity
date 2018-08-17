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
        [Tooltip("Action to raise when pointer event is raised by Touch Screen Input Source.")]
        private MixedRealityInputAction pointerAction;

        /// <summary>
        /// Action to raise when pointer event is raised by Touch Screen Input Source.
        /// </summary>
        public MixedRealityInputAction PointerAction => pointerAction;

        [SerializeField]
        [Tooltip("Action to raise when hold event is raised by Touch Screen Input Source.")]
        private MixedRealityInputAction holdAction;

        /// <summary>
        /// Action to raise when hold event is raised by Touch Screen Input Source.
        /// </summary>
        public MixedRealityInputAction HoldAction => holdAction;
    }
}