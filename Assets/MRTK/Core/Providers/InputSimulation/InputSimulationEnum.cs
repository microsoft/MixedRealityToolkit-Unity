// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{

    public enum InputSimulationControlMode
    {
        // Move in the main camera forward direction
        Fly,
        // Move on a X/Z plane
        Walk,
    }

    public enum EyeGazeSimulationMode
    {
        // Disable eye gaze simulation
        Disabled,
        // Eye gaze follows the camera forward axis
        CameraForwardAxis,
        // Eye gaze follows the mouse
        Mouse,
    }

    public enum HandSimulationMode
    {
        // Disable hand simulation
        Disabled,
        // Raises gesture events only
        Gestures,
        // Provide a fully articulated hand controller
        Articulated,
    }

}