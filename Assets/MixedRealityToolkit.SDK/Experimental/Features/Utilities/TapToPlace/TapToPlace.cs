// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    /// <summary>
    /// Tap to place is a far interaction component used to place objects on a surface.
    /// </summary>
    public class TapToPlace : Solver, IMixedRealityPointerHandler
    {
        [Experimental]
        [SerializeField]
        [Tooltip("The game object that will be placed if it is selected")]
        private GameObject gameObjectToPlace;

        /// <summary>
        /// The game object that will be placed if it is selected
        /// </summary>
        public GameObject GameObjectToPlace
        {
            get => gameObjectToPlace;
            set
            {
                if (value != null && gameObjectToPlace != value)
                {
                    gameObjectToPlace = value;
                }
            }
        }

        /// <summary>
        /// Check if a collider is present on the GameObjectToPlace
        /// </summary>
        public bool ColliderPresent
        {
            get
            {
                if (GameObjectToPlace.GetComponent<Collider>() == null)
                {
                    return false;
                }

                return true;
            }
        }

        [SerializeField]
        [Tooltip("The default distance (in meters) an object will be placed relative to the TrackedTargetType forward in the SolverHandler." +
            "The GameObjectToPlace will be placed at the default placement distance if a surface is not hit by the raycast.")]
        private float defaultPlacementDistance = 1.5f;

        /// <summary>
        /// The default distance (in meters) an object will be placed relative to the TrackedTargetType forward in the SolverHandler.
        /// The GameObjectToPlace will be placed at the default placement distance if a surface is not hit by the raycast.
        /// </summary>
        public float DefaultPlacementDistance
        {
            get => defaultPlacementDistance;
            set
            {
                if (defaultPlacementDistance != value)
                {
                    defaultPlacementDistance = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("Max distance to place an object if there is a raycast hit on a surface")]
        private float maxRaycastDistance = 20.0f;

        /// <summary>
        /// The max distance to place an object if there is a raycast hit on a surface
        /// </summary>
        public float MaxRaycastDistance
        {
            get => maxRaycastDistance;
            set => maxRaycastDistance = value;
        }

        [SerializeField]
        [Tooltip("Array of LayerMask to execute from highest to lowest priority. First layermask to provide a raycast hit will be used by component")]
        private LayerMask[] magneticSurfaces = { UnityEngine.Physics.DefaultRaycastLayers };

        /// <summary>
        /// Array of LayerMask to execute from highest to lowest priority. First layermask to provide a raycast hit will be used by component.
        /// </summary>
        public LayerMask[] MagneticSurfaces
        {
            get => magneticSurfaces;
            set => magneticSurfaces = value;
        }

        //[SerializeField]
        //[Tooltip("Is the gameObjectToPlace currently in a state where it is being placed? This state is activated when you selecthe GameObjectToPlace.")]
        //private bool isBeingPlaced = false;

        /// <summary>
        /// Is the gameObjectToPlace currently in a state where it is being placed? This state is activated when you select
        /// the GameObjectToPlace.
        /// </summary>
        public bool IsBeingPlaced { get; protected set; }

        [SerializeField]
        [Tooltip("If true, the gameObjectToPlace will start in the placing state.  Meaning that the object will immediately start" +
        "by following the TrackedTargetType (Head or Controller Ray) and then a tap is required to place the object.  AutoStart" +
         "is only called once because it is in Start();")]
        private bool autoStart = false;

        /// <summary>
        /// If true, the gameObjectToPlace will start in the placing state.  Meaning that the object will immediately start
        /// by following the TrackedTargetType (Head or Controller Ray) and then a tap is required to place the object.  AutoStart
        /// is only called once because it is in Start();
        /// </summary>
        public bool AutoStart
        {
            get => autoStart;
            set
            {
                if (autoStart != value)
                {
                    autoStart = value;
                }
            }
        }

        /// <summary>
        /// If true, the raycast did hit a surface
        /// </summary>
        public bool DidHit { get; protected set; }

        [SerializeField]
        [Tooltip("The distance between the center of the gameobject to place and a surface along the surface normal, if the raycast hits a surface")]
        private float surfaceNormalOffset = 0.0f;

        /// <summary>
        /// SurfaceNormalOffset is the offset between the game object's center and The default value for SurfaceNormalOffset is half the depth of a game object. 
        /// </summary>
        public float SurfaceNormalOffset
        {
            get => surfaceNormalOffset;
            set
            {
                if (surfaceNormalOffset != value)
                {
                    surfaceNormalOffset = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("If true, the GameObjectToPlace will remain upright and in line with Vector3.up")]
        private bool keepOrientationVertical = false;

        /// <summary>
        /// If true, the GameObjectToPlace will remain upright and in line with Vector3.up
        /// </summary>
        public bool KeepOrientationVertical
        {
            get => keepOrientationVertical;
            set
            {
                if (keepOrientationVertical != value)
                {
                    keepOrientationVertical = value;
                }
            }
        }


        [SerializeField]
        [Tooltip("If true, the spatial mesh will be visible while the object is in the placing state.")]
        private bool spatialMeshVisible = true;

        /// <summary>
        /// If true, the spatial mesh will be visible while the object is in the placing state.
        /// </summary>
        public bool SpatialMeshVisible
        {
            get => spatialMeshVisible;
            set
            {
                if (spatialMeshVisible != value)
                {
                    spatialMeshVisible = value; 
                }
            }
        }



        [SerializeField]
        [Tooltip("")]
        private SpatialAwarenessMeshDisplayOptions initialSpatialMeshDisplayOption;

        /// <summary>
        /// Get the spatial mesh visibility on start.  If the the spatial mesh is visible by default then do not toggle the
        /// spatial mesh visibility while placing the game object.  Spatial mesh visibility refers to if the wire frame material is
        /// on the spatial mesh or not.
        /// </summary>
        public SpatialAwarenessMeshDisplayOptions InitialSpatialMeshDisplayOption
        {
            get
            {

                IReadOnlyList<IMixedRealitySpatialAwarenessMeshObserver> observers = spatialAwarenessSystemAcecess.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

                foreach (IMixedRealitySpatialAwarenessMeshObserver observer in observers)
                {
                    return observer.DisplayOption;
                }

                // if the spatial awareness is not enabled
                return SpatialAwarenessMeshDisplayOptions.None;   
            }
        }

        private IMixedRealityDataProviderAccess spatialAwarenessSystemAcecess;

        [SerializeField]
        [Tooltip("If false, the game object to place will not change its rotation according to the surface hit.  The object will" +
            "remain facing the camera while it is in the placing state.  If true, the object will rotate according to the surface normal" +
            "if there is a hit.")]
        private bool rotateAccordingToSurface = false;

        /// <summary>
        /// If false, the game object to place will not change its rotation according to the surface hit.  The object will 
        /// remain facing the camera while it is in the placing state.  If true, the object will rotate according to the surface normal
        /// if there is a hit."
        /// </summary>
        public bool RotateAccordingToSurface
        {
            get => rotateAccordingToSurface;
            set
            {
                if (rotateAccordingToSurface != value)
                {
                    rotateAccordingToSurface = value;
                }
            }
        }

        private const int IgnoreRaycastLayer = 2;

        private const int DefaultLayer = 0;

        // The current ray is based on the TrackedTargetType (Controller Ray, Head, Hand Joint)
        protected RayStep currentRay;

        protected RaycastHit currentHit;

        protected float previousFrameNumber = 0;

        protected override void Start()
        {
            // Solver base class
            base.Start();

            // If tap to place is added via script, set the GameObjectToPlace as this gameobject 
            if (GameObjectToPlace == null)
            {
                GameObjectToPlace = gameObject;
            }

            if(!ColliderPresent)
            {
                Debug.LogError("The GameObjectToPlace does not have a collider attached, please attach a collider");
            }

            SurfaceNormalOffset = GetComponent<Collider>().bounds.extents.z;

            // What if the spatial awareness is not enabled?
            spatialAwarenessSystemAcecess = CoreServices.SpatialAwarenessSystem as IMixedRealityDataProviderAccess;

            // Get the spatial mesh display option on start
            // I do not know if this is legal, check on this
            initialSpatialMeshDisplayOption = InitialSpatialMeshDisplayOption;

            if (AutoStart)
            {
                StartPlacement();
            }
            else
            {
                SolverHandler.UpdateSolvers = false;
            }
        }

        private void StartPlacement()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);

            // Temporarily change the game object layer to IgnoreRaycastLayer to enable a surface hit beyond the game object
            gameObject.layer = IgnoreRaycastLayer;

            if (SpatialMeshVisible)
            {
                ToggleSpatialMeshVisibility(true);
            }

            SolverHandler.UpdateSolvers = true;

            IsBeingPlaced = true;
        }

        private void StopPlacement()
        {
            // Change the game object layer back to default which enables raycast hits again
            gameObject.layer = DefaultLayer;

            if (SpatialMeshVisible)
            {
                ToggleSpatialMeshVisibility(false);
            }

            SolverHandler.UpdateSolvers = false;

            IsBeingPlaced = false;

            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
        }

        /// <inheritdoc/>
        public override void SolverUpdate()
        {
            // Make sure the Transform target is not null, added for the case where auto start is true 
            // and the tracked target type is the controller ray, if the hand is not in the frame we cannot
            // calculate the postion of the object
            if (SolverHandler.TransformTarget != null)
            {
                PerformRaycast();
                SetPosition();
                SetRotation();
            }
        }

        protected virtual void PerformRaycast()
        {
            // The transform target is the transform of the TrackedTargetType, i.e. Controller Ray, Head or Hand Joint
            var transform = SolverHandler.TransformTarget;

            Vector3 origin = transform.position;
            Vector3 endpoint = transform.position + transform.forward;
            currentRay.UpdateRayStep(ref origin, ref endpoint);

            // Check if the current ray hits a magnetic surface
            DidHit = MixedRealityRaycaster.RaycastSimplePhysicsStep(currentRay, MaxRaycastDistance, MagneticSurfaces, false, out currentHit);  
        }

        protected virtual void SetPosition()
        {
            // Change the position of the GameObjectToPlace if there was a hit, if not then place the object at the default distance
            // relative to the TrackedTargetType position
            
            if (DidHit)
            {
                // take the current hit point and add an offset relative to the surface to avoid half of the object in the surface
                GoalPosition = currentHit.point;  
                AddOffset(currentHit.normal * SurfaceNormalOffset);
                
                // Draw the normal of the raycast hit for debugging 
                Debug.DrawRay(currentHit.point, currentHit.normal * 0.5f, Color.yellow);
            }
            else
            {
                GoalPosition = SolverHandler.TransformTarget.position + (SolverHandler.TransformTarget.forward * DefaultPlacementDistance);
            }
        }

        protected virtual void SetRotation()
        {
            Vector3 direction = currentRay.Direction;
            Vector3 surfaceNormal = currentHit.normal;

            if (KeepOrientationVertical)
            {
                direction.y = 0;
                surfaceNormal.y = 0;
            }

            // if the object is on a surface then change the rotation according to the normal of the hit point
            if (DidHit && rotateAccordingToSurface)
            {
                GoalRotation = Quaternion.LookRotation(-surfaceNormal, Vector3.up);
            }
            else 
            {
                // If (DidHit and !rotateAccordingToSurface) or if there is no raycast hit, 
                GoalRotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }

        private void ToggleSpatialMeshVisibility(bool spatialMeshVisibility)
        {
            // If the spatial mesh is already visible on start, then we do not need to toggle the spatial mesh 
            // if the object is in the placing state
            if (InitialSpatialMeshDisplayOption == SpatialAwarenessMeshDisplayOptions.None || InitialSpatialMeshDisplayOption == SpatialAwarenessMeshDisplayOptions.Occlusion)
            {
                IReadOnlyList<IMixedRealitySpatialAwarenessMeshObserver> observers = spatialAwarenessSystemAcecess.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

                foreach (IMixedRealitySpatialAwarenessMeshObserver observer in observers)
                {
                    // If the user wants the spatial mesh visible while in the placing state
                    if (spatialMeshVisibility)
                    {
                        observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Visible;
                    }
                    else
                    {
                        observer.DisplayOption = InitialSpatialMeshDisplayOption;
                    }
                }
            }
        }

        #region IMixedRealityPointerHandler

        /// <inheritdoc/>
        public void OnPointerDown(MixedRealityPointerEventData eventData) { }

        /// <inheritdoc/>
        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        /// <inheritdoc/>
        public void OnPointerUp(MixedRealityPointerEventData eventData) { }

        /// <inheritdoc/>
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {

            // When a click is called in the same second then it is a mistake and no action needs to be taken
            Debug.Log("Time Difference: " + (Time.time - previousFrameNumber));
            if ((Time.time - previousFrameNumber) < 1.0f)
            {
                Debug.Log("Double Click has been caught, no actions triggered");
                return;

            }

            if (!IsBeingPlaced)
            {
                Debug.Log("Start Placement");
                StartPlacement();

            }
            else
            {
                Debug.Log("Stop Placement");
                StopPlacement();
            }

            // Get the time of this click action
            previousFrameNumber = Time.time;
        }

        #endregion

    }
}
