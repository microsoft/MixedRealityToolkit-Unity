// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// This script allows to zoom into and pan the texture of a GameObject. 
    /// It also allows for scrolling by restricting panning to one direction.  
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/PanZoomBaseRectTransform")]
    public class PanZoomBaseRectTransform : PanZoomBase
    {
        internal RectTransform navigationRectTransform = null;
        internal RectTransform viewportRectTransform = null;
        internal bool isScrollText = false;

        private bool IsValid => navigationRectTransform != null;

        /// <inheritdoc />
        protected override void Initialize()
        {
            if (IsValid)
            {
                offset = navigationRectTransform.anchoredPosition; // pivot
                scale = navigationRectTransform.localScale;
                originalRatio = new Vector3(scale.x, scale.y);
                dynamicZoomInvert = -1;
            }
        }

        /// <inheritdoc />
        protected override float ComputePanSpeed(float uvCursorPos, float maxSpeed, float minDistFromCenterForAutoPan)
        {
            // UV space from [0,1] -> Center: [-0.5, 0.5]
            float centeredVal = uvCursorPos - 0.5f;

            // If the UV cursor is close to the center of the image, prevent continuous movements to limit distractions
            if (Mathf.Abs(centeredVal) < minDistFromCenterForAutoPan)
            {
                return 0f;
            }

            float normalizedVal = (centeredVal - Mathf.Sign(centeredVal) * minDistFromCenterForAutoPan) / (Mathf.Sign(centeredVal) * (0.5f - minDistFromCenterForAutoPan));
            float speed = normalizedVal * normalizedVal * maxSpeed * Mathf.Sign(centeredVal);
            return speed;
        }

        /// <inheritdoc />
        protected override int ZoomDir(bool zoomIn)
        {
            return zoomIn ? 1 : -1;
        }

        /// <inheritdoc />
        protected override void ZoomIn()
        {
            ZoomInOut_RectTransform(zoomDirection * zoomSpeed, cursorPosition);

            // Panning across entire target (-0.5, +0.5) to move target of interest towards center while zooming in
            PanHorizontally(ComputePanSpeed(cursorPosition.x, panSpeedLeftRight, minDistFromCenterForAutoPan.x));
            PanVertically(ComputePanSpeed(cursorPosition.y, panSpeedUpDown, minDistFromCenterForAutoPan.y));
        }

        /// <inheritdoc />
        protected override void ZoomOut()
        {
            ZoomInOut_RectTransform(zoomDirection * zoomSpeed, new Vector2(0.5f, 0.5f));
        }

        /// <inheritdoc />
        protected override void UpdatePanZoom()
        {
            offset = LimitPanning();

            // Assign new values
            navigationRectTransform.anchoredPosition = offset; // pivot
            navigationRectTransform.localScale = new Vector3(scale.x, scale.y, 1f);
        }

        private Vector2 LimitPanning()
        {
            if (limitPanning && navigationRectTransform.rect.size.x != 0f && navigationRectTransform.rect.size.y != 0f)
            {
                // Limit offsets to prevent the image to float out of the viewport
                // Assuming the pivot being at the center
                if (!isScrollText)
                {

                    float leftBounds = navigationRectTransform.rect.width * (scale.x - 1f) / 2f;
                    float rightBounds = -leftBounds;
                    offset.x = Mathf.Clamp(offset.x, Mathf.Min(leftBounds, rightBounds), Mathf.Max(leftBounds, rightBounds));

                    float lowerBounds = navigationRectTransform.rect.height * (scale.y - 1f) / 2f;
                    float upperBounds = -lowerBounds;
                    offset.y = Mathf.Clamp(offset.y, Mathf.Min(lowerBounds, upperBounds), Mathf.Max(lowerBounds, upperBounds));

                    if (viewportRectTransform != null)
                    {
                        Vector2 actualSize = (scale * navigationRectTransform.rect.size);

                        if (actualSize.x < viewportRectTransform.rect.width)
                        {
                            offset.x = 0f;
                        }

                        if (actualSize.y < viewportRectTransform.rect.height)
                        {
                            offset.y = 0f;
                        }
                    }
                }

                // Assuming the pivot being at the top left
                if (isScrollText && viewportRectTransform != null)
                {
                    float leftBounds = navigationRectTransform.rect.width * (scale.x - 1f) / 2f;
                    float rightBounds = -leftBounds;
                    offset.x = Mathf.Clamp(offset.x, Mathf.Min(leftBounds, rightBounds), Mathf.Max(leftBounds, rightBounds));

                    float lowerBounds = navigationRectTransform.rect.height * scale.y - viewportRectTransform.rect.height;
                    float upperBounds = 0f;
                    offset.y = Mathf.Clamp(offset.y, Mathf.Min(lowerBounds, upperBounds), Mathf.Max(lowerBounds, upperBounds));


                    Vector2 actualSize = (scale * navigationRectTransform.rect.size);
                    Vector2 tmpOffset = (viewportRectTransform.rect.size - actualSize) / 2f;

                    if (actualSize.x < viewportRectTransform.rect.width)
                    {
                        offset.x = tmpOffset.x;
                    }

                    if (actualSize.y < viewportRectTransform.rect.height)
                    {
                        offset.y = tmpOffset.y;
                    }
                }
            }
            return offset;
        }

        /// <summary>
        /// Update the scale and pivot of the RectTransform for zooming.
        /// </summary>
        /// <param name="speed">Zoom speed</param>
        /// <param name="pivot">Zoom pivot</param>
        private void ZoomInOut_RectTransform(float speed, Vector2 pivot)
        {
            Vector2 oldScale = new Vector2(scale.x, scale.y); ;
            Vector2 newScale = new Vector2(scale.x + speed * originalRatio.x, scale.y + speed * originalRatio.y);
            newScale = LimitScaling(newScale);

            // Update the offset based on the scale diff and the zoom pivot.

            // Transform cursor position in hitbox [0,1] to image space [-0.5, 0.5]
            Vector2 transformCursorPosition = cursorPosition - new Vector2(0.5f, 0.5f);
            pivot = transformCursorPosition * navigationRectTransform.rect.size;
            offsetRateZoom = (oldScale - newScale) * pivot;

            // Update the texture's scale to the computed value.
            scale = newScale;
        }

        /// <summary>
        /// Determine the position of the cursor within the hitbox. 
        /// </summary>
        /// <returns>True if this GameObject is hit.</returns>
        protected override bool UpdateCursorPosInHitBox(Vector3 hitPosition)
        {
            Vector3 center = gameObject.transform.position;
            Vector3 halfsize = gameObject.transform.lossyScale * 0.5f;

            // Let's transform back to the origin: Translate & Rotate
            Vector3 transformHitPoint = hitPosition - center;

            // Rotate around the y axis
            transformHitPoint = Quaternion.AngleAxis(-(gameObject.transform.rotation.eulerAngles.y - 180f), Vector3.up) * transformHitPoint;

            // Rotate around the x axis
            transformHitPoint = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.x, Vector3.right) * transformHitPoint;

            // Normalize the transformed hit point to as UV coordinates are in [0,1].
            float uvx = 1f - (Mathf.Clamp(transformHitPoint.x, -halfsize.x, halfsize.x) + halfsize.x) / (2f * halfsize.x);
            float uvy = (Mathf.Clamp(transformHitPoint.y, -halfsize.y, halfsize.y) + halfsize.y) / (2f * halfsize.y);
            cursorPosition = new Vector2(uvx, uvy);

            return true;
        }
    }
}
