// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Add a NearInteractionTouchable to your scene and configure a touchable surface
    /// in order to get PointerDown and PointerUp events whenever a PokePointer touches this surface.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Services/NearInteractionTouchable")]
    public class NearInteractionTouchable : NearInteractionTouchableSurface
    {
        [SerializeField]
        [Tooltip("Local space forward direction")]
        protected Vector3 localForward = -Vector3.forward;

        /// <summary>
        /// Local space forward direction
        /// </summary>
        public Vector3 LocalForward { get => localForward; }

        [SerializeField]
        [Tooltip("Local space up direction")]
        protected Vector3 localUp = Vector3.up;

        /// <summary>
        /// Local space up direction
        /// </summary>
        public Vector3 LocalUp { get => localUp; }

        /// <summary>
        /// Returns true if the LocalForward and LocalUp vectors are orthogonal.
        /// </summary>
        /// <remarks>
        /// LocalRight is computed using the cross product and is always orthogonal to LocalForward and LocalUp.
        /// </remarks>
        public bool AreLocalVectorsOrthogonal => Vector3.Dot(localForward, localUp) == 0;

        [SerializeField]
        [Tooltip("Local space object center")]
        protected Vector3 localCenter = Vector3.zero;

        /// <summary>
        /// Local space object center
        /// </summary>
        public override Vector3 LocalCenter { get => localCenter; }

        /// <summary>
        /// Local space and gameObject right
        /// </summary>
        public Vector3 LocalRight
        {
            get
            {
                Vector3 cross = Vector3.Cross(localUp, localForward);
                if (cross == Vector3.zero)
                {
                    // vectors are collinear return default right
                    return Vector3.right;
                }
                else
                {
                    return cross;
                }
            }
        }

        /// <summary>
        /// Forward direction of the gameObject
        /// </summary>
        public Vector3 Forward => transform.TransformDirection(localForward);

        /// <summary>
        /// Forward direction of the NearInteractionTouchable plane, the press direction needs to face the 
        /// camera.
        /// </summary>
        public override Vector3 LocalPressDirection => -localForward;

        [SerializeField]
        [Tooltip("Bounds or size of the 2D NearInteractionTouchablePlane")]
        protected Vector2 bounds = Vector2.zero;

        /// <summary>
        /// Bounds or size of the 2D NearInteractionTouchablePlane
        /// </summary>
        public override Vector2 Bounds { get => bounds; }

        /// <summary>
        /// Check if the touchableCollider is enabled and in the gameObject hierarchy
        /// </summary>
        public bool ColliderEnabled { get { return touchableCollider.enabled && touchableCollider.gameObject.activeInHierarchy; } }


        [SerializeField]
        [FormerlySerializedAs("collider")]
        [Tooltip("BoxCollider used to calculate bounds and local center, if not set before runtime the gameObjects's BoxCollider will be used by default")]
        private Collider touchableCollider;

        /// <summary>
        /// BoxCollider used to calculate bounds and local center, if not set before runtime the gameObjects's BoxCollider will be used by default
        /// </summary>
        public Collider TouchableCollider => touchableCollider;

        protected override void OnValidate()
        {
            if (Application.isPlaying)
            {   // Don't validate during play mode
                return;
            }

            base.OnValidate();

            touchableCollider = GetComponent<Collider>();

            Debug.Assert(localForward.magnitude > 0);
            Debug.Assert(localUp.magnitude > 0);
            string hierarchy = gameObject.transform.EnumerateAncestors(true).Aggregate("", (result, next) => next.gameObject.name + "=>" + result);
            if (localUp.sqrMagnitude == 1 && localForward.sqrMagnitude == 1)
            {
                Debug.Assert(Vector3.Dot(localForward, localUp) == 0, $"localForward and localUp not perpendicular for object {hierarchy}. Did you set Local Forward correctly?");
            }

            localForward = localForward.normalized;
            localUp = localUp.normalized;

            bounds.x = Mathf.Max(bounds.x, 0);
            bounds.y = Mathf.Max(bounds.y, 0);
        }

        private void OnEnable()
        {
            if (touchableCollider == null)
            {
                SetTouchableCollider(GetComponent<BoxCollider>());
            }
        }

        /// <summary>
        /// Set local forward direction and ensure that local up is perpendicular to the new local forward and 
        /// local right direction.  The forward position should be facing the camera. The direction is indicated in scene view by a 
        /// white arrow in the center of the plane.
        /// </summary>
        public void SetLocalForward(Vector3 newLocalForward)
        {
            localForward = newLocalForward;
            localUp = Vector3.Cross(localForward, LocalRight).normalized;
        }

        /// <summary>
        /// Set new local up direction and ensure that local forward is perpendicular to the new local up and 
        /// local right direction.
        /// </summary>
        public void SetLocalUp(Vector3 newLocalUp)
        {
            localUp = newLocalUp;
            localForward = Vector3.Cross(LocalRight, localUp).normalized;
        }

        /// <summary>
        /// Set the position (center) of the NearInteractionTouchable plane relative to the gameObject.  
        /// The position of the plane should be in front of the gameObject.
        /// </summary>
        public void SetLocalCenter(Vector3 newLocalCenter)
        {
            localCenter = newLocalCenter;
        }

        /// <summary>
        /// Set the size (bounds) of the 2D NearInteractionTouchable plane.
        /// </summary>
        public void SetBounds(Vector2 newBounds)
        {
            bounds = newBounds;
        }

        /// <summary>
        /// Adjust the bounds, local center and local forward to match a given box collider.  This method
        /// also changes the size of the box collider attached to the gameObject.
        /// Default Behavior:  if touchableCollider is null at runtime, the object's box collider will be used
        /// to size and place the NearInteractionTouchable plane in front of the gameObject
        /// </summary>
        public void SetTouchableCollider(BoxCollider newCollider)
        {
            if (newCollider != null)
            {
                // Set touchableCollider for possible reference in the future
                touchableCollider = newCollider;

                SetLocalForward(-Vector3.forward);

                Vector2 adjustedSize = new Vector2(
                            Math.Abs(Vector3.Dot(newCollider.size, LocalRight)),
                            Math.Abs(Vector3.Dot(newCollider.size, LocalUp)));

                SetBounds(adjustedSize);

                // Set x and y center to match the newCollider but change the position of the
                // z axis so the plane is always in front of the object
                SetLocalCenter(newCollider.center + Vector3.Scale(newCollider.size / 2.0f, LocalForward));

                // Set size and center of the gameObject's box collider to match the collider given, if there 
                // is no box collider behind the NearInteractionTouchable plane, an event will not be raised
                BoxCollider attachedBoxCollider = GetComponent<BoxCollider>();
                attachedBoxCollider.size = newCollider.size;
                attachedBoxCollider.center = newCollider.center;
            }
            else
            {
                Debug.LogWarning("BoxCollider is null, cannot set bounds of NearInteractionTouchable plane");
            }
        }

        /// <inheritdoc />
        public override float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal)
        {
            normal = Forward;

            Vector3 localPoint = transform.InverseTransformPoint(samplePoint) - localCenter;

            // Get surface coordinates
            Vector3 planeSpacePoint = new Vector3(
                Vector3.Dot(localPoint, LocalRight),
                Vector3.Dot(localPoint, localUp),
                Vector3.Dot(localPoint, localForward));

            // touchables currently can only be touched within the bounds of the rectangle.
            // We return infinity to ensure that any point outside the bounds does not get touched.
            if (planeSpacePoint.x < -bounds.x / 2 ||
                planeSpacePoint.x > bounds.x / 2 ||
                planeSpacePoint.y < -bounds.y / 2 ||
                planeSpacePoint.y > bounds.y / 2)
            {
                return float.PositiveInfinity;
            }

            // Scale back to 3D space
            planeSpacePoint = transform.TransformSize(planeSpacePoint);

            return Math.Abs(planeSpacePoint.z);
        }
    }
}