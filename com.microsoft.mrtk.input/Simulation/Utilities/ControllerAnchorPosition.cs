// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// Values specifying the mode in which the <see cref="SimulatedController"/>
    /// is to operate.
    /// </summary>
    public enum ControllerAnchorPoint
    {
        /// <summary>
        /// The Controller's anchor will match the device's position
        /// </summary>
        Device = 0,

        // Something which might be useful in the future. Applications like Beat Saber greatly benefitted from being able to offset the rotation and position of the controllers
        ///// <summary>
        ///// The Controller's anchor will be offset from the device's position by a fixed relative offset
        ///// </summary>
        // FixedOffset = 1,

        /// <summary>
        /// The Controller's anchor will match the index finger
        /// </summary>
        IndexFinger = 2,

        /// <summary>
        /// The Controller's anchor will match the grab point
        /// </summary>
        Grab = 3
    }
}
