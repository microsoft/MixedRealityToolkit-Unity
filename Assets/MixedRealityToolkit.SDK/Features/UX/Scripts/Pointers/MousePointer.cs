// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Pointers
{
    /// <summary>
    /// Internal Touch Pointer Implementation.
    /// </summary>
    public class MousePointer : BaseControllerPointer, IMixedRealityMousePointer
    {
        private float timeoutTimer = 0.0f;

        private bool isInteractionEnabled = false;

        private bool cursorWasDisabledOnDown = false;

        private bool isDisabled = true;

        #region IMixedRealityMousePointer Implementation

        [SerializeField]
        [Tooltip("Should the mouse cursor be hidden when no active input is received?")]
        private bool hideCursorWhenInactive = true;

        /// <inheritdoc />
        bool IMixedRealityMousePointer.HideCursorWhenInactive => hideCursorWhenInactive;

        [SerializeField]
        [Range(0.01f, 1f)]
        [Tooltip("What is the movement threshold to reach before un-hiding mouse cursor?")]
        private float movementThresholdToUnHide = 0.1f;

        /// <inheritdoc />
        float IMixedRealityMousePointer.MovementThresholdToUnHide => movementThresholdToUnHide;

        [SerializeField]
        [Range(0f, 10f)]
        [Tooltip("How long should it take before the mouse cursor is hidden?")]
        private float hideTimeout = 3.0f;

        /// <inheritdoc />
        float IMixedRealityMousePointer.HideTimeout => hideTimeout;

        #endregion IMixedRealityMousePointer Implementation

        #region IMixedRealityPointer Implementation

        /// <inheritdoc />
        public override bool IsInteractionEnabled => isInteractionEnabled;

        private IMixedRealityController controller;

        /// <inheritdoc />
        public override IMixedRealityController Controller
        {
            get { return controller; }
            set
            {
                controller = value;
                InputSourceParent = value.InputSource;
                Handedness = value.ControllerHandedness;
                gameObject.name = "Spatial Mouse Pointer";
                TrackingState = TrackingState.NotApplicable;
            }
        }

        /// <inheritdoc />
        public override void OnPreRaycast()
        {
            transform.position = CameraCache.Main.transform.position;

            Ray pointingRay;
            if (TryGetPointingRay(out pointingRay))
            {
                Rays[0].CopyRay(pointingRay, PointerExtent);

                if (MixedRealityRaycaster.DebugEnabled)
                {
                    Debug.DrawRay(pointingRay.origin, pointingRay.direction * PointerExtent, Color.green);
                }
            }
        }

        #endregion IMixedRealityPointer Implementation

        #region IMixedRealitySourcePoseHandler Implementation

        /// <inheritdoc />
        public override void OnSourceDetected(SourceStateEventData eventData)
        {
            if (RayStabilizer != null)
            {
                RayStabilizer = null;
            }

            base.OnSourceDetected(eventData);

            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                isInteractionEnabled = true;
            }
        }

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            base.OnSourceLost(eventData);

            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                isInteractionEnabled = false;
            }
        }

        /// <inheritdoc />
        public override void OnSourcePoseChanged(SourcePoseEventData<Vector2> eventData)
        {
            if (Controller == null ||
                eventData.Controller == null ||
                eventData.Controller.InputSource.SourceId != Controller.InputSource.SourceId)
            {
                return;
            }

            if (UseSourcePoseData)
            {
                UpdateMousePosition(eventData.SourceData.x, eventData.SourceData.y);
            }
        }

        #endregion IMixedRealitySourcePoseHandler Implementation

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            cursorWasDisabledOnDown = isDisabled;

            if (cursorWasDisabledOnDown)
            {
                BaseCursor?.SetVisibility(true);
                transform.rotation = CameraCache.Main.transform.rotation;
            }
            else
            {
                base.OnInputDown(eventData);
            }
        }

        /// <inheritdoc />
        public override void OnInputUp(InputEventData eventData)
        {
            if (!isDisabled && !cursorWasDisabledOnDown)
            {
                base.OnInputUp(eventData);
            }
        }

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!UseSourcePoseData &&
                    PoseAction == eventData.MixedRealityInputAction)
                {
                    UpdateMousePosition(eventData.InputData.x, eventData.InputData.y);
                }
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        #region MonoBehaviour Implementation

        protected override void Start()
        {
            isDisabled = DisableCursorOnStart;

            base.Start();

            if (RayStabilizer != null)
            {
                RayStabilizer = null;
            }

            foreach (var inputSource in MixedRealityToolkit.InputSystem.DetectedInputSources)
            {
                if (inputSource.SourceId == Controller.InputSource.SourceId)
                {
                    isInteractionEnabled = true;
                    break;
                }
            }
        }

        private void Update()
        {
            if (!hideCursorWhenInactive || isDisabled) { return; }

            timeoutTimer += Time.unscaledDeltaTime;

            if (timeoutTimer >= hideTimeout)
            {
                timeoutTimer = 0.0f;
                BaseCursor?.SetVisibility(false);
                isDisabled = true;
            }
        }

        #endregion MonoBehaviour Implementation

        private void UpdateMousePosition(float mouseX, float mouseY)
        {
            if (Mathf.Abs(mouseX) >= movementThresholdToUnHide ||
                Mathf.Abs(mouseY) >= movementThresholdToUnHide)
            {
                if (isDisabled)
                {
                    BaseCursor?.SetVisibility(true);
                    transform.rotation = CameraCache.Main.transform.rotation;
                }

                isDisabled = false;
            }

            if (!isDisabled)
            {
                timeoutTimer = 0.0f;
            }

            var newRotation = Vector3.zero;
            newRotation.x += mouseX;
            newRotation.y += mouseY;
            transform.Rotate(newRotation, Space.World);
        }
    }
}