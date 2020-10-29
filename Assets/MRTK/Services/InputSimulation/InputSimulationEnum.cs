// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{

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