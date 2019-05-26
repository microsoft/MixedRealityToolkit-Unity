// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Numerics;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.Common
{
    /// <summary>
    /// Defines the potential states of alignment for a strategy.
    /// </summary>
    public enum LocatedState
    {
        /// <summary>
        /// An error has occurred with the alignment strategy.
        /// </summary>
        Error = -1,

        /// <summary>
        /// Alignment has never been resolved in this session.
        /// </summary>
        Unresolved = 0,

        //TODO this vs Resolved, what are the use cases.
        /// <summary>
        /// Alignment is temporarily inhibited (usually due to some form of tracking loss).
        /// </summary>
        Inhibited,

        /// <summary>
        /// Alignment has been resolved at least once but there is no active tracking.
        /// </summary>
        Resolved,

        /// <summary>
        /// Alignment has been resolved at least once and there is active tracking.
        /// </summary>
        Tracking,
    }

    /// <summary>
    /// This represents a spatial coordinate that can then be used to convert position and rotation to and from this coordinate space.
    /// </summary>
    public interface ISpatialCoordinate
    {
        /// <summary>
        /// Occurs when the value of the <see cref="State"/> property has changed.
        /// </summary>
        event Action StateChanged;

        /// <summary>
        /// Gets the Id representing this coordinate.
        /// </summary>
        string Id { get; }

        // TODO agree with Jared
        //Vector3 Accuracy { get; }

        /// <summary>
        /// Gets the current state of the coordinate.
        /// </summary>
        LocatedState State { get; }

        /// <summary>
        /// Converts world space position to coordinate space position.
        /// For example, applying this transform to Vector3.zero would return the position of the local application's world space origin in the coordinate space.
        /// </summary>
        Vector3 WorldToCoordinateSpace(Vector3 vector);

        /// <summary>
        /// Converts world space rotation to coordinate space rotation.
        /// For example, applying this transform to Quaternion.identity would return the quaternion of the local application's world space origin in the coordinate space.
        /// </summary>
        Quaternion WorldToCoordinateSpace(Quaternion quaternion);

        /// <summary>
        /// Converts coordinate space position to world space position.
        /// For example, applying this transform to Vector3.zero would return the position of the coordinate in the local application's world space.
        /// </summary>
        Vector3 CoordinateToWorldSpace(Vector3 vector);

        /// <summary>
        /// Converts coordinate space position to world space position.
        /// For example, applying this transform to Quaternion.identity would return the quaternion of the coordinate in the local application's world space.
        /// </summary>
        Quaternion CoordinateToWorldSpace(Quaternion quaternion);
    }
}
