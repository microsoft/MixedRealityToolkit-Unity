// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [AddComponentMenu("Scripts/Microsoft/MRTK/Input/MRTK Spatial Mouse Interactor")]
    public class SpatialMouseInteractor : XRRayInteractor, IRayInteractor
    {
        [SerializeField]
        private InputActionProperty mouseMoveAction;

        [SerializeField]
        private InputActionProperty mouseScrollAction;

        [SerializeField]
        [Tooltip("The scale factor to apply to the mouse deltas")]
        public float mouseSensitivity = .05f;

        [SerializeField]
        [Tooltip("The scale factor to apply to the mouse wheel")]
        public float mouseWheelSensitivity = .002f;

        [SerializeField]
        [Tooltip("The time (in seconds) of no mouse activity before hiding the mouse cursor")]
        public float mouseHideThreshold = 20.0f;

        [SerializeField]
        [Tooltip("The time (in seconds) of no mouse activity before reseting the mouse cursor to the center of the FoV")]
        public float mouseResetThreshold = 0.2f;

        private CursorLockMode restoreLockState;

        private float timeSinceLastMouseEvent = Mathf.Infinity;

        /// <summary>
        /// Returns true if mouse is actively in use. Should be used to show/hide mouse-specific feedback (e.g. cursor).
        /// </summary>
        public bool IsInUse => hasSelection || (timeSinceLastMouseEvent < mouseHideThreshold);

        private void Awake()
        {
            InputSystem.onDeviceChange +=
               (device, change) =>
               {
                   switch (change)
                   {
                       case InputDeviceChange.Added:
                           Debug.Log($"Device {device} was added");
                           break;
                       case InputDeviceChange.Removed:
                           Debug.Log($"Device {device} was removed");
                           break;
                   }
               };
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            //test();

            Debug.Log("Enabling");

            if (mouseMoveAction != null)
            {
                Debug.Log("MouseMoveAction");
                Debug.Log(mouseMoveAction);
                Debug.Log(mouseMoveAction.action.controls);
                Debug.Log(mouseMoveAction.action.controls.Count);
                mouseMoveAction.action.performed += OnMouseMove;
                mouseMoveAction.EnableDirectAction();
            }
            if (mouseScrollAction != null)
            {
                mouseScrollAction.action.performed += OnMouseScroll;
                mouseScrollAction.EnableDirectAction();
            }

            restoreLockState = Cursor.lockState;
            Cursor.lockState =  CursorLockMode.Locked;
        }

        System.Collections.IEnumerator test()
        {
            yield return null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Cursor.lockState =  restoreLockState;

            if (mouseMoveAction != null)
            {
                mouseMoveAction.action.performed -= OnMouseMove;
                mouseMoveAction.DisableDirectAction();
            }
            if (mouseScrollAction != null)
            {
                mouseScrollAction.action.performed -= OnMouseScroll;
                mouseScrollAction.DisableDirectAction();
            }
        }

        private void Update()
        {
            timeSinceLastMouseEvent += Time.deltaTime;
        }

        private void OnMouseMove(InputAction.CallbackContext context)
        {
            Debug.Log("MouseMove");
            Vector2 mouseDelta = context.ReadValue<Vector2>();

            Vector3 screenPoint = Camera.main.WorldToViewportPoint(rayOriginTransform.position + rayOriginTransform.forward);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            // Reset the cursor to the center of the FoV if it's hidden or it's out of the user's FoV and 
            // the user hasn't interacted with it in the mouseResetThreshold
            bool shouldResetMousePosition = !IsInUse || 
                (!onScreen && (timeSinceLastMouseEvent >= mouseResetThreshold));

            if (shouldResetMousePosition)
            {
                rayOriginTransform.rotation = Camera.main.transform.rotation;
            }
            else
            {
                float rotateByRadiansX = mouseDelta.x * mouseSensitivity;
                float rotateByRadiansY = -mouseDelta.y * mouseSensitivity;

                rayOriginTransform.RotateAround(rayOriginTransform.position, Camera.main.transform.up, rotateByRadiansX);
                rayOriginTransform.RotateAround(rayOriginTransform.position, Camera.main.transform.right, rotateByRadiansY);
                
                if (hasSelection)
                {
                    float distanceToAttachTransform = Vector3.Distance(rayOriginTransform.position, attachTransform.position);
                    attachTransform.position = rayOriginTransform.position + rayOriginTransform.forward * distanceToAttachTransform;
                }
            }

            timeSinceLastMouseEvent = 0;
        }

        private void OnMouseScroll(InputAction.CallbackContext context)
        {
            Debug.Log("MouseScroll");
            Vector2 scrollDelta = context.ReadValue<Vector2>();

            if (hasSelection)
            {
                float translateByZ = (float)scrollDelta.y * mouseWheelSensitivity;
                float currentDistanceToAttachTransform = Vector3.Distance(rayOriginTransform.position, attachTransform.position);
                float newDistanceToAttachTransform = Mathf.Max(0.1f, Mathf.Min(maxRaycastDistance, currentDistanceToAttachTransform + translateByZ));

                attachTransform.position = rayOriginTransform.position + rayOriginTransform.forward * newDistanceToAttachTransform;
            }

            timeSinceLastMouseEvent = 0;
        }

        #region XRBaseControllerInteractor

        /// <inheritdoc />
        public override bool CanHover(IXRHoverInteractable interactable)
        {
            // We stay hovering if we have selected anything.
            bool stickyHover = hasSelection && IsSelecting(interactable);
            if (stickyHover)
            {
                return true;
            }

            // We are ready to pinch if we are in the PinchReady position,
            // or if we are already selecting something.
            bool ready = isHoverActive || isSelectActive;

            return ready && base.CanHover(interactable);
        }

        /// <inheritdoc />
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            return base.CanSelect(interactable) && (!hasSelection || IsSelecting(interactable)) && IsInUse;
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            // When selection is active, force valid targets to be the current selection. This is done to ensure that selected objects remained hovered.
            if (hasSelection && isActiveAndEnabled)
            {
                targets.Clear();
                for (int i = 0; i < interactablesSelected.Count; i++)
                {
                    targets.Add(interactablesSelected[i]);
                }
            }
            else
            {
                base.GetValidTargets(targets);
            }
        }

        #endregion
    }
}
