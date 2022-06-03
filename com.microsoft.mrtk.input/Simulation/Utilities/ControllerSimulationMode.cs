// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// Values specifying the mode in which the <see cref="SimulatedController"/>
    /// is to operate.
    /// </summary>
    public enum ControllerSimulationMode
    {
        /// <summary>
        /// Simulation is disabled. No controller movement or actions will occur.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Articulated hand simulation. Movement and controls available to hands are supported.
        /// </summary>
        ArticulatedHand = 1,

        /// <summary>
        /// Motion controller simulation. Movement and all controls are supported.
        /// </summary>
        MotionController = 2
    }
}