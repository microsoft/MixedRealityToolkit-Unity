// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.SDK.Inspectors")]
namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Solvers
{
    /// <summary>
    /// Tap to place is a far interaction component used to place objects on a surface.
    /// </summary>
    public class TapToPlace : Solver, IMixedRealityPointerHandler
    {
        
        [SerializeField]
        [Tooltip("The default distance (in meters) an object will be placed relative to the TrackedTargetType forward in the SolverHandler." +
            " The GameObjectToPlace will be placed at the default placement distance if a surface is not hit by the raycast.")]
        private float defaultPlacementDistance = 1.5f;

        /// <summary>
        /// The default distance (in meters) an object will be placed relative to the TrackedTargetType forward in the SolverHandler.
        /// The GameObjectToPlace will be placed at the default placement distance if a surface is not hit by the raycast.
        /// </summary>
        public float DefaultPlacementDistance
        {
            get => defaultPlacementDistance;
            set => defaultPlacementDistance = value;    
        }

        [SerializeField]
        [Tooltip("Max distance (in meters) to place an object if there is a raycast hit on a surface.")]
        private float maxRaycastDistance = 20.0f;

        /// <summary>
        /// The max distance (in meters) to place an object if there is a raycast hit on a surface
        /// </summary>
        public float MaxRaycastDistance
        {
            get => maxRaycastDistance;
            set => maxRaycastDistance = value;
        }

        [SerializeField]
        [Tooltip("Array of LayerMask to execute from highest to lowest priority. First layermask to provide a raycast hit will be used by component.")]
        private LayerMask[] magneticSurfaces = { UnityEngine.Physics.DefaultRaycastLayers };

        /// <summary>
        /// Array of LayerMask to execute from highest to lowest priority. First layermask to provide a raycast hit will be used by component.
        /// </summary>
        public LayerMask[] MagneticSurfaces
        {
            get => magneticSurfaces;
            set => magneticSurfaces = value;
        }

        /// <summary>
        /// Is the gameObjectToPlace currently in the placing state? This state is activated when the GameObjectToPlace
        /// is selected.
        /// </summary>
        public bool IsBeingPlaced { get; protected set; }

        [SerializeField]
        [Tooltip("If true, the gameObjectToPlace will start in the placing state.  The object will immediately start" +
        " following the TrackedTargetType (Head or Controller Ray) and then a tap is required to place the object.  AutoStart" +
         " is only called once in Start();")]
        private bool autoStart = false;

        /// <summary>
        /// If true, the gameObjectToPlace will start in the placing state.  The object will immediately start
        /// following the TrackedTargetType (Head or Controller Ray) and then a tap is required to place the object.  AutoStart
        /// is only called once in Start();
        /// </summary>
        public bool AutoStart
        {
            get => autoStart;
            set => autoStart = value;
        }

        [SerializeField]
        [Tooltip("The distance between the center of the gameobject to place and a surface along the surface normal, if the raycast hits a surface")]
        private float surfaceNormalOffset = 0.0f;

        /// <summary>
        /// The distance between the center of the gameobject to place and a surface along the surface normal, if the raycast hits a surface.
        /// </summary>
        public float SurfaceNormalOffset
        {
            get => surfaceNormalOffset;
            set => surfaceNormalOffset = value;
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
            set => keepOrientationVertical = value;
        }

        [SerializeField]
        [Tooltip("If true and in the Unity Editor, the normal of the raycast hit will be drawn in yellow.")]
        private bool debugEnabled = true;

        /// <summary>
        /// If true and in the Unity Editor, the normal of the raycast hit will be drawn in yellow.
        /// </summary>
        public bool DebugEnabled
        {
            get => debugEnabled;
            set => debugEnabled = value;
        }

        [SerializeField]
        [Tooltip("If false, the game object to place will not change its rotation according to the surface hit.  The object will" +
            " remain facing the camera while it is in the placing state.  If true, the object will rotate according to the surface normal" +
            " if there is a hit.")]
        private bool rotateAccordingToSurface = false;

        /// <summary>
        /// If false, the game object to place will not change its rotation according to the surface hit.  The object will 
        /// remain facing the camera while it is in the placing state.  If true, the object will rotate according to the surface normal
        /// if there is a hit."
        /// </summary>
        public bool RotateAccordingToSurface
        {
            get => rotateAccordingToSurface;
            set => rotateAccordingToSurface = value;
        }

        [Tooltip("This event is triggered once at the start of the placing state.")]
        /// <summary>
        /// This event is triggered once at the start of the placing state.
        /// </summary>
        public UnityEvent OnPlacingStarted = new UnityEvent();

        [Tooltip("This event is triggered once when the placing state has ended.")]
        /// <summary>
        /// This event is triggered once when the placing state has ended.
        /// </summary>
        public UnityEvent OnPlacingStopped = new UnityEvent();

        /// <summary>
        /// Get the game object layer before the object is placed.  When an object is in the placing state, the layer
        /// is changed to IgnoreRaycast and then back to the original layer.
        /// </summary>
        public int GameObjectLayer { get; protected set; }

        protected internal bool IsColliderPresent => gameObject != null ? gameObject.GetComponent<Collider>() != null : false;

        private const int IgnoreRaycastLayer = 2;

        // The current ray is based on the TrackedTargetType (Controller Ray, Head, Hand Joint)
        protected RayStep CurrentRay;

        protected bool DidHitSurface;

        protected RaycastHit CurrentHit;

        protected float TimeSinceClick = 0;

        #region MonoBehaviour Implementation
        protected override void Start()
        {
            // Solver base class
            base.Start();

            if (!IsColliderPresent)
            {
                Debug.LogError("The GameObjectToPlace does not have a collider attached, please attach a collider");
            }

            SurfaceNormalOffset = gameObject.GetComponent<Collider>().bounds.extents.z;

            // Store the initial game object layer
            GameObjectLayer = gameObject.layer;

            if (AutoStart)
            {
                StartPlacement();
                OnPlacingStarted?.Invoke();
            }
            else
            {
                SolverHandler.UpdateSolvers = false;
            } 
        }

        private void OnDisable()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
        }

        #endregion

        private void StartPlacement()
        {
            // Store the initial game object layer
            GameObjectLayer = gameObject.layer;

            // Temporarily change the game object layer to IgnoreRaycastLayer to enable a surface hit beyond the game object
            gameObject.layer = IgnoreRaycastLayer;

            SolverHandler.UpdateSolvers = true;

            IsBeingPlaced = true;

            CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
        }

        private void StopPlacement()
        {
            // Change the game object layer back to the game object's layer on start
            gameObject.layer = GameObjectLayer;

            SolverHandler.UpdateSolvers = false;

            IsBeingPlaced = false;

            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
        }

        /// <inheritdoc/>
        public override void SolverUpdate()
        {
            // Make sure the Transform target is not null, added for the case where auto start is true 
            // and the tracked target type is the controller ray, if the hand is not in the frame we cannot
            // calculate the position of the object
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
            CurrentRay.UpdateRayStep(ref origin, ref endpoint);

            // Check if the current ray hits a magnetic surface
            DidHitSurface = MixedRealityRaycaster.RaycastSimplePhysicsStep(CurrentRay, MaxRaycastDistance, MagneticSurfaces, false, out CurrentHit);  
        }

        protected virtual void SetPosition()
        {
            // Change the position of the gameObject if there was a hit, if not then place the object at the default distance
            // relative to the TrackedTargetType origin position
            
            if (DidHitSurface)
            {
                // Take the current hit point and add an offset relative to the surface to avoid half of the object in the surface
                GoalPosition = CurrentHit.point;  
                AddOffset(CurrentHit.normal * SurfaceNormalOffset);

                #if UNITY_EDITOR
                if(DebugEnabled)
                {
                    // Draw the normal of the raycast hit for debugging 
                    Debug.DrawRay(CurrentHit.point, CurrentHit.normal * 0.5f, Color.yellow);
                }
                #endif
            }
            else
            {
                GoalPosition = SolverHandler.TransformTarget.position + (SolverHandler.TransformTarget.forward * DefaultPlacementDistance);
            }
        }

        protected virtual void SetRotation()
        {
            Vector3 direction = CurrentRay.Direction;
            Vector3 surfaceNormal = CurrentHit.normal;
            
            if (KeepOrientationVertical)
            {
                direction.y = 0;
                surfaceNormal.y = 0;
            }

            // If the object is on a surface then change the rotation according to the normal of the hit point
            if (DidHitSurface && rotateAccordingToSurface)
            {
                GoalRotation = Quaternion.LookRotation(-surfaceNormal, Vector3.up);
            }
            else 
            {
                // If (DidHit and !rotateAccordingToSurface) or if there is no raycast hit, 
                GoalRotation = Quaternion.LookRotation(direction, Vector3.up);
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
            if ((Time.time - TimeSinceClick) < 1.0f)
            {
                return;
            }

            if (!IsBeingPlaced)
            {
                StartPlacement();
                OnPlacingStarted?.Invoke();
            }
            else
            {
                StopPlacement();
                OnPlacingStopped?.Invoke();
            }

            // Get the time of this click action
            TimeSinceClick = Time.time;
        }

        #endregion
    }
}
