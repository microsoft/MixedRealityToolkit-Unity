// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    /// Defines for how input simulation handles hands
    /// </summary>
    public enum HandSimulationMode
    {

        /// <summary>
        /// Disable hand simulation
        /// </summary>
        Disabled,

        /// <summary>
        /// Raises gesture events only
        /// </summary>
        Gestures,

        /// <summary>
        /// Provide a fully articulated hand controller
        /// </summary>
        Articulated,
    }

}