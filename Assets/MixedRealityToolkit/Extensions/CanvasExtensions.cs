// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extensions for the Canvas class.
    /// </summary>
    public static class CanvasExtensions
    {
        /// <summary>
        /// Convenience method for getting a plane for this canvas in world coordinates.
        /// </summary>
        /// <param name="canvas">The canvas to get the plane from.</param>
        /// <returns>A Plane for this canvas.</returns>
        public static Plane GetPlane(this Canvas canvas)
        {
            Vector3[] corners = canvas.GetWorldCorners();

            // Now set a plane from any of the 3 corners (clockwise) so that we can compute our gaze intersection
            Plane plane = new Plane(corners[0], corners[1], corners[2]);

            return plane;
        }

        /// <summary>
        /// Convenience method for getting the corners of the canvas in world coordinates. Ordered clockwise from bottom-left.
        /// </summary>
        /// <param name="canvas">The canvas to get the world corners from.</param>
        /// <returns>An array of Vector3s that represent the corners of the canvas in world coordinates.</returns>
        public static Vector3[] GetWorldCorners(this Canvas canvas)
        {
            Vector3[] worldCorners = new Vector3[4];
            RectTransform rect = canvas.GetComponent<RectTransform>();
            rect.GetWorldCorners(worldCorners);
            return worldCorners;
        }

        /// <summary>
        /// Convenience method for getting the corners of the canvas in local coordinates. Ordered clockwise from bottom-left.
        /// </summary>
        /// <param name="canvas">The canvas to get the local corners from.</param>
        /// <returns>An array of Vector3s that represent the corners of the canvas in local coordinates.</returns>
        public static Vector3[] GetLocalCorners(this Canvas canvas)
        {
            Vector3[] localCorners = new Vector3[4];
            RectTransform rect = canvas.GetComponent<RectTransform>();
            rect.GetLocalCorners(localCorners);
            return localCorners;
        }

        /// <summary>
        /// Convenience method for getting the corners of the canvas in viewport coordinates. Note
        /// that the points have the same ordering as the array returned in GetWorldCorners()
        /// </summary>
        /// <param name="canvas">The canvas to get the viewport corners from</param>
        /// <returns>An array of Vector3s that represent the corners of the canvas in viewport coordinates</returns>
        public static Vector3[] GetViewportCorners(this Canvas canvas)
        {
            Vector3[] viewportCorners = new Vector3[4];

            Vector3[] worldCorners = canvas.GetWorldCorners();

            for (int i = 0; i < 4; i++)
            {
                viewportCorners[i] = Camera.main.WorldToViewportPoint(worldCorners[i]);
            }

            return viewportCorners;
        }

        /// <summary>
        /// Gets the position of the corners for a canvas in screen space.
        /// 1 -- 2
        /// |    |
        /// 0 -- 3
        /// </summary>
        /// <param name="canvas">The canvas to get the screen corners for.</param>
        public static Vector3[] GetScreenCorners(this Canvas canvas)
        {
            Vector3[] screenCorners = new Vector3[4];
            Vector3[] worldCorners = canvas.GetWorldCorners();

            for (int i = 0; i < 4; i++)
            {
                screenCorners[i] = Camera.main.WorldToScreenPoint(worldCorners[i]);
            }

            return screenCorners;
        }

        /// <summary>
        /// Returns a rectangle in screen coordinates that encompasses the bounds of the target canvas.
        /// </summary>
        /// <param name="canvas">The canvas the get the screen rect for</param>
        public static Rect GetScreenRect(this Canvas canvas)
        {
            Vector3[] screenCorners = canvas.GetScreenCorners();
            float x = Mathf.Min(screenCorners[0].x, screenCorners[1].x);
            float y = Mathf.Min(screenCorners[0].y, screenCorners[3].y);
            float xMax = Mathf.Max(screenCorners[2].x, screenCorners[3].x);
            float yMax = Mathf.Max(screenCorners[1].y, screenCorners[2].y);
            return new Rect(x, y, xMax - x, yMax - y);
        }

        /// <summary>
        /// Raycast against a canvas using a ray.
        /// </summary>
        /// <param name="canvas">The canvas to raycast against</param>
        /// <param name="rayOrigin">The origin of the ray</param>
        /// <param name="rayDirection">The direction of the ray</param>
        /// <param name="distance">The distance of the ray</param>
        /// <param name="hitPoint">The hitpoint of the ray</param>
        /// <param name="hitChildObject">The child object that was hit or the canvas itself if it has no active children that were within the hit range.</param>
        public static bool Raycast(this Canvas canvas, Vector3 rayOrigin, Vector3 rayDirection, out float distance, out Vector3 hitPoint, out GameObject hitChildObject)
        {
            hitChildObject =null;
            Plane plane = canvas.GetPlane();
            Ray ray = new Ray(rayOrigin, rayDirection);

            if (plane.Raycast(ray, out distance))
            {
                // See if the point lies within the local canvas rect of the plane
                Vector3[] corners = canvas.GetLocalCorners();
                hitPoint = rayOrigin + (rayDirection.normalized * distance);
                Vector3 localHitPoint = canvas.transform.InverseTransformPoint(hitPoint);
                if (localHitPoint.x >= corners[0].x
                    && localHitPoint.x <= corners[3].x
                    && localHitPoint.y <= corners[2].y
                    && localHitPoint.y >= corners[3].y)
                {
                    hitChildObject = canvas.gameObject;

                    // look for the child object that was hit
                    RectTransform rectTransform = GetChildRectTransformAtPoint(canvas.GetComponent<RectTransform>(), hitPoint, true, true, true);
                    if (rectTransform != null)
                    {
                        hitChildObject = rectTransform.gameObject;
                    }
                    else
                    {
                        hitChildObject = canvas.gameObject;
                    }
                    
                    return true;
                }
            }

            hitPoint = Vector3.zero;

            return false;
        }

        /// <summary>
        /// Gets a child rect transform for the given point and parameters.
        /// </summary>
        /// <param name="rectTransformParent">The rect transform to look for children that may contain the projected (orthogonal to the child's normal) world point</param>
        /// <param name="worldPoint">The world point</param>
        /// <param name="recursive">Indicates if the check should be done recursively</param>
        /// <param name="shouldReturnActive">If true, will only check children that are active, otherwise it will check all children.</param>
        /// <param name="shouldReturnRaycastable">If true, will only check children that if they have a graphic and have it's member raycastTarget set to true, otherwise will ignore the raycastTarget value. Will still allow children to be checked that do not have a graphic component.</param>
        public static RectTransform GetChildRectTransformAtPoint(this RectTransform rectTransformParent, Vector3 worldPoint, bool recursive, bool shouldReturnActive, bool shouldReturnRaycastable)
        {
            Vector3[] localCorners = new Vector3[4];
            Vector3 childLocalPoint;
            RectTransform rectTransform;
            bool shouldRaycast = false;

            for (int i=rectTransformParent.childCount-1; i >= 0; i--)
            {
                rectTransform = rectTransformParent.GetChild(i).GetComponent<RectTransform>();
                Graphic graphic = rectTransform.GetComponent<Graphic>();
                shouldRaycast = ((shouldReturnRaycastable && graphic != null && graphic.raycastTarget) || graphic == null || !shouldReturnRaycastable);

                if (((shouldReturnActive && rectTransform.gameObject.activeSelf) || !shouldReturnActive))
                {
                    rectTransform.GetLocalCorners(localCorners);
                    childLocalPoint = rectTransform.InverseTransformPoint(worldPoint);
                    
                    if (recursive)
                    {
                        RectTransform childRect = GetChildRectTransformAtPoint(rectTransform, worldPoint, recursive, shouldReturnActive, shouldReturnRaycastable);

                        if (childRect != null)
                        {
                            return childRect;
                        }
                    }

                    if (shouldRaycast
                        && childLocalPoint.x >= localCorners[0].x
                        && childLocalPoint.x <= localCorners[3].x
                        && childLocalPoint.y <= localCorners[2].y
                        && childLocalPoint.y >= localCorners[3].y)
                    {
                        return rectTransform;
                    }
                }
            }

            return null;
        }
    }
}