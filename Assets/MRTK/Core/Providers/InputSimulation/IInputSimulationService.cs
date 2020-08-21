// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public interface IInputSimulationService : IMixedRealityInputDeviceManager
    {
        /// <summary>
        /// Typed representation of the ConfigurationProfile property.
        /// </summary>
        MixedRealityInputSimulationProfile InputSimulationProfile { get; }

        /// <summary>
        /// Simulated eye gaze behavior.
        /// </summary>
        EyeGazeSimulationMode EyeGazeSimulationMode { get; set; }

        /// <summary>
        /// Simulated controller behavior.
        /// </summary>
        ControllerSimulationMode ControllerSimulationMode { get; set; }

        /// <summary>
        /// Pose data for the left hand.
        /// </summary>
        SimulatedHandData HandDataLeft { get; }
        /// <summary>
        /// Pose data for the right hand.
        /// </summary>
        SimulatedHandData HandDataRight { get; }

        /// <summary>
        /// Pose data for the left motion controller.
        /// </summary>
        SimulatedMotionControllerData MotionControllerDataLeft { get; }
        /// <summary>
        /// Pose data for the right motion controller.
        /// </summary>
        SimulatedMotionControllerData MotionControllerDataRight { get; }

        /// <summary>
        /// If true then keyboard and mouse input are used to simulate controllers.
        /// </summary>
        bool UserInputEnabled { get; set; }

        /// <summary>
        /// The left controller is controlled by user input.
        /// </summary>
        bool IsSimulatingControllerLeft { get; }
        /// <summary>
        /// The right controller is controlled by user input.
        /// </summary>
        bool IsSimulatingControllerRight { get; }

        /// <summary>
        /// The left controller is always tracking.
        /// </summary>
        bool IsAlwaysVisibleControllerLeft { get; set; }
        /// <summary>
        /// The right controller is always tracking.
        /// </summary>
        bool IsAlwaysVisibleControllerRight { get; set; }

        /// <summary>
        /// Position of the left controller in view space.
        /// </summary>
        Vector3 ControllerPositionLeft { get; set; }
        /// <summary>
        /// Position of the right controller in view space.
        /// </summary>
        Vector3 ControllerPositionRight { get; set; }

        /// <summary>
        /// Rotation euler angles of the left controller in view space.
        /// </summary>
        Vector3 ControllerRotationLeft { get; set; }
        /// <summary>
        /// Rotation euler angles of the right controller in view space.
        /// </summary>
        Vector3 ControllerRotationRight { get; set; }

        /// <summary>
        /// Reset the left controller.
        /// </summary>
        void ResetControllerLeft();
        /// <summary>
        /// Reset the right controller.
        /// </summary>
        void ResetControllerRight();

        #region Obsolete Properties
        /// <summary>
        /// Simulated hand behavior.
        /// </summary>
        [Obsolete("Use ControllerSimulationMode instead.")]
        HandSimulationMode HandSimulationMode { get; set; }

        /// <summary>
        /// The left hand is controlled by user input.
        /// </summary>
        [Obsolete("Use IsSimulatingControllerLeft instead.")]
        bool IsSimulatingHandLeft { get; }
        /// <summary>
        /// The right hand is controlled by user input.
        /// </summary>
        [Obsolete("Use IsSimulatingControllerRight instead.")]
        bool IsSimulatingHandRight { get; }

        /// <summary>
        /// The left hand is always tracking.
        /// </summary>
        [Obsolete("Use IsAlwaysVisibleControllerLeft instead.")]
        bool IsAlwaysVisibleHandLeft { get; set; }
        /// <summary>
        /// The right hand is always tracking.
        /// </summary>
        [Obsolete("Use IsAlwaysVisibleControllerRight instead.")]
        bool IsAlwaysVisibleHandRight { get; set; }

        /// <summary>
        /// Position of the left hand in view space.
        /// </summary>
        [Obsolete("Use ControllerPositionLeft instead.")]
        Vector3 HandPositionLeft { get; set; }
        /// <summary>
        /// Position of the right hand in view space.
        /// </summary>
        [Obsolete("Use ControllerPositionRight instead.")]
        Vector3 HandPositionRight { get; set; }

        /// <summary>
        /// Rotation euler angles of the left hand in view space.
        /// </summary>
        [Obsolete("Use ControllerRotationLeft instead.")]
        Vector3 HandRotationLeft { get; set; }
        /// <summary>
        /// Rotation euler angles of the right hand in view space.
        /// </summary>
        [Obsolete("Use ControllerRotationRight instead.")]
        Vector3 HandRotationRight { get; set; }

        /// <summary>
        /// Reset the left hand.
        /// </summary>
        [Obsolete("Use ResetControllerLeft instead.")]
        void ResetHandLeft();
        /// <summary>
        /// Reset the right hand.
        /// </summary>
        [Obsolete("Use ResetControllerRight instead.")]
        void ResetHandRight();
        #endregion
    }
}