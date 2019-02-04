//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// Renders meshes at the corners of a tool tip
    /// </summary>
    public class ToolTipBackgroundCorners : MonoBehaviour, IToolTipBackground
    {
        private enum ScaleModeEnum
        {
            /// <summary>
            /// Always keep the corners the same size, regardless of tooltip size
            /// </summary>
            World,
            /// <summary>
            /// Make the corners scale to the tooltip content parent's lossy scale
            /// </summary>
            Local,
        }

        public bool IsVisible
        {
            set
            {
                cornerBotLeft.gameObject.SetActive(value);
                cornerTopRight.gameObject.SetActive(value);
                cornerBotRight.gameObject.SetActive(value);
                cornerBotLeft.gameObject.SetActive(value);
            }
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

        public void OnContentChange(Vector3 localContentSize, Vector3 localContentOffset, Transform contentParentTransform)
        {
            // Get the local size of the content - this is the scale of the text under the content parent
            localContentSize.z = 1;
            // Multiply it by 0.5 to get extents
            localContentSize *= 0.5f;

            // Put the corner objects at the corners
            var topLeft = new Vector3(-localContentSize.x + localContentOffset.x, localContentSize.y + localContentOffset.y, localContentOffset.x);
            var topRight = new Vector3(localContentSize.x + localContentOffset.x, localContentSize.y + localContentOffset.y, localContentOffset.x);
            var botRight = new Vector3(localContentSize.x + localContentOffset.x, -localContentSize.y + localContentOffset.y, localContentOffset.x);
            var botLeft = new Vector3(-localContentSize.x + localContentOffset.x, -localContentSize.y + localContentOffset.y, localContentOffset.x);

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
                    Vector3 lossyScale = contentParentTransform.lossyScale;
                    globalScale.x /= lossyScale.x;
                    globalScale.y /= lossyScale.y;
                    globalScale.z /= lossyScale.z;
                    break;
                case ScaleModeEnum.Local:
                    float smallestDimension = Mathf.Min(Mathf.Min(globalScale.x, globalScale.y), globalScale.z);
                    globalScale = Vector3.one * smallestDimension;
                    break;
                default:
                    Debug.LogError($"Invalid scale mode {scaleMode}");
                    break;
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
    }
}
