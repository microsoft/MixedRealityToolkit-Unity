// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// This script allows to zoom into and pan the texture of a GameObject. 
    /// It also allows for scrolling by restricting panning to one direction.  
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/PanZoomBaseRectTransf")]
    public class PanZoomBaseRectTransf : PanZoomBase
    {
        internal RectTransform navRectTransf = null;
        internal RectTransform viewportRectTransf = null;
        internal bool isScrollText = false;

        private bool IsValid => navRectTransf != null;

        public override void Initialize()
        {
            if (IsValid)
            {
                offset = navRectTransf.anchoredPosition; // pivot
                scale = navRectTransf.localScale;
                originalRatio = new Vector3(scale.x, scale.y);
                dynaZoomInvert = -1;
            }
        }

        public override float ComputePanSpeed(float uvCursorPos, float maxSpeed, float minDistFromCenterForAutoPan)
        {
            // UV space from [0,1] -> Center: [-0.5, 0.5]
            float centeredVal = (uvCursorPos - 0.5f);

            // If the UV cursor is close to the center of the image, prevent continuous movements to limit distractions
            if (Mathf.Abs(centeredVal) < minDistFromCenterForAutoPan)
            {
                return 0;
            }
            else
            {
                float normalizedVal = (centeredVal - Mathf.Sign(centeredVal) * minDistFromCenterForAutoPan) / (Mathf.Sign(centeredVal) * (0.5f - minDistFromCenterForAutoPan));
                float speed = normalizedVal * normalizedVal * maxSpeed * Mathf.Sign(centeredVal);
                return speed;
            }
        }

        public override int ZoomDir(bool zoomIn)
        {
            return (zoomIn ? 1 : -1);
        }

        public override void ZoomIn()
        {
            ZoomInOut_RectTransform(zoomDir * zoomSpeed, cursorPos);

            // Panning across entire target (-0.5, +0.5) to move target of interest towards center while zooming in
            PanHorizontally(ComputePanSpeed(cursorPos.x, panSpeedLeftRight, minDistFromCenterForAutoPan.x));
            PanVertically(ComputePanSpeed(cursorPos.y, panSpeedUpDown, minDistFromCenterForAutoPan.y));
        }

        public override void ZoomOut()
        {
            ZoomInOut_RectTransform(zoomDir * zoomSpeed, new Vector2(0.5f, 0.5f));
        }

        public override void UpdatePanZoom()
        {
            offset = LimitPanning();

            // Assign new values
            navRectTransf.anchoredPosition = offset; // pivot
            navRectTransf.localScale = new Vector3(scale.x, scale.y, 1);
        }

        private Vector2 LimitPanning()
        {
            if (limitPanning && (navRectTransf.rect.size.x != 0) && (navRectTransf.rect.size.y != 0))
            {
                // Limit offsets to prevent the image to float out of the viewport
                // Assuming the pivot being at the center
                if (!isScrollText)
                {

                    float leftBounds = navRectTransf.rect.width * (scale.x - 1) / 2;
                    float rightBounds = -leftBounds;
                    offset.x = Mathf.Clamp(offset.x, Mathf.Min(leftBounds, rightBounds), Mathf.Max(leftBounds, rightBounds));

                    float lowerBounds = navRectTransf.rect.height * (scale.y - 1) / 2;
                    float upperBounds = -lowerBounds;
                    offset.y = Mathf.Clamp(offset.y, Mathf.Min(lowerBounds, upperBounds), Mathf.Max(lowerBounds, upperBounds));

                    if (viewportRectTransf != null)
                    {
                        Vector2 actualSize = (scale * navRectTransf.rect.size);

                        if (actualSize.x < viewportRectTransf.rect.width)
                        {
                            offset.x = 0;
                        }

                        if (actualSize.y < viewportRectTransf.rect.height)
                        {
                            offset.y = 0;
                        }
                    }
                }

                // Assuming the pivot being at the top left
                if ((isScrollText) && (viewportRectTransf != null))
                {
                    float leftBounds = navRectTransf.rect.width * (scale.x - 1) / 2;
                    float rightBounds = -leftBounds;
                    offset.x = Mathf.Clamp(offset.x, Mathf.Min(leftBounds, rightBounds), Mathf.Max(leftBounds, rightBounds));

                    float lowerBounds = navRectTransf.rect.height * scale.y - viewportRectTransf.rect.height;
                    float upperBounds = 0;
                    offset.y = Mathf.Clamp(offset.y, Mathf.Min(lowerBounds, upperBounds), Mathf.Max(lowerBounds, upperBounds));


                    Vector2 actualSize = (scale * navRectTransf.rect.size);
                    Vector2 tmpOffset = (viewportRectTransf.rect.size - actualSize) / 2;

                    if (actualSize.x < viewportRectTransf.rect.width)
                    {
                        offset.x = tmpOffset.x;
                    }

                    if (actualSize.y < viewportRectTransf.rect.height)
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

            // Transform cursor pos in hitbox [0,1] to image space [-0.5, 0.5]
            Vector2 transfCursorPos = cursorPos - new Vector2(0.5f, 0.5f);
            pivot = transfCursorPos * navRectTransf.rect.size;
            offsetRate_Zoom = (oldScale - newScale) * pivot;

            // Update the texture's scale to the computed value.
            scale = newScale;
        }

        /// <summary>
        /// Determine the position of the cursor within the hitbox. 
        /// </summary>
        /// <returns>True if this GameObject is hit.</returns>
        public override bool UpdateCursorPosInHitBox()
        {
            bool objIsHit = false;

            try
            {
                if (myEyeTarget.IsLookedAt)
                {
                    objIsHit = true;

                    Vector3 center = gameObject.transform.position;
                    Vector3 halfsize = gameObject.transform.lossyScale / 2;

                    // Let's transform back to the origin: Translate & Rotate
                    Vector3 transfHitPnt = CoreServices.InputSystem.EyeGazeProvider.HitPosition - center;

                    // Rotate around the y axis
                    transfHitPnt = Quaternion.AngleAxis(-(gameObject.transform.rotation.eulerAngles.y - 180), Vector3.up) * transfHitPnt;

                    // Rotate around the x axis
                    transfHitPnt = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.x, Vector3.right) * transfHitPnt;

                    // Normalize the transformed hit point to as UV coordinates are in [0,1].
                    float uvx = 1 - (Mathf.Clamp(transfHitPnt.x, -halfsize.x, halfsize.x) + halfsize.x) / (2 * halfsize.x);
                    float uvy = (Mathf.Clamp(transfHitPnt.y, -halfsize.y, halfsize.y) + halfsize.y) / (2 * halfsize.y);
                    cursorPos = new Vector2(uvx, uvy);
                }
                else
                {
                    objIsHit = false;
                }
            }
            catch (UnityEngine.Assertions.AssertionException)
            {
                Debug.LogError(">> AssertionException in PanZoomBase");
            }
            return objIsHit;
        }
    }
}