// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// The base layout component for a button or UI elements - easily build UI with Unity Primitives.
    /// Helps to create consistency by using values that scale to a designer's 2D layout program.
    /// Based on a ratio of 2048 pixels for 1 meter of surface area.
    /// 
    /// Use case:
    /// A designer creates a concept image of UI based on a 2048 artboard.
    /// 2048 pixels is a nice resolution for a meter of content, two meters away from the user.
    ///     The FOV of the HoloLens is about 1 meter wide at 2 meters from the user meaning the
    ///     designer can assume an image area of 2048 x 1184 pixels at 2 meters from the user.
    /// The designer or engineer can take pixel based redlines and create UI at 1:1 scale.
    /// </summary>
    [ExecuteInEditMode]
    public class ButtonBackgroundSize : MonoBehaviour
    {
        /// <summary>
        /// A scale factor for button layouts, default is based on 2048 pixels to 1 meter.
        /// Similar to values used in designer and 2D art programs and helps create consistency across teams.
        /// </summary>
        [Tooltip("A pixel to Unity unit conversion, Default: 2048x2048 pixels covers a 1x1 Unity Unit or default primitive size")]
        [SerializeField]
        private float BasePixelScale = 2048;

        /// <summary>
        /// The size of this object in 3D space, based on the scale factor.
        /// This value should match 2D design pixel values.
        /// </summary>
        [Tooltip("Size of the primitive using pixel values from our design program.")]
        [SerializeField]
        protected Vector3 ItemSize = new Vector3(594, 246, 15);

        /// <summary>
        /// These scales are applied in Unity Editor only while doing layout.
        /// Turn off for responsive UI type results when editing ItemSize during runtime.
        /// Scales will be applied each frame.
        /// </summary>
        [Tooltip("should this only run in Edit mode, to avoid updating as items move?")]
        [SerializeField]
        private bool OnlyInEditMode = true;

        /// <summary>
        /// Set the size at Runtime or through code
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Vector3 size)
        {
            ItemSize = size;
        }

        /// <summary>
        /// Get the current size
        /// </summary>
        /// <returns></returns>
        public Vector3 GetSize()
        {
            return ItemSize;
        }

        /// <summary>
        /// Get the base pixel scale
        /// </summary>
        /// <returns></returns>
        public float GetBasePixelScale()
        {
            return BasePixelScale;
        }

        /// <summary>
        /// Set the base pixel scale
        /// </summary>
        /// <param name="scale"></param>
        public void SetBasePixelScale(float scale)
        {
            BasePixelScale = scale;
        }
        
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
            // only run in edit mode?
            if ((Application.isPlaying && !OnlyInEditMode) || (!Application.isPlaying))
            {
                UpdateSize();
            }
        }
    }
}
