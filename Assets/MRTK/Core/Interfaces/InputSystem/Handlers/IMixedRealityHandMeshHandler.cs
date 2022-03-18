// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to implement for hand mesh information.
    /// </summary>
    public interface IMixedRealityHandMeshHandler : IEventSystemHandler
    {
        void OnHandMeshUpdated(InputEventData<HandMeshInfo> eventData);
    }

    /// <summary>
    /// Stores pointers and transform information for Hand Mesh data provided by current platform. This is the data container for the IMixedRealityHandMeshHandler input system event interface.
    /// </summary>
    public class HandMeshInfo
    {
        /// <summary>
        /// Pointer to vertices buffer of the hand mesh in the local coordinate system (i.e relative to center of hand)
        /// </summary>
        public Vector3[] vertices;

        /// <summary>
        /// Pointer to the triangle indices buffer of the hand mesh. 
        /// </summary>
        public int[] triangles;

        /// <summary>
        /// Pointer to the normals buffer of the hand mesh in the local coordinate system
        /// </summary>
        public Vector3[] normals;

        /// <summary>
        /// Pointer to UV mapping of the hand mesh triangles
        /// </summary>
        public Vector2[] uvs;

        /// <summary>
        /// Translation to apply to mesh to go from local coordinates to world coordinates
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// Rotation to apply to mesh to go from local coordinates to world coordinates
        /// </summary>
        public Quaternion rotation;
    }
}
