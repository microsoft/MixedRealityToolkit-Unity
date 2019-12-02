// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Scales an object relative the scale of the AnchorTransform
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Scripts/MRTK/SDK/ButtonSizeOffset")]
    public class ButtonSizeOffset : MonoBehaviour
    {

        [Tooltip("A pixel to Unity unit conversion, Default: 2048x2048 pixels covers a 1x1 Unity Unit or default primitive size")]
        public float BasePixelScale = 2048;

        [Tooltip("The transform this object should be linked and aligned to")]
        public Transform AnchorTransform;

        [Tooltip(" How much to scale compared to the Anchor's size")]
        public Vector3 Scale = Vector3.one;

        [Tooltip("That absolute amount to offset the scale")]
        public Vector3 Offset;

        [Tooltip("should this only run in Edit mode, to avoid updating as items move?")]
        public bool OnlyInEditMode;

        /// <summary>
        /// Set the size based on the anchor's size and the buffers
        /// </summary>
        private void UpdateSize()
        {
            Vector3 scale = Vector3.Scale(AnchorTransform.localScale, Scale) + Offset / BasePixelScale;
            transform.localScale = scale;
        }

        // Update is called once per frame
        void Update()
        {
            if (AnchorTransform != null)
            {
                if ((Application.isPlaying && !OnlyInEditMode) || (!Application.isPlaying))
                {
                    UpdateSize();
                }
            }
        }
    }
}
