// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces
{
    /// <summary>
    /// Manager interface for a Boundary system in the Mixed Reality Toolkit
    /// All replacement systems for providing Boundary functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityBoundarySystem : IMixedRealityManager
    {
        /// <summary>
        /// The size of the physical space available for Mixed Reality.
        /// </summary>
        TrackingSpaceType TrackingSpaceType { get; set; }

        /// <summary>
        /// The height of the playspace, in meters.
        /// </summary>
        /// <remarks>
        /// This is used to create a three dimensional boundary volume.
        /// </remarks>
        float BoundaryHeight { get; set; }

        /// <summary>
        /// Enable / disable the platform's playspace boundary rendering.
        /// </summary>
        /// <remarks>
        /// Not all platforms support specifying whether or not to render the playspace boundary.
        /// For platforms without boundary rendering control, the default behavior will be unchanged 
        /// regardless of the value provided.
        /// </remarks>
        bool EnablePlatformBoundaryRendering { get; set; }

        /// <summary>
        /// A three dimensional volume as described by the playspace boundary and
        /// the configured height.
        /// </summary>
        Bounds BoundaryVolume { get; }

        /// <summary>
        /// A three dimensional volume as described by the inscribed rectangle and
        /// the configured height.
        /// </summary>
        Bounds InscribedVolume { get; }
    }
}