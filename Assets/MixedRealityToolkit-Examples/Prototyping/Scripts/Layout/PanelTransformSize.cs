// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Prototyping.Layout
{
    /// <summary>
    /// Takes size values, similar to RectTransorm size values or pixel values used in designer tools and applies them to primitive objects.
    /// 
    /// Turns Unity primitive shapes, like the Quad or Cube, into panels, buttons and other basic UI elements.
    /// It's a solution to mocking up UI and building click-throughs.
    /// 
    /// For prototyping, a designer can export some art as a texture (PNG/TIFF/TRG) and apply it to a quad.
    /// Using this script the designer uses the sample pixel dimensions from their design program to set the
    /// height, width and depth to match their art.
    /// 
    /// Another use is recreating gray-box wireframes with Unity primitives.
    /// A designer can work in Unity in the same way they would work in Photoshop or Illustrator,
    /// building boxes and stacking them on top of each other.
    /// </summary>
    [ExecuteInEditMode]
    public class PanelTransformSize : MonoBehaviour
    {
        [Tooltip("A pixel to Unity unit conversion, Default: 2048x2048 pixels covers a 1x1 Unity Unit or default primitive size")]
        public float BasePixelScale = 2048;

        [Tooltip("Size of the primitive using pixel values from our design program.")]
        public Vector3 ItemSize = new Vector3(594, 246, 15);

        /// <summary>
        /// Set the size
        /// </summary>
        private void UpdateSize()
        {
            Vector3 newScale = new Vector3(ItemSize.x / BasePixelScale, ItemSize.y / BasePixelScale, ItemSize.z / BasePixelScale);
            transform.localScale = newScale;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateSize();
        }
    }
}
