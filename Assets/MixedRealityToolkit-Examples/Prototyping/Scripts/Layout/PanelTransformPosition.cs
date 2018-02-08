// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Prototyping.Layout
{
    /// <summary>
    /// Takes size values, similar to RectTransorm position values or pixel values used in designer tools and applies them to primitive objects.
    /// 
    /// A quick way to align and anchor 3D objects in Unity for mocking up UI and click-throughs.
    /// This solution is not as robust as Unity's RectTransform layout system,
    /// but it's quicker for prototypes.
    /// 
    /// Use with PanelTransformSize and PanelTransformSizeOffset to anchor and layout groups of elements.
    /// </summary>
    [ExecuteInEditMode]
    public class PanelTransformPosition : MonoBehaviour
    {
        // alignment choices, sets the center of the transform to the chosen position of the anchor
        public enum AlignmentTypes { None, TopLeft, Top, TopRight, CenterLeft, Center, CenterRight, BottomLeft, Bottom, BottomRight }

        [Tooltip("Where to set this object's center point in relation to the Anchor's center point")]
        public AlignmentTypes Alignment;

        [Tooltip("A pixel to Unity unit conversion, Default: 2048x2048 pixels covers a 1x1 Unity Unit or default primitive size")]
        public float BasePixelSize = 2048;

        [Tooltip("The transform this object should be linked and aligned to")]
        public Transform Anchor;

        [Tooltip("Offset this object's position based on the same pixel based size ratio")]
        public Vector3 AnchorOffset;

        [Tooltip("Ignore the anchor's z scaling when positioning this object")]
        public bool IgnoreZScale;
        
        protected Vector3 mAnchorScale;
        protected Vector3 mAnchorPosition;

        /// <summary>
        /// A transform is required for alignment
        /// </summary>
        protected virtual void Awake()
        {
            if (Anchor == null)
            {
                Anchor = this.transform;
            }
        }

        /// <summary>
        /// Get the scale and position of the anchor
        /// </summary>
        protected virtual void SetScale()
        {
            mAnchorScale = Anchor.localScale;
            mAnchorPosition = Anchor.localPosition;
        }

        /// <summary>
        /// Set this object's position
        /// </summary>
        protected virtual void UpdatePosition()
        {
            SetScale();

            // set the default directions
            Vector3 horizontalVector = Vector3.right;
            Vector3 verticalVector = Vector3.up;
            Vector3 startPosition = mAnchorPosition;

            // set the offset distances
            float xMagnitude = AnchorOffset.x / BasePixelSize;
            float yMagnitude = AnchorOffset.y / BasePixelSize;
            float zMagnitude = AnchorOffset.z / BasePixelSize;

            // set the default position
            if (Alignment == AlignmentTypes.None)
            {

                Vector3 newPosition = startPosition + horizontalVector * xMagnitude + verticalVector * yMagnitude + Vector3.forward * zMagnitude;
                transform.localPosition = newPosition;
                return;
            }

            // update the directions and anchor alignment position based on the alignment setting
            switch (Alignment)
            {
                case AlignmentTypes.TopLeft:
                    horizontalVector = Vector3.right;
                    verticalVector = Vector3.down;
                    startPosition.x = mAnchorPosition.x - mAnchorScale.x * 0.5f;
                    startPosition.y = mAnchorPosition.y + mAnchorScale.y * 0.5f;
                    break;
                case AlignmentTypes.Top:
                    horizontalVector = Vector3.right;
                    verticalVector = Vector3.down;
                    startPosition.x = mAnchorPosition.x;
                    startPosition.y = mAnchorPosition.y + mAnchorScale.y * 0.5f;
                    break;
                case AlignmentTypes.TopRight:
                    horizontalVector = Vector3.left;
                    verticalVector = Vector3.down;
                    startPosition.x = mAnchorPosition.x + mAnchorScale.x * 0.5f;
                    startPosition.y = mAnchorPosition.y + mAnchorScale.y * 0.5f;
                    break;
                case AlignmentTypes.CenterLeft:
                    horizontalVector = Vector3.right;
                    verticalVector = Vector3.up;
                    startPosition.x = mAnchorPosition.x - mAnchorScale.x * 0.5f;
                    startPosition.y = mAnchorPosition.y;
                    break;
                case AlignmentTypes.Center:
                    horizontalVector = Vector3.right;
                    verticalVector = Vector3.up;
                    startPosition.x = mAnchorPosition.x;
                    startPosition.y = mAnchorPosition.y;
                    break;
                case AlignmentTypes.CenterRight:
                    horizontalVector = Vector3.left;
                    verticalVector = Vector3.up;
                    startPosition.x = mAnchorPosition.x + mAnchorScale.x * 0.5f;
                    startPosition.y = mAnchorPosition.y;
                    break;
                case AlignmentTypes.BottomLeft:
                    horizontalVector = Vector3.right;
                    verticalVector = Vector3.up;
                    startPosition.x = mAnchorPosition.x - mAnchorScale.x * 0.5f;
                    startPosition.y = mAnchorPosition.y - mAnchorScale.y * 0.5f;
                    break;
                case AlignmentTypes.Bottom:
                    horizontalVector = Vector3.right;
                    verticalVector = Vector3.up;
                    startPosition.x = mAnchorPosition.x;
                    startPosition.y = mAnchorPosition.y - mAnchorScale.y * 0.5f;
                    break;
                case AlignmentTypes.BottomRight:
                    horizontalVector = Vector3.left;
                    verticalVector = Vector3.up;
                    startPosition.x = mAnchorPosition.x + mAnchorScale.x * 0.5f;
                    startPosition.y = mAnchorPosition.y - mAnchorScale.y * 0.5f;
                    break;
                default:
                    break;
            }

            // set the aligned position
            if (!IgnoreZScale)
            {
                startPosition.z = mAnchorPosition.z - mAnchorScale.z * 0.5f;
            }

            transform.localPosition = startPosition + horizontalVector * xMagnitude + verticalVector * yMagnitude + Vector3.forward * zMagnitude;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (Anchor != null)
            {
                UpdatePosition();

            }
        }
    }
}
