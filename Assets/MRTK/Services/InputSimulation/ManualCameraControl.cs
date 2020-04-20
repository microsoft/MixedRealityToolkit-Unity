// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Class for manually controlling the camera in the Unity editor. Used by the Input Simulation Service.
    /// </summary>
    public class ManualCameraControl
    {
        private MixedRealityInputSimulationProfile profile;

        private bool isGamepadLookEnabled = true;
        private bool isFlyKeypressEnabled = true;
        private Vector3 lastTrackerToUnityTranslation = Vector3.zero;
        private Quaternion lastTrackerToUnityRotation = Quaternion.identity;

        private static readonly KeyBinding cancelRotationKey = KeyBinding.FromKey(KeyCode.Escape);
        private readonly MouseRotationProvider mouseRotation = new MouseRotationProvider();

        public ManualCameraControl(MixedRealityInputSimulationProfile _profile)
        {
            profile = _profile;
        }

        private static float InputCurve(float x)
        {
            // smoothing input curve, converts from [-1,1] to [-2,2]
            return Mathf.Sign(x) * (1.0f - Mathf.Cos(0.5f * Mathf.PI * Mathf.Clamp(x, -1.0f, 1.0f)));
        }

        /// <summary>
        /// Translate and rotate the camera transform based on keyboard and mouse input.
        /// </summary>
        public void UpdateTransform(Transform transform, MouseDelta mouseDelta)
        {
            // Undo the last tracker to Unity transforms applied
            transform.Translate(-this.lastTrackerToUnityTranslation, Space.World);
            transform.Rotate(-this.lastTrackerToUnityRotation.eulerAngles, Space.World);

            // Calculate and apply the camera control movement this frame
            Vector3 rotate = GetCameraControlRotation(mouseDelta);
            Vector3 translate = GetCameraControlTranslation(transform);

            transform.Rotate(rotate.x, 0.0f, 0.0f);
            transform.Rotate(0.0f, rotate.y, 0.0f, Space.World);
            transform.Rotate(0.0f, 0.0f, rotate.z);
            transform.Translate(translate, Space.World);

            transform.Rotate(this.lastTrackerToUnityRotation.eulerAngles, Space.World);
            transform.Translate(this.lastTrackerToUnityTranslation, Space.World);
        }

        private static float GetKeyDir(string neg, string pos)
        {
            return UnityEngine.Input.GetKey(neg) ? -1.0f : UnityEngine.Input.GetKey(pos) ? 1.0f : 0.0f;
        }

        private Vector3 GetCameraControlTranslation(Transform transform)
        {
            Vector3 deltaPosition = Vector3.zero;

            // Support fly up/down keypresses if the current project maps it. This isn't a standard
            // Unity InputManager mapping, so it has to gracefully fail if unavailable.
            if (this.isFlyKeypressEnabled)
            {
                try
                {
                    deltaPosition += InputCurve(UnityEngine.Input.GetAxis("Fly")) * transform.up;
                }
                catch (System.Exception)
                {
                    this.isFlyKeypressEnabled = false;
                }
            }
            else
            {
                // use page up/down in this case
                deltaPosition += GetKeyDir("page down", "page up") * Vector3.up;
            }

            deltaPosition += InputCurve(UnityEngine.Input.GetAxis(profile.MoveHorizontal)) * transform.right;

            Vector3 forward;
            Vector3 up;
            if (profile.CurrentControlMode == InputSimulationControlMode.Walk)
            {
                up = Vector3.up;
                forward = Vector3.ProjectOnPlane(transform.forward, up).normalized;
            }
            else
            {
                forward = transform.forward;
                up = transform.up;
            }
            deltaPosition += InputCurve(UnityEngine.Input.GetAxis(profile.MoveVertical)) * forward;
            deltaPosition += InputCurve(UnityEngine.Input.GetAxis(profile.MoveUpDown)) * up;

            float accel = KeyInputSystem.GetKey(profile.FastControlKey) ? profile.ControlFastSpeed : profile.ControlSlowSpeed;
            return accel * deltaPosition * Time.deltaTime;
        }

        private Vector3 GetCameraControlRotation(MouseDelta mouseDelta)
        {
            float inversionFactor = profile.IsControllerLookInverted ? -1.0f : 1.0f;

            Vector3 rot = Vector3.zero;

            if (this.isGamepadLookEnabled)
            {
                try
                {
                    // Get the axes information from the right stick of X360 controller
                    rot.x += InputCurve(UnityEngine.Input.GetAxis(profile.LookVertical)) * inversionFactor;
                    rot.y += InputCurve(UnityEngine.Input.GetAxis(profile.LookHorizontal));
                }
                catch (System.Exception)
                {
                    this.isGamepadLookEnabled = false;
                }
            }

            mouseRotation.Update(profile.MouseLookButton, cancelRotationKey, profile.MouseLookToggle);
            if (mouseRotation.IsRotating)
            {
                rot.x += -InputCurve(mouseDelta.screenDelta.y * profile.MouseRotationSensitivity);
                rot.y += InputCurve(mouseDelta.screenDelta.x * profile.MouseRotationSensitivity);
                rot.z += InputCurve(mouseDelta.screenDelta.z * profile.MouseRotationSensitivity);
            }

            rot *= profile.MouseLookSpeed;

            return rot;
        }

    }
}