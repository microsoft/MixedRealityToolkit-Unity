//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HoloToolkit.UX.ToolTips
{
    /// <summary>
    /// Renders meshes at the corners of a tool tip
    /// </summary>
    public class ToolTipBackgroundCorners : ToolTipBackground
    {
        private enum ScaleModeEnum
        {
            World,  // Always keep the corners the same size, regardless of tooltip size
            Local,  // Make the corners scale to the tooltip content parent's lossy scale
        }

        [SerializeField]
        private Transform cornerTopLeft = null;
        [SerializeField]
        private Transform cornerTopRight = null;
        [SerializeField]
        private Transform cornerBotRight = null;
        [SerializeField]
        private Transform cornerBotLeft = null;
        [SerializeField]
        [Range(0.01f, 2f)]
        private float cornerScale = 1f;
        [SerializeField]
        private ScaleModeEnum scaleMode = ScaleModeEnum.World;
        
        protected override void ContentChange()
        {
            ScaleToFitContent();
        }

        protected override void ScaleToFitContent()
        {
            // Get the local size of the content - this is the scale of the text under the content parent
            Vector3 localContentSize = toolTip.LocalContentSize;
            localContentSize.z = 1;
            // Multiply it by 0.5 to get extents
            localContentSize *= 0.5f;
            Vector3 localContentOffset = toolTip.LocalContentOffset;
            // Put the corner objects at the corners
            Vector3 topLeft = new Vector3(-localContentSize.x + localContentOffset.x, localContentSize.y + localContentOffset.y, localContentOffset.x);
            Vector3 topRight = new Vector3(localContentSize.x + localContentOffset.x, localContentSize.y + localContentOffset.y, localContentOffset.x);
            Vector3 botRight = new Vector3(localContentSize.x + localContentOffset.x, -localContentSize.y + localContentOffset.y, localContentOffset.x);
            Vector3 botLeft = new Vector3(-localContentSize.x + localContentOffset.x, -localContentSize.y + localContentOffset.y, localContentOffset.x);
            if (cornerTopLeft != null)
            {
                cornerTopLeft.localPosition = topLeft;
            }

            if (cornerTopRight != null)
            {
                cornerTopRight.localPosition = topRight;
            }

            if (cornerBotRight != null)
            {
                cornerBotRight.localPosition = botRight;
            }

            if (cornerBotLeft != null)
            {
                cornerBotLeft.localPosition = botLeft;
            }

            // Now set the corner object scale based on preference
            Vector3 globalScale = Vector3.one;
            switch (scaleMode)
            {
                case ScaleModeEnum.World:
                    Vector3 lossyScale = toolTip.ContentParentTransform.lossyScale;
                    globalScale.x /= lossyScale.x;
                    globalScale.y /= lossyScale.y;
                    globalScale.z /= lossyScale.z;
                    break;

                case ScaleModeEnum.Local:
                    Vector3 localScale = cornerTopLeft.lossyScale;
                    float smallestDimension = Mathf.Min(Mathf.Min(globalScale.x, globalScale.y), globalScale.z);
                    globalScale = Vector3.one * smallestDimension;
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException("ScaleMode not set to valid enum value.");
            }

            if (cornerTopLeft != null)
            {
                cornerTopLeft.localScale = globalScale * cornerScale;
            }

            if (cornerTopRight != null)
            {
                cornerTopRight.localScale = globalScale * cornerScale;
            }

            if (cornerBotRight != null)
            {
                cornerBotRight.localScale = globalScale * cornerScale;
            }

            if (cornerBotLeft != null)
            {
                cornerBotLeft.localScale = globalScale * cornerScale;
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }

            ScaleToFitContent();
        }
    }
}
