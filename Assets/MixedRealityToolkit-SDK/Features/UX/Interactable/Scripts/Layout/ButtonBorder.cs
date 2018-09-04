// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    [ExecuteInEditMode]
    public class ButtonBorder : MonoBehaviour
    {
        [Tooltip("A pixel to Unity unit conversion, Default: 2048x2048 pixels covers a 1x1 Unity Unit or default primitive size")]
        public float BasePixelScale = 2048;

        [Tooltip("The transform this object should be linked and aligned to")]
        public Transform AnchorTransform;

        [Tooltip("Size of the border using pixel values from our design program.")]
        public float Weight = 10;

        [Tooltip("Depth of the border using pixel values from our design program.")]
        public float Depth = 20;

        [Tooltip("Where to set this object's center point in relation to the Anchor's center point")]
        public Vector3 Alignment;

        [Tooltip("That absolute amount to offset the position")]
        public Vector3 PositionOffset;

        [Tooltip("Will extend the height or width of the border to create corners.")]
        public bool AddCorner = true;

        [Tooltip("should this only run in Edit mode, to avoid updating as items move?")]
        public bool OnlyInEditMode;

        /// <summary>
        /// Set the size
        /// </summary>
        private void UpdateSize()
        {
            Vector3 weighDireciton = new Vector3(Mathf.Abs(Alignment.x), Mathf.Abs(Alignment.y), Mathf.Abs(Alignment.z));
            Vector3 scale = weighDireciton * (Weight / BasePixelScale);// Vector3.Scale(Alignment, Scale) + Offset / BasePixelScale;
            float size = ((Weight * 2) / BasePixelScale);
            if (scale.x > scale.y)
            {
                scale.y = AddCorner ? AnchorTransform.localScale.y + size : AnchorTransform.localScale.y;
            }
            else
            {
                scale.x = AddCorner ? AnchorTransform.localScale.x + size : AnchorTransform.localScale.x;
            }
            scale.z = Depth / BasePixelScale;

            transform.localScale = scale;

            Vector3 startPosition = AnchorTransform.localPosition;

            if (AnchorTransform != this.transform)
            {
                startPosition = AnchorTransform.localPosition + (Vector3.Scale(AnchorTransform.localScale * 0.5f, Alignment));
            }

            transform.localPosition = startPosition + (Alignment * Weight * 0.5f / BasePixelScale) + (PositionOffset / BasePixelScale);
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if ((Application.isPlaying && !OnlyInEditMode) || (!Application.isPlaying))
            {
                UpdateSize();
            }
#else
                UpdateSize();
#endif
        }
    }
}
