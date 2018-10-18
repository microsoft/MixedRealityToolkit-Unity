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
        private Vector3 newRotation = Vector3.zero;

        private bool isInteractionEnabled = false;

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
                newRotation = transform.rotation.eulerAngles;
                newRotation.x += eventData.SourceData.y;
                newRotation.y += eventData.SourceData.x;
                transform.rotation = Quaternion.Euler(newRotation);
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
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;
                    newRotation = transform.rotation.eulerAngles;
                    newRotation.x += eventData.InputData.x;
                    newRotation.y += eventData.InputData.y;
                    transform.rotation = Quaternion.Euler(newRotation);
                }
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

        /// <inheritdoc />
        public override void OnSourceDetected(SourceStateEventData eventData)
        {
            if (RayStabilizer != null)
            {
                RayStabilizer = null;
            }

            base.OnSourceDetected(eventData);

            if (eventData.InputSource.SourceId == Controller.InputSource.SourceId)
            {
                Debug.Log("Pointer detected");
                isInteractionEnabled = true;
            }
        }

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            base.OnSourceLost(eventData);

            if (Controller != null &&
                eventData.Controller != null &&
                eventData.Controller.InputSource.SourceId == Controller.InputSource.SourceId)
            {
                isInteractionEnabled = false;
            }
        }
    }
}