// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using UnityEngine;
using UnityEngine.XR;

#if UNITY_WSA
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Script teleports the user to the location being gazed at when Y was pressed on a Gamepad.
    /// </summary>
    [RequireComponent(typeof(SetGlobalListener))]
    public class MixedRealityTeleport : Singleton<MixedRealityTeleport>, IInputHandler
    {
        [Tooltip("Name of the thumbstick axis to check for teleport and strafe.")]
        public XboxControllerInputType HorizontalStrafe = XboxControllerInputType.XboxLeftStickHorizontal;

        [Tooltip("Name of the thumbstick axis to check for movement forwards and backwards.")]
        public XboxControllerInputType ForwardMovement = XboxControllerInputType.XboxLeftStickVertical;

        [Tooltip("Name of the thumbstick axis to check for rotation.")]
        public XboxControllerInputType HorizontalRotation = XboxControllerInputType.XboxRightStickHorizontal;

        [Tooltip("Name of the thumbstick axis to check for rotation.")]
        public XboxControllerInputType VerticalRotation = XboxControllerInputType.XboxRightStickVertical;

        [Tooltip("Custom Input Mapping for horizontal teleport and strafe")]
        public string LeftThumbstickX = InputMappingAxisUtility.CONTROLLER_LEFT_STICK_HORIZONTAL;

        [Tooltip("Name of the thumbstick axis to check for movement forwards and backwards.")]
        public string LeftThumbstickY = InputMappingAxisUtility.CONTROLLER_LEFT_STICK_VERTICAL;

        [Tooltip("Custom Input Mapping for horizontal rotation")]
        public string RightThumbstickX = InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_HORIZONTAL;

        [Tooltip("Custom Input Mapping for vertical rotation")]
        public string RightThumbstickY = InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_VERTICAL;

        public bool EnableTeleport = true;
        public bool EnableRotation = true;
        public bool EnableStrafe = true;

        [Tooltip("Makes sure you don't get put 'on top' of holograms, just on the floor")]
        public bool StayOnTheFloor = false;

        public float RotationSize = 45.0f;
        public float StrafeAmount = 0.5f;

        [SerializeField]
        private GameObject teleportMarker;
        private Animator animationController;

        [SerializeField]
        private bool useCustomMapping;

        /// <summary>
        /// The fade control allows us to fade out and fade in the scene.
        /// This is done to improve comfort when using an immersive display.
        /// </summary>
        private FadeManager fadeControl;

        private bool isTeleportValid;
        private IPointingSource currentPointingSource;
        private uint currentSourceId;

        private void Start()
        {
            FadeManager.AssertIsInitialized();

            fadeControl = FadeManager.Instance;

            // If our FadeManager is missing, or if we're on the HoloLens
            // Remove this component.
            if (!XRDevice.isPresent || fadeControl == null
#if UNITY_WSA
                                    || !HolographicSettings.IsDisplayOpaque
#endif
                )
            {
                Destroy(this);
                return;
            }

            if (teleportMarker != null)
            {
                teleportMarker = Instantiate(teleportMarker);
                teleportMarker.SetActive(false);

                animationController = teleportMarker.GetComponentInChildren<Animator>();
                if (animationController != null)
                {
                    animationController.StopPlayback();
                }
            }
        }

        private void Update()
        {
            if (currentPointingSource != null)
            {
                PositionMarker();
            }
        }

        void IInputHandler.OnInputUp(InputEventData eventData) { }

        void IInputHandler.OnInputDown(InputEventData eventData) { }

        void IInputHandler.OnInputPressed(InputPressedEventData eventData) { }

        void IInputHandler.OnInputPositionChanged(InputPositionEventData eventData)
        {
            if (
#if UNITY_WSA
                eventData.PressType != InteractionSourcePressType.Thumbstick ||
#endif
                eventData.InputPositionType != InputPositionType.Thumbstick)
            {
                return;
            }

            if (EnableTeleport)
            {
                if (currentPointingSource == null && eventData.InputPosition.y > 0.8 && Math.Abs(eventData.InputPosition.x) < 0.3)
                {
                    if (FocusManager.Instance.TryGetPointingSource(eventData, out currentPointingSource))
                    {
                        currentSourceId = eventData.SourceId;
                        StartTeleport();
                    }
                }
                else if (currentPointingSource != null && currentSourceId == eventData.SourceId && eventData.InputPosition.magnitude < 0.2)
                {
                    FinishTeleport();
                }
            }

            if (EnableStrafe && currentPointingSource == null)
            {
                if (eventData.InputPosition.y < -0.8 && Math.Abs(eventData.InputPosition.x) < 0.3)
                {
                    DoStrafe(Vector3.back * StrafeAmount);
                }
            }

            if (EnableRotation && currentPointingSource == null)
            {
                if (eventData.InputPosition.x < -0.8 && Math.Abs(eventData.InputPosition.y) < 0.3)
                {
                    DoRotation(-RotationSize);
                }
                else if (eventData.InputPosition.x > 0.8 && Math.Abs(eventData.InputPosition.y) < 0.3)
                {
                    DoRotation(RotationSize);
                }
            }
        }

        public void StartTeleport()
        {
            if (currentPointingSource != null && !fadeControl.Busy)
            {
                EnableMarker();
                PositionMarker();
            }
        }

        private void FinishTeleport()
        {
            if (currentPointingSource != null)
            {
                currentPointingSource = null;

                if (isTeleportValid)
                {
                    RaycastHit hitInfo;
                    Vector3 hitPos = teleportMarker.transform.position + Vector3.up * (Physics.Raycast(CameraCache.Main.transform.position, Vector3.down, out hitInfo, 5.0f) ? hitInfo.distance : 2.6f);

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
                        transform.RotateAround(CameraCache.Main.transform.position, Vector3.up, rotationAmount);
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
                        Transform transformToRotate = CameraCache.Main.transform;
                        transformToRotate.rotation = Quaternion.Euler(0, transformToRotate.rotation.eulerAngles.y, 0);
                        transform.Translate(strafeAmount, CameraCache.Main.transform);
                    }, null); // Action after fade in
            }
        }

        /// <summary>
        /// Places the player in the specified position of the world
        /// </summary>
        /// <param name="worldPosition"></param>
        public void SetWorldPosition(Vector3 worldPosition)
        {
            var originalY = transform.position.y;

            // There are two things moving the camera: the camera parent (that this script is attached to)
            // and the user's head (which the MR device is attached to. :)). When setting the world position,
            // we need to set it relative to the user's head in the scene so they are looking/standing where 
            // we expect.
            var newPosition = worldPosition - (CameraCache.Main.transform.position - transform.position);
            if (StayOnTheFloor)
            {
                newPosition.y = originalY;
            }
            transform.position = newPosition;
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
            FocusDetails focusDetails;
            if (FocusManager.Instance.TryGetFocusDetails(currentPointingSource, out focusDetails) &&
                focusDetails.Object != null &&
                Vector3.Dot(focusDetails.Normal, Vector3.up) > 0.90f)
            {
                isTeleportValid = true;

                teleportMarker.transform.position = focusDetails.Point;
            }
            else
            {
                isTeleportValid = false;
            }

            animationController.speed = isTeleportValid ? 1 : 0;
        }
    }
}
