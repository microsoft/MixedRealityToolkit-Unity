// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        /// Simulated hand behavior.
        /// </summary>
        HandSimulationMode HandSimulationMode { get; set; }

        /// <summary>
        /// Pose data for the left hand.
        /// </summary>
        SimulatedHandData HandDataLeft { get; }
        /// <summary>
        /// Pose data for the right hand.
        /// </summary>
        SimulatedHandData HandDataRight { get; }

        /// <summary>
        /// If true then keyboard and mouse input are used to simulate hands.
        /// </summary>
        bool UserInputEnabled { get; set; }

        /// <summary>
        /// The left hand is controlled by user input.
        /// </summary>
        bool IsSimulatingHandLeft { get; }
        /// <summary>
        /// The right hand is controlled by user input.
        /// </summary>
        bool IsSimulatingHandRight { get; }

        /// <summary>
        /// The left hand is always tracking.
        /// </summary>
        bool IsAlwaysVisibleHandLeft { get; set; }
        /// <summary>
        /// The right hand is always tracking.
        /// </summary>
        bool IsAlwaysVisibleHandRight { get; set; }

        /// <summary>
        /// Position of the left hand in view space.
        /// </summary>
        Vector3 HandPositionLeft { get; set; }
        /// <summary>
        /// Position of the right hand in view space.
        /// </summary>
        Vector3 HandPositionRight { get; set; }

        /// <summary>
        /// Rotation euler angles of the left hand in view space.
        /// </summary>
        Vector3 HandRotationLeft { get; set; }
        /// <summary>
        /// Rotation euler angles of the right hand in view space.
        /// </summary>
        Vector3 HandRotationRight { get; set; }

        /// <summary>
        /// Reset the left hand.
        /// </summary>
        void ResetHandLeft();
        /// <summary>
        /// Reset the right hand.
        /// </summary>
        void ResetHandRight();
    }
}