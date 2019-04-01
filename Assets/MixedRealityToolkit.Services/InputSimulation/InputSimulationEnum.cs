// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{

    /// <summary>
    /// This enum is used to customize how/when users will look around in the Unity player
    /// using the mouse.
    /// </summary>
    public enum InputSimulationMouseButton
    {
        Left,       // Left mouse button
        Right,      // Right mouse button
        Middle,     // Middle or scroll wheel button
        Control,    // Control on keyboard
        Shift,      // Shift on keyboard
        Focused,    // When Unity player has focus
        None        // No mouse look functionality
    }

    public enum InputSimulationControlMode
    {
        // Move in the main camera forward direction
        Fly,
        // Move on a X/Z plane
        Walk,
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