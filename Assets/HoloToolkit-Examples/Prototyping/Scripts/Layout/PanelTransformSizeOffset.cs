// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.Prototyping
{
    [ExecuteInEditMode]
    public class PanelTransformSizeOffset : MonoBehaviour
    {
        public float BasePixelScale = 2048;
        public Transform AnchorTransform;
        public float Depth = 20;
        public float TopBuffer = 10;
        public float RightBuffer = 10;
        public float LeftBuffer = 10;
        public float BottomBuffer = 10;
        public float ZOffset = -5;

        private void UpdatePosition()
        {
            float xOffset = (RightBuffer - LeftBuffer) / 2;
            float yOffset = (TopBuffer - BottomBuffer) / 2;

            Vector3 newPosition = AnchorTransform.localPosition + Vector3.right * (xOffset / BasePixelScale) + Vector3.down * (yOffset / BasePixelScale) + Vector3.forward * (ZOffset / BasePixelScale);
            transform.localPosition = newPosition;
        }

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
                if (AnchorTransform != null)
                {
                    UpdateSize();
                    UpdatePosition();
                }
            }
        }
    }
}
