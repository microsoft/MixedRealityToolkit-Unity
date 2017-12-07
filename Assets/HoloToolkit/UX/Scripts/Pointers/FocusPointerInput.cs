// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif

namespace HoloToolkit.Unity.UX
{
    /// <summary>
    /// Routes controller input to a physics pointer
    /// </summary>
    public class FocusPointerInput : AttachToController, IInputHandler
    {
        [SerializeField]
        [DropDownComponent(true,true)]
        private ControllerPointerBase pointer = null;
        [SerializeField]
        private InteractionSourcePressInfo activeHoldType = InteractionSourcePressInfo.Select;
        [SerializeField]
        private InteractionSourcePressInfo selectPressType = InteractionSourcePressInfo.Select;

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
            if (eventData.SourceId == SourceId)
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

        public void OnInputUp(InputEventData eventData)
        {
            if (eventData.SourceId == SourceId)
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

        private void Update()
        {
            if (!InputManager.IsInitialized)
                return;

            if (!IsAttached)
                return;

            if (pointer.InteractionEnabled)
            {
                InputManager.Instance.ApplyEventOrigin(SourceId, pointer.EventOrign);
            }
            else
            {
                InputManager.Instance.RemoveEventOrigin(SourceId, pointer.EventOrign);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (!InputManager.IsInitialized)
            {
                return;
            }

            InputManager.Instance.RemoveEventOrigin(SourceId, pointer.EventOrign);
            InputManager.Instance.RemoveGlobalListener(gameObject);
        }
        
        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(FocusPointerInput))]
        public class CustomEditor : MRTKEditor { }
#endif
        #endregion
    }
}