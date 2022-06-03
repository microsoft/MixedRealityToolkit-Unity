// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// The current state of an object being scaled.
    /// </summary>
    public enum ScalingState
    {
        /// <summary>
        /// The scale of the object is not changing.
        /// </summary>
        Static = 0,

        /// <summary>
        /// The scale of the object is being reduced.
        /// </summary>
        Shrinking = 1,

        /// <summary>
        /// The scale of the object is being increased.
        /// </summary>
        Growing = 2
    }
}