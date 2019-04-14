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

    // See BaseHandVisualizer.OnHandMeshUpdated for an example of how to use the
    // hand mesh info to render a mesh.
    public class HandMeshInfo
    {
        // The vertices of the hand mesh in the initial coordinate system
        public Vector3[] vertices;

        // Mesh triangle indices
        public int[] triangles;

        // Hand mesh normals, in initial coordinate system
        public Vector3[] normals;

        // UV mapping of the hand. TODO: Give more specific details about UVs.
        public Vector2[] uvs;

        // Translation to apply to mesh to go from initial coordinates to world coordinates
        public Vector3 position;

        // Rotation to apply to mesh to go from initial coordinates to world coordinates
        public Quaternion rotation;
    }
}