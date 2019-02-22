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
    public class DragAndDropHandler : BaseFocusHandler,
        IMixedRealityInputHandler<MixedRealityPose>,
        IMixedRealityPointerHandler,
        IMixedRealitySourceStateHandler
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

        /// <summary>
        /// Gets the pivot position for the hand, which is approximated to the base of the neck.
        /// </summary>
        /// <returns>Pivot position for the hand.</returns>
        private Vector3 HandPivotPosition => CameraCache.Main.transform.position + new Vector3(0, -0.2f, 0) - CameraCache.Main.transform.forward * 0.2f; // a bit lower and behind

        private bool isDragging;
        private bool isDraggingEnabled = true;
        private bool isDraggingWithSourcePose;

        // Used for moving with a pointer ray
        private float stickLength;
        private Vector3 previousPointerPositionHeadSpace;

        // Used for moving with a source position
        private float handRefDistance = -1;
        private float objectReferenceDistance;
        private Vector3 objectReferenceDirection;
        private Quaternion gazeAngularOffset;

        private Vector3 objectReferenceUp;
        private Vector3 objectReferenceForward;
        private Vector3 objectReferenceGrabPoint;

        private Vector3 draggingPosition;
        private Quaternion draggingRotation;

        private Rigidbody hostRigidbody;
        private bool hostRigidbodyWasKinematic;

        private IMixedRealityPointer currentPointer;
        private IMixedRealityInputSource currentInputSource;

        // If the dot product between hand movement and head forward is less than this amount,
        // don't exponentially increase the length of the stick
        private readonly float zPushTolerance = 0.1f;

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
            if (!isDraggingEnabled || !isDragging || eventData.MixedRealityInputAction != dragAction || eventData.SourceId != currentInputSource?.SourceId)
            {
                // If we're not handling drag input or we're not releasing the right action, don't try to end a drag operation.
                return;
            }

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.

            StopDragging();
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

            isDraggingWithSourcePose = currentPointer == MixedRealityToolkit.InputSystem.GazeProvider.GazePointer;

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

        #region BaseFocusHandler Overrides

        /// <inheritdoc />
        public override void OnFocusExit(FocusEventData eventData)
        {
            if (isDragging)
            {
                StopDragging();
            }
        }

        #endregion BaseFocusHandler Overrides

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

            Transform cameraTransform = CameraCache.Main.transform;

            currentPointer.IsFocusLocked = true;
            isDragging = true;

            if (hostRigidbody != null)
            {
                hostRigidbodyWasKinematic = hostRigidbody.isKinematic;
                hostRigidbody.isKinematic = true;
            }

            if (isDraggingWithSourcePose)
            {
                Vector3 pivotPosition = HandPivotPosition;
                objectReferenceDistance = Vector3.Magnitude(initialDraggingPosition - pivotPosition);
                objectReferenceDirection = cameraTransform.InverseTransformDirection(Vector3.Normalize(initialDraggingPosition - pivotPosition));
            }
            else
            {
                Vector3 inputPosition;
                currentPointer.TryGetPointerPosition(out inputPosition);

                previousPointerPositionHeadSpace = cameraTransform.InverseTransformPoint(inputPosition);
                stickLength = Vector3.Distance(initialDraggingPosition, inputPosition);
            }

            // Store where the object was grabbed from
            objectReferenceGrabPoint = cameraTransform.transform.InverseTransformDirection(hostTransform.position - initialDraggingPosition);

            // in camera space
            objectReferenceForward = cameraTransform.InverseTransformDirection(hostTransform.forward);
            objectReferenceUp = cameraTransform.InverseTransformDirection(hostTransform.up);

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

        #region IMixedRealityInputHandler<MixedRealityPose> Implementation

        void IMixedRealityInputHandler<MixedRealityPose>.OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.MixedRealityInputAction != dragPositionAction || !isDraggingEnabled || !isDragging || eventData.SourceId != currentInputSource?.SourceId)
            {
                return;
            }

            Transform cameraTransform = CameraCache.Main.transform;
            Vector3 pivotPosition = Vector3.zero;

            if (isDraggingWithSourcePose)
            {
                Vector3 inputPosition = eventData.InputData.Position;
                pivotPosition = HandPivotPosition;
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
            }
            else
            {
                pivotPosition = cameraTransform.position;

                Vector3 pointerPosition;
                currentPointer.TryGetPointerPosition(out pointerPosition);

                Ray pointingRay;
                currentPointer.TryGetPointingRay(out pointingRay);

                Vector3 currentPosition = pointerPosition;
                Vector3 currentPositionHeadSpace = cameraTransform.InverseTransformPoint(currentPosition);
                Vector3 positionDeltaHeadSpace = currentPositionHeadSpace - previousPointerPositionHeadSpace;

                float pushDistance = Vector3.Dot(positionDeltaHeadSpace,
                    cameraTransform.InverseTransformDirection(pointingRay.direction.normalized));
                if (Mathf.Abs(Vector3.Dot(positionDeltaHeadSpace.normalized, Vector3.forward)) > zPushTolerance)
                {
                    stickLength = DistanceRamp(stickLength, pushDistance);
                }

                draggingPosition = pointingRay.GetPoint(stickLength);

                previousPointerPositionHeadSpace = currentPositionHeadSpace;
            }

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

        #endregion IMixedRealityInputHandler<MixedRealityPose> Implementation

        #region Private Helpers

        /// <summary>
        /// Gets the pivot position for the hand, which is approximated to the base of the neck.
        /// </summary>
        /// <remarks>
        /// An exponential distance ramping where distance is determined by:
        /// f(t) = (e^At - 1)/B
        /// where:
        /// A is a scaling factor: how fast the function ramps to infinity
        /// B is a second scaling factor: a denominator that shallows out the ramp near the origin
        /// t is a linear input
        /// f(t) is the distance exponentially ramped along variable t
        /// 
        /// Here's a quick derivation for the expression below.
        /// A = constant
        /// B = constant
        /// d = ramp(t) = (e^At - 1)/B
        /// t = ramp_inverse(d) =  ln(B*d+1)/A
        /// In general, if y=f(x), then f(currentY, deltaX) = f( f_inverse(currentY) + deltaX )
        /// So,
        /// ramp(currentD, deltaT) = (e^(A*(ln(B*currentD + 1)/A + deltaT)) - 1)/B
        /// simplified:
        /// ramp(currentD, deltaT) = (e^(A*deltaT) * (B*currentD + 1) - 1) / B
        /// </remarks>
        private static float DistanceRamp(float currentDistance, float deltaT, float A = 4.0f, float B = 75.0f)
        {
            return (Mathf.Exp(A * deltaT) * (B * currentDistance + 1) - 1) / B;
        }

        #endregion Private Helpers
    }
}
