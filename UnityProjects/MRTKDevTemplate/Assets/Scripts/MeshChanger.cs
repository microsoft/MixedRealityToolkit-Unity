// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Change the mesh of the on a mesh filter. 
    /// </summary>
    /// <remarks>
    /// This is useful for visualizing button presses.
    /// </remarks>
    [AddComponentMenu("MRTK/Examples/Mesh Changer")]
    public class MeshChanger : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private Mesh[] meshes;

        private int currentMeshSelection;

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        private void Start()
        {
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }
        }

        /// <summary>
        /// Increments to the next mesh in the input list of meshes and applies it to the mesh filter.
        /// </summary>
        public void Increment()
        {
            if (meshes != null && meshes.Length > 0)
            {
                currentMeshSelection = (currentMeshSelection + 1) % meshes.Length;
                if (meshFilter != null)
                {
                    meshFilter.sharedMesh = meshes[currentMeshSelection];
                }
            }
        }

        /// <summary>
        /// Decrements to the previous mesh in the input list of meshes and applies it to the mesh filter..
        /// </summary>
        public void Decrement()
        {
            if (meshes != null && meshes.Length > 0)
            {
                currentMeshSelection = (currentMeshSelection - 1 + meshes.Length) % meshes.Length;
                if (meshFilter != null)
                {
                    meshFilter.sharedMesh = meshes[currentMeshSelection];
                }
            }
        }
    }
}
#pragma warning restore CS1591
