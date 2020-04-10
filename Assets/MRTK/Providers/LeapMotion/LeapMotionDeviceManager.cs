// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System.Collections.Generic;

#if LEAPMOTIONCORE_PRESENT
using Leap;
using Leap.Unity;
using Leap.Unity.Attachments;
#endif

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Input
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsEditor,
        "Leap Motion Device Manager",
        "LeapMotion/Profiles/LeapMotionDeviceManagerProfile.asset",
        "MixedRealityToolkit.Providers",
        true)]
    /// <summary>
    /// Class that detects the tracking state of leap motion hands.  This class will only run if the Leap Motion Core Assets are in the project and the Leap Motion Device
    /// Manager data provider has been added in the input system configuration profile.
    /// </summary>
    public class LeapMotionDeviceManager : BaseInputDeviceManager, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public LeapMotionDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }


#region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            // Leap Motion only supports Articulated Hands
            return (capability == MixedRealityCapability.ArticulatedHand);
        }


#endregion IMixedRealityCapabilityCheck Implementation
#if LEAPMOTIONCORE_PRESENT

        /// <summary>
        /// The profile that contains settings for the Leap Motion Device Manager input data provider.  This profile is nested under 
        /// Input > Input Data Providers > Leap Motion Device Manager in the MixedRealityToolkit object in the hierarchy.
        /// </summary>
        public LeapMotionDeviceManagerProfile SettingsProfile => ConfigurationProfile as LeapMotionDeviceManagerProfile;

        // This value can only be set in the profile, the default is LeapControllerOrientation.Headset
        private LeapControllerOrientation leapControllerOrientation => SettingsProfile.LeapControllerOrientation;

        /// <summary>
        /// Adds an offset to the game object with LeapServiceProvider attached.  This offset is only applied if the leapControllerOrientation
        /// is LeapControllerOrientation.Desk and is nessesary for the hand to appear in front of the main camera. If the leap controller is on the 
        /// desk, the LeapServiceProvider is added to the scene instead of the LeapXRServiceProvider. The anchor point for the position of the leap hands is 
        /// the position of the game object with the LeapServiceProvider attached.
        /// </summary>
        private Vector3 leapHandsOffset => SettingsProfile.LeapControllerOffset;

        /// <summary>
        /// If true, the leap motion controller connected and detected.
        /// </summary>
        public bool IsLeapConnected => leapServiceProvider.IsConnected();

        /// <summary>
        /// Dictionary to capture all active leap motion hands detected.
        /// </summary>
        private readonly Dictionary<Handedness, LeapMotionArticulatedHand> trackedHands = new Dictionary<Handedness, LeapMotionArticulatedHand>();

        // The LeapServiceProvider is added to the scene at runtime in OnEnable 
        public LeapServiceProvider leapServiceProvider { get; protected set;}

        // The Leap attachment hands, used to determine which hand is currently tracked by leap
        public AttachmentHands attachmentHands { get; protected set; }

        private AttachmentHand leftAttachmentHand = null;
        private AttachmentHand rightAttachmentHand = null;

        // List of hands that are currently in frame and detected by the leap motion controller. If there are no hands in the current frame, this list will be empty.
        public List<Hand> CurrentHandsDetectedByLeap => leapServiceProvider.CurrentFrame.Hands;

        public override void Enable()
        {
            base.Enable();

            if (leapControllerOrientation == LeapControllerOrientation.Headset)
            {
                // If the leap controller is mounted on a headset then add the LeapXRServiceProvider to the scene
                // The LeapXRServiceProvider can only be attached to a camera 
                leapServiceProvider = CameraCache.Main.gameObject.AddComponent<LeapXRServiceProvider>();
            }

            if (leapControllerOrientation == LeapControllerOrientation.Desk)
            {
                // Create a separate gameobject if the leap controller is on the desk
                GameObject leapProvider = new GameObject("LeapProvider");

                // The LeapServiceProvider does not need to be attached to a camera, but the location of this gameobject is the anchor for the desk hands
                leapServiceProvider = leapProvider.AddComponent<LeapServiceProvider>();

                // Follow the transform of the main camera by adding the service provider as a child of the main camera
                leapProvider.transform.parent = CameraCache.Main.transform;

                // Apply hand position offset, an offset is required to render the hands in view and in front of the camera
                leapServiceProvider.transform.position += leapHandsOffset;
            }

            // Add the attachment hands to the scene for the purpose of getting the tracking state of each hand and joint positions
            GameObject leapAttachmentHands = new GameObject("LeapAttachmentHands");
            attachmentHands = leapAttachmentHands.AddComponent<AttachmentHands>();

            foreach (var hand in attachmentHands.attachmentHands)
            {
                if (hand.chirality == Chirality.Left)
                {
                    leftAttachmentHand = hand;
                }
                else
                {
                    rightAttachmentHand = hand;
                }
            }
        }

        /// <summary>
        /// Adds a new LeapMotionArticulatedHand to the scene.
        /// </summary>
        /// <param name="handedness">The handedness (Handedness.Left or Handedness.Right) of the hand to be added</param>
        private void OnHandDetected(Handedness handedness)
        {
            // Only create a new hand if the hand does not exist
            if (!trackedHands.ContainsKey(handedness))
            {
                var pointers = RequestPointers(SupportedControllerType.ArticulatedHand, handedness);
                var inputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Leap {handedness} Controller", pointers, InputSourceType.Hand);
                var leapHand = new LeapMotionArticulatedHand(TrackingState.Tracked, handedness, inputSource);

                // Set the pointers for an articulated hand to the leap hand
                foreach (var pointer in pointers)
                {
                    pointer.Controller = leapHand;
                }

                trackedHands.Add(handedness, leapHand);

                CoreServices.InputSystem.RaiseSourceDetected(inputSource, leapHand);
            }
        }

        /// <summary>
        /// Removes the LeapMotionArticulated hand from the scene when the tracking is lost.
        /// </summary>
        /// <param name="handedness">The handedness (Handedness.Left or Handedness.Right) of the hand to be removed</param>
        private void OnHandDetectionLost(Handedness handedness)
        {
            if (CoreServices.InputSystem != null)
            {
                CoreServices.InputSystem.RaiseSourceLost(trackedHands[handedness].InputSource);
            }

            // Remove the pointer gameobjects
            // Should I be destroying the gameobjects or change the tracking state of the hand?
            foreach (var pointer in trackedHands[handedness].InputSource.Pointers)
            {
                if (pointer != null && (MonoBehaviour)pointer != null && ((MonoBehaviour)pointer).gameObject != null)
                {
                    GameObject.Destroy(((MonoBehaviour)pointer).gameObject);
                }
            }

            // Remove tracked hands
            trackedHands.Remove(trackedHands[handedness].ControllerHandedness);
        }

        /// <summary>
        /// Update the number of tracked leap hands.
        /// </summary>
        /// <param name="isLeftTracked">The tracking state of the left leap hand</param>
        /// <param name="isRightTracked">The tracking state of the right leap hand</param>
        private void UpdateLeapTrackedHands(bool isLeftTracked, bool isRightTracked)
        {
            // Left Hand Update
            if (isLeftTracked && !trackedHands.ContainsKey(Handedness.Left))
            {
                OnHandDetected(Handedness.Left);
            }
            
            if (!isLeftTracked && trackedHands.ContainsKey(Handedness.Left))
            {
                OnHandDetectionLost(Handedness.Left);
            }

            // Right Hand Update
            if (isRightTracked && !trackedHands.ContainsKey(Handedness.Right))
            {
                OnHandDetected(Handedness.Right);
            }

            if (!isRightTracked && trackedHands.ContainsKey(Handedness.Right))
            {
                OnHandDetectionLost(Handedness.Right);
            }
        }

        public override void Update()
        {
            base.Update();

            if (IsLeapConnected)
            {
                // if the number of tracked hands in frame has changed
                if (CurrentHandsDetectedByLeap.Count != trackedHands.Count)
                {
                    UpdateLeapTrackedHands(leftAttachmentHand.isTracked, rightAttachmentHand.isTracked);
                }

                // Update the hand/hands that are in trackedhands
                foreach (KeyValuePair<Handedness, LeapMotionArticulatedHand> hand in trackedHands)
                {
                    hand.Value.UpdateState();
                }
            }
        }
#endif
    }

}

