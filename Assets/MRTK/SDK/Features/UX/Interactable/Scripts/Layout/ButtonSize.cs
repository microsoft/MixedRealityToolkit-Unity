// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [ExecuteInEditMode]
    [AddComponentMenu("Scripts/MRTK/SDK/ButtonSize")]
    public class ButtonSize : MonoBehaviour
    {
        [Tooltip("A pixel to Unity unit conversion, Default: 2048x2048 pixels covers a 1x1 Unity Unit or default primitive size")]
        public float BasePixelScale = 2048;

        [Tooltip("Size of the primitive using pixel values from our design program.")]
        public Vector3 ItemSize = new Vector3(594, 246, 15);

        [Tooltip("should this only run in Edit mode, to avoid updating as items move?")]
        public bool OnlyInEditMode;

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
            if ((Application.isPlaying && !OnlyInEditMode) || (!Application.isPlaying))
            {
                UpdateSize();
            }
        }
    }
}
