// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Input
{
    /// <summary>
    /// The profile for the Leap Motion Device Manager. The settings for this profile can be viewed if the Leap Motion Device Manager input data provider is 
    /// added to the MRTK input configuration profile.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Leap Motion Profile", fileName = "LeapMotionDeviceManagerProfile", order = 4)]
    [MixedRealityServiceProfile(typeof(LeapMotionDeviceManager))]
    public class LeapMotionDeviceManagerProfile : BaseMixedRealityProfile
    {
        [Space(10)]
        [SerializeField]
        [Tooltip("The location of the leap motion controller. LeapControllerOrientation.Headset indicates the controller is mounted on a headset. " +
            "LeapControllerOrientation.Desk indicates the controller is placed flat on desk. The default value is set to LeapControllerOrientation.Headset")]
        private LeapControllerOrientation leapControllerOrientation = LeapControllerOrientation.Headset;

        /// <summary>
        /// The location of the leap motion controller. LeapControllerOrientation.Headset indicates the controller is mounted on a headset. 
        /// LeapControllerOrientation.Desk indicates the controller is placed flat on desk. The default value is set to LeapControllerOrientation.Headset.
        /// </summary>
        public LeapControllerOrientation LeapControllerOrientation => leapControllerOrientation;

        [SerializeField]
        [Tooltip("Adds an offset to the game object with LeapServiceProvider attached.  This offset is only applied if the leapControllerOrientation " +
        "is LeapControllerOrientation.Desk and is necessary for the hand to appear in front of the main camera. If the leap controller is on the " +
        "desk, the LeapServiceProvider is added to the scene instead of the LeapXRServiceProvider. The anchor point for the hands is the position of the " + 
        "game object with the LeapServiceProvider attached.")]
        private Vector3 leapControllerOffset = new Vector3(0, -0.2f, 0.2f);

        /// <summary>
        /// Adds an offset to the game object with LeapServiceProvider attached.  This offset is only applied if the leapControllerOrientation
        /// is LeapControllerOrientation.Desk and is necessary for the hand to appear in front of the main camera. If the leap controller is on the 
        /// desk, the LeapServiceProvider is added to the scene instead of the LeapXRServiceProvider. The anchor point for the hands is the position of the 
        /// game object with the LeapServiceProvider attached.
        /// </summary>
        public Vector3 LeapControllerOffset
        {
            get => leapControllerOffset;
            set => leapControllerOffset = value;
        }

        [SerializeField]
        [Tooltip("The distance between the index finger tip and the thumb tip required to enter the pinch/air tap selection gesture. " +
            "The pinch gesture enter will be registered for all values less than the EnterPinchDistance. The default EnterPinchDistance value is 0.02 and must be between 0.015 and 0.1. ")]
        private float enterPinchDistance = 0.02f;

        /// <summary>
        /// The distance between the index finger tip and the thumb tip required to enter the pinch/air tap selection gesture.
        /// The pinch gesture enter will be registered for all values less than the EnterPinchDistance. The default EnterPinchDistance value is 0.02 and must be between 0.015 and 0.1. 
        /// </summary>
        public float EnterPinchDistance
        {
            get => enterPinchDistance;
            set => enterPinchDistance = value;
        }

        [SerializeField]
        [Tooltip("The minimum distance between the index finger tip and the thumb tip required to exit the pinch/air tap gesture to deselect.  The distance between the thumb and  " +
            "the index tip must be greater than the ExitPinchDistance to raise the OnInputUp event")]
        private float exitPinchDistance = 0.05f;

        /// <summary>
        /// The minimum distance between the index finger tip and the thumb tip required to exit the pinch/air tap gesture to deselect.  The distance between the thumb and 
        /// the index tip must be greater than the ExitPinchDistance to raise the OnInputUp event
        /// </summary>
        public float ExitPinchDistance
        {        
            get => exitPinchDistance;
            set => exitPinchDistance = value;
        }
    }

}

