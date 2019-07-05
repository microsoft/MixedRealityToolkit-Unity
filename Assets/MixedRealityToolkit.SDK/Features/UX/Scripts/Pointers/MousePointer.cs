// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Internal Touch Pointer Implementation.
    /// </summary>
    public class MousePointer : DefaultMousePointer
    {
        private MixedRealityMouseInputProfile mouseInputProfile = null;

        private MixedRealityMouseInputProfile MouseInputProfile
        {
            get
            {
                if (mouseInputProfile == null)
                {
                    // Get the profile from the input system's registered mouse device manager.
                    IMixedRealityMouseDeviceManager mouseManager = (InputSystem as IMixedRealityDataProviderAccess)?.GetDataProvider<IMixedRealityMouseDeviceManager>();
                    mouseInputProfile = mouseManager?.MouseInputProfile;
                }
                return mouseInputProfile;
            }
        }

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            // screenspace to ray conversion
            transform.position = CameraCache.Main.transform.position;

            Ray ray = new Ray(transform.position, transform.forward);
            Rays[0].CopyRay(ray, PointerExtent);

            if (MixedRealityRaycaster.DebugEnabled)
            {
                Debug.DrawRay(ray.origin, ray.direction * PointerExtent, Color.green);
            }

            // ray to worldspace conversion
            gameObject.transform.position = transform.position + transform.forward * DefaultPointerExtent;
        }

        public override void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!UseSourcePoseData &&
                    PoseAction == eventData.MixedRealityInputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;
                    transform.position = eventData.InputData.Position;
                    transform.rotation = eventData.InputData.Rotation;
                }
            }
        }


        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (PoseAction == eventData.MixedRealityInputAction && !UseSourcePoseData)
                {
                    Vector3 mouseDeltaRotation = Vector3.zero;
                    mouseDeltaRotation.x += eventData.InputData.x;
                    mouseDeltaRotation.y += eventData.InputData.y;
                    if (MouseInputProfile != null)
                    {
                        mouseDeltaRotation *= MouseInputProfile.MouseSpeed;
                    }
                    UpdateMouseRotation(mouseDeltaRotation);
                }
            }
        }
        private void UpdateMouseRotation(Vector3 mouseDeltaRotation)
        {
            if (mouseDeltaRotation.magnitude >= MovementThresholdToUnHide)
            {
                if (isDisabled)
                {
                    // if cursor was hidden reset to center
                    SetVisibility(true);
                    transform.rotation = CameraCache.Main.transform.rotation;
                }

                isDisabled = false;
            }

            if (!isDisabled)
            {
                timeoutTimer = 0.0f;
            }

            transform.Rotate(mouseDeltaRotation, Space.World);
        }

        protected override void Start()
        {
            base.Start();

#if UNITY_EDITOR
            if (UnityEditor.EditorWindow.focusedWindow != null)
            {
                UnityEditor.EditorWindow.focusedWindow.ShowNotification(new GUIContent("Press \"ESC\" to regain mouse control"));
            }
#endif

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        protected override void SetVisibility(bool visible)
        {
            BaseCursor?.SetVisibility(visible);
        }
    }
}