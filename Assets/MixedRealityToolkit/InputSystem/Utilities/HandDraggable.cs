// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Focus;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Utilities
{
    /// <summary>
    /// Component that allows dragging an object with your hand on HoloLens.
    /// Dragging is done by calculating the angular delta and z-delta between the current and previous hand positions,
    /// and then repositioning the object based on that.
    /// </summary>
    public class HandDraggable : FocusTarget, IMixedRealitySourceStateHandler, IMixedRealityPointerHandler
    {
        private enum RotationModeEnum
        {
            Default,
            LockObjectRotation,
            OrientTowardUser,
            OrientTowardUserAndKeepUpright
        }

        [SerializeField]
        private MixedRealityInputAction grabAction;

        [SerializeField]
        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        private Transform hostTransform;

        [SerializeField]
        [Tooltip("Scale by which hand movement in z is multiplied to move the dragged object.")]
        private float distanceScale = 2f;

        [SerializeField]
        private RotationModeEnum rotationMode = RotationModeEnum.Default;

        [SerializeField]
        [Tooltip("Controls the speed at which the object will interpolate toward the desired position")]
        [Range(0.01f, 1.0f)]
        private float positionLerpSpeed = 0.2f;

        [SerializeField]
        [Tooltip("Controls the speed at which the object will interpolate toward the desired rotation")]
        [Range(0.01f, 1.0f)]
        private float rotationLerpSpeed = 0.2f;

        private bool isDragging;
        private bool isDraggingEnabled = true;

        private float objRefDistance;
        private float handRefDistance;

        private Vector3 objRefUp;
        private Vector3 objRefForward;
        private Vector3 objRefGrabPoint;
        private Vector3 draggingPosition;

        private Quaternion gazeAngularOffset;
        private Quaternion draggingRotation;

        private Rigidbody hostRigidbody;
        private bool hostRigidbodyWasKinematic;

        private IMixedRealityPointer currentPointer;
        private IMixedRealityInputSystem inputSystem;
        private IMixedRealityInputSource currentInputSource;

        private void Awake()
        {
            inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
        }

        private void Start()
        {
            if (hostTransform == null)
            {
                hostTransform = transform;
            }

            hostRigidbody = hostTransform.GetComponent<Rigidbody>();
        }

        private void OnDestroy()
        {
            if (isDragging)
            {
                StopDragging();
            }
        }

        private void Update()
        {
            if (isDraggingEnabled && isDragging)
            {
                UpdateDragging();
            }
        }

        /// <summary>
        /// Starts dragging the object.
        /// </summary>
        public void StartDragging(Vector3 initialDraggingPosition)
        {
            if (!isDraggingEnabled)
            {
                return;
            }

            if (isDragging)
            {
                return;
            }

            // TODO: robertes: Fix push/pop and single-handler model so that multiple HandDraggable components can be active at once.

            // Add self as a modal input handler, to get all inputs during the manipulation
            inputSystem.PushModalInputHandler(gameObject);

            isDragging = true;

            if (hostRigidbody != null)
            {
                hostRigidbodyWasKinematic = hostRigidbody.isKinematic;
                hostRigidbody.isKinematic = true;
            }

            Transform cameraTransform = CameraCache.Main.transform;

            Vector3 inputPosition;
            currentPointer.TryGetPointerPosition(out inputPosition);

            Vector3 pivotPosition = GetHandPivotPosition(cameraTransform);
            handRefDistance = Vector3.Magnitude(inputPosition - pivotPosition);
            objRefDistance = Vector3.Magnitude(initialDraggingPosition - pivotPosition);

            Vector3 objForward = hostTransform.forward;
            Vector3 objUp = hostTransform.up;

            // Store where the object was grabbed from
            objRefGrabPoint = cameraTransform.transform.InverseTransformDirection(hostTransform.position - initialDraggingPosition);

            Vector3 objDirection = Vector3.Normalize(initialDraggingPosition - pivotPosition);
            Vector3 handDirection = Vector3.Normalize(inputPosition - pivotPosition);

            // in camera space
            objForward = cameraTransform.InverseTransformDirection(objForward);
            objUp = cameraTransform.InverseTransformDirection(objUp);
            objDirection = cameraTransform.InverseTransformDirection(objDirection);
            handDirection = cameraTransform.InverseTransformDirection(handDirection);

            objRefForward = objForward;
            objRefUp = objUp;

            // Store the initial offset between the hand and the object, so that we can consider it when dragging
            gazeAngularOffset = Quaternion.FromToRotation(handDirection, objDirection);
            draggingPosition = initialDraggingPosition;
        }

        /// <summary>
        /// Gets the pivot position for the hand, which is approximated to the base of the neck.
        /// </summary>
        /// <returns>Pivot position for the hand.</returns>
        private static Vector3 GetHandPivotPosition(Transform cameraTransform)
        {
            return cameraTransform.position + new Vector3(0, -0.2f, 0) - cameraTransform.forward * 0.2f; // a bit lower and behind
        }

        /// <summary>
        /// Enables or disables dragging.
        /// </summary>
        /// <param name="isEnabled">Indicates whether dragging should be enabled or disabled.</param>
        public void SetDragging(bool isEnabled)
        {
            if (isDraggingEnabled == isEnabled)
            {
                return;
            }

            isDraggingEnabled = isEnabled;

            if (isDragging)
            {
                StopDragging();
            }
        }

        /// <summary>
        /// Update the position of the object being dragged.
        /// </summary>
        private void UpdateDragging()
        {
            Transform cameraTransform = CameraCache.Main.transform;

            Vector3 inputPosition;
            currentPointer.TryGetPointerPosition(out inputPosition);

            Vector3 pivotPosition = GetHandPivotPosition(cameraTransform);
            Vector3 newHandDirection = Vector3.Normalize(inputPosition - pivotPosition);

            // in camera space
            newHandDirection = cameraTransform.InverseTransformDirection(newHandDirection);
            Vector3 targetDirection = Vector3.Normalize(gazeAngularOffset * newHandDirection);
            // back to world space
            targetDirection = cameraTransform.TransformDirection(targetDirection);

            float currentHandDistance = Vector3.Magnitude(inputPosition - pivotPosition);
            float distanceRatio = currentHandDistance / handRefDistance;
            float distanceOffset = distanceRatio > 0 ? (distanceRatio - 1f) * distanceScale : 0;
            float targetDistance = objRefDistance + distanceOffset;

            draggingPosition = pivotPosition + (targetDirection * targetDistance);

            switch (rotationMode)
            {
                case RotationModeEnum.OrientTowardUser:
                case RotationModeEnum.OrientTowardUserAndKeepUpright:
                    draggingRotation = Quaternion.LookRotation(hostTransform.position - pivotPosition);
                    break;
                case RotationModeEnum.LockObjectRotation:
                    draggingRotation = hostTransform.rotation;
                    break;
                default:
                    // in world space
                    Vector3 objForward = cameraTransform.TransformDirection(objRefForward);
                    // in world space
                    Vector3 objUp = cameraTransform.TransformDirection(objRefUp);
                    draggingRotation = Quaternion.LookRotation(objForward, objUp);
                    break;
            }

            Vector3 newPosition = Vector3.Lerp(hostTransform.position, draggingPosition + cameraTransform.TransformDirection(objRefGrabPoint), positionLerpSpeed);
            // Apply Final Position
            if (hostRigidbody == null)
            {
                hostTransform.position = newPosition;
            }
            else
            {
                hostRigidbody.MovePosition(newPosition);
            }

            // Apply Final Rotation
            Quaternion newRotation = Quaternion.Lerp(hostTransform.rotation, draggingRotation, rotationLerpSpeed);
            if (hostRigidbody == null)
            {
                hostTransform.rotation = newRotation;
            }
            else
            {
                hostRigidbody.MoveRotation(newRotation);
            }

            if (rotationMode == RotationModeEnum.OrientTowardUserAndKeepUpright)
            {
                Quaternion upRotation = Quaternion.FromToRotation(hostTransform.up, Vector3.up);
                hostTransform.rotation = upRotation * hostTransform.rotation;
            }
        }

        /// <summary>
        /// Stops dragging the object.
        /// </summary>
        public void StopDragging()
        {
            if (!isDragging)
            {
                return;
            }

            // Remove self as a modal input handler
            inputSystem.PopModalInputHandler();

            isDragging = false;

            if (hostRigidbody != null)
            {
                hostRigidbody.isKinematic = hostRigidbodyWasKinematic;
            }
        }

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (eventData.SourceId == currentInputSource.SourceId)
            {
                eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.

                StopDragging();
            }
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (isDragging)
            {
                // We're already handling drag input, so we can't start a new drag operation.
                return;
            }

            if (eventData.MixedRealityInputAction.Id != grabAction.Id)
            {
                // If we're not grabbing.
                return;
            }

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.

            currentInputSource = eventData.InputSource;
            currentPointer = eventData.Pointer;

            FocusDetails focusDetails;
            Vector3 initialDraggingPosition = inputSystem.FocusProvider.TryGetFocusDetails(eventData, out focusDetails)
                ? focusDetails.Point
                : hostTransform.position;

            StartDragging(initialDraggingPosition);
        }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            if (eventData.SourceId == currentInputSource.SourceId)
            {
                StopDragging();
            }
        }
    }
}
