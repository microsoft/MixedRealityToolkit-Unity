// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Which direction to orient the radial view object.
    /// </summary>
    public enum RadialViewReferenceDirection
    {
        /// <summary>
        /// Orient towards the target including roll, pitch and yaw
        /// </summary>
        ObjectOriented = 0,

        /// <summary>
        /// Orient toward the target but ignore roll
        /// </summary>
        FacingWorldUp = 1,

        /// <summary>
        /// Orient towards the target but remain vertical or gravity aligned
        /// </summary>
        GravityAligned = 2
    }
}