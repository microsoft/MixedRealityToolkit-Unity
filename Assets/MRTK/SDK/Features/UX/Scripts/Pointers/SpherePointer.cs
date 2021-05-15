// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [AddComponentMenu("Scripts/MRTK/SDK/SpherePointer")]
    public class SpherePointer : BaseControllerPointer, IMixedRealityNearPointer
    {
        private SceneQueryType raycastMode = SceneQueryType.SphereOverlap;

        /// <inheritdoc />
        public override SceneQueryType SceneQueryType
        {
            get => raycastMode;
            set => raycastMode = value;
        }

        [SerializeField]
        [Min(0.0f)]
        [Tooltip("Amount to pull back the center of the sphere behind the hand for detecting when to turn off far interaction.")]
        private float pullbackDistance = 0.0f;

        /// <summary>
        /// Amount to pull back the center of the sphere behind the hand for detecting when to turn off far interaction.
        /// </summary>
        public float PullbackDistance
        {
            get => pullbackDistance;
            set
            {
                pullbackDistance = value;
                queryBufferNearObjectRadius.queryMinDistance = pullbackDistance;
            }
        }


        [SerializeField]
        [Min(0.0f)]
        [Tooltip("Angle range of the forward axis to query in degrees. Angle >= 360 means the entire sphere is queried")]
        private float nearObjectSectorAngle = 360.0f;

        /// <summary>
        /// Angle range of the forward axis to query in degrees. Angle >= 360 means the entire sphere is queried.
        /// </summary>
        public float NearObjectSectorAngle
        {
            get => nearObjectSectorAngle;
            set
            {
                nearObjectSectorAngle = value;
                queryBufferNearObjectRadius.queryAngle = NearObjectSectorAngle * 0.5f;
            }
        }

        [SerializeField]
        [Min(0.0f)]
        [Tooltip("Additional distance on top of sphere cast radius when pointer is considered 'near' an object and far interaction will turn off")]
        private float nearObjectMargin = 0.2f;

        /// <summary>
        /// Additional distance on top of<see cref="BaseControllerPointer.SphereCastRadius"/> when pointer is considered 'near' an object and far interaction will turn off.
        /// </summary>
        /// <remarks>
        /// This creates a dead zone in which far interaction is disabled before objects become grabbable.
        /// </remarks>
        public float NearObjectMargin
        {
            get => nearObjectMargin;
            set
            {
                nearObjectMargin = value;
                queryBufferNearObjectRadius.queryRadius = NearObjectRadius;
            }
        }

        [SerializeField]
        [Min(0.0f)]
        [Tooltip("Lerp factor between the palm direction and the index finger direction used to build the cone direction.")]
        private float nearObjectAxisLerp = 0.9f;
        /// <summary>
        /// Lerp factor between the palm direction and the index finger direction used to build the cone direction.
        /// </summary>
        public float NearObjectAxisLerp
        {
            get => nearObjectAxisLerp;
            set => nearObjectAxisLerp = value;
        }

        [SerializeField]
        [Min(0.0f)]
        [Tooltip("Smoothing factor for query detection. If an object is detected in the NearObjectRadius, the queried radius then becomes NearObjectRadius * (1 + nearObjectSmoothingFactor) to reduce the sensitivity")]
        private float nearObjectSmoothingFactor = 0.4f;
        /// <summary>
        /// Smoothing factor for query detection. If an object is detected in the NearObjectRadius, the queried radius then becomes NearObjectRadius * (1 + nearObjectSmoothingFactor) to reduce the sensitivity.
        /// </summary>
        public float NearObjectSmoothingFactor
        {
            get => nearObjectSmoothingFactor;
            set => nearObjectSmoothingFactor = value;
        }
        /// <summary>
        /// Distance at which the pointer is considered "near" an object.
        /// </summary>
        /// <remarks>
        /// Sum of <see cref="BaseControllerPointer.SphereCastRadius"/> and <see cref="NearObjectMargin"/>. Entering the <see cref="NearObjectRadius"/> disables far interaction.
        /// </remarks>
        public float NearObjectRadius => SphereCastRadius + NearObjectMargin;

        [SerializeField]
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the grabbable objects. Remember to also add NearInteractionGrabbable! Only collidables with NearInteractionGrabbable will raise events.")]
        private LayerMask[] grabLayerMasks = { UnityEngine.Physics.DefaultRaycastLayers };

        /// <summary>
        /// The LayerMasks, in prioritized order, that are used to determine the touchable objects.
        /// </summary>
        /// <remarks>
        /// Only [NearInteractionGrabbables](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable) in one of the LayerMasks will raise events.
        /// </remarks>
        public LayerMask[] GrabLayerMasks => grabLayerMasks;

        [SerializeField]
        [Tooltip("Specify whether queries for grabbable objects hit triggers.")]
        protected QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal;

        /// <summary>
        /// Specify whether queries for grabbable objects hit triggers.
        /// </summary>
        public QueryTriggerInteraction TriggerInteraction => triggerInteraction;

        [SerializeField]
        [Tooltip("Maximum number of colliders that can be detected in a scene query.")]
        [Min(1)]
        private int sceneQueryBufferSize = 64;

        /// <summary>
        /// Maximum number of colliders that can be detected in a scene query.
        /// </summary>
        public int SceneQueryBufferSize => sceneQueryBufferSize;

        [SerializeField]
        [Tooltip("Whether to ignore colliders that may be near the pointer, but not actually in the visual FOV. This can prevent accidental grabs, and will allow hand rays to turn on when you may be near a grabbable but cannot see it. Visual FOV is defined by cone centered about display center, radius equal to half display height.")]
        private bool ignoreCollidersNotInFOV = true;

        /// <summary>
        /// Whether to ignore colliders that may be near the pointer, but not actually in the visual FOV.
        /// This can prevent accidental grabs, and will allow hand rays to turn on when you may be near 
        /// a grabbable but cannot see it. Visual FOV is defined by cone centered about display center, 
        /// radius equal to half display height.
        /// </summary>
        public bool IgnoreCollidersNotInFOV
        {
            get => ignoreCollidersNotInFOV;
            set => ignoreCollidersNotInFOV = value;
        }

        private SpherePointerQueryInfo queryBufferNearObjectRadius;
        private SpherePointerQueryInfo queryBufferInteractionRadius;

        /// <summary>
        /// Test if the pointer is near any collider that's both on a grabbable layer mask, and has a NearInteractionGrabbable.
        /// Uses SphereCastRadius + NearObjectMargin to determine if near an object within the sector angle
        /// Also returns true of any grabbable objects are within SphereCastRadius even if they aren't within the sector angle
        /// Ignores bounds handlers for the IsNearObject check.
        /// </summary>
        /// <returns>True if the pointer is near any collider that's both on a grabbable layer mask, and has a NearInteractionGrabbable.</returns>
        public virtual bool IsNearObject => queryBufferNearObjectRadius.ContainsGrabbable || queryBufferInteractionRadius.NearObjectDetected;

        /// <summary>
        /// Test if the pointer is within the grabbable radius of collider that's both on a grabbable layer mask, and has a NearInteractionGrabbable.
        /// Uses SphereCastRadius to determine if near an object.
        /// Note: if focus on pointer is locked, will always return true.
        /// </summary>
        /// <returns>True if the pointer is within the grabbable radius of collider that's both on a grabbable layer mask, and has a NearInteractionGrabbable.</returns>
        public override bool IsInteractionEnabled => IsFocusLocked || (base.IsInteractionEnabled && queryBufferInteractionRadius.ContainsGrabbable);

        private void Awake()
        {
            queryBufferNearObjectRadius = new SpherePointerQueryInfo(sceneQueryBufferSize, Mathf.Max(NearObjectRadius, SphereCastRadius), NearObjectSectorAngle, PullbackDistance, nearObjectSmoothingFactor, true);
            queryBufferInteractionRadius = new SpherePointerQueryInfo(sceneQueryBufferSize, SphereCastRadius, 360.0f, 0.0f, 0.0f);
        }

        private static readonly ProfilerMarker OnPreSceneQueryPerfMarker = new ProfilerMarker("[MRTK] SpherePointer.OnPreSceneQuery");

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            using (OnPreSceneQueryPerfMarker.Auto())
            {
                if (Rays == null)
                {
                    Rays = new RayStep[1];
                }

                Vector3 pointerPosition;
                Vector3 pointerAxis;
                if (TryGetNearGraspPoint(out pointerPosition) && TryGetNearGraspAxis(out pointerAxis))
                {
                    Vector3 endPoint = Vector3.forward * SphereCastRadius;
                    Rays[0].UpdateRayStep(ref pointerPosition, ref endPoint);
                    PrioritizedLayerMasksOverride = PrioritizedLayerMasksOverride ?? GrabLayerMasks;

                    for (int i = 0; i < PrioritizedLayerMasksOverride.Length; i++)
                    {
                        if (queryBufferNearObjectRadius.TryUpdateQueryBufferForLayerMask(PrioritizedLayerMasksOverride[i], pointerPosition - pointerAxis * PullbackDistance, pointerAxis, triggerInteraction, ignoreCollidersNotInFOV))
                        {
                            break;
                        }
                    }

                    for (int i = 0; i < PrioritizedLayerMasksOverride.Length; i++)
                    {
                        if (queryBufferInteractionRadius.TryUpdateQueryBufferForLayerMask(PrioritizedLayerMasksOverride[i], pointerPosition, pointerAxis, triggerInteraction, ignoreCollidersNotInFOV))
                        {
                            break;
                        }
                    }
                }
            }
        }

        private static readonly ProfilerMarker TryGetNearGraspPointPerfMarker = new ProfilerMarker("[MRTK] SpherePointer.TryGetNearGraspPoint");

        /// <summary>
        /// Gets the position of where grasp happens
        /// For IMixedRealityHand it's the average of index and thumb.
        /// For any other IMixedRealityController, return just the pointer origin
        /// </summary>
        public bool TryGetNearGraspPoint(out Vector3 result)
        {
            using (TryGetNearGraspPointPerfMarker.Auto())
            {
                if (Controller != null)
                {
                    // If controller is of kind IMixedRealityHand, return average of index and thumb
                    if (Controller is IMixedRealityHand hand)
                    {
                        if (hand.TryGetJoint(TrackedHandJoint.IndexTip, out MixedRealityPose index) && index != null)
                        {
                            if (hand.TryGetJoint(TrackedHandJoint.ThumbTip, out MixedRealityPose thumb) && thumb != null)
                            {
                                result = 0.5f * (index.Position + thumb.Position);
                                return true;
                            }
                        }
                    }

                    // If controller isn't an IMixedRealityHand or one of the required joints isn't available, check for position
                    if (Controller.IsPositionAvailable)
                    {
                        result = Position;
                        return true;
                    }
                }

                result = Vector3.zero;
                return false;
            }
        }


        private static readonly ProfilerMarker TryGetNearGraspAxisPerfMarker = new ProfilerMarker("[MRTK] ConePointer.TryGetNearGraspAxis");

        /// <summary>
        /// Because pointers shouldn't be able to interact with objects that are "behind" it, it is necessary to determine the forward axis of the pointer when making interaction checks.
        /// 
        /// For example, a grab pointer's axis should is the result of Vector3.Lerp(palm forward axis, palm to index finger axis).
        ///
        /// This method provides a mechanism to get this normalized forward axis.
        /// </summary>
        /// <param name="axis">Out parameter filled with the grasp's forward axis if available, otherwise returns the forward axis of the transform.</param>
        /// <returns>True if a grasp's forward axis was retrieved, false if not.</returns>
        private bool TryGetNearGraspAxis(out Vector3 axis)
        {
            using (TryGetNearGraspAxisPerfMarker.Auto())
            {
                if (Controller is IMixedRealityHand hand)
                {
                    if (hand.TryGetJoint(TrackedHandJoint.IndexTip, out MixedRealityPose index) && index != null)
                    {
                        if (hand.TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose palm) && palm != null)
                        {
                            Vector3 palmToIndex = index.Position - palm.Position;
                            axis = Vector3.Lerp(palm.Forward, palmToIndex.normalized, NearObjectAxisLerp).normalized;
                            return true;
                        }
                    }
                }

                axis = transform.forward;
                return true;
            }
        }

        private static readonly ProfilerMarker TryGetDistanceToNearestSurfacePerfMarker = new ProfilerMarker("[MRTK] SpherePointer.TryGetDistanceToNearestSurface");

        /// <inheritdoc />
        public bool TryGetDistanceToNearestSurface(out float distance)
        {
            using (TryGetNormalToNearestSurfacePerfMarker.Auto())
            {
                var focusProvider = CoreServices.InputSystem?.FocusProvider;
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
        }

        private static readonly ProfilerMarker TryGetNormalToNearestSurfacePerfMarker = new ProfilerMarker("[MRTK] SpherePointer.TryGetNormalToNearestSurface");

        /// <inheritdoc />
        public bool TryGetNormalToNearestSurface(out Vector3 normal)
        {
            using (TryGetNormalToNearestSurfacePerfMarker.Auto())
            {
                var focusProvider = CoreServices.InputSystem?.FocusProvider;
                if (focusProvider != null)
                {
                    FocusDetails focusDetails;
                    if (focusProvider.TryGetFocusDetails(this, out focusDetails))
                    {
                        normal = focusDetails.Normal;
                        return true;
                    }
                }
                normal = Rotation * Vector3.forward;
                return false;
            }
        }

        /// <summary>
        /// Helper class for storing and managing near grabbables close to a point
        /// </summary>
        private class SpherePointerQueryInfo
        {
            /// <summary>
            /// How many colliders are near the point from the latest call to TryUpdateQueryBufferForLayerMask 
            /// </summary>
            private int numColliders;

            /// <summary>
            /// Fixed-length array used to story physics queries
            /// </summary>
            private readonly Collider[] queryBuffer;

            /// <summary>
            /// Distance for performing queries.
            /// </summary>
            public float queryRadius;

            /// <summary>
            /// Minimum required distance from the query center.
            /// </summary>
            public float queryMinDistance;

            /// <summary>
            /// Angle in degrees that a point is allowed to be off from the query axis. Angle >= 180 means points can be anywhere in relation to the query axis
            /// </summary>
            public float queryAngle;

            /// <summary>
            /// Smoothing factor for query detection. If an object is detected in the query, the queried radius then becomes queryRadius * (1 + querySmoothingFactor) to reduce the sensitivity
            /// </summary>
            public float querySmoothingFactor;

            /// <summary>
            /// Variable that controls ignoring handles for this interaction
            /// </summary>
            public bool ignoreBoundsHandlesForQuery = false;

            /// <summary>
            /// The grabbable near the QueryRadius. 
            /// </summary>
            private NearInteractionGrabbable grabbable;

            /// <summary>
            /// Constructor for a sphere overlap query
            /// </summary>
            /// <param name="bufferSize">Size to make the physics query buffer array</param>
            /// <param name="radius">Radius of the sphere </param>
            /// <param name="angle">Angle range of the forward axis to query in degrees. Angle > 360 means the entire sphere is queried</param>
            /// <param name="minDistance">"Minimum required distance to be registered in the query"</param>
            /// <param name="ignoreBoundsHandles">"Whether or not this sphere cast ignores detecting bounds handles"</param>
            public SpherePointerQueryInfo(int bufferSize, float radius, float angle, float minDistance, float smoothingFactor, bool ignoreBoundsHandles = false)
            {
                numColliders = 0;
                queryBuffer = new Collider[bufferSize];
                queryRadius = radius;
                queryMinDistance = minDistance;
                queryAngle = angle * 0.5f;
                querySmoothingFactor = smoothingFactor;
                ignoreBoundsHandlesForQuery = ignoreBoundsHandles;
            }

            private static readonly ProfilerMarker TryUpdateQueryBufferForLayerMaskPerfMarker = new ProfilerMarker("[MRTK] SpherePointerQueryInfo.TryUpdateQueryBufferForLayerMask");

            /// <summary>
            /// Intended to be called once per frame, this method performs a sphere intersection test against
            /// all colliders in the layers defined by layerMask at the given pointer position.
            /// All colliders intersecting the sphere at queryRadius and pointerPosition are stored in queryBuffer,
            /// and the first grabbable in the list of returned colliders is stored.
            /// Also provides an option to ignore colliders that are not visible.
            /// </summary>
            /// <param name="layerMask">Filter to only perform sphere cast on these layers.</param>
            /// <param name="pointerPosition">The position of the pointer to query against.</param>
            /// <param name="triggerInteraction">Passed along to the OverlapSphereNonAlloc call.</param>
            /// <param name="ignoreCollidersNotInFOV">Whether to ignore colliders that are not visible.</param>
            public bool TryUpdateQueryBufferForLayerMask(LayerMask layerMask, Vector3 pointerPosition, Vector3 pointerAxis, QueryTriggerInteraction triggerInteraction, bool ignoreCollidersNotInFOV)
            {
                using (TryUpdateQueryBufferForLayerMaskPerfMarker.Auto())
                {
                    float radius;
                    if (ContainsGrabbable)
                    {
                        radius = queryRadius * (1 + querySmoothingFactor);
                    }
                    else
                    {
                        radius = queryRadius;
                    }

                    grabbable = null;
                    numColliders = UnityEngine.Physics.OverlapSphereNonAlloc(
                        pointerPosition,
                        radius,
                        queryBuffer,
                        layerMask,
                        triggerInteraction);

                    if (numColliders == queryBuffer.Length)
                    {
                        Debug.LogWarning($"Maximum number of {numColliders} colliders found in SpherePointer overlap query. Consider increasing the query buffer size in the pointer profile.");
                    }

                    Camera mainCam = CameraCache.Main;
                    for (int i = 0; i < numColliders; i++)
                    {
                        Collider collider = queryBuffer[i];
                        MeshCollider meshCollider = collider as MeshCollider;
                        if (meshCollider != null && meshCollider.convex == false)
                        {
                            // Physics.ClosestPoint is only allowed on a convex collider.
                            continue;
                        }
                        
                        grabbable = collider.GetComponent<NearInteractionGrabbable>();
                        if (grabbable != null)
                        {
                            if (ignoreCollidersNotInFOV)
                            {
                                if (!mainCam.IsInFOVCached(collider))
                                {
                                    // Additional check: is grabbable in the camera frustum
                                    // We do this so that if grabbable is not visible it is not accidentally grabbed
                                    // Also to not turn off the hand ray if hand is near a grabbable that's not actually visible
                                    grabbable = null;
                                }
                            }
                        }

                        Vector3 closestPointToCollider = collider.ClosestPoint(pointerPosition);
                        Vector3 relativeColliderPosition = closestPointToCollider - pointerPosition;

                        // Check if the collider is within the activation cone
                        float queryAngleRadians = queryAngle * Mathf.Deg2Rad;
                        bool inAngle = Vector3.Dot(pointerAxis, relativeColliderPosition.normalized) >= Mathf.Cos(queryAngleRadians) || queryAngle >= 180.0f;

                        // Check to ensure the object is beyond the minimum distance
                        bool pastMinDistance = relativeColliderPosition.sqrMagnitude >= queryMinDistance * queryMinDistance;

                        bool isBoundHandles = ignoreBoundsHandlesForQuery && grabbable != null && grabbable.IsBoundsHandles;

                        if (!pastMinDistance || !inAngle || isBoundHandles)
                        {
                            grabbable = null;
                            continue;
                        }

                        if (grabbable != null)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            /// <summary>
            /// Returns true if any of the objects inside QueryBuffer contain a grabbable
            /// </summary>
            public bool ContainsGrabbable => grabbable != null;

            /// <summary>
            /// Returns true if any of the objects inside QueryBuffer contain a grabbable that is not a bounds handle
            /// </summary>
            public bool NearObjectDetected => ContainsGrabbable && !grabbable.IsBoundsHandles;
        }

#if UNITY_EDITOR
        /// <summary>
        /// When in editor, draws an approximation of what is the "Near Object" area
        /// </summary>
        private void OnDrawGizmos()
        {
            bool NearObjectCheck = queryBufferNearObjectRadius != null && IsNearObject;
            bool IsInteractionEnabledCheck = queryBufferInteractionRadius != null && IsInteractionEnabled;

            TryGetNearGraspAxis(out Vector3 sectorForwardAxis);
            TryGetNearGraspPoint(out Vector3 point);
            Vector3 centralAxis = sectorForwardAxis.normalized;

            float gizmoNearObjectRadius;
            if (NearObjectCheck)
                gizmoNearObjectRadius = NearObjectRadius * (1 + nearObjectSmoothingFactor);
            else
                gizmoNearObjectRadius = NearObjectRadius;


            if (NearObjectSectorAngle >= 360.0f)
            {
                // Draw the sphere and the inner near interaction deadzone (governed by the pullback distance)
                Gizmos.color = (NearObjectCheck ? Color.red : Color.cyan) - Color.black * 0.8f;
                Gizmos.DrawSphere(point - centralAxis * PullbackDistance, gizmoNearObjectRadius);

                Gizmos.color = Color.blue - Color.black * 0.8f;
                Gizmos.DrawSphere(point - centralAxis * PullbackDistance, PullbackDistance);
            }
            else
            {
                // Draw something approximating the sphere's sector
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(point, point + centralAxis * (gizmoNearObjectRadius - PullbackDistance));

                UnityEditor.Handles.color = NearObjectCheck ? Color.red : Color.cyan;
                float GizmoAngle = NearObjectSectorAngle * 0.5f * Mathf.Deg2Rad;
                UnityEditor.Handles.DrawWireDisc(point,
                                                 centralAxis,
                                                 PullbackDistance * Mathf.Sin(GizmoAngle));

                UnityEditor.Handles.DrawWireDisc(point + sectorForwardAxis.normalized * (gizmoNearObjectRadius * Mathf.Cos(GizmoAngle) - PullbackDistance),
                                                 centralAxis,
                                                 gizmoNearObjectRadius * Mathf.Sin(GizmoAngle));
            }

            // Draw the sphere representing the grabbable area
            Gizmos.color = Color.green - Color.black * (IsInteractionEnabledCheck ? 0.3f : 0.8f);
            Gizmos.DrawSphere(point, SphereCastRadius);
        }
#endif
    }
}
