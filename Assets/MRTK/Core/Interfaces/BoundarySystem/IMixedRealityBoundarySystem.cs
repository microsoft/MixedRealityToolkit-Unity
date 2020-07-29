// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Boundary
{
    /// <summary>
    /// Manager interface for a Boundary system in the Mixed Reality Toolkit
    /// All replacement systems for providing Boundary functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityBoundarySystem : IMixedRealityEventSystem, IMixedRealityEventSource
    {
        /// <summary>
        /// Typed representation of the ConfigurationProfile property.
        /// </summary>
        MixedRealityBoundaryVisualizationProfile BoundaryVisualizationProfile { get; }

        /// <summary>
        /// The scale (ex: World Scale) of the experience.
        /// </summary>
        ExperienceScale Scale { get; set; }

        /// <summary>
        /// The height of the play space, in meters.
        /// </summary>
        /// <remarks>
        /// This is used to create a three dimensional boundary volume.
        /// </remarks>
        float BoundaryHeight { get; set; }

        /// <summary>
        /// Enable / disable floor rendering.
        /// </summary>
        bool ShowFloor { get; set; }

        /// <summary>
        /// The physics layer that the generated floor is assigned to.
        /// </summary>
        int FloorPhysicsLayer { get; set; }

        /// <summary>
        /// Enable / disable play area rendering.
        /// </summary>
        bool ShowPlayArea { get; set; }

        /// <summary>
        /// The physics layer that the generated play area is assigned to.
        /// </summary>
        int PlayAreaPhysicsLayer { get; set; }

        /// <summary>
        /// Enable / disable tracked area rendering.
        /// </summary>
        bool ShowTrackedArea { get; set; }

        /// <summary>
        /// The physics layer that the generated tracked area is assigned to.
        /// </summary>
        int TrackedAreaPhysicsLayer { get; set; }

        /// <summary>
        /// Enable / disable boundary wall rendering.
        /// </summary>
        bool ShowBoundaryWalls { get; set; }

        /// <summary>
        /// The physics layer that the generated boundary walls are assigned to.
        /// </summary>
        int BoundaryWallsPhysicsLayer { get; set; }

        /// <summary>
        /// Enable / disable ceiling rendering.
        /// </summary>
        /// <remarks>
        /// The ceiling is defined as a <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> positioned <see cref="BoundaryHeight"/> above the floor.
        /// </remarks>
        bool ShowBoundaryCeiling { get; set; }

        /// <summary>
        /// The physics layer that the generated boundary ceiling is assigned to.
        /// </summary>
        int CeilingPhysicsLayer { get; set; }

        /// <summary>
        /// Two dimensional representation of the geometry of the boundary, as provided
        /// by the platform.
        /// </summary>
        /// <remarks>
        /// BoundaryGeometry should be treated as the outline of the player's space, placed
        /// on the floor.
        /// </remarks>
        Edge[] Bounds { get; }

        /// <summary>
        /// Indicates the height of the floor, in relation to the coordinate system origin.
        /// </summary>
        /// <remarks>
        /// If a floor has been located, FloorHeight.HasValue will be true, otherwise it will be false.
        /// </remarks>
        float? FloorHeight { get; }

        /// <summary>
        /// Determines if a location is within the specified area of the boundary space.
        /// </summary>
        /// <param name="location">The location to be checked.</param>
        /// <param name="boundaryType">The type of boundary space being checked.</param>
        /// <returns>True if the location is within the specified area of the boundary space.</returns>
        /// <remarks>
        /// Use:
        /// BoundaryType.PlayArea for the inscribed volume
        /// BoundaryType.TrackedArea for the area defined by the boundary edges.
        /// </remarks>
        bool Contains(Vector3 location, BoundaryType boundaryType = BoundaryType.TrackedArea);

        /// <summary>
        /// Returns the description of the inscribed rectangular bounds.
        /// </summary>
        /// <param name="center">The center of the rectangle.</param>
        /// <param name="angle">The orientation of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <returns>True if an inscribed rectangle was found in the boundary geometry, false otherwise.</returns>
        bool TryGetRectangularBoundsParams(out Vector2 center, out float angle, out float width, out float height);

        /// <summary>
        /// Gets the <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> that represents the user's floor.
        /// </summary>
        /// <returns>The floor visualization object or null if one does not exist.</returns>
        GameObject GetFloorVisualization();

        /// <summary>
        /// Gets the <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> that represents the user's play area.
        /// </summary>
        /// <returns>The play area visualization object or null if one does not exist.</returns>
        GameObject GetPlayAreaVisualization();

        /// <summary>
        /// Gets the <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> that represents the user's tracked area.
        /// </summary>
        /// <returns>The tracked area visualization object or null if one does not exist.</returns>
        GameObject GetTrackedAreaVisualization();

        /// <summary>
        /// Gets the <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> that represents the user's boundary walls.
        /// </summary>
        /// <returns>The boundary wall visualization object or null if one does not exist.</returns>
        GameObject GetBoundaryWallVisualization();

        /// <summary>
        /// Gets the <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> that represents the upper surface of the user's boundary.
        /// </summary>
        /// <returns>The boundary ceiling visualization object or null if one does not exist.</returns>
        GameObject GetBoundaryCeilingVisualization();
    }
}