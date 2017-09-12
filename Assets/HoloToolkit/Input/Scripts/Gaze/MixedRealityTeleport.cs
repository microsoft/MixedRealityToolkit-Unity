// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Script teleports the user to the location being gazed at when Y was pressed on a Gamepad.
    /// </summary>
    public class MixedRealityTeleport : Singleton<MixedRealityTeleport>
    {
        [Tooltip("Name of the joystick axis to move along X.")]
        public string LeftJoystickX = "ControllerLeftStickX";

        [Tooltip("Name of the joystick axis to move along Y.")]
        public string LeftJoystickY = "ControllerLeftStickY";

        public bool EnableTeleport = true;
        public bool EnableRotation = true;
        public bool EnableStrafe = true;

        public bool EnableJoystickMovement = false;

        public float SpeedScale { get; set; }

        public float RotationSize = 45.0f;
        public float StrafeAmount = 0.5f;

        public GameObject TeleportMarker;
        private Animator animationController;

        /// <summary>
        /// The fade control allows us to fade out and fade in the scene.
        /// This is done to improve comfort when using an immersive display.
        /// </summary>
        private FadeScript fadeControl;

        private GazeManager gazeManager;
        private Vector3 positionBeforeJump = Vector3.zero;
        private GameObject teleportMarker;
        private bool teleportValid;
        private bool teleporting;

        private void Start()
        {
            if (!XRDevice.isPresent)
            {
                Destroy(this);
                return;
            }

            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;

            gazeManager = GazeManager.Instance;
            fadeControl = FadeScript.Instance;
            SpeedScale = 0.6f;

            teleportMarker = Instantiate(TeleportMarker);
            teleportMarker.SetActive(false);

            animationController = teleportMarker.GetComponentInChildren<Animator>();
            if (animationController != null)
            {
                animationController.StopPlayback();
            }
        }

        void Update()
        {
            if (InteractionManager.numSourceStates == 0)
            {
                HandleGamepad();
            }

            if (teleporting)
            {
                PositionMarker();
            }
        }

        private void HandleGamepad()
        {
            if (EnableTeleport && !fadeControl.Busy)
            {
                float leftX = Input.GetAxis("ControllerLeftStickX");
                float leftY = Input.GetAxis("ControllerLeftStickY");

                if (!teleporting && leftY > 0.8 && Math.Abs(leftX) < 0.2)
                {
                    StartTeleport();
                }
                else if (teleporting && Math.Sqrt(Math.Pow(leftX, 2) + Math.Pow(leftY, 2)) < 0.1)
                {
                    FinishTeleport();
                }
            }

            if (EnableStrafe && !teleporting && !fadeControl.Busy)
            {
                float leftX = Input.GetAxis("ControllerLeftStickX");
                float leftY = Input.GetAxis("ControllerLeftStickY");

                if (leftX < -0.8 && Math.Abs(leftY) < 0.2)
                {
                    DoStrafe(Vector3.left * StrafeAmount);
                }
                else if (leftX > 0.8 && Math.Abs(leftY) < 0.2)
                {
                    DoStrafe(Vector3.right * StrafeAmount);
                }
                else if (leftY < -0.8 && Math.Abs(leftX) < 0.2)
                {
                    DoStrafe(Vector3.back * StrafeAmount);
                }
            }

            if (EnableRotation && !teleporting && !fadeControl.Busy)
            {
                float rightX = Input.GetAxis("ControllerRightStickX");
                float rightY = Input.GetAxis("ControllerRightStickY");

                if (rightX < -0.8 && Math.Abs(rightY) < 0.2)
                {
                    DoRotation(-RotationSize);
                }
                else if (rightX > 0.8 && Math.Abs(rightY) < 0.2)
                {
                    DoRotation(RotationSize);
                }
            }
        }
        
        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
        {
            if (EnableTeleport)
            {
                if (!teleporting && obj.state.thumbstickPosition.y > 0.8 && Math.Abs(obj.state.thumbstickPosition.x) < 0.2)
                {
                    StartTeleport();
                }
                else if (teleporting && obj.state.thumbstickPosition.magnitude < 0.1)
                {
                    FinishTeleport();
                }
            }

            if (EnableStrafe && !teleporting && !fadeControl.Busy)
            {
                if (obj.state.thumbstickPosition.y < -0.8 && Math.Abs(obj.state.thumbstickPosition.x) < 0.2)
                {
                    DoStrafe(Vector3.back * StrafeAmount);
                }
            }

            if (EnableRotation && !teleporting && !fadeControl.Busy)
            {
                if (obj.state.thumbstickPosition.x < -0.8 && Math.Abs(obj.state.thumbstickPosition.y) < 0.2)
                {
                    DoRotation(-RotationSize);
                }
                else if (obj.state.thumbstickPosition.x > 0.8 && Math.Abs(obj.state.thumbstickPosition.y) < 0.2)
                {
                    DoRotation(RotationSize);
                }
            }
        }

        public void StartTeleport()
        {
            if (!teleporting && !fadeControl.Busy)
            {
                teleporting = true;
                EnableMarker();
                PositionMarker();
            }
        }

        private void FinishTeleport()
        {
            if (teleporting)
            {
                teleporting = false;

                if (teleportValid)
                {
                    RaycastHit hitInfo;
                    Vector3 hitPos = teleportMarker.transform.position + Vector3.up * (Physics.Raycast(Camera.main.transform.position, Vector3.down, out hitInfo, 5.0f) ? hitInfo.distance : 2.6f);

                    fadeControl.DoFade(0.25f, 0.5f, () =>
                    {
                        SetWorldPosition(hitPos);
                    }, null);
                }

                DisableMarker();
            }
        }

        public void DoRotation(float rotationAmount)
        {
            if (rotationAmount != 0 && !fadeControl.Busy)
            {
                fadeControl.DoFade(
                    0.25f, // Fade out time
                    0.25f, // Fade in time
                    () => // Action after fade out
                    {
                        transform.RotateAround(Camera.main.transform.position, Vector3.up, rotationAmount);
                    }, null); // Action after fade in
            }
        }

        public void DoStrafe(Vector3 strafeAmount)
        {
            if (strafeAmount.magnitude != 0 && !fadeControl.Busy)
            {
                fadeControl.DoFade(
                    0.25f, // Fade out time
                    0.25f, // Fade in time
                    () => // Action after fade out
                    {
                        Transform transformToRotate = Camera.main.transform;
                        transformToRotate.rotation = Quaternion.Euler(0, transformToRotate.rotation.eulerAngles.y, 0);
                        transform.Translate(strafeAmount, Camera.main.transform);
                    }, null); // Action after fade in
            }
        }

        /// <summary>
        /// Places the player in the specified position of the world
        /// </summary>
        /// <param name="worldPosition"></param>
        public void SetWorldPosition(Vector3 worldPosition)
        {
            // There are two things moving the camera: the camera parent (that this script is attached to)
            // and the user's head (which the MR device is attached to. :)). When setting the world position,
            // we need to set it relative to the user's head in the scene so they are looking/standing where 
            // we expect.
            transform.position = worldPosition - Camera.main.transform.localPosition;
        }

        private void EnableMarker()
        {
            teleportMarker.SetActive(true);
            if (animationController != null)
            {
                animationController.StartPlayback();
            }
        }

        private void DisableMarker()
        {
            if (animationController != null)
            {
                animationController.StopPlayback();
            }
            teleportMarker.SetActive(false);
        }

        private void PositionMarker()
        {
            Vector3 hitNormal = HitNormal();
            if (Vector3.Dot(hitNormal, Vector3.up) > 0.90f)
            {
                teleportValid = true;

                IPointingSource pointingSource;
                if (FocusManager.Instance.TryGetSinglePointer(out pointingSource))
                {
                    teleportMarker.transform.position = FocusManager.Instance.GetFocusDetails(pointingSource).Point;
                }
            }
            else
            {
                teleportValid = false;
            }

            animationController.speed = teleportValid ? 1 : 0;
        }

        private Vector3 HitNormal()
        {
            Vector3 retval = Vector3.zero;

            IPointingSource pointingSource;
            if (FocusManager.Instance.TryGetSinglePointer(out pointingSource))
            {
                FocusDetails focusDetails = FocusManager.Instance.GetFocusDetails(pointingSource);

                if (focusDetails.Object != null)
                {
                    retval = focusDetails.Normal;
                }
            }

            return retval;
        }
    }
}