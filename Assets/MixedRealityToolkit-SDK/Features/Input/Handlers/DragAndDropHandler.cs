// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Input.Handlers
{
    /// <summary>
    /// Component that allows dragging a <see cref="GameObject"/>.
    /// Dragging is done by calculating the angular delta and z-delta between the current and previous hand positions,
    /// and then repositioning the object based on that.
    /// </summary>
    public class DragAndDropHandler : BaseFocusHandler, IMixedRealitySourceStateHandler, IMixedRealityPointerHandler, IMixedRealityInputHandler<MixedRealityPose>
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
        [Tooltip("The action that will provide the drag position.")]
        private MixedRealityInputAction dragPositionAction = MixedRealityInputAction.None;

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

        private float handRefDistance = -1;
        private float objectReferenceDistance;

        private Vector3 draggingPosition;
        private Vector3 objectReferenceUp;
        private Vector3 objectReferenceForward;
        private Vector3 objectReferenceGrabPoint;
        private Vector3 objectReferenceDirection;

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
            if (eventData.SourceId == currentInputSource?.SourceId)
            {
                eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.

                StopDragging();
            }
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (!isDraggingEnabled || isDragging || eventData.MixedRealityInputAction != dragAction)
            {
                // If we're already handling drag input or we're not grabbing, don't start a new drag operation.
                return;
            }

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.

            currentInputSource = eventData.InputSource;
            currentPointer = eventData.Pointer;

            FocusDetails focusDetails;
            Vector3 initialDraggingPosition = MixedRealityToolkit.InputSystem.FocusProvider.TryGetFocusDetails(currentPointer, out focusDetails)
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
            if (eventData.SourceId == currentInputSource?.SourceId)
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

            currentPointer.IsFocusLocked = true;
            isDragging = true;

            if (hostRigidbody != null)
            {
                hostRigidbodyWasKinematic = hostRigidbody.isKinematic;
                hostRigidbody.isKinematic = true;
            }

            Transform cameraTransform = CameraCache.Main.transform;

            Vector3 pivotPosition = GetHandPivotPosition(cameraTransform);
            objectReferenceDistance = Vector3.Magnitude(initialDraggingPosition - pivotPosition);

            // Store where the object was grabbed from
            objectReferenceGrabPoint = cameraTransform.transform.InverseTransformDirection(hostTransform.position - initialDraggingPosition);

            // in camera space
            objectReferenceForward = cameraTransform.InverseTransformDirection(hostTransform.forward);
            objectReferenceUp = cameraTransform.InverseTransformDirection(hostTransform.up);
            objectReferenceDirection = cameraTransform.InverseTransformDirection(Vector3.Normalize(initialDraggingPosition - pivotPosition));

            draggingPosition = initialDraggingPosition;
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

            currentPointer.IsFocusLocked = false;
            isDragging = false;
            handRefDistance = -1;

            if (hostRigidbody != null)
            {
                hostRigidbody.isKinematic = hostRigidbodyWasKinematic;
            }
        }

        #region IMixedRealitySourcePoseHandler Implementation

        void IMixedRealityInputHandler<MixedRealityPose>.OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.MixedRealityInputAction != dragPositionAction || !isDraggingEnabled || !isDragging || eventData.SourceId != currentInputSource?.SourceId)
            {
                return;
            }

            Transform cameraTransform = CameraCache.Main.transform;
            Vector3 inputPosition = eventData.InputData.Position;
            Vector3 pivotPosition = GetHandPivotPosition(cameraTransform);
            Vector3 newHandDirection = Vector3.Normalize(inputPosition - pivotPosition);

            if (handRefDistance < 0)
            {
                handRefDistance = Vector3.Magnitude(inputPosition - pivotPosition);

                Vector3 handDirection = cameraTransform.InverseTransformDirection(Vector3.Normalize(inputPosition - pivotPosition));

                // Store the initial offset between the hand and the object, so that we can consider it when dragging
                gazeAngularOffset = Quaternion.FromToRotation(handDirection, objectReferenceDirection);
            }

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

        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData) { }

        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData) { }

        void IMixedRealityInputHandler.OnInputPressed(InputEventData<float> eventData) { }

        void IMixedRealityInputHandler.OnPositionInputChanged(InputEventData<Vector2> eventData) { }

        #endregion IMixedRealitySourcePoseHandler Implementation
    }
}
