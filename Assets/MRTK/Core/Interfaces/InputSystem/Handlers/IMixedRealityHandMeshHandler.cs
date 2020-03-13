// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using UnityEngine;

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
    /// TODO: Troy - better comment
    /// See BaseHandVisualizer.OnHandMeshUpdated for an example of how to use the hand mesh info to render a mesh.
    /// </summary>
    public class HandMeshInfo
    {
        /// <summary>
        /// The vertices of the hand mesh in the initial coordinate system
        /// </summary>
        public Vector3[] vertices;

        /// <summary>
        /// Mesh triangle indices
        /// </summary>
        public int[] triangles;

        /// <summary>
        /// Hand mesh normals, in initial coordinate system
        /// </summary>
        public Vector3[] normals;

        /// <summary>
        /// UV mapping of the hand. TODO: Give more specific details about UVs.
        /// </summary>
        public Vector2[] uvs;

        /// <summary>
        /// Translation to apply to mesh to go from initial coordinates to world coordinates
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// Rotation to apply to mesh to go from initial coordinates to world coordinates
        /// </summary>
        public Quaternion rotation;
    }
}
 