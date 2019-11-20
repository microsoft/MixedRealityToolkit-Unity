// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Physics;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The MousePointer represents a mouse cursor in world space.
    /// It uses spherical movement around the camera.
    /// Its movement is bound to screenspace, but based in the delta movement of the computer mouse.
    /// </summary>
    public class MousePointer : BaseMousePointer
    {
        /// <inheritdoc />
        protected override string ControllerName => "Spatial Mouse Pointer";

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
        }

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (PoseAction == eventData.MixedRealityInputAction && !UseSourcePoseData)
                {
                    UpdateMouseRotation(eventData.InputData);
                }
            }
        }

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (UseSourcePoseData)
                {
                    UpdateMouseRotation(eventData.InputData.Rotation.eulerAngles);
                }
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        private void UpdateMouseRotation(Vector3 mouseDeltaRotation)
        {
            if (isDisabled)
            {
                if (mouseDeltaRotation.magnitude >= MovementThresholdToUnHide)
                {
                    // if cursor was hidden reset to center
                    SetVisibility(true);
                    transform.rotation = CameraCache.Main.transform.rotation;
                }
            }
            else
            {
                timeoutTimer = 0.0f;
            }

            transform.Rotate(mouseDeltaRotation, Space.Self);
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

        /// <inheritdoc />
        protected override void SetVisibility(bool visible)
        {
            base.SetVisibility(visible);
            BaseCursor?.SetVisibility(visible);
        }
    }
}