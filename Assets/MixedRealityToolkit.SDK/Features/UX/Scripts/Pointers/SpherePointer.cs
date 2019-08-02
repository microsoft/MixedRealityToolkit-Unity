// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class SpherePointer : BaseControllerPointer, IMixedRealityNearPointer
    {
        private SceneQueryType raycastMode = SceneQueryType.SphereOverlap;

        public override SceneQueryType SceneQueryType { get { return raycastMode; } set { raycastMode = value; } }

        [SerializeField]
        [Min(0.0f)]
        [Tooltip("Additional distance between SphereCastRadius and NearObjectRadius")]
        private float nearObjectMargin = 0.2f;
        /// <summary>
        /// Additional distance between <see cref="BaseControllerPointer.SphereCastRadius"/> and <see cref="NearObjectRadius"/>.
        /// </summary>
        /// <remarks>
        /// This creates a dead zone in which far interaction is disabled before objects become grabbable.
        /// </remarks>
        public float NearObjectMargin => nearObjectMargin;

        /// <summary>
        /// Distance at which the pointer is considered "near" an object.
        /// </summary>
        /// <remarks>
        /// Sum of <see cref="BaseControllerPointer.SphereCastRadius"/> and <see cref="NearObjectMargin"/>. Entering the <see cref="NearObjectRadius"/> disables far interaction.
        /// </remarks>
        public float NearObjectRadius => SphereCastRadius + NearObjectMargin;

        [SerializeField]
        private bool debugMode = false;

        private Transform debugSphere;

        /// <summary>
        /// Currently performs a sphere check.
        /// Currently anything that has a collider is considered "Grabbable".
        /// Eventually we need to filter based on things that can respond
        /// to grab events.
        /// </summary>
        /// <returns>True if the hand is near anything that's grabbable.</returns>
        public bool IsNearObject
        {
            get
            {
                if (TryGetNearGraspPoint(out Vector3 position))
                {
                    return UnityEngine.Physics.CheckSphere(position, NearObjectRadius, ~UnityEngine.Physics.IgnoreRaycastLayer);
                }

                return false;
            }
        }

        public override bool IsInteractionEnabled
        {
            get
            {
                if (IsFocusLocked)
                {
                    return true;
                }
                else if (base.IsInteractionEnabled && TryGetNearGraspPoint(out Vector3 position))
                {
                    return UnityEngine.Physics.CheckSphere(position, SphereCastRadius, ~UnityEngine.Physics.IgnoreRaycastLayer);
                }

                return false;
            }
        }

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            if (Rays == null)
            {
                Rays = new RayStep[1];
            }

            Vector3 pointerPosition;
            if (TryGetNearGraspPoint(out pointerPosition))
            {
                if (debugMode)
                {
                    if (debugSphere == null)
                    {
                        debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                        debugSphere.localScale = Vector3.one * SphereCastRadius * 2;
                        Destroy(debugSphere.gameObject.GetComponent<Collider>());
                    }

                    debugSphere.position = pointerPosition;
                }

                Vector3 endPoint = Vector3.forward * SphereCastRadius;
                Rays[0].UpdateRayStep(ref pointerPosition, ref endPoint);
            }
        }

        /// <summary>
        /// Gets the position of where grasp happens
        /// For sixdof it's just the pointer origin
        /// for hand it's the average of index and thumb.
        /// </summary>
        public bool TryGetNearGraspPoint(out Vector3 result)
        {
            // For now, assume that anything that is a sphere pointer is a hand
            // TODO: have a way to determine if this is a fully articulated hand and return 
            // ray origin if it's a sixdof
            if (Controller != null && Controller is IMixedRealityHand)
            {
                HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Controller.ControllerHandedness, out MixedRealityPose index);
                HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Controller.ControllerHandedness, out MixedRealityPose thumb);
                if (index != null && thumb != null)
                {
                    result = 0.5f * (index.Position + thumb.Position);
                    return true;
                }
            }
            else
            {
                result = Position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetDistanceToNearestSurface(out float distance)
        {
            var focusProvider = InputSystem?.FocusProvider;
            if (focusProvider != null)
            {
                FocusDetails focusDetails;
                if (focusProvider.TryGetFocusDetails(this, out focusDetails))
                {
                    distance = focusDetails.RayDistance;
                    return true;
                }
            }

            distance = 0.0f;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetNormalToNearestSurface(out Vector3 normal)
        {
            var focusProvider = InputSystem?.FocusProvider;
            if (focusProvider != null)
            {
                FocusDetails focusDetails;
                if (focusProvider.TryGetFocusDetails(this, out focusDetails))
                {
                    normal = focusDetails.Normal;
                    return true;
                }
            }

            normal = Vector3.forward;
            return false;
        }

        private void OnDestroy()
        {
            if (debugSphere)
            {
                Destroy(debugSphere.gameObject);
            }
        }
    }
}