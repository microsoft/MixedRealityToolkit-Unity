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
        [SerializeField]
        [Tooltip("Should the mouse cursor be hidden when no active input is received?")]
        private bool hideCursorWhenInactive = true;

        /// <inheritdoc />
        public bool HideCursorWhenInactive => hideCursorWhenInactive;

        [SerializeField]
        [Range(0.01f, 1f)]
        [Tooltip("What is the movement threshold to reach before un-hiding mouse cursor?")]
        private float movementThresholdToUnHide = 0.1f;

        /// <inheritdoc />
        public float MovementThresholdToUnHide => movementThresholdToUnHide;

        [SerializeField]
        [Range(0f, 10f)]
        [Tooltip("How long should it take before the mouse cursor is hidden?")]
        private float hideTimeout = 3.0f;

        /// <inheritdoc />
        public float HideTimeout => hideTimeout;

        private float timeoutTimer;

        private bool isInteractionEnabled = false;

        private bool cursorWasDisabledOnDown = false;

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
                if (!BaseCursor.IsVisible &&
                    (eventData.SourceData.x >= movementThresholdToUnHide ||
                     eventData.SourceData.y >= MovementThresholdToUnHide))
                {
                    BaseCursor?.SetVisibility(true);
                    transform.rotation = CameraCache.Main.transform.rotation;
                }

                var newRotation = Vector3.zero;
                newRotation.x += eventData.SourceData.y;
                newRotation.y += eventData.SourceData.x;
                transform.Rotate(newRotation, Space.World);
            }
        }

        /// <inheritdoc />
        public override void OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!UseSourcePoseData &&
                    PoseAction == eventData.MixedRealityInputAction)
                {
                    if (!BaseCursor.IsVisible &&
                        (eventData.InputData.x >= movementThresholdToUnHide ||
                         eventData.InputData.y >= MovementThresholdToUnHide))
                    {
                        BaseCursor?.SetVisibility(true);
                        transform.rotation = CameraCache.Main.transform.rotation;
                    }

                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;

                    var newRotation = Vector3.zero;
                    newRotation.x += eventData.InputData.x;
                    newRotation.y += eventData.InputData.y;
                    transform.Rotate(newRotation, Space.World);
                }
            }
        }

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            cursorWasDisabledOnDown = !BaseCursor.IsVisible;

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
            if (BaseCursor.IsVisible && !cursorWasDisabledOnDown)
            {
                base.OnInputUp(eventData);
            }
        }

        protected override void Start()
        {
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
            if (!isInteractionEnabled) { return; }

            timeoutTimer += Time.unscaledDeltaTime;

            if (timeoutTimer >= hideTimeout)
            {
                timeoutTimer = 0.0f;
                BaseCursor?.SetVisibility(false);
            }
        }

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
    }
}