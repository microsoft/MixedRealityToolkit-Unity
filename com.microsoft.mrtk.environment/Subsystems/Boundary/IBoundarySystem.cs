// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Environment
{
    /// <summary>
    /// Cross-platform, portable set of specifications for what
    /// a Boundary System is capable of. Both the Boundary subsystem
    /// and the associated provider must implement this interface,
    /// preferably with a direct mapping or wrapping between the
    /// provider surface and the subsystem surface.
    /// </summary>
    public interface IBoundarySystem
    {
        /// <summary>
        /// The scale (ex: World Scale) of the experience.
        /// </summary>
        ExperienceScale Scale { get; set; }

        /// <summary>
        /// Retrieves the boundary geometry.
        /// </summary>
        /// <returns>A list of geometry points, or null if geometry was unavailable.</returns>
        List<Vector3> GetBoundaryGeometry();

        /// <summary>
        /// Updates the tracking space on the XR device.
        /// </summary>
        void SetTrackingSpace();
    }
}