// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.Prototyping
{

    [ExecuteInEditMode]
    public class PanelTransformPosition : MonoBehaviour
    {
        public enum AlignmentTypes { None, TopLeft, Top, TopRight, CenterLeft, Center, CenterRight, BottomLeft, Bottom, BottomRight }
        public AlignmentTypes Alignment;
        public float BasePixelSize = 2048;

        public Transform Anchor;
        public Vector3 AnchorOffset;
        public bool IgnoreZScale;

        protected Vector3 mAnchorScale;
        protected Vector3 mAnchorPosition;

        protected virtual void Awake()
        {
            if (Anchor == null)
            {
                Anchor = this.transform;
            }
        }

        protected virtual void SetScale()
        {
            mAnchorScale = Anchor.localScale;
            mAnchorPosition = Anchor.localPosition;
        }

        protected virtual void UpdatePosition()
        {
            SetScale();

            Vector3 horizontalVector = Vector3.right;
            Vector3 verticalVector = Vector3.up;
            Vector3 startPosition = mAnchorPosition;

            float xMagnitude = AnchorOffset.x / BasePixelSize;
            float yMagnitude = AnchorOffset.y / BasePixelSize;
            float zMagnitude = AnchorOffset.z / BasePixelSize;

            if (Alignment == AlignmentTypes.None)
            {

                Vector3 newPosition = startPosition + horizontalVector * xMagnitude + verticalVector * yMagnitude + Vector3.forward * zMagnitude;
                transform.localPosition = newPosition;
                return;
            }

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
