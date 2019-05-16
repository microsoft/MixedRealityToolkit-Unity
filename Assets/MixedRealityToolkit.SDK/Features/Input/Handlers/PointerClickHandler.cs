// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This component handles pointer clicks from all types of input sources.<para/>
    /// i.e. a primary mouse button click, motion controller selection press, or hand tap.
    /// </summary>
    public class PointerClickHandler : BaseInputHandler, IMixedRealityPointerHandler
    {
        [SerializeField]
        [Tooltip("The input actions to be recognized on pointer up.")]
        private InputActionEventPair onPointerUpActionEvent;

        [SerializeField]
        [Tooltip("The input actions to be recognized on pointer down.")]
        private InputActionEventPair onPointerDownActionEvent;

        [SerializeField]
        [Tooltip("The input actions to be recognized on pointer clicked.")]
        private InputActionEventPair onPointerClickedActionEvent;

        #region IMixedRealityPointerHandler Implementation

        /// <inheritdoc />
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (onPointerUpActionEvent.InputAction == MixedRealityInputAction.None) { return; }

            if (onPointerUpActionEvent.InputAction == eventData.MixedRealityInputAction)
            {
                onPointerUpActionEvent.UnityEvent.Invoke();
            }
        }

        /// <inheritdoc />
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (onPointerDownActionEvent.InputAction == MixedRealityInputAction.None) { return; }

            if (onPointerDownActionEvent.InputAction == eventData.MixedRealityInputAction)
            {
                onPointerDownActionEvent.UnityEvent.Invoke();
            }
        }

        /// <inheritdoc />
        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        /// <inheritdoc />
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (onPointerClickedActionEvent.InputAction == MixedRealityInputAction.None) { return; }

            if (onPointerClickedActionEvent.InputAction == eventData.MixedRealityInputAction)
            {
                onPointerClickedActionEvent.UnityEvent.Invoke();
            }
        }

        #endregion IMixedRealityPointerHandler Implementation
    }
}
