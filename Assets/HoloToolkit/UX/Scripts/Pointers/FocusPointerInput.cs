// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.UX
{
    /// <summary>
    /// Routes controller input to a physics pointer
    /// </summary>
    public class FocusPointerInput : AttachToController, IPointerHandler
    {
        [SerializeField]
        [DropDownComponent(true, true)]
        private ControllerPointerBase pointer = null;

#if UNITY_WSA
        [SerializeField]
        private InteractionSourcePressType activeHoldType = InteractionSourcePressType.Select;

        [SerializeField]
        private InteractionSourcePressType selectPressType = InteractionSourcePressType.Select;
#endif

        [SerializeField]
        private bool interactionRequiresHold = false;

        protected override void OnAttachToController()
        {
            if (pointer == null)
            {
                Debug.LogError("Pointer cannot be null.");
                return;
            }

            // Subscribe to interaction events
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        protected override void OnDetachFromController()
        {
            if (pointer == null)
            {
                Debug.LogError("Pointer cannot be null.");
                return;
            }

            // Unsubscribe from interaction events
            InputManager.Instance.RemoveGlobalListener(gameObject);
        }

        public void OnInputDown(InputEventData eventData)
        {
            if (eventData.SourceId == pointer.SourceId)
            {
                if (interactionRequiresHold && eventData.PressType == activeHoldType)
                {
                    pointer.InteractionEnabled = true;
                }

                if (eventData.PressType == selectPressType)
                {
                    pointer.OnSelectPressed();
                }
            }
        }

        private void Update()
        {
            if (!InputManager.IsInitialized)
                return;

            if (!IsAttached)
                return;

        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (!InputManager.IsInitialized)
            {
                return;
            }

            InputManager.Instance.RemoveGlobalListener(gameObject);
        }

        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(FocusPointerInput))]
        public class CustomEditor : MRTKEditor { }
#endif
        #endregion

        void IPointerHandler.OnPointerUp(ClickEventData eventData)
        {
            if (eventData.SourceId == pointer.SourceId)
            {
                if (interactionRequiresHold && eventData.PressType == activeHoldType)
                {
                    pointer.InteractionEnabled = false;
                }

                if (eventData.PressType == selectPressType)
                {
                    pointer.OnSelectReleased();
                }
            }
        }

        void IPointerHandler.OnPointerDown(ClickEventData eventData) { }

        void IPointerHandler.OnPointerClicked(ClickEventData eventData) { }
    }
}