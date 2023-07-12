// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Tap to place is a far interaction component used to place objects on a surface.
    /// </summary>
    [AddComponentMenu("MRTK/Spatial Manipulation/Solvers/Tap To Place")]
    public class TapToPlace : Solver
    {

        // todo: needed? [Space(10)]
        [SerializeField]
        [Tooltip("If true, the game object to place will start selected.  The object will immediately start" +
                " following the TrackedTargetType (Head or Controller) and then a tap is required to place the object." +
                " This value must be modified before Start() is invoked in order to have any effect")]
        private bool autoStart = false;

        /// <summary>
        /// If true, the game object to place will start out selected.  The object will immediately start
        /// following the TrackedTargetType (Head or Controller Ray) and then a tap is required to place the object.  
        /// This value must be modified before Start() is invoked in order to have any effect.
        /// </summary>
        public bool AutoStart
        {
            get => autoStart;
            set => autoStart = value;
        }

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

        /// <summary>
        /// If true, the game object to place is selected.
        /// </summary>
        public bool IsBeingPlaced { get; protected set; }

        [SerializeField]
        [Tooltip("The distance between the center of the game object to place and a surface along the surface normal, if the raycast hits a surface")]
        private float surfaceNormalOffset = 0.0f;

        /// <summary>
        /// The distance between the center of the game object to place and a surface along the surface normal, if the raycast hits a surface.
        /// </summary>
        public float SurfaceNormalOffset
        {
            get => surfaceNormalOffset;
            set
            {
                // If a user were to configure Tap to Place via script and they try to set the SurfaceNormalOffset while UseDefaultSurfaceNormalOffset is true, display the following error:
                Debug.Assert(!UseDefaultSurfaceNormalOffset, $"The new value for SurfaceNormalOffset on the Tap to Place object will not be applied because UseDefaultSurfaceNormalOffset is true, set UseDefaultSurfaceNormalOffset to false.");

                surfaceNormalOffset = value;
            }
        }

        [SerializeField]
        [Tooltip("If true, the default surface normal offset will be used instead of any value specified for the SurfaceNormalOffset property. If false, the " +
                "SurfaceNormalOffset is used. The default surface normal offset is the Z extents of the bounds on the attached collider, this ensures the object being " +
                "placed is aligned on a surface. This property is automatically set to false if the SurfaceNormalOffset property is set and is not " +
                "the default value.")]
        private bool useDefaultSurfaceNormalOffset = true;

        /// <summary>
        /// If true, the default surface normal offset will be used instead of any value specified for the SurfaceNormalOffset property.  
        /// If false, the SurfaceNormalOffset is used. The default surface normal offset is the Z extents of the bounds on the attached collider, this 
        /// ensures the object being placed is aligned on a surface.
        /// </summary>
        public bool UseDefaultSurfaceNormalOffset
        {
            get => useDefaultSurfaceNormalOffset;
            set => useDefaultSurfaceNormalOffset = value;
        }

        [SerializeField]
        [Tooltip("If true, the game object to place will remain upright and in line with Vector3.up")]
        private bool keepOrientationVertical = false;

        /// <summary>
        /// If true, the game object to place will remain upright and in line with Vector3.up
        /// </summary>
        public bool KeepOrientationVertical
        {
            get => keepOrientationVertical;
            set => keepOrientationVertical = value;
        }

        [SerializeField]
        [Tooltip("If false, the game object to place will not change its rotation according to the surface hit.  The object will" +
                " remain facing the camera while IsBeingPlaced is true.  If true, the object will rotate according to the surface normal" +
                " if there is a hit.")]
        private bool rotateAccordingToSurface = false;

        /// <summary>
        /// If false, the game object to place will not change its rotation according to the surface hit.  The object will
        /// remain facing the camera while IsBeingPlaced is true.  If true, the object will rotate according to the surface normal
        /// if there is a hit.
        /// </summary>
        public bool RotateAccordingToSurface
        {
            get => rotateAccordingToSurface;
            set => rotateAccordingToSurface = value;
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
        [Tooltip("This event is triggered once when the game object to place is selected.")]
        private UnityEvent onPlacingStarted = new UnityEvent();

        /// <summary>
        /// This event is triggered once when the game object to place is selected.
        /// </summary>
        public UnityEvent OnPlacingStarted
        {
            get => onPlacingStarted;
            set => onPlacingStarted = value;
        }

        [SerializeField]
        [Tooltip("This event is triggered once when the game object to place is unselected, placed.")]
        private UnityEvent onPlacingStopped = new UnityEvent();

        /// <summary>
        /// This event is triggered once when the game object to place is unselected, placed.
        /// </summary>
        public UnityEvent OnPlacingStopped
        {
            get => onPlacingStopped;
            set => onPlacingStopped = value;
        }

        /// <summary>
        /// The current game object layer before it is temporarily switched to IgnoreRaycast while placing the game object.
        /// </summary>
        protected internal int GameObjectLayer { get; protected set; }

        protected internal bool IsColliderPresent => gameObject != null && gameObject.GetComponent<Collider>() != null;

        /// <summary>
        /// The default value for SurfaceNormalOffset if UseDefaultSurfaceNormalOffset is true.  This value ensures an object
        /// will be placed in alignment with a surface. This value is not cached to specifically support adjustments to object scale 
        /// while in the placing state.
        /// </summary>
        private float DefaultSurfaceNormalOffset => gameObject.GetComponent<Collider>().bounds.extents.z;

        private int ignoreRaycastLayer;

        /// <summary>
        /// The current ray is based on the TrackedTargetType (Controller Ray, Head, Hand Joint).
        /// The following properties are updated each frame while the game object is selected to determine
        /// object placement if there is a hit on a surface.
        /// </summary>
        protected RayStep CurrentRay;

        protected bool DidHitSurface;

        protected RaycastHit CurrentHit;

        // Used to record the time (seconds) between OnPointerClicked calls to avoid two calls in a row.
        protected float LastTimeClicked = 0;

        protected float DoubleClickTimeout = 0.5f;

        // Used to mark whether Start() has been called.
        private bool startCalled;

        // Used to mark whether StartPlacement() is called before Start() is called.
        private bool placementRequested;

        // Used to obtain list of known interactors
        private XRInteractionManager interactionManager;

        // Used to cache a known set of interactor
        private List<IXRInteractor> interactorsCache;

        #region MonoBehaviour Implementation

        protected override void Start()
        {
            base.Start();

            Debug.Assert(IsColliderPresent, $"The game object {gameObject.name} does not have a collider attached, please attach a collider to use Tap to Place");

            // When a game object is created via script, the bounds of the collider remain at the default size 
            // of (1, 1, 1) which always returns a 0.5 SurfaceNormalOffset.  Adding SyncTransforms updates the
            // size of the collider to match the game object before we calculate the SurfaceNormalOffset.
            UnityPhysics.SyncTransforms();

            ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");

            startCalled = true;

            if (AutoStart || placementRequested)
            {
                StartPlacement();
            }
            else
            {
                SolverHandler.UpdateSolvers = false;
            }
        }

        protected override void OnDisable()
        {
            StopPlacement();
            base.OnDisable();
        }

        #endregion

        private static readonly ProfilerMarker StartPlacementPerfMarker =
            new ProfilerMarker("[MRTK] TapToPlace.StartPlacement");

        /// <summary>
        /// Start the placement of a game object without the need of the OnPointerClicked event. The game object will begin to follow the 
        /// TrackedTargetType (Head by default) at a default distance. StopPlacementViaPerformedAction() must be called after StartPlacement() to stop the 
        /// game object from following the TrackedTargetType.  The game object layer is changed to IgnoreRaycast temporarily and then
        /// restored to its original layer in StopPlacementViaPerformedAction().
        /// </summary>
        public void StartPlacement()
        {
            // Checking the amount of time passed between when StartPlacement or StopPlacementViaPerformedAction is called twice in
            // succession. If these methods are called twice very rapidly, the object will be
            // selected and then immediately unselected. If two calls occur within the
            // double click timeout, then return to prevent an immediate object state switch.
            // Also, check that time is no 0 to allow for auto start functionality.
            if (Time.time != 0 && (Time.time - LastTimeClicked) < DoubleClickTimeout)
            {
                return;
            }

            // Get the time of this click action
            LastTimeClicked = Time.time;

            using (StartPlacementPerfMarker.Auto())
            {
                // Check to see if Start() has been called, if not set placementRequested to true. This will make sure StartPlacement() will be
                // called again when Start() is called.
                if (!startCalled)
                {
                    placementRequested = true;
                    return;
                }

                // Added for code configurability to avoid multiple calls to StartPlacement in a row
                if (!IsBeingPlaced)
                {
                    // Store the initial game object layer
                    GameObjectLayer = gameObject.layer;

                    // Temporarily change the game object layer to IgnoreRaycastLayer to enable a surface hit beyond the game object
                    gameObject.layer = ignoreRaycastLayer;

                    SolverHandler.UpdateSolvers = true;

                    IsBeingPlaced = true;

                    OnPlacingStarted?.Invoke();

                    RegisterPlacementAction();
                }
            }
        }

        private static readonly ProfilerMarker StopPlacementPerfMarker =
            new ProfilerMarker("[MRTK] TapToPlace.StopPlacementViaPerformedAction");

        /// <summary>
        /// Stop the placement of a game object via an action's performance.
        /// </summary>
        private void StopPlacementViaPerformedAction(InputAction.CallbackContext context)
        {
            StopPlacement();
        }

        /// <summary>
        /// Stop the placement of a game object via an interactor's select event.
        /// </summary>
        private void StopPlacementViaSelect(SelectEnterEventArgs args)
        {
            StopPlacement();
        }

        /// <summary>
        /// Stop the placement of a game object. 
        /// </summary>
        public void StopPlacement()
        {
            // Checking the amount of time passed between when StartPlacement or StopPlacementViaPerformedAction is called twice in
            // succession. If these methods are called twice very rapidly, the object will be
            // selected and then immediately unselected. If two calls occur within the
            // double click timeout, then return to prevent an immediate object state switch.
            if ((Time.time - LastTimeClicked) < DoubleClickTimeout)
            {
                return;
            }
            // Get the time of this click action
            LastTimeClicked = Time.time;

            using (StopPlacementPerfMarker.Auto())
            {
                // Added for code configurability to avoid multiple calls to StopPlacementViaPerformedAction in a row
                if (IsBeingPlaced)
                {
                    // Change the game object layer back to the game object's layer on start
                    gameObject.layer = GameObjectLayer;

                    SolverHandler.UpdateSolvers = false;

                    IsBeingPlaced = false;

                    OnPlacingStopped?.Invoke();

                    UnregisterPlacementAction();
                }
            }
        }

        private static readonly ProfilerMarker SolverUpdatePerfMarker =
            new ProfilerMarker("[MRTK] TapToPlace.SolverUpdate");

        /// <inheritdoc/>
        public override void SolverUpdate()
        {
            using (SolverUpdatePerfMarker.Auto())
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
        }

        private static readonly ProfilerMarker PerformRaycastPerfMarker =
            new ProfilerMarker("[MRTK] TapToPlace.PerformRaycast");

        protected virtual void PerformRaycast()
        {
            using (PerformRaycastPerfMarker.Auto())
            {
                // The transform target is the transform of the TrackedTargetType, i.e. Controller Ray, Head or Hand Joint
                Transform transform = SolverHandler.TransformTarget;

                Vector3 origin = transform.position;
                Vector3 endpoint = transform.position + transform.forward;
                CurrentRay.UpdateRayStep(in origin, in endpoint);

                // Check if the current ray hits a magnetic surface
                DidHitSurface = MixedRealityRaycaster.RaycastSimplePhysicsStep(CurrentRay, MaxRaycastDistance, MagneticSurfaces, false, out CurrentHit);
            }
        }

        /// <summary>
        /// Change the position of the game object if there was a hit, if not then place the object at the default distance
        /// relative to the TrackedTargetType origin position
        /// </summary>
        protected virtual void SetPosition()
        {
            if (DidHitSurface)
            {
                // Take the current hit point and add an offset relative to the surface to avoid half of the object in the surface
                GoalPosition = CurrentHit.point;

                // Allow switching between a specified SurfaceNormalOffset and the defaultSurfaceNormalOffset while the object is in the placing state
                // The defaultSurfaceNormalOffset is based on the Z extents of the bounds on a collider which is subject to change while the object is in the placing state
                float currentSurfaceNormalOffset = UseDefaultSurfaceNormalOffset ? DefaultSurfaceNormalOffset : SurfaceNormalOffset;

                AddOffset(CurrentHit.normal * currentSurfaceNormalOffset);

#if UNITY_EDITOR
                if (DebugEnabled)
                {
                    // Draw the normal of the raycast hit for debugging 
                    Debug.DrawRay(CurrentHit.point, CurrentHit.normal * 0.5f, Color.yellow);
                }
#endif // UNITY_EDITOR
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
                GoalRotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }

        /// <summary>
        /// Registers the input action which performs placement.
        /// </summary>
        private void RegisterPlacementAction()
        {
            // Refresh the registeration if they already exist
            UnregisterPlacementAction();

            if (interactionManager == null)
            {
                interactionManager = ComponentCache<XRInteractionManager>.FindFirstActiveInstance();
                if (interactionManager == null)
                {
                    Debug.LogError("No interaction manager found in scene. Please add an interaction manager to the scene.");
                }
            }

            if (interactorsCache == null)
            {
                interactorsCache = new List<IXRInteractor>();
            }

            // Try registering for the controller's "action" so object selection isn't required for placement.
            // If no controller, then fallback to using object selections for placement.
            interactionManager.GetRegisteredInteractors(interactorsCache);
            foreach (IXRInteractor interactor in interactorsCache)
            {
                if (interactor is XRBaseControllerInteractor controllerInteractor &&
                    controllerInteractor.xrController is ActionBasedController actionController)
                {
                    actionController.selectAction.action.performed += StopPlacementViaPerformedAction;
                }
                else if (interactor is IXRSelectInteractor selectInteractor)
                {
                    selectInteractor.selectEntered.AddListener(StopPlacementViaSelect);
                }
            }
        }

        /// <summary>
        /// Unregisters the input action which performs placement.
        /// </summary>
        private void UnregisterPlacementAction()
        {
            if (interactorsCache != null)
            {
                foreach (IXRInteractor interactor in interactorsCache)
                {
                    if (interactor is XRBaseControllerInteractor controllerInteractor &&
                        controllerInteractor.xrController is ActionBasedController actionController)
                    {
                        actionController.selectAction.action.performed -= StopPlacementViaPerformedAction;
                    }
                    else if (interactor is IXRSelectInteractor selectInteractor)
                    {
                        selectInteractor.selectEntered.RemoveListener(StopPlacementViaSelect);
                    }
                }
                interactorsCache.Clear();
            }
        }

        /// <summary>
        /// Extracts the InputAction from the InputActionReference.
        /// </summary>
        /// <param name="actionReference">
        /// The InputActionReference containing the desired InputAction.
        /// </param>
        /// <returns>An InputAction, or null.</returns>
        public static InputAction GetInputActionFromReference(InputActionReference actionReference)
        {
            if (actionReference == null) { return null; }
            return actionReference.action;
        }
    }
}
