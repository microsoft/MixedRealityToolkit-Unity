// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Numerics;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing
{
    /// <summary>
    /// This represents a spatial coordinate that can then be used to convert position and rotation to and from this coordinate space.
    /// </summary>
    public interface ISpatialCoordinate
    {
        /// <summary>
        /// Gets the Id representing this coordinate.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets whether this coordinate is currently located (seen).
        /// </summary>
        bool IsLocated { get; }

        /// <summary>
        /// Converst world space position to coordinate space position.
        /// </summary>
        Vector3 WorldToCoordinateSpace(Vector3 vector);

        /// <summary>
        /// Converst world space rotation to coordinate space rotation.
        /// </summary>
        Quaternion WorldToCoordinateSpace(Quaternion quaternion);

        /// <summary>
        /// Converst coordinate space position to world space position.
        /// </summary>
        Vector3 CoordinateToWorldSpace(Vector3 vector);

        /// <summary>
        /// Converst coordinate space position to world space position.
        /// </summary>
        Quaternion CoordinateToWorldSpace(Quaternion quaternion);
    }
}
