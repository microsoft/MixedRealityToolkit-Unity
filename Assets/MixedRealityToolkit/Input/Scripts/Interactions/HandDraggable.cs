// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Component that allows dragging an object with your hand on HoloLens.
    /// Dragging is done by calculating the angular delta and z-delta between the current and previous hand positions,
    /// and then repositioning the object based on that.
    /// </summary>
    public class HandDraggable : MonoBehaviour, IFocusable, IInputHandler, ISourceStateHandler
    {
        /// <summary>
        /// Event triggered when dragging starts.
        /// </summary>
        public event Action StartedDragging;

        /// <summary>
        /// Event triggered when dragging stops.
        /// </summary>
        public event Action StoppedDragging;

        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        public Transform HostTransform;

        [Tooltip("Scale by which hand movement in z is multiplied to move the dragged object.")]
        public float DistanceScale = 2f;

        public enum RotationModeEnum
        {
            Default,
            LockObjectRotation,
            OrientTowardUser,
            OrientTowardUserAndKeepUpright
        }

        public RotationModeEnum RotationMode = RotationModeEnum.Default;

        [Tooltip("Controls the speed at which the object will interpolate toward the desired position")]
        [Range(0.01f, 1.0f)]
        public float PositionLerpSpeed = 0.2f;

        [Tooltip("Controls the speed at which the object will interpolate toward the desired rotation")]
        [Range(0.01f, 1.0f)]
        public float RotationLerpSpeed = 0.2f;

        public bool IsDraggingEnabled = true;

        private bool isDragging;
        private bool isGazed;
        private Vector3 objRefForward;
        private Vector3 objRefUp;
        private float objRefDistance;
        private Quaternion gazeAngularOffset;
        private float handRefDistance;
        private Vector3 objRefGrabPoint;

        private Vector3 draggingPosition;
        private Quaternion draggingRotation;

        private IInputSource currentInputSource;
        private uint currentInputSourceId;

        private void Start()
        {
            if (HostTransform == null)
            {
                HostTransform = transform;
            }
        }

        private void OnDestroy()
        {
            if (isDragging)
            {
                StopDragging();
            }

            if (isGazed)
            {
                OnFocusExit();
            }
        }

        private void Update()
        {
            if (IsDraggingEnabled && isDragging)
            {
                UpdateDragging();
            }
        }

        /// <summary>
        /// Starts dragging the object.
        /// </summary>
        public void StartDragging()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (isDragging)
            {
                return;
            }

            // Add self as a modal input handler, to get all inputs during the manipulation
            InputManager.Instance.PushModalInputHandler(gameObject);

            isDragging = true;

            Vector3 gazeHitPosition = GazeManager.Instance.HitInfo.point;
            Transform cameraTransform = CameraCache.Main.transform;
            Vector3 handPosition;
            currentInputSource.TryGetPosition(currentInputSourceId, out handPosition);

            Vector3 pivotPosition = GetHandPivotPosition(cameraTransform);
            handRefDistance = Vector3.Magnitude(handPosition - pivotPosition);
            objRefDistance = Vector3.Magnitude(gazeHitPosition - pivotPosition);

            Vector3 objForward = HostTransform.forward;
            Vector3 objUp = HostTransform.up;
            // Store where the object was grabbed from
            objRefGrabPoint = cameraTransform.InverseTransformDirection(HostTransform.position - gazeHitPosition);

            Vector3 objDirection = Vector3.Normalize(gazeHitPosition - pivotPosition);
            Vector3 handDirection = Vector3.Normalize(handPosition - pivotPosition);

            objForward = cameraTransform.InverseTransformDirection(objForward);       // in camera space
            objUp = cameraTransform.InverseTransformDirection(objUp);                 // in camera space
            objDirection = cameraTransform.InverseTransformDirection(objDirection);   // in camera space
            handDirection = cameraTransform.InverseTransformDirection(handDirection); // in camera space

            objRefForward = objForward;
            objRefUp = objUp;

            // Store the initial offset between the hand and the object, so that we can consider it when dragging
            gazeAngularOffset = Quaternion.FromToRotation(handDirection, objDirection);
            draggingPosition = gazeHitPosition;

            StartedDragging.RaiseEvent();
        }

        /// <summary>
        /// Gets the pivot position for the hand, which is approximated to the base of the neck.
        /// </summary>
        /// <returns>Pivot position for the hand.</returns>
        private Vector3 GetHandPivotPosition(Transform cameraTransform)
        {
            Vector3 pivot = cameraTransform.position + new Vector3(0, -0.2f, 0) - cameraTransform.forward * 0.2f; // a bit lower and behind
            return pivot;
        }

        /// <summary>
        /// Enables or disables dragging.
        /// </summary>
        /// <param name="isEnabled">Indicates whether dragging should be enabled or disabled.</param>
        public void SetDragging(bool isEnabled)
        {
            if (IsDraggingEnabled == isEnabled)
            {
                return;
            }

            IsDraggingEnabled = isEnabled;

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
            Vector3 newHandPosition;
            Transform cameraTransform = CameraCache.Main.transform;
            currentInputSource.TryGetPosition(currentInputSourceId, out newHandPosition);

            Vector3 pivotPosition = GetHandPivotPosition(cameraTransform);

            Vector3 newHandDirection = Vector3.Normalize(newHandPosition - pivotPosition);

            newHandDirection = cameraTransform.InverseTransformDirection(newHandDirection); // in camera space
            Vector3 targetDirection = Vector3.Normalize(gazeAngularOffset * newHandDirection);
            targetDirection = cameraTransform.TransformDirection(targetDirection); // back to world space

            float currentHandDistance = Vector3.Magnitude(newHandPosition - pivotPosition);

            float distanceRatio = currentHandDistance / handRefDistance;
            float distanceOffset = distanceRatio > 0 ? (distanceRatio - 1f) * DistanceScale : 0;
            float targetDistance = objRefDistance + distanceOffset;

            draggingPosition = pivotPosition + (targetDirection * targetDistance);

            if (RotationMode == RotationModeEnum.OrientTowardUser || RotationMode == RotationModeEnum.OrientTowardUserAndKeepUpright)
            {
                draggingRotation = Quaternion.LookRotation(HostTransform.position - pivotPosition);
            }
            else if (RotationMode == RotationModeEnum.LockObjectRotation)
            {
                draggingRotation = HostTransform.rotation;
            }
            else // RotationModeEnum.Default
            {
                Vector3 objForward = cameraTransform.TransformDirection(objRefForward); // in world space
                Vector3 objUp = cameraTransform.TransformDirection(objRefUp);   // in world space
                draggingRotation = Quaternion.LookRotation(objForward, objUp);
            }

            // Apply Final Position
            HostTransform.position = Vector3.Lerp(HostTransform.position, draggingPosition + cameraTransform.TransformDirection(objRefGrabPoint), PositionLerpSpeed);
            // Apply Final Rotation
            HostTransform.rotation = Quaternion.Lerp(HostTransform.rotation, draggingRotation, RotationLerpSpeed);

            if (RotationMode == RotationModeEnum.OrientTowardUserAndKeepUpright)
            {
                Quaternion upRotation = Quaternion.FromToRotation(HostTransform.up, Vector3.up);
                HostTransform.rotation = upRotation * HostTransform.rotation;
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
            InputManager.Instance.PopModalInputHandler();

            isDragging = false;
            currentInputSource = null;
            StoppedDragging.RaiseEvent();
        }

        public void OnFocusEnter()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (isGazed)
            {
                return;
            }

            isGazed = true;
        }

        public void OnFocusExit()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (!isGazed)
            {
                return;
            }

            isGazed = false;
        }

        public void OnInputUp(InputEventData eventData)
        {
            if (currentInputSource != null &&
                eventData.SourceId == currentInputSourceId)
            {
                StopDragging();
            }
        }

        public void OnInputDown(InputEventData eventData)
        {
            if (isDragging)
            {
                // We're already handling drag input, so we can't start a new drag operation.
                return;
            }

            if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.Position))
            {
                // The input source must provide positional data for this script to be usable
                return;
            }

            currentInputSource = eventData.InputSource;
            currentInputSourceId = eventData.SourceId;
            StartDragging();
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            // Nothing to do
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (currentInputSource != null && eventData.SourceId == currentInputSourceId)
            {
                StopDragging();
            }
        }
    }
}
