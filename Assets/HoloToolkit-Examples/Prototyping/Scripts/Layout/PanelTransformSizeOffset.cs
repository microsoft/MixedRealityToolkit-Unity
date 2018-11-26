// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// Adds margins or buffers using size values, similar to RectTransorm size values or pixel values used in designer tools.
    /// 
    /// A quick way to size and position sub elements in a UI mock up.
    /// Use to stack elements and create more interesting designs.
    /// 
    /// The size buffers create margins around this object in relation of the AnchorTransforms size and position.
    /// </summary>
    [ExecuteInEditMode]
    public class PanelTransformSizeOffset : MonoBehaviour
    {
        [Tooltip("A pixel to Unity unit conversion, Default: 2048x2048 pixels covers a 1x1 Unity Unit or default primitive size")]
        public float BasePixelScale = 2048;

        [Tooltip("The transform this object should be linked and aligned to")]
        public Transform AnchorTransform;

        [Tooltip(" The z-depth or size of this element using the pixel base size ratio")]
        public float Depth = 20;

        [Tooltip("The amount of pixel based distance between the top edge of the Anchor and this element's top edge")]
        public float TopBuffer = 10;

        [Tooltip("The amount of pixel based distance between the top edge of the Anchor and this element's right edge")]
        public float RightBuffer = 10;

        [Tooltip("The amount of pixel based distance between the left edge of the Anchor and this element's left edge")]
        public float LeftBuffer = 10;

        [Tooltip("The amount of pixel based distance between the top edge of the Anchor and this element's bottom edge")]
        public float BottomBuffer = 10;

        [Tooltip("The amount of pixel based distance to offset the position along the z axis")]
        public float ZOffset = -5;

        /// <summary>
        /// Set the position based on the Anchor's size and the buffers
        /// </summary>
        private void UpdatePosition()
        {
            float xOffset = (RightBuffer - LeftBuffer) / 2;
            float yOffset = (TopBuffer - BottomBuffer) / 2;

            Vector3 newPosition = AnchorTransform.localPosition + Vector3.right * (xOffset / BasePixelScale) + Vector3.down * (yOffset / BasePixelScale) + Vector3.forward * (ZOffset / BasePixelScale);
            transform.localPosition = newPosition;
        }

        /// <summary>
        /// Set the size based on the Anchor's size and the buffers
        /// </summary>
        private void UpdateSize()
        {
            Vector3 newScale = new Vector3((AnchorTransform.localScale.x * BasePixelScale - (RightBuffer + LeftBuffer)) / BasePixelScale, (AnchorTransform.localScale.y * BasePixelScale - (TopBuffer + BottomBuffer)) / BasePixelScale, Depth / BasePixelScale);
            transform.localScale = newScale;
        }

        // Update is called once per frame
        void Update()
        {
            if (AnchorTransform != null)
            {
                UpdateSize();
                UpdatePosition();
            }
        }
    }
}
