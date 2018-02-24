// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using UnityEngine;

namespace MixedRealityToolkit.Utilities
{
    /// <summary>
    /// A Tagalong that extends SimpleTagalong that allows for specifying the
    /// minimum and target percentage of the object to keep in the view frustum
    /// of the camera and that keeps the Tagalong object in front of other
    /// holograms including the Spatial Mapping Mesh.
    /// </summary>
    public class Tagalong : SimpleTagalong
    {
        // These members allow for specifying target and minimum percentage in
        // the FOV.
        [Range(0.0f, 1.0f), Tooltip("The minimum horizontal percentage visible before the object starts tagging along.")]
        public float MinimumHorizontalOverlap = 0.1f;
        [Range(0.0f, 1.0f), Tooltip("The target horizontal percentage the Tagalong attempts to achieve.")]
        public float TargetHorizontalOverlap = 1.0f;
        [Range(0.0f, 1.0f), Tooltip("The minimum vertical percentage visible before the object starts tagging along.")]
        public float MinimumVerticalOverlap = 0.1f;
        [Range(0.0f, 1.0f), Tooltip("The target vertical percentage the Tagalong attempts to achieve.")]
        public float TargetVerticalOverlap = 1.0f;

        // These members control how many rays to cast when looking for
        // collisions with other holograms.
        [Range(3, 11), Tooltip("The number of rays to cast horizontally across the Tagalong.")]
        public int HorizontalRayCount = 3;
        [Range(3, 11), Tooltip("The number of rays to cast vertically across the Tagalong.")]
        public int VerticalRayCount = 3;

        [Tooltip("Don't allow the Tagalong to come closer than this distance.")]
        public float MinimumTagalongDistance = 1.0f;
        [Tooltip("When true, the Tagalong object maintains a fixed angular size.")]
        public bool MaintainFixedSize = true;

        [Tooltip("The speed to update the Tagalong's distance when compensating for depth (meters/second).")]
        public float DepthUpdateSpeed = 4.0f;

        private float defaultTagalongDistance;

        // These members are useful for debugging the Tagalong in Unity's
        // editor or the HoloLens.
        [Tooltip("Set to true to draw lines of interest in Unity's scene view during play-mode.")]
        public bool DebugDrawLines = false;
        [Tooltip("Useful for visualizing the Raycasts used for determining the depth to place the Tagalong. Set to 'None' to disable.")]
        public Light DebugPointLight;

        protected override void Start()
        {
            base.Start();

            // Remember the default for distance.
            defaultTagalongDistance = TagalongDistance;

            // If the specified minimum distance for the tagalong would be within the
            // camera's near clipping plane, adjust it to be 10% beyond the near
            // clipping plane.
            if (CameraCache.Main.nearClipPlane > MinimumTagalongDistance)
            {
                MinimumTagalongDistance = CameraCache.Main.nearClipPlane * 1.1f;
            }

            // The EnforceDistance functionality of the SimmpleTagalong has a
            // detrimental effect on this Tagalong's desired behavior.
            // Disable that behavior here.
            EnforceDistance = false;

            // Add the FixedAngularSize script if MaintainFixedSize is true.
            if (MaintainFixedSize)
            {
                gameObject.AddComponent<FixedAngularSize>();
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!interpolator.AnimatingPosition)
            {
                // If we aren't animating towards a new position, check to see if
                // we need to update the Tagalong's position because it is behind
                // some other hologram or the Spatial Mapping mesh.
                Vector3 newPosition;
                if (AdjustTagalongDistance(CameraCache.Main.transform.position, out newPosition))
                {
                    interpolator.PositionPerSecond = DepthUpdateSpeed;
                    interpolator.SetTargetPosition(newPosition);
                    TagalongDistance = Mathf.Min(defaultTagalongDistance, Vector3.Distance(CameraCache.Main.transform.position, newPosition));
                }
            }
        }

        protected override bool CalculateTagalongTargetPosition(Vector3 fromPosition, out Vector3 toPosition)
        {
            bool needsToMoveX = false;
            bool needsToMoveY = false;
            toPosition = fromPosition;

            // Cache some things that we will need later.
            Transform cameraTransform = CameraCache.Main.transform;
            Vector3 cameraPosition = cameraTransform.position;

            // Get the bounds of the Tagalong's collider.
            Bounds colliderBounds = tagalongCollider.bounds;

            // Default the new position to be the current position.
            Vector3 newToPosition = tagalongCollider.bounds.center;

            // Adjust the center of the bounds to be TagalongDistance away from
            // the camera. We will use this point instead of tranform.position for
            // the rest of our calculations.
            Ray rayTemp = new Ray(cameraPosition, colliderBounds.center - cameraPosition);
            colliderBounds.center = rayTemp.GetPoint(TagalongDistance);

#if UNITY_EDITOR
            DebugDrawColliderBox(DebugDrawLines, colliderBounds);
#endif // UNITY_EDITOR

            // Get the actual width and height of the Tagalong's BoxCollider.
            float width = tagalongCollider.size.x * transform.lossyScale.x;
            float height = tagalongCollider.size.y * transform.lossyScale.y;

            // Determine if the Tagalong is to the left or right of the Camera's
            // forward vector.
            Plane verticalCenterPlane = new Plane(cameraTransform.right, cameraPosition + cameraTransform.forward);
            bool tagalongIsRightOfCenter = verticalCenterPlane.GetDistanceToPoint(colliderBounds.center) > 0;

            // Based on left/right of center choose the appropriate directional
            // vector and frustum plane for the rest of our horizontal calculations.
            Vector3 horizontalTowardCenter = tagalongIsRightOfCenter ? -transform.right : transform.right;
            Plane verticalFrustumPlane = tagalongIsRightOfCenter ? frustumPlanes[frustumRight] : frustumPlanes[frustumLeft];

            // Find the edge of the collider that is closest to the center plane.
            Vector3 centermostHorizontalEdge = colliderBounds.center + (horizontalTowardCenter * (width / 2f));

            // Find the point on the collider that is the MinimumHorizontalOverlap
            // as percentage away from the centermostHorizontalEdge.
            Vector3 targetPoint = centermostHorizontalEdge + (-horizontalTowardCenter * (width * MinimumHorizontalOverlap));

            // If the calculated targetPoint is outside the verticalFrustumPlane
            // of interest, we need to move the tagalong so it is at least
            // TargetHorizontalOverlap inside the view frustum.
            needsToMoveX = verticalFrustumPlane.GetDistanceToPoint(targetPoint) < 0;
            if (needsToMoveX || DebugDrawLines)
            {
                // Calculate the new target position, ignoring the vertical.
                Vector3 newCalculatedTargetPosition =
                    CalculateTargetPosition(true, centermostHorizontalEdge, horizontalTowardCenter, width,
                    colliderBounds.center, verticalFrustumPlane, tagalongIsRightOfCenter);

                if (needsToMoveX)
                {
                    newToPosition.x = newCalculatedTargetPosition.x;
                    newToPosition.z = newCalculatedTargetPosition.z;
                }
            }

            // Repeat everything we did above, but for the vertical dimension.
            // Comments will be abbreviated.

            colliderBounds = tagalongCollider.bounds;
            rayTemp = new Ray(cameraPosition, colliderBounds.center - cameraPosition);
            colliderBounds.center = rayTemp.GetPoint(TagalongDistance);
            Plane horizontalCenterPlane = new Plane(cameraTransform.up, cameraPosition + cameraTransform.forward);
            bool tagalongIsAboveCenter = horizontalCenterPlane.GetDistanceToPoint(colliderBounds.center) > 0;
            Vector3 verticalTowardCenter = tagalongIsAboveCenter ? -transform.up : transform.up;
            Plane horizontalFrustumPlane = tagalongIsAboveCenter ? frustumPlanes[frustumTop] : frustumPlanes[frustumBottom];
            Vector3 centermostVerticalEdge = colliderBounds.center + (verticalTowardCenter * (height / 2f));
            targetPoint = centermostVerticalEdge + (-verticalTowardCenter * (height * MinimumVerticalOverlap));
            // We've determined the Tagalong needs to move in the YZ plane.
            needsToMoveY = horizontalFrustumPlane.GetDistanceToPoint(targetPoint) < 0;
            if (needsToMoveY || DebugDrawLines)
            {
                // Calculate the new target position, ignoring the vertical.
                Vector3 newCalculatedTargetPosition =
                    CalculateTargetPosition(false, centermostVerticalEdge, verticalTowardCenter, height,
                    colliderBounds.center, horizontalFrustumPlane, !tagalongIsAboveCenter);
                if (needsToMoveY)
                {
                    newToPosition.y = newCalculatedTargetPosition.y;
                    newToPosition.z = newCalculatedTargetPosition.z;
                }
            }

            if (needsToMoveX || needsToMoveY)
            {
                Ray ray = new Ray(cameraPosition, newToPosition - cameraPosition);
                toPosition = ray.GetPoint(TagalongDistance);
            }

            return needsToMoveX || needsToMoveY;
        }

        /// <summary>
        /// Calculates a target position for the Tagalong in either the horizontal or vertical direction.
        /// </summary>
        /// <param name="isHorizontal">If true, the calculate horizontally; vertically otherwise.</param>
        /// <param name="centermostEdge">A point along the collider that is the closest to the center of the FOV.</param>
        /// <param name="vectorTowardCenter">A vector that points from the Tagalong toward the center of the FOV.</param>
        /// <param name="width">The actual width of the object's collider.</param>
        /// <param name="center">The center of the bounding box that surrounds the collider.</param>
        /// <param name="frustumPlane">The edge of the frustum we are tagging along towards.</param>
        /// <param name="invertAngle">True if the tagalong is to the right of or below the center of the FOV; false otherwise.</param>
        /// <returns>The new target position for the Tagalong.</returns>
        private Vector3 CalculateTargetPosition(bool isHorizontal, Vector3 centermostEdge, Vector3 vectorTowardCenter, float width,
            Vector3 center, Plane frustumPlane, bool invertAngle)
        {
            Transform cameraTransform = CameraCache.Main.transform;
            Vector3 cameraPosition = cameraTransform.position;

            // The target overlap can't be less than the minimum overlap. Pick
            // the bigger of the two.
            float desiredOverlap = isHorizontal
                ? Mathf.Max(MinimumHorizontalOverlap, TargetHorizontalOverlap)
                : Mathf.Max(MinimumVerticalOverlap, TargetVerticalOverlap);

            // Recalculate the targetPoint so it has the desired overlap.
            Vector3 targetPoint = centermostEdge + (-vectorTowardCenter * (width * desiredOverlap));

            // Find a point on the frustum we care about. Start with a point
            // in front of the camera and cast a ray from there to the frustum.
            Vector3 centeredPoint = cameraPosition + cameraTransform.forward * TagalongDistance;
            Ray rayTemp = new Ray(centeredPoint, (invertAngle ? 1 : -1) * (isHorizontal ? cameraTransform.right : cameraTransform.up));
            float distToFrustum = 0.0f;
            frustumPlane.Raycast(rayTemp, out distToFrustum);
            Vector3 pointOnFrustum = rayTemp.GetPoint(distToFrustum);

            // Adjust the point found on the frustum plane to be the same
            // distance from the camera as targetPoint is, but still on the
            // frustum plane.
            rayTemp = new Ray(cameraPosition, pointOnFrustum - cameraPosition);
            float distanceToTarget = Vector3.Distance(cameraPosition, targetPoint);
            Vector3 recalculatedPointOnFrustum = rayTemp.GetPoint(distanceToTarget);

            // Find the new calculated target position. First get the rotation
            // between the target and center of the collider.
            Quaternion rotQuat = Quaternion.FromToRotation(targetPoint - cameraPosition, center - cameraPosition);
            // Create the vector we want to rotate.
            Vector3 vectorToRotate = recalculatedPointOnFrustum - cameraPosition;
            // Create the new target position.
            Vector3 newCalculatedTargetPosition = cameraPosition + rotQuat * vectorToRotate;

#if UNITY_EDITOR
            DebugDrawDebuggingLines(DebugDrawLines, center, cameraPosition,
                cameraPosition + (targetPoint - cameraPosition),
                centeredPoint, pointOnFrustum, recalculatedPointOnFrustum,
                newCalculatedTargetPosition);
#endif // UNITY_EDITOR

            return newCalculatedTargetPosition;
        }

        private bool AdjustTagalongDistance(Vector3 cameraPosition, out Vector3 newPosition)
        {
            bool needsUpdating = false;

            // Get the actual width and height of the Tagalong's BoxCollider.
            float width = tagalongCollider.size.x * transform.lossyScale.x;
            float height = tagalongCollider.size.y * transform.lossyScale.y;

            // Find the lower-left corner of the Tagalong's BoxCollider.
            Vector3 lowerLeftCorner = transform.position - (transform.right * (width / 2)) - (transform.up * (height / 2));

            // Cast a grid of rays across the Tagalong's collider. Keep track of
            // of the closest hit, ignoring collisions with ourselves and those
            // that are closer than MinimumColliderDistance.
            RaycastHit closestHit = new RaycastHit();
            float closestHitDistance = float.PositiveInfinity;
            RaycastHit[] allHits;
            for (int x = 0; x < HorizontalRayCount; x++)
            {
                Vector3 xCoord = lowerLeftCorner + transform.right * (x * width / (HorizontalRayCount - 1));
                for (int y = 0; y < VerticalRayCount; y++)
                {
                    Vector3 targetCoord = xCoord + transform.up * (y * height / (VerticalRayCount - 1));

                    allHits = Physics.RaycastAll(cameraPosition, targetCoord - cameraPosition, defaultTagalongDistance * 1.5f);
                    for (int h = 0; h < allHits.Length; h++)
                    {
                        if (allHits[h].distance >= MinimumTagalongDistance &&
                            allHits[h].distance < closestHitDistance &&
                            !allHits[h].transform.IsChildOf(transform))
                        {
                            closestHit = allHits[h];
                            closestHitDistance = closestHit.distance;
                            if (DebugPointLight != null)
                            {
                                Light clonedLight = Instantiate(DebugPointLight, closestHit.point, Quaternion.identity) as Light;
                                clonedLight.color = Color.red;
                                Destroy(clonedLight, 1.0f);
                            }
#if UNITY_EDITOR
                            DebugDrawLine(DebugDrawLines, cameraPosition, targetCoord, Color.red);
#endif // UNITY_EDITOR
                        }
                    }
                }
            }

            // If we hit something, the closestHitDistance will be < infinity.
            needsUpdating = closestHitDistance < float.PositiveInfinity;
            if (needsUpdating)
            {
                // The closestHitDistance is a straight-line from the camera to the
                // point on the collider that was hit. Unless the closest hit was
                // encountered on the center Raycast, using the distance found will
                // actually push the tagalong too far away, and part of the object
                // that was hit will show through the Tagalong. We can fix that
                // with a little thing we like to call Trigonometry.
                Vector3 cameraToTransformPosition = transform.position - cameraPosition;
                Vector3 cameraToClosestHitPoint = closestHit.point - cameraPosition;
                float angleBetween = Vector3.Angle(cameraToTransformPosition, cameraToClosestHitPoint);
                closestHitDistance = closestHitDistance * Mathf.Cos(angleBetween * Mathf.Deg2Rad);

                // Make sure we aren't trying to move too close.
                closestHitDistance = Mathf.Max(closestHitDistance, MinimumTagalongDistance);
            }
            else if (TagalongDistance != defaultTagalongDistance)
            {
                // If we didn't hit anything but the TagalongDistance is different
                // from the defaultTagalongDistance, we still need to update.
                needsUpdating = true;
                closestHitDistance = defaultTagalongDistance;
            }

            newPosition = cameraPosition + (transform.position - cameraPosition).normalized * closestHitDistance;
            return needsUpdating;
        }

#if UNITY_EDITOR
        protected void DebugDrawLine(bool draw, Vector3 start, Vector3 end)
        {
            DebugDrawLine(draw, start, end, Color.white);
        }

        protected void DebugDrawLine(bool draw, Vector3 start, Vector3 end, Color color)
        {
            if (draw)
            {
                Debug.DrawLine(start, end, color);
            }
        }

        /// <summary>
        /// This function draws a box at the bounds provided.
        /// </summary>
        /// <param name="draw">If true, drawing happens.</param>
        /// <param name="colliderBounds">The bounds to draw the box.</param>
        void DebugDrawColliderBox(bool draw, Bounds colliderBounds)
        {
            Vector3 extents = colliderBounds.extents;

            Vector3 frontUpperLeft, backUpperLeft, backUpperRight, frontUpperRight;
            frontUpperLeft = colliderBounds.center + new Vector3(-extents.x, extents.y, -extents.z);
            backUpperLeft = colliderBounds.center + new Vector3(-extents.x, extents.y, extents.z);
            backUpperRight = colliderBounds.center + new Vector3(extents.x, extents.y, extents.z);
            frontUpperRight = colliderBounds.center + new Vector3(extents.x, extents.y, -extents.z);

            DebugDrawLine(draw, frontUpperLeft, backUpperLeft, Color.blue);
            DebugDrawLine(draw, backUpperLeft, backUpperRight, Color.red);
            DebugDrawLine(draw, backUpperRight, frontUpperRight, Color.blue);
            DebugDrawLine(draw, frontUpperRight, frontUpperLeft, Color.red);

            Vector3 frontLowerLeft, backLowerLeft, backLowerRight, frontLowerRight;
            frontLowerLeft = colliderBounds.center + new Vector3(-extents.x, -extents.y, -extents.z);
            backLowerLeft = colliderBounds.center + new Vector3(-extents.x, -extents.y, extents.z);
            backLowerRight = colliderBounds.center + new Vector3(extents.x, -extents.y, extents.z);
            frontLowerRight = colliderBounds.center + new Vector3(extents.x, -extents.y, -extents.z);

            DebugDrawLine(draw, frontLowerLeft, backLowerLeft, Color.blue);
            DebugDrawLine(draw, backLowerLeft, backLowerRight, Color.red);
            DebugDrawLine(draw, backLowerRight, frontLowerRight, Color.blue);
            DebugDrawLine(draw, frontLowerRight, frontLowerLeft, Color.red);

            DebugDrawLine(draw, frontUpperLeft, frontLowerLeft, Color.green);
            DebugDrawLine(draw, backUpperLeft, backLowerLeft, Color.green);
            DebugDrawLine(draw, backUpperRight, backLowerRight, Color.green);
            DebugDrawLine(draw, frontUpperRight, frontLowerRight, Color.green);
        }

        void DebugDrawDebuggingLines(bool draw, Vector3 center, Vector3 cameraPosition,
            Vector3 cameraToTarget,
            Vector3 centeredPoint, Vector3 pointOnFrustum, Vector3 recalculatedPointOnFrustum,
            Vector3 calculatedPosition)
        {
            DebugDrawLine(draw, cameraPosition, center, Color.blue);
            DebugDrawLine(draw, cameraPosition, cameraToTarget, Color.yellow);
            DebugDrawLine(draw, cameraPosition, centeredPoint, Color.red);
            DebugDrawLine(draw, centeredPoint, pointOnFrustum, Color.red);
            DebugDrawLine(draw, cameraPosition, recalculatedPointOnFrustum, Color.red);
            DebugDrawLine(draw, cameraPosition, calculatedPosition, Color.cyan);
        }
#endif // UNITY_EDITOR
    }
}