using System;
using UnityEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using System.Runtime.CompilerServices;


namespace Microsoft.MixedReality.Toolkit.UI.Experimental
{
    /// <summary>
    /// ProximityEffect scales and switches out materials for registered objects whenever a pointer is in proximity.
    /// Scaling is done on three different stages: far / medium and close proximity whereas material switching 
    /// will only be done on close proximity.
    /// </summary>
    [Serializable]
    public class ProximityEffect 
    {
        [SerializeField]
        [Tooltip("Determines whether proximity feature (scaling and material toggling) is activated")]
        private bool proximityEffectActive = false;

        /// <summary>
        /// Determines whether proximity feature (scaling and material toggling) is activated
        /// </summary>
        public bool ProximityEffectActive => proximityEffectActive;

        [SerializeField]
        [Tooltip("How far away should the hand be from an object before it starts scaling the object?")]
        [Range(0.005f, 0.2f)]
        private float objectMediumProximity = 0.1f;

        [SerializeField]
        [Tooltip("How far away should the hand be from an object before it activates the close-proximity scaling effect?")]
        [Range(0.001f, 0.1f)]
        private float objectCloseProximity = 0.03f;

        [SerializeField]
        [Tooltip("A Proximity-enabled object scales by this amount when a hand moves out of range. Default is 0, invisible object.")]
        private float farScale = 0.0f;

        /// <summary>
        /// A Proximity-enabled object scales by this amount when a hand moves out of range. Default is 0, invisible object.
        /// </summary>
        public float FarScale => farScale;

        [SerializeField]
        [Tooltip("A Proximity-enabled object scales by this amount when a hand moves into the Medium Proximity range. Default is 1.0, original object size.")]
        private float mediumScale = 1.0f;

        /// <summary>
        /// A Proximity-enabled object scales by this amount when a hand moves into the Medium Proximity range. Default is 1.0, original object size.
        /// </summary>
        public float MediumScale => mediumScale;
    
        [SerializeField]
        [Tooltip("A Proximity-enabled object scales by this amount when a hand moves into the Close Proximity range. Default is 1.5, larger object size.")]
        private float closeScale = 1.5f;

        /// <summary>
        /// A Proximity-enabled object scales by this amount when a hand moves into the Close Proximity range. Default is 1.5, larger object size
        /// </summary>
        public float CloseScale => closeScale;
      
        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled object scale when the Hand moves from Medium proximity to Far proximity?")]
        [Range(0.0f, 1.0f)]
        private float farGrowRate = 0.3f;

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled Object scale when the Hand moves to a distance that activates Medium Scale ?")]
        [Range(0.0f, 1.0f)]
        private float mediumGrowRate = 0.2f;

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled object scale when the Hand moves to a distance that activates Close Scale ?")]
        [Range(0.0f, 1.0f)]
        private float closeGrowRate = 0.3f;

        /// <summary>
        /// Internal state tracking for proximity of a object
        /// </summary>
        internal enum ProximityState
        {
            FullsizeNoProximity = 0,
            MediumProximity,
            CloseProximity
        }

        /// <summary>
        /// Container for object references and states
        /// </summary>
        private class ObjectProximityInfo
        {
            public Transform ScaledObject;
            public Renderer ObjectVisualRenderer; 
            public ProximityState ProximityState = ProximityState.FullsizeNoProximity;
        }

        /// <summary>
        /// Container for registered object providers and their proximity infos
        /// </summary>
        private class RegisteredObjects
        {
            public IProximityEffectObjectProvider objectProvider;
            public List<ObjectProximityInfo> proximityInfos;
        }

        private List<RegisteredObjects> registeredObjects = new List<RegisteredObjects>();


        private HashSet<IMixedRealityPointer> proximityPointers = new HashSet<IMixedRealityPointer>();
        private List<Vector3> proximityPoints = new List<Vector3>();

        #region public methods
        /// <summary>
        /// register objects for proximity effect via a <see cref="IProximityEffectObjectProvider"/>
        /// </summary>
        public void AddObjects(IProximityEffectObjectProvider provider)
        {
            RegisteredObjects registeredObject = new RegisteredObjects() { objectProvider = provider, proximityInfos = new List<ObjectProximityInfo>() };
            provider.ForEachProximityObject(proximityObject =>
            {
                registeredObject.proximityInfos.Add(new ObjectProximityInfo()
                {
                    ScaledObject = proximityObject,
                    ObjectVisualRenderer = proximityObject.gameObject.GetComponentInChildren<Renderer>()
                });
            });
            registeredObjects.Add(registeredObject);
        }

        /// <summary>
        /// Clears all registered objects in the proximity effect
        /// </summary>
        public void ClearObjects()
        {
            if (registeredObjects != null)
            {
                registeredObjects.Clear();
            }
        }

        /// <summary>
        /// Resets all objects that had a proximity effect applied. This will reset them to their default size and reset to the base material
        /// </summary>
        public void ResetProximityScale()
        {
            if (proximityEffectActive == false)
            {
                return;
            }

            foreach (var registeredObject in registeredObjects)
            {
                foreach (var item in registeredObject.proximityInfos)
                {
                    if (item.ProximityState != ProximityState.FullsizeNoProximity)
                    {
                        item.ProximityState = ProximityState.FullsizeNoProximity;
                       
                        if (item.ObjectVisualRenderer)
                        {
                            item.ObjectVisualRenderer.material = registeredObject.objectProvider.GetBaseMaterial();
                        }

                        ScaleObject(item.ProximityState, item.ScaledObject, registeredObject.objectProvider.GetObjectSize());
                    }
                }
            }
        }

        /// <summary>
        /// Updates proximity effect and it's registered objects.
        /// Highlights and scales objects in proximity according to the pointer distance
        /// </summary>
        /// <param name="boundsCenter">gameobject position the proximity effect is attached to</param>
        /// <param name="boundsExtents">extents of the gameobject the proximity effect is attached to</param>
        public void Update(Vector3 boundsCenter, Vector3 boundsExtents)
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
            float maxRadius = Mathf.Max(Mathf.Max(boundsExtents.x, boundsExtents.y), boundsExtents.z);
            maxRadius *= maxRadius;
            maxRadius += objectCloseProximity + objectMediumProximity;

            // Grab points within sphere of influence from valid pointers
            foreach (var pointer in proximityPointers)
            {
                if (IsPointWithinBounds(boundsCenter, pointer.Position, maxRadius))
                {
                    proximityPoints.Add(pointer.Position);
                }

                Vector3? point = pointer.Result?.Details.Point;
                if (point.HasValue && IsPointWithinBounds(boundsCenter, pointer.Result.Details.Point, maxRadius))
                {
                    proximityPoints.Add(pointer.Result.Details.Point);
                }
            }

            // Loop through all objects and find closest one
            Transform closestObject = null;
            float closestDistanceSqr = float.MaxValue;
            foreach (var point in proximityPoints)
            {

                foreach (var provider in registeredObjects)
                {

                    foreach (var item in provider.proximityInfos)
                    {
                        // If object can't be visible, skip calculations
                        if (!provider.objectProvider.IsActive())
                        {
                            continue;
                        }

                        // Perform comparison on sqr distance since sqrt() operation is expensive in Vector3.Distance()
                        float sqrDistance = (item.ScaledObject.transform.position - point).sqrMagnitude;
                        if (sqrDistance < closestDistanceSqr)
                        {
                            closestObject = item.ScaledObject;
                            closestDistanceSqr = sqrDistance;
                        }
                    }
                }
            }

            // Loop through all objects and update visual state based on closest point
            foreach (var provider in registeredObjects)
            {
                foreach (var item in provider.proximityInfos)
                {
                    ProximityState newState = (closestObject == item.ScaledObject) ? GetProximityState(closestDistanceSqr) : ProximityState.FullsizeNoProximity;

                    // Only apply updates if object is in a new state or closest object needs to lerp scaling
                    if (item.ProximityState != newState)
                    {
                        // Update and save new state
                        item.ProximityState = newState;

                        if (item.ObjectVisualRenderer)
                        {
                            item.ObjectVisualRenderer.material = newState == ProximityState.CloseProximity ? provider.objectProvider.GetHighlightedMaterial() : provider.objectProvider.GetBaseMaterial();
                        }
                    }

                    ScaleObject(newState, item.ScaledObject, provider.objectProvider.GetObjectSize(), true);
                }
            }
        }

        #endregion public methods

        #region private methods

        private bool IsAnyRegisteredObjectVisible()
        {
            foreach (var registeredObject in registeredObjects)
            {
                if (registeredObject.objectProvider.IsActive())
                {
                    return true;
                }
            }

            return false;
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
        /// <returns>ProximityState for given distance</returns>
        private ProximityState GetProximityState(float sqrDistance)
        {
            if (sqrDistance < objectCloseProximity * objectCloseProximity)
            {
                return ProximityState.CloseProximity;
            }
            else if (sqrDistance < objectMediumProximity * objectMediumProximity)
            {
                return ProximityState.MediumProximity;
            }
            else
            {
                return ProximityState.FullsizeNoProximity;
            }
        }

        #endregion private methods
    }
}
