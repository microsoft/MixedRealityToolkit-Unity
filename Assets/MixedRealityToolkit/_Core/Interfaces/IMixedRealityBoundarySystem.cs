// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEngine;
using UnityEngine.Experimental.XR;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces
{
    /// <summary>
    /// Manager interface for a Boundary system in the Mixed Reality Toolkit
    /// All replacement systems for providing Boundary functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityBoundarySystem : IMixedRealityManager
    {
        /// <summary>
        /// The scale (ex: World Scale) of the experience.
        /// </summary>
        ExperienceScale Scale { get; set; }

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
        /// Two dimensional representation of the geometry of the boundary, as provided
        /// by the platform.
        /// </summary>
        /// <remarks>
        /// BoundaryGeometry should be treated as the outline of the player's space, placed
        /// on the floor.
        /// </remarks>
        Edge[] GeometryBounds { get; }

        /// <summary>
        /// The largest rectangle that is contained withing the playspace geometry.
        /// </summary>
        InscribedRectangle InscribedRectangularBounds { get; }

        /// <summary>
        /// Indicates the height of the floor, in relation to the coordinate system origin.
        /// </summary>
        /// <remarks>
        /// If a floor has been located, FloorHeight.HasValue will be true, otherwise it will be false.
        /// </remarks>
        float? FloorHeight { get; }

        /// <summary>
        /// Determines if a location is within the tracked area of the boundary space.
        /// </summary>
        /// <param name="location">The location to be checked.</param>
        /// <returns>True if the location is within the tracked area of the boundary space.</returns>
        bool Contains(Vector3 location);

        /// <summary>
        /// Determines if a location is within the specified area of the boundary space.
        /// </summary>
        /// <param name="location">The location to be checked.</param>
        /// <returns>True if the location is within the specified area of the boundary space.</returns>
        /// <remarks>
        /// Use:
        /// * Boundary.Type.PlayArea for the inscribed volume
        /// * Boundary.Type.TrackedArea for the area defined by the boundary edges.
        /// </remarks>
        bool Contains(Vector3 location, Boundary.Type boundaryType);
    }
}