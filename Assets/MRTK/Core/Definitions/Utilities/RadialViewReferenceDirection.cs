// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Which direction to orient the radial view object.
    /// </summary>
    public enum RadialViewReferenceDirection
    {
        /// <summary>
        /// Orient towards the target including roll, pitch and yaw
        /// </summary>
        ObjectOriented,
        /// <summary>
        /// Orient toward the target but ignore roll
        /// </summary>
        FacingWorldUp,
        /// <summary>
        /// Orient towards the target but remain vertical or gravity aligned
        /// </summary>
        GravityAligned
    }
}