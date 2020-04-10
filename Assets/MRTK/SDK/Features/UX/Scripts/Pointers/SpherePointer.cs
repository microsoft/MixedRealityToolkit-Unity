// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        [Tooltip("Additional distance on top of sphere cast radius when pointer is considered 'near' an object and far interaction will turn off")]
        private float nearObjectMargin = 0.2f;

        /// <summary>
        /// Additional distance on top of<see cref="BaseControllerPointer.SphereCastRadius"/> when pointer is considered 'near' an object and far interaction will turn off.
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
        /// Uses SphereCastRadius + NearObjectMargin to determine if near an object.
        /// </summary>
        /// <returns>True if the pointer is near any collider that's both on a grabbable layer mask, and has a NearInteractionGrabbable.</returns>
        public bool IsNearObject => queryBufferNearObjectRadius.ContainsGrabbable;

        /// <summary>
        /// Test if the pointer is within the grabbable radius of collider that's both on a grabbable layer mask, and has a NearInteractionGrabbable.
        /// Uses SphereCastRadius to determine if near an object.
        /// Note: if focus on pointer is locked, will always return true.
        /// </summary>
        /// <returns>True if the pointer is within the grabbable radius of collider that's both on a grabbable layer mask, and has a NearInteractionGrabbable.</returns>
        public override bool IsInteractionEnabled => IsFocusLocked || (base.IsInteractionEnabled && queryBufferInteractionRadius.ContainsGrabbable);

        private void Awake()
        {
            queryBufferNearObjectRadius = new SpherePointerQueryInfo(sceneQueryBufferSize, NearObjectRadius);
            queryBufferInteractionRadius = new SpherePointerQueryInfo(sceneQueryBufferSize, SphereCastRadius);
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
                if (TryGetNearGraspPoint(out pointerPosition))
                {
                    Vector3 endPoint = Vector3.forward * SphereCastRadius;
                    Rays[0].UpdateRayStep(ref pointerPosition, ref endPoint);
                    PrioritizedLayerMasksOverride = PrioritizedLayerMasksOverride ?? GrabLayerMasks;

                    for (int i = 0; i < PrioritizedLayerMasksOverride.Length; i++)
                    {
                        if (queryBufferNearObjectRadius.TryUpdateQueryBufferForLayerMask(PrioritizedLayerMasksOverride[i], pointerPosition, triggerInteraction, ignoreCollidersNotInFOV))
                        {
                            break;
                        }
                    }

                    for (int i = 0; i < PrioritizedLayerMasksOverride.Length; i++)
                    {
                        if (queryBufferInteractionRadius.TryUpdateQueryBufferForLayerMask(PrioritizedLayerMasksOverride[i], pointerPosition, triggerInteraction, ignoreCollidersNotInFOV))
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

        private static readonly ProfilerMarker TryGetDistanceToNearestSurfacePerfMarker = new ProfilerMarker("[MRTK] SpherePointer.TryGetDistanceToNearestSurface");

        /// <inheritdoc />
        public bool TryGetDistanceToNearestSurface(out float distance)
        {
            using (TryGetDistanceToNearestSurfacePerfMarker.Auto())
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

                normal = Vector3.forward;
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
            private readonly float queryRadius;

            /// <summary>
            /// The grabbable near the QueryRadius. 
            /// </summary>
            private NearInteractionGrabbable grabbable;

            public SpherePointerQueryInfo(int bufferSize, float radius)
            {
                numColliders = 0;
                queryBuffer = new Collider[bufferSize];
                queryRadius = radius;
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
            public bool TryUpdateQueryBufferForLayerMask(LayerMask layerMask, Vector3 pointerPosition, QueryTriggerInteraction triggerInteraction, bool ignoreCollidersNotInFOV)
            {
                using (TryUpdateQueryBufferForLayerMaskPerfMarker.Auto())
                {
                    grabbable = null;
                    numColliders = UnityEngine.Physics.OverlapSphereNonAlloc(
                        pointerPosition,
                        queryRadius,
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
                        grabbable = collider.GetComponent<NearInteractionGrabbable>();
                        if (grabbable != null)
                        {
                            if (ignoreCollidersNotInFOV)
                            {
                                if (!mainCam.IsInFOVCached(collider))
                                {
                                    // Additional check: is grabbable in the camera frustrum
                                    // We do this so that if grabbable is not visible it is not accidentally grabbed
                                    // Also to not turn off the hand ray if hand is near a grabbable that's not actually visible
                                    grabbable = null;
                                }
                            }
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
        }
    }
}
