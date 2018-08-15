// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up input source actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Input Source Options Profile", fileName = "MixedRealitySpeechCommandsProfile", order = 7)]
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
        [Tooltip("Action to raise when pointer down event is raised by Touch Screen Input Source.")]
        private MixedRealityInputAction pointerDownAction;

        /// <summary>
        /// Action to raise when pointer down event is raised by Touch Screen Input Source.
        /// </summary>
        public MixedRealityInputAction PointerDownAction => pointerDownAction;

        [SerializeField]
        [Tooltip("Action to raise when pointer clicked event is raised by Touch Screen Input Source.")]
        private MixedRealityInputAction pointerClickedAction;

        /// <summary>
        /// Action to raise when pointer clicked event is raised by Touch Screen Input Source.
        /// </summary>
        public MixedRealityInputAction PointerClickedAction => pointerClickedAction;

        [SerializeField]
        [Tooltip("Action to raise when pointer up event is raised by Touch Screen Input Source.")]
        private MixedRealityInputAction pointerUpAction;

        /// <summary>
        /// Action to raise when pointer up event is raised by Touch Screen Input Source.
        /// </summary>
        public MixedRealityInputAction PointerUpAction => pointerUpAction;

        [SerializeField]
        [Tooltip("Action to raise when hold event is raised by Touch Screen Input Source.")]
        private MixedRealityInputAction holdStartedAction;

        /// <summary>
        /// Action to raise when hold event is raised by Touch Screen Input Source.
        /// </summary>
        public MixedRealityInputAction HoldStartedAction => holdStartedAction;

        [SerializeField]
        [Tooltip("Action to raise when hold updated event is raised by Touch Screen Input Source.")]
        private MixedRealityInputAction holdUpdatedAction;

        /// <summary>
        /// Action to raise when hold updated event is raised by Touch Screen Input Source.
        /// </summary>
        public MixedRealityInputAction HoldUpdatedAction => holdUpdatedAction;

        [SerializeField]
        [Tooltip("Action to raise when hold completed event is raised by Touch Screen Input Source.")]
        private MixedRealityInputAction holdCompletedAction;

        /// <summary>
        /// Action to raise when hold completed event is raised by Touch Screen Input Source.
        /// </summary>
        public MixedRealityInputAction HoldCompletedAction => holdCompletedAction;

        [SerializeField]
        [Tooltip("Action to raise when hold canceled event is raised by Touch Screen Input Source.")]
        private MixedRealityInputAction holdCanceledAction;

        /// <summary>
        /// Action to raise when hold canceled event is raised by Touch Screen Input Source.
        /// </summary>
        public MixedRealityInputAction HoldCanceledAction => holdCanceledAction;
    }
}