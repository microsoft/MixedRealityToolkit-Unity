// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Boundary
{
    /// <summary>
    /// Defines different types of boundaries that can be requested.
    /// </summary>
    public enum BoundaryType
    {
        /// <summary>
        /// A rectangular area calculated as the largest rectangle within the tracked area, good for placing content near the user.
        /// </summary>
        PlayArea,
        /// <summary>
        /// The full tracked boundary, typically manually drawn by a user while setting up their device.
        /// </summary>
        TrackedArea
    }
}
