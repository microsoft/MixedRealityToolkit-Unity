using System;
using UnityEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using System.Runtime.CompilerServices;


namespace Microsoft.MixedReality.Toolkit.UI.Experimental
{
    [Serializable]
    public class ProximityEffect 
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
        internal enum ProximityState
        {
            FullsizeNoProximity = 0,
            MediumProximity,
            CloseProximity
        }

        /// <summary>
        /// Container for handle references and states (including scale and rotation type handles)
        /// </summary>
        private class ObjectProximityInfo
        {
            public Transform ScaledObject;
            public Renderer ObjectVisualRenderer;
            public ProximityState ProximityState = ProximityState.FullsizeNoProximity;
        }



        /// Container for registered bounding box handles and their proximity states
        /// 
        private class RegisteredObjects
        {
            public IProximityScaleObjectProvider objectProvider;
            public List<ObjectProximityInfo> proximityInfos;
        }

        private List<RegisteredObjects> registeredObjects = new List<RegisteredObjects>();


        private HashSet<IMixedRealityPointer> proximityPointers = new HashSet<IMixedRealityPointer>();
        private List<Vector3> proximityPoints = new List<Vector3>();

        public void ClearObjects()
        {
            if (registeredObjects != null)
            {
                registeredObjects.Clear();
            }
        }

        public void ResetHandleProximityScale()
        {
            if (proximityEffectActive == false)
            {
                return;
            }

            foreach (var baseHandles in registeredObjects)
            {
                foreach (var item in baseHandles.proximityInfos)
                {
                    if (item.ProximityState != ProximityState.FullsizeNoProximity)
                    {
                        item.ProximityState = ProximityState.FullsizeNoProximity;
                       
                        if (item.ObjectVisualRenderer)
                        {
                            item.ObjectVisualRenderer.material = baseHandles.objectProvider.GetBaseMaterial();
                        }

                        ScaleObject(item.ProximityState, item.ScaledObject, baseHandles.objectProvider.GetObjectSize());
                    }
                }
            }
        }

        private bool IsAnyRegisteredObjectVisible()
        {
            foreach (var baseHandles in registeredObjects)
            {
                if (baseHandles.objectProvider.IsActive())
                {
                    return true;
                }
            }

            return false;
        }

        public void HandleProximityScaling(Vector3 boundingBoxPosition, Vector3 currentBoundsExtents)
        {
            // early out if effect is disabled
            if (proximityEffectActive == false || !IsAnyRegisteredObjectVisible())
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

                foreach (var baseHandles in registeredObjects)
                {

                    foreach (var item in baseHandles.proximityInfos)
                    {
                        // If handle can't be visible, skip calculations
                        if (!baseHandles.objectProvider.IsActive())
                        {
                            continue;
                        }

                        // Perform comparison on sqr distance since sqrt() operation is expensive in Vector3.Distance()
                        float sqrDistance = (item.ScaledObject.transform.position - point).sqrMagnitude;
                        if (sqrDistance < closestDistanceSqr)
                        {
                            closestHandle = item.ScaledObject;
                            closestDistanceSqr = sqrDistance;
                        }

                    }

                }
            }

            // Loop through all handles and update visual state based on closest point
            foreach (var baseHandles in registeredObjects)
            {
                foreach (var item in baseHandles.proximityInfos)
                {
                    ProximityState newState = (closestHandle == item.ScaledObject) ? GetProximityState(closestDistanceSqr) : ProximityState.FullsizeNoProximity;

                    // Only apply updates if handle is in a new state or closest handle needs to lerp scaling
                    if (item.ProximityState != newState)
                    {
                        // Update and save new state
                        item.ProximityState = newState;

                        if (item.ObjectVisualRenderer)
                        {
                            item.ObjectVisualRenderer.material = newState == ProximityState.CloseProximity ? baseHandles.objectProvider.GetHighlightedMaterial() : baseHandles.objectProvider.GetBaseMaterial();
                        }
                    }

                    ScaleObject(newState, item.ScaledObject, baseHandles.objectProvider.GetObjectSize(), true);
                }
            }
        }


        private void ScaleObject(ProximityState state, Transform scaleVisual, float objectSize, bool lerp = false)
        {
            float targetScale = 1.0f, weight = 0.0f;

            switch (state)
            {
                case ProximityState.FullsizeNoProximity:
                    targetScale = farScale;
                    weight = lerp ? farGrowRate : 1.0f;
                    break;
                case ProximityState.MediumProximity:
                    targetScale = mediumScale;
                    weight = lerp ? mediumGrowRate : 1.0f;
                    break;
                case ProximityState.CloseProximity:
                    targetScale = closeScale;
                    weight = lerp ? closeGrowRate : 1.0f;
                    break;
            }

            float newLocalScale = (scaleVisual.localScale.x * (1.0f - weight)) + (objectSize * targetScale * weight);
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
        private ProximityState GetProximityState(float sqrDistance)
        {
            if (sqrDistance < handleCloseProximity * handleCloseProximity)
            {
                return ProximityState.CloseProximity;
            }
            else if (sqrDistance < handleMediumProximity * handleMediumProximity)
            {
                return ProximityState.MediumProximity;
            }
            else
            {
                return ProximityState.FullsizeNoProximity;
            }
        }

        // register handles for proximity effect
        internal void AddHandles(IProximityScaleObjectProvider provider)
        {
            RegisteredObjects registeredObject = new RegisteredObjects() { objectProvider = provider, proximityInfos = new List<ObjectProximityInfo>() };
            provider.ForEachProximityObject(proximityObject => {
                registeredObject.proximityInfos.Add(new ObjectProximityInfo()
                {
                    ScaledObject = proximityObject,
                    ObjectVisualRenderer = proximityObject.gameObject.GetComponentInChildren<Renderer>()
                });
            });
            registeredObjects.Add(registeredObject);
        }
    }
}


