// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Microsoft.MixedReality.Toolkit.Input
{

    /// <summary>
    /// Defines for how input simulation handles movement
    /// </summary>
    public enum InputSimulationControlMode
    {
        /// <summary>
        /// Move in the main camera forward direction
        /// </summary>
        Fly,

        /// <summary>
        /// Move on a X/Z plane
        /// </summary>
        Walk,
    }


    /// <summary>
    /// Defines for how input simulation handles eye gaze
    /// </summary>
    public enum EyeGazeSimulationMode
    {
        /// <summary>
        /// Disable eye gaze simulation
        /// </summary>
        Disabled,

        /// <summary>
        /// Eye gaze follows the camera forward axis
        /// </summary>
        CameraForwardAxis,

        /// <summary>
        /// Eye gaze follows the mouse
        /// </summary>
        Mouse,
    }

    /// <summary>
    /// Defines for how input simulation handles controllers
    /// </summary>
    public enum ControllerSimulationMode
    {

        /// <summary>
        /// Disable controller simulation
        /// </summary>
        Disabled,

        /// <summary>
        /// Raises hand gesture events only
        /// </summary>
        HandGestures,

        /// <summary>
        /// Provide a fully articulated hand controller
        /// </summary>
        ArticulatedHand,

        /// <summary>
        /// Provide a 6DoF motion controller
        /// </summary>
        MotionController,
    }

    #region Obsolete Enum
    /// <summary>
    /// Defines for how input simulation handles controllers
    /// </summary>
    [Obsolete("Use ControllerSimulationMode instead.")]
    public enum HandSimulationMode
    {

        /// <summary>
        /// Disable controller simulation
        /// </summary>
        Disabled = ControllerSimulationMode.Disabled,

        /// <summary>
        /// Raises hand gesture events only
        /// </summary>
        Gestures = ControllerSimulationMode.HandGestures,

        /// <summary>
        /// Provide a fully articulated hand controller
        /// </summary>
        Articulated = ControllerSimulationMode.ArticulatedHand,

        /// <summary>
        /// Provide a 6DoF motion controller
        /// </summary>
        MotionController = ControllerSimulationMode.MotionController,
    }
    #endregion

}