// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Input.Handlers
{
    /// <summary>
    /// Component that allows dragging a <see cref="GameObject"/>.
    /// Dragging is done by calculating the angular delta and z-delta between the current and previous hand positions,
    /// and then repositioning the object based on that.
    /// </summary>
    public class DragAndDropHandler : BaseFocusHandler, IMixedRealitySourceStateHandler, IMixedRealityPointerHandler
    {
        private enum RotationModeEnum
        {
            Default,
            LockObjectRotation,
            OrientTowardUser,
            OrientTowardUserAndKeepUpright
        }

        [SerializeField]
        [Tooltip("The action that will start/stop the dragging.")]
        private MixedRealityInputAction dragAction = MixedRealityInputAction.None;

        [SerializeField]
        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        private Transform hostTransform;

        [SerializeField]
        [Tooltip("Scale by which hand movement in Z is multiplied to move the dragged object.")]
        private float distanceScale = 2f;

        [SerializeField]
        [Tooltip("How should the GameObject be rotated while being dragged?")]
        private RotationModeEnum rotationMode = RotationModeEnum.Default;

        [SerializeField]
        [Range(0.01f, 1.0f)]
        [Tooltip("Controls the speed at which the object will interpolate toward the desired position")]
        private float positionLerpSpeed = 0.2f;

        [SerializeField]
        [Range(0.01f, 1.0f)]
        [Tooltip("Controls the speed at which the object will interpolate toward the desired rotation")]
        private float rotationLerpSpeed = 0.2f;

        private bool isDragging;
        private bool isDraggingEnabled = true;

        private float handRefDistance;
        private float objectReferenceDistance;

        private Vector3 draggingPosition;
        private Vector3 objectReferenceUp;
        private Vector3 objectReferenceForward;
        private Vector3 objectReferenceGrabPoint;

        private Quaternion draggingRotation;
        private Quaternion gazeAngularOffset;

        private Rigidbody hostRigidbody;
        private bool hostRigidbodyWasKinematic;

        private IMixedRealityPointer currentPointer;
        private IMixedRealityInputSource currentInputSource;

        #region MonoBehaviour Implementation

        private void Start()
        {
            if (hostTransform == null)
            {
                hostTransform = transform;
            }

            hostRigidbody = hostTransform.GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (isDraggingEnabled && isDragging)
            {
                UpdateDragging();
            }
        }

        private void OnDestroy()
        {
            if (isDragging)
            {
                StopDragging();
            }
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealityPointerHandler Implementation

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

            if (eventData.MixedRealityInputAction != dragAction)
            {
                // If we're not grabbing.
                return;
            }

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.

            currentInputSource = eventData.InputSource;
            currentPointer = eventData.Pointer;

            FocusDetails focusDetails;
            Vector3 initialDraggingPosition = InputSystem.FocusProvider.TryGetFocusDetails(eventData, out focusDetails)
                    ? focusDetails.Point
                    : hostTransform.position;

            StartDragging(initialDraggingPosition);
        }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }

        #endregion IMixedRealityPointerHandler Implementation

        #region IMixedRealitySourceStateHandler Implementation

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            if (eventData.SourceId == currentInputSource.SourceId)
            {
                StopDragging();
            }
        }

        #endregion IMixedRealitySourceStateHandler Implementation

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
        /// Starts dragging the object.
        /// </summary>
        private void StartDragging(Vector3 initialDraggingPosition)
        {
            if (!isDraggingEnabled || isDragging)
            {
                return;
            }

            // TODO: robertes: Fix push/pop and single-handler model so that multiple HandDraggable components can be active at once.

            // Add self as a modal input handler, to get all inputs during the manipulation
            InputSystem.PushModalInputHandler(gameObject);

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
            objectReferenceDistance = Vector3.Magnitude(initialDraggingPosition - pivotPosition);

            Vector3 objForward = hostTransform.forward;
            Vector3 objUp = hostTransform.up;

            // Store where the object was grabbed from
            objectReferenceGrabPoint = cameraTransform.transform.InverseTransformDirection(hostTransform.position - initialDraggingPosition);

            Vector3 objDirection = Vector3.Normalize(initialDraggingPosition - pivotPosition);
            Vector3 handDirection = Vector3.Normalize(inputPosition - pivotPosition);

            // in camera space
            objForward = cameraTransform.InverseTransformDirection(objForward);
            objUp = cameraTransform.InverseTransformDirection(objUp);
            objDirection = cameraTransform.InverseTransformDirection(objDirection);
            handDirection = cameraTransform.InverseTransformDirection(handDirection);

            objectReferenceForward = objForward;
            objectReferenceUp = objUp;

            // Store the initial offset between the hand and the object, so that we can consider it when dragging
            gazeAngularOffset = Quaternion.FromToRotation(handDirection, objDirection);
            draggingPosition = initialDraggingPosition;
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
            float targetDistance = objectReferenceDistance + distanceOffset;

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
                    Vector3 objForward = cameraTransform.TransformDirection(objectReferenceForward);
                    // in world space
                    Vector3 objUp = cameraTransform.TransformDirection(objectReferenceUp);
                    draggingRotation = Quaternion.LookRotation(objForward, objUp);
                    break;
            }

            Vector3 newPosition = Vector3.Lerp(hostTransform.position, draggingPosition + cameraTransform.TransformDirection(objectReferenceGrabPoint), positionLerpSpeed);
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
        private void StopDragging()
        {
            if (!isDragging)
            {
                return;
            }

            // Remove self as a modal input handler
            InputSystem.PopModalInputHandler();

            isDragging = false;

            if (hostRigidbody != null)
            {
                hostRigidbody.isKinematic = hostRigidbodyWasKinematic;
            }
        }
    }
}
