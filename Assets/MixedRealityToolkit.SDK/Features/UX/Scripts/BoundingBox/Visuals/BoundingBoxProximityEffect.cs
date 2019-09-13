using System;
using UnityEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using System.Runtime.CompilerServices;


namespace Microsoft.MixedReality.Toolkit.UI 
{
   // using ProximityStates = Dictionary<GameObject, BoundingBoxProximityEffect.ProximityHandleInfo>;

    [Serializable]
    public class BoundingBoxProximityEffect 
    {
        [SerializeField]
        [Tooltip("Determines whether proximity feature (scaling and material toggling) for bounding box handles is activated")]
        private bool proximityEffectActive = false;

        /// <summary>
        /// Determines whether proximity feature (scaling and material toggling) for bounding box handles is activated
        /// </summary>
        public bool ProximityEffectActive => proximityEffectActive;


        [SerializeField]
        [Tooltip("How far away should the hand be from a handle before it starts scaling the handle?")]
        [Range(0.005f, 0.2f)]
        private float handleMediumProximity = 0.1f;

        [SerializeField]
        [Tooltip("How far away should the hand be from a handle before it activates the close-proximity scaling effect?")]
        [Range(0.001f, 0.1f)]
        private float handleCloseProximity = 0.03f;

        [SerializeField]
        [Tooltip("A Proximity-enabled Handle scales by this amount when a hand moves out of range. Default is 0, invisible handle.")]
        private float farScale = 1.0f;

        /// <summary>
        /// A Proximity-enabled Handle scales by this amount when a hand moves out of range. Default is 0, invisible handle.
        /// </summary>
        public float FarScale => farScale;

        [SerializeField]
        [Tooltip("A Proximity-enabled Handle scales by this amount when a hand moves into the Medium Proximity range. Default is 1.0, original handle size.")]
        private float mediumScale = 1.2f;

        /// <summary>
        /// A Proximity-enabled Handle scales by this amount when a hand moves into the Medium Proximity range. Default is 1.0, original handle size.
        /// </summary>
        public float MediumScale => mediumScale;
    
        [SerializeField]
        [Tooltip("A Proximity-enabled Handle scales by this amount when a hand moves into the Close Proximity range. Default is 1.5, larger handle size.")]
        private float closeScale = 1.5f;

        /// <summary>
        /// A Proximity-enabled Handle scales by this amount when a hand moves into the Close Proximity range. Default is 1.5, larger handle size
        /// </summary>
        public float CloseScale => closeScale;
      
        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled Handle scale when the Hand moves from Medium proximity to Far proximity?")]
        [Range(0.0f, 1.0f)]
        private float farGrowRate = 0.3f;

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled Handle scale when the Hand moves to a distance that activates Medium Scale ?")]
        [Range(0.0f, 1.0f)]
        private float mediumGrowRate = 0.2f;

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled Handle scale when the Hand moves to a distance that activates Close Scale ?")]
        [Range(0.0f, 1.0f)]
        private float closeGrowRate = 0.3f;

        /// <summary>
        /// Internal state tracking for proximity of a handle
        /// </summary>
        internal enum HandleProximityState
        {
            FullsizeNoProximity = 0,
            MediumProximity,
            CloseProximity
        }

        /// <summary>
        /// Container for handle references and states (including scale and rotation type handles)
        /// </summary>
        private class HandleProximityInfo
        {
            public Transform Handle;
            public Renderer HandleVisualRenderer;
            public HandleProximityState ProximityState = HandleProximityState.FullsizeNoProximity;
        }



        /// Container for registered bounding box handles and their proximity states
        /// 
        private class RegisteredHandles
        {
            public BoundingBoxHandlesBase handleCollection;
            public List<HandleProximityInfo> proximityInfos;
        }

        //internal class ProximityHandleInfo
        //{
        //    public HandleProximityState proximityState;
        //    public Renderer HandleVisualRenderer;
        //}


        private List<RegisteredHandles> registeredHandles = new List<RegisteredHandles>();


        private HashSet<IMixedRealityPointer> proximityPointers = new HashSet<IMixedRealityPointer>();
        private List<Vector3> proximityPoints = new List<Vector3>();

        public void ClearHandles()
        {
            if (registeredHandles != null)
            {
                registeredHandles.Clear();
            }
        }

        public void ResetHandleProximityScale()
        {
            if (proximityEffectActive == false)
            {
                return;
            }

            foreach (var baseHandles in registeredHandles)
            {
                foreach (var item in baseHandles.proximityInfos)
                {
                    if (item.ProximityState != HandleProximityState.FullsizeNoProximity)
                    {
                        item.ProximityState = HandleProximityState.FullsizeNoProximity;
                       
                        if (item.HandleVisualRenderer)
                        {
                            item.HandleVisualRenderer.material = baseHandles.handleCollection.HandleMaterial;
                        }

                        ScaleHandle(item.ProximityState, item.Handle, baseHandles.handleCollection.HandleSize);
                    }
                }
            }
        }

        private bool IsAnyRegisteredHandleVisible()
        {
            foreach (var baseHandles in registeredHandles)
            {
                if (baseHandles.handleCollection.IsHandleTypeActive())
                {
                    return true;
                }
            }

            return false;
        }

        public void HandleProximityScaling(Vector3 boundingBoxPosition, Vector3 currentBoundsExtents)
        {
            // early out if effect is disabled
            if (proximityEffectActive == false || !IsAnyRegisteredHandleVisible())
            {
                return;
            }

            proximityPointers.Clear();
            proximityPoints.Clear();

            // Find all valid pointers
            foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
            {
                foreach (var pointer in inputSource.Pointers)
                {
                    if (pointer.IsInteractionEnabled && !proximityPointers.Contains(pointer))
                    {
                        proximityPointers.Add(pointer);
                    }
                }
            }

            // Get the max radius possible of our current bounds plus the proximity
            float maxRadius = Mathf.Max(Mathf.Max(currentBoundsExtents.x, currentBoundsExtents.y), currentBoundsExtents.z);
            maxRadius *= maxRadius;
            maxRadius += handleCloseProximity + handleMediumProximity;

            // Grab points within sphere of influence from valid pointers
            foreach (var pointer in proximityPointers)
            {
                if (IsPointWithinBounds(boundingBoxPosition, pointer.Position, maxRadius))
                {
                    proximityPoints.Add(pointer.Position);
                }

                if (IsPointWithinBounds(boundingBoxPosition, pointer.Result.Details.Point, maxRadius))
                {
                    proximityPoints.Add(pointer.Result.Details.Point);
                }
            }

            // Loop through all handles and find closest one
            Transform closestHandle = null;
            float closestDistanceSqr = float.MaxValue;
            foreach (var point in proximityPoints)
            {

                foreach (var baseHandles in registeredHandles)
                {

                    foreach (var item in baseHandles.proximityInfos)
                    {
                        // If handle can't be visible, skip calculations
                        if (!baseHandles.handleCollection.IsHandleTypeActive())
                        {
                            continue;
                        }

                        // Perform comparison on sqr distance since sqrt() operation is expensive in Vector3.Distance()
                        float sqrDistance = (item.Handle.transform.position - point).sqrMagnitude;
                        if (sqrDistance < closestDistanceSqr)
                        {
                            closestHandle = item.Handle;
                            closestDistanceSqr = sqrDistance;
                        }

                    }

                }
            }

            // Loop through all handles and update visual state based on closest point
            foreach (var baseHandles in registeredHandles)
            {
                foreach (var item in baseHandles.proximityInfos)
                {
                    HandleProximityState newState = (closestHandle == item.Handle) ? GetProximityState(closestDistanceSqr) : HandleProximityState.FullsizeNoProximity;

                    // Only apply updates if handle is in a new state or closest handle needs to lerp scaling
                    if (item.ProximityState != newState)
                    {
                        // Update and save new state
                        item.ProximityState = newState;

                        if (item.HandleVisualRenderer)
                        {
                            item.HandleVisualRenderer.material = newState == HandleProximityState.CloseProximity ? baseHandles.handleCollection.HandleGrabbedMaterial : baseHandles.handleCollection.HandleMaterial;
                        }
                    }

                    ScaleHandle(newState, item.Handle, baseHandles.handleCollection.HandleSize, true);
                }
            }
        }


        private void ScaleHandle(HandleProximityState state, Transform scaleVisual, float handleSize, bool lerp = false)
        {
            float targetScale = 1.0f, weight = 0.0f;

            switch (state)
            {
                case HandleProximityState.FullsizeNoProximity:
                    targetScale = farScale;
                    weight = lerp ? farGrowRate : 1.0f;
                    break;
                case HandleProximityState.MediumProximity:
                    targetScale = mediumScale;
                    weight = lerp ? mediumGrowRate : 1.0f;
                    break;
                case HandleProximityState.CloseProximity:
                    targetScale = closeScale;
                    weight = lerp ? closeGrowRate : 1.0f;
                    break;
            }

            float newLocalScale = (scaleVisual.localScale.x * (1.0f - weight)) + (handleSize * targetScale * weight);
            scaleVisual.localScale = new Vector3(newLocalScale, newLocalScale, newLocalScale);
        }

        /// <summary>
        /// Determine if passed point is within sphere of radius around this GameObject
        /// To avoid function overhead, request compiler to inline this function since repeatedly called every Update() for every pointer position and result
        /// </summary>
        /// <param name="point">world space position</param>
        /// <param name="radiusSqr">radius of sphere in distance squared for faster comparison</param>
        /// <returns>true if point is within sphere</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsPointWithinBounds(Vector3 position, Vector3 point, float radiusSqr)
        {
            return (position - point).sqrMagnitude < radiusSqr;
        }


        /// <summary>
        /// Get the ProximityState value based on the distanced provided
        /// </summary>
        /// <param name="sqrDistance">distance squared in proximity in meters</param>
        /// <returns>HandleProximityState for given distance</returns>
        private HandleProximityState GetProximityState(float sqrDistance)
        {
            if (sqrDistance < handleCloseProximity * handleCloseProximity)
            {
                return HandleProximityState.CloseProximity;
            }
            else if (sqrDistance < handleMediumProximity * handleMediumProximity)
            {
                return HandleProximityState.MediumProximity;
            }
            else
            {
                return HandleProximityState.FullsizeNoProximity;
            }
        }

        // register handles for proximity effect
        internal void AddHandles(BoundingBoxHandlesBase bbhandles)
        {
            RegisteredHandles handlesEntry = new RegisteredHandles() { handleCollection = bbhandles, proximityInfos = new List<HandleProximityInfo>() };
            bbhandles.ForEachHandle(handle => {
                handlesEntry.proximityInfos.Add(new HandleProximityInfo()
                {
                    Handle = bbhandles.GetVisual(handle),
                    HandleVisualRenderer = handle.gameObject.GetComponentInChildren<Renderer>()
                });
            });
            registeredHandles.Add(handlesEntry);
        }
    }
}


