// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This component handles pointer clicks from all types of input sources.<para/>
    /// i.e. a primary mouse button click, motion controller selection press, or hand tap.
    /// </summary>
    [System.Obsolete("Use PointerHandler instead of PointerClickHandler", true)]
    [AddComponentMenu("Scripts/MRTK/Obsolete/PointerClickHandler")]
    public class PointerClickHandler : BaseInputHandler, IMixedRealityPointerHandler
    {
        [SerializeField]
        [Tooltip("The input actions to be recognized on pointer up.")]
        private InputActionEventPair onPointerUpActionEvent = default(InputActionEventPair);

        [SerializeField]
        [Tooltip("The input actions to be recognized on pointer down.")]
        private InputActionEventPair onPointerDownActionEvent = default(InputActionEventPair);

        [SerializeField]
        [Tooltip("The input actions to be recognized on pointer clicked.")]
        private InputActionEventPair onPointerClickedActionEvent = default(InputActionEventPair);

        private void Awake()
        {
            Debug.LogError("PointerClickHandler is deprecated. Use PointerHandler instead", this.gameObject);
        }

        #region InputSystemGlobalHandlerListener Implementation

        /// <inheritdoc />
        protected override void RegisterHandlers()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
        }

        /// <inheritdoc />
        protected override void UnregisterHandlers()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
        }

        #endregion InputSystemGlobalHandlerListener Implementation

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
