// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// A Tagalong that stays at a fixed distance from the camera and always
    /// seeks to have a part of itself in the view frustum of the camera.
    /// </summary>
    [RequireComponent(typeof(BoxCollider), typeof(Interpolator))]
    public class SimpleTagalong : MonoBehaviour
    {
        // Simple Tagalongs seek to stay at a fixed distance from the Camera.
        [Tooltip("The distance in meters from the camera for the Tagalong to seek when updating its position.")]
        public float TagalongDistance = 2.0f;
        [Tooltip("If true, forces the Tagalong to be TagalongDistance from the camera, even if it didn't need to move otherwise.")]
        public bool EnforceDistance = true;

        [Tooltip("The speed at which to move the Tagalong when updating its position (meters/second).")]
        public float PositionUpdateSpeed = 9.8f;
        [Tooltip("When true, the Tagalong's motion is smoothed.")]
        public bool SmoothMotion = true;
        [Range(0.0f, 1.0f), Tooltip("The factor applied to the smoothing algorithm. 1.0f is super smooth. But slows things down a lot.")]
        public float SmoothingFactor = 0.75f;

        // The BoxCollider represents the volume of the object that is tagging
        // along. It is a required component.
        protected BoxCollider tagalongCollider;

        // The Interpolator is a helper class that handles various changes to an
        // object's transform. It is used by Tagalong to adjust the object's
        // transform.position.
        protected Interpolator interpolator;

        // This is an array of planes that define the camera's view frustum along
        // with some helpful indices into the array. The array is updated each
        // time through FixedUpdate().
        protected Plane[] frustumPlanes;
        protected const int frustumLeft = 0;
        protected const int frustumRight = 1;
        protected const int frustumBottom = 2;
        protected const int frustumTop = 3;

        virtual protected void Start()
        {
            // Make sure the Tagalong object has a BoxCollider.
            tagalongCollider = GetComponent<BoxCollider>();
            
            // Get the Interpolator component and set some default parameters for
            // it. These parameters can be adjusted in Unity's Inspector as well.
            interpolator = gameObject.GetComponent<Interpolator>();
            interpolator.SmoothLerpToTarget = SmoothMotion;
            interpolator.SmoothPositionLerpRatio = SmoothingFactor;
        }

        protected virtual void Update()
        {
            // Retrieve the frustum planes from the camera.
            frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

            // Determine if the Tagalong needs to move based on whether its
            // BoxCollider is in or out of the camera's view frustum.
            Vector3 tagalongTargetPosition;
            if (CalculateTagalongTargetPosition(transform.position, out tagalongTargetPosition))
            {
                // Derived classes will use the same Interpolator and may have
                // adjusted its PositionUpdateSpeed for some other purpose.
                // Restore the value we care about and tell the Interpolator
                // to move the Tagalong to its new target position.
                interpolator.PositionPerSecond = PositionUpdateSpeed;
                interpolator.SetTargetPosition(tagalongTargetPosition);
            }
            else if (!interpolator.Running && EnforceDistance)
            {
                // If the Tagalong is inside the camera's view frustum, and it is
                // supposed to stay a fixed distance from the camera, force the
                // tagalong to that location (without using the Interpolator).
                Ray ray = new Ray(Camera.main.transform.position, transform.position - Camera.main.transform.position);
                transform.position = ray.GetPoint(TagalongDistance);
            }
        }

        /// <summary>
        /// Determines if the Tagalong needs to move based on the provided
        /// position.
        /// </summary>
        /// <param name="fromPosition">Where the Tagalong is.</param>
        /// <param name="toPosition">Where the Tagalong needs to go.</param>
        /// <returns>True if the Tagalong needs to move to satisfy requirements; false otherwise.</returns>
        protected virtual bool CalculateTagalongTargetPosition(Vector3 fromPosition, out Vector3 toPosition)
        {
            // Check to see if any part of the Tagalong's BoxCollider's bounds is
            // inside the camera's view frustum. Note, the bounds used are an Axis
            // Aligned Bounding Box (AABB).
            bool needsToMove = !GeometryUtility.TestPlanesAABB(frustumPlanes, tagalongCollider.bounds);

            // If we already know we don't need to move, bail out early.
            if (!needsToMove)
            {
                toPosition = fromPosition;
                return false;
            }

            // Calculate a default position where the Tagalong should go. In this
            // case TagalongDistance from the camera along the gaze vector.
            toPosition = Camera.main.transform.position + Camera.main.transform.forward * TagalongDistance;

            // Create a Ray and set it's origin to be the default toPosition that
            // was calculated above.
            Ray ray = new Ray(toPosition, Vector3.zero);
            Plane plane = new Plane();
            float distanceOffset = 0f;

            // Determine if the Tagalong needs to move to the right or the left
            // to get back inside the camera's view frustum. The normals of the
            // planes that make up the camera's view frustum point inward.
            bool moveRight = frustumPlanes[frustumLeft].GetDistanceToPoint(fromPosition) < 0;
            bool moveLeft = frustumPlanes[frustumRight].GetDistanceToPoint(fromPosition) < 0;
            if (moveRight)
            {
                // If the Tagalong needs to move to the right, that means it is to
                // the left of the left frustum plane. Remember that plane and set
                // our Ray's direction to point towards that plane (remember the
                // Ray's origin is already inside the view frustum.
                plane = frustumPlanes[frustumLeft];
                ray.direction = -Camera.main.transform.right;
            }
            else if (moveLeft)
            {
                // Apply similar logic to above for the case where the Tagalong
                // needs to move to the left.
                plane = frustumPlanes[frustumRight];
                ray.direction = Camera.main.transform.right;
            }
            if (moveRight || moveLeft)
            {
                // If the Tagalong needed to move in the X direction, cast a Ray
                // from the default position to the plane we are working with.
                plane.Raycast(ray, out distanceOffset);

                // Get the point along that ray that is on the plane and update
                // the x component of the Tagalong's desired position.
                toPosition.x = ray.GetPoint(distanceOffset).x;
            }

            // Similar logic follows below for for determining if and how the
            // Tagalong would need to move up or down.
            bool moveDown = frustumPlanes[frustumTop].GetDistanceToPoint(fromPosition) < 0;
            bool moveUp = frustumPlanes[frustumBottom].GetDistanceToPoint(fromPosition) < 0;
            if (moveDown)
            {
                plane = frustumPlanes[frustumTop];
                ray.direction = Camera.main.transform.up;
            }
            else if (moveUp)
            {
                plane = frustumPlanes[frustumBottom];
                ray.direction = -Camera.main.transform.up;
            }
            if (moveUp || moveDown)
            {
                plane.Raycast(ray, out distanceOffset);
                toPosition.y = ray.GetPoint(distanceOffset).y;
            }

            // Create a ray that starts at the camera and points in the direction
            // of the calculated toPosition.
            ray = new Ray(Camera.main.transform.position, toPosition - Camera.main.transform.position);

            // Find the point along that ray that is the right distance away and
            // update the calculated toPosition to be that point.
            toPosition = ray.GetPoint(TagalongDistance);

            // If we got here, needsToMove will be true.
            return needsToMove;
        }
    }
}