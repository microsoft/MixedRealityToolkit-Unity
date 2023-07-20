// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Change the color of the material on a renderer. 
    /// </summary>
    /// <remarks>
    /// This is useful for visualizing button presses.
    /// </remarks>
    [AddComponentMenu("MRTK/Examples/Color Changer")]
    public class ColorChanger : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer meshRenderer;

        [SerializeField]
        private Material[] materials;

        private int currentMaterialSelection;

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        private void Start()
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }
        }

        /// <summary>
        /// Increments to the next material in the input list of materials and applies it to the renderer.
        /// </summary>
        public void Increment()
        {
            if (materials != null && materials.Length > 0)
            {
                currentMaterialSelection = (currentMaterialSelection + 1) % materials.Length;
                if (meshRenderer != null)
                {
                    meshRenderer.material = materials[currentMaterialSelection];
                }
            }
        }

        /// <summary>
        /// Decrements to the previous material in the input list of materials and applies it to the renderer.
        /// </summary>
        public void Decrement()
        {
            if (materials != null && materials.Length > 0)
            {
                currentMaterialSelection = (currentMaterialSelection - 1 + materials.Length) % materials.Length;
                if (meshRenderer != null)
                {
                    meshRenderer.material = materials[currentMaterialSelection];
                }
            }
        }

        /// <summary>
        /// Sets a random color on the renderer's material.
        /// </summary>
        public void RandomColor()
        {
            meshRenderer.material.color = Random.ColorHSV();
        }
    }
}
#pragma warning restore CS1591