// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Change the color of the material on a renderer.  Useful for visualizing button presses.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/ColorChanger")]
    public class ColorChanger : MonoBehaviour
    {
        public MeshRenderer rend;
        public Material[] mats;
        public int cur;

        private void Start()
        {
            if (rend == null)
            {
                rend = GetComponent<MeshRenderer>();
            }
        }

        /// <summary>
        /// Increments to the next material in the input list of materials and applies it to the renderer.
        /// </summary>
        public void Increment()
        {
            if (mats != null && mats.Length > 0)
            {
                cur = (cur + 1) % mats.Length;
                if (rend != null)
                {
                    rend.material = mats[cur];
                }
            }
        }
        
        /// <summary>
        /// Decrements to the previous material in the input list of materials and applies it to the renderer.
        /// </summary>
        public void Decrement()
        {
            if (mats != null && mats.Length > 0)
            {
                cur = (cur - 1 + mats.Length) % mats.Length;
                if (rend != null)
                {
                    rend.material = mats[cur];
                }
            }
        }

        /// <summary>
        /// Sets a random color on the renderer's material.
        /// </summary>
        public void RandomColor()
        {
            rend.material.color = UnityEngine.Random.ColorHSV();
        }
    }
}