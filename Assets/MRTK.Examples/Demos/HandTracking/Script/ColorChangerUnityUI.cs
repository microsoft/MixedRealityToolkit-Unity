// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Change the color of the material on a UnityUI Graphic (ex. Image).  Useful for visualizing button presses.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/ColorChangerUnityUI")]
    public class ColorChangerUnityUI : MonoBehaviour
    {
        [SerializeField]
        private Graphic graphic;

        private void Start()
        {
            if (graphic == null)
            {
                graphic = GetComponent<Graphic>();
            }
        }

        /// <summary>
        /// Sets a random color on the renderer's material.
        /// </summary>
        public void RandomColor()
        {
            graphic.color = UnityEngine.Random.ColorHSV();
        }
    }
}
