// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// This class handles the solver components that are attached to this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>
    /// </summary>
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/solvers/solver")]
    [AddComponentMenu("Scripts/MRTK/SDK/SolverHandler")]
    public class SolverHandler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Tracked object to calculate position and orientation from. If you want to manually override and use a scene object, use the TransformTarget field.")]
        [FormerlySerializedAs("trackedObjectToReference")]
        private TrackedObjectType trackedTargetType = TrackedObjectType.Head;

        /// <summary>
        /// Tracked object to calculate position and orientation from. If you want to manually override and use a scene object, use the TransformTarget field.
        /// </summary>
        public TrackedObjectType TrackedTargetType
        {
            get => trackedTargetType;
            set
            {
                if (trackedTargetType != value && IsValidTrackedObjectType(value))
                {
                    trackedTargetType = value;
                    RefreshTrackedObject();
                }
            }
        }

        [SerializeField]
        [Tooltip("If tracking hands or motion controllers, determines which hand(s) are valid attachments")]
        private Handedness trackedHandness = Handedness.Both;

        /// <summary>
        /// If tracking hands or motion controllers, determines which hand(s) are valid attachments.
        /// </summary>
        /// <remarks>
        /// Only None, Left, Right, and Both are valid values
        /// </remarks>
        public Handedness TrackedHandness
        {
            get => trackedHandness;
            set
            {
                if (trackedHandness != value && IsValidHandedness(value))
                {
                    trackedHandness = value;
                    RefreshTrackedObject();
                }
            }
        }

        [SerializeField]
        [Tooltip("When TrackedTargetType is set to hands, use this specific joint to calculate position and orientation")]
        private TrackedHandJoint trackedHandJoint = TrackedHandJoint.Palm;

        /// <summary>
        /// When TrackedTargetType is set to hands, use this specific joint to calculate position and orientation
        /// </summary>
        public TrackedHandJoint TrackedHandJoint
        {
            get => trackedHandJoint;
            set
            {
                if (trackedHandJoint != value)
                {
                    trackedHandJoint = value;
                    RefreshTrackedObject();
                }
            }
        }

        [SerializeField]
        [Tooltip("Manual override for when TrackedTargetType is set to CustomOverride")]
        private Transform transformOverride;

        /// <summary>
        /// Manual override for when TrackedTargetType is set to CustomOverride
        /// </summary>
        public Transform TransformOverride
        {
            set
            {
                if (value != null && transformOverride != value)
                {
                    transformOverride = value;
                    RefreshTrackedObject();
                }
            }
        }

        [SerializeField]
        [Tooltip("Add an additional offset of the tracked object to base the solver on. Useful for tracking something like a halo position above your head or off the side of a controller.")]
        private Vector3 additionalOffset;

        /// <summary>
        /// Add an additional offset of the tracked object to base the solver on. Useful for tracking something like a halo position above your head or off the side of a controller.
        /// </summary>
        public Vector3 AdditionalOffset
        {
            get => additionalOffset;
            set
            {
                if (additionalOffset != value)
                {
                    additionalOffset = value;
                    RefreshTrackedObject();
                }
            }
        }

        [SerializeField]
        [Tooltip("Add an additional rotation on top of the tracked object. Useful for tracking what is essentially the up or right/left vectors.")]
        private Vector3 additionalRotation;

        /// <summary>
        /// Add an additional rotation on top of the tracked object. Useful for tracking what is essentially the up or right/left vectors.
        /// </summary>
        public Vector3 AdditionalRotation
        {
            get => additionalRotation;
            set
            {
                if (additionalRotation != value)
                {
                    additionalRotation = value;
                    RefreshTrackedObject();
                }
            }
        }

        [SerializeField]
        [Tooltip("Whether or not this SolverHandler calls SolverUpdate() every frame. Only one SolverHandler should manage SolverUpdate(). This setting does not affect whether the Target Transform of this SolverHandler gets updated or not.")]
        private bool updateSolvers = true;

        /// <summary>
        /// Whether or not this SolverHandler calls SolverUpdate() every frame. Only one SolverHandler should manage SolverUpdate(). This setting does not affect whether the Target Transform of this SolverHandler gets updated or not.
        /// </summary>
        public bool UpdateSolvers
        {
            get => updateSolvers;
            set => updateSolvers = value;
        }

        protected readonly List<Solver> solvers = new List<Solver>();
        private bool updateSolversList = false;

        /// <summary>
        /// List of solvers that this handler will manage and update
        /// </summary>
        public IReadOnlyCollection<Solver> Solvers
        {
            get => solvers.AsReadOnly();
            set
            {
                if (value != null)
                {
                    solvers.Clear();
                    solvers.AddRange(value);
                }
            }
        }

        /// <summary>
        /// The position the solver is trying to move to.
        /// </summary>
        public Vector3 GoalPosition { get; set; }

        /// <summary>
        /// The rotation the solver is trying to rotate to.
        /// </summary>
        public Quaternion GoalRotation { get; set; }

        /// <summary>
        /// The scale the solver is trying to scale to.
        /// </summary>
        public Vector3 GoalScale { get; set; }

        /// <summary>
        /// Alternate scale.
        /// </summary>
        public Vector3Smoothed AltScale { get; set; }

        /// <summary>
        /// The timestamp the solvers will use to calculate with.
        /// </summary>
        public float DeltaTime { get; set; }

        /// <summary>
        /// The target transform that the solvers will act upon.
        /// </summary>
        public Transform TransformTarget
        {
            get
            {
                if (IsInvalidTracking())
                {
                    RefreshTrackedObject();
                }

                return trackingTarget != null ? trackingTarget.transform : null;
            }
        }

        // Stores currently attached hand if valid (only possible values Left, Right, or None)
        protected Handedness currentTrackedHandedness = Handedness.None;

        /// <summary>
        /// Currently tracked hand or motion controller if applicable
        /// </summary>
        /// <remarks>
        /// Only possible values Left, Right, or None
        /// </remarks>
        public Handedness CurrentTrackedHandedness => currentTrackedHandedness;

        // Stores controller side to favor if TrackedHandedness is set to both
        protected Handedness preferredTrackedHandedness = Handedness.Left;

        /// <summary>
        /// Controller side to favor and pick first if TrackedHandedness is set to both
        /// </summary>
        /// <remarks>
        /// Only possible values, Left or Right
        /// </remarks>
        public Handedness PreferredTrackedHandedness
        {
            get => preferredTrackedHandedness;
            set
            {
                if ((value.IsLeft() || value.IsRight())
                    && preferredTrackedHandedness != value)
                {
                    preferredTrackedHandedness = value;
                }
            }
        }


        // Hidden GameObject managed by this component and attached as a child to the tracked target type (i.e head, hand etc)
        private GameObject trackingTarget;

        private LinePointer controllerRay;

        private float lastUpdateTime;

        private IMixedRealityHandJointService HandJointService
            => handJointService ?? (handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>());

        private IMixedRealityHandJointService handJointService = null;

        #region MonoBehaviour Implementation

        private void Awake()
        {
            GoalScale = Vector3.one;
            AltScale = new Vector3Smoothed(Vector3.one, 0.1f);
            DeltaTime = Time.deltaTime;
            lastUpdateTime = Time.realtimeSinceStartup;

            if (!IsValidHandedness(trackedHandness))
            {
                Debug.LogError("Using invalid SolverHandler.TrackedHandness value. Defaulting to Handedness.Both");
                TrackedHandness = Handedness.Both;
            }

            if (!IsValidTrackedObjectType(trackedTargetType))
            {
                Debug.LogError("Using Obsolete SolverHandler.TrackedTargetType. Defaulting to type Head");
                TrackedTargetType = TrackedObjectType.Head;
            }
        }

        protected virtual void Start()
        {
            RefreshTrackedObject();
        }

        protected virtual void Update()
        {
            if (IsInvalidTracking())
            {
                RefreshTrackedObject();
            }

            DeltaTime = Time.realtimeSinceStartup - lastUpdateTime;
            lastUpdateTime = Time.realtimeSinceStartup;
        }

        private void LateUpdate()
        {
            if (updateSolversList)
            {
                IEnumerable<Solver> inspectorOrderedSolvers = GetComponents<Solver>().Intersect(solvers);
                Solvers = inspectorOrderedSolvers.Union(Solvers).ToReadOnlyCollection();

                updateSolversList = false;
            }

            if (UpdateSolvers)
            {
                // Before calling solvers, update goal to be the transform so that working and transform will match
                GoalPosition = transform.position;
                GoalRotation = transform.rotation;
                GoalScale = transform.localScale;

                for (int i = 0; i < solvers.Count; ++i)
                {
                    Solver solver = solvers[i];

                    if (solver != null && solver.enabled)
                    {
                        solver.SolverUpdateEntry();
                    }
                }
            }
        }

        protected void OnDestroy()
        {
            DetachFromCurrentTrackedObject();
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Clears the transform target and attaches to the current <see cref="TrackedTargetType"/>.
        /// </summary>
        public void RefreshTrackedObject()
        {
            DetachFromCurrentTrackedObject();
            AttachToNewTrackedObject();
        }

        /// <summary>
        /// Adds <paramref name="solver"/> to the list of <see cref="Solvers"/> guaranteeing inspector ordering.
        /// </summary>
        public void RegisterSolver(Solver solver)
        {
            if (!solvers.Contains(solver))
            {
                solvers.Add(solver);
                updateSolversList = true;
            }
        }

        /// <summary>
        /// Removes <paramref name="solver"/> from the list of <see cref="Solvers"/>.
        /// </summary>
        public void UnregisterSolver(Solver solver)
        {
            solvers.Remove(solver);
        }

        protected virtual void DetachFromCurrentTrackedObject()
        {
            if (trackingTarget != null)
            {
                Destroy(trackingTarget);
                trackingTarget = null;
            }
        }

        protected virtual void AttachToNewTrackedObject()
        {
            currentTrackedHandedness = Handedness.None;
            controllerRay = null;

            Transform target = null;
            if (TrackedTargetType == TrackedObjectType.Head)
            {
                target = CameraCache.Main.transform;
            }
            else if (TrackedTargetType == TrackedObjectType.ControllerRay)
            {
                if (TrackedHandness == Handedness.Both)
                {
                    currentTrackedHandedness = PreferredTrackedHandedness;
                    controllerRay = PointerUtils.GetPointer<LinePointer>(currentTrackedHandedness);

                    if (controllerRay == null)
                    {
                        // If no pointer found, try again on the opposite hand
                        currentTrackedHandedness = currentTrackedHandedness.GetOppositeHandedness();
                        controllerRay = PointerUtils.GetPointer<LinePointer>(currentTrackedHandedness);
                    }
                }
                else
                {
                    currentTrackedHandedness = TrackedHandness;
                    controllerRay = PointerUtils.GetPointer<LinePointer>(currentTrackedHandedness);
                }

                if (controllerRay != null)
                {
                    target = controllerRay.transform;
                }
                else
                {
                    currentTrackedHandedness = Handedness.None;
                }
            }
            else if (TrackedTargetType == TrackedObjectType.HandJoint)
            {
                if (HandJointService != null)
                {
                    currentTrackedHandedness = TrackedHandness;
                    if (currentTrackedHandedness == Handedness.Both)
                    {
                        if (HandJointService.IsHandTracked(PreferredTrackedHandedness))
                        {
                            currentTrackedHandedness = PreferredTrackedHandedness;
                        }
                        else if (HandJointService.IsHandTracked(PreferredTrackedHandedness.GetOppositeHandedness()))
                        {
                            currentTrackedHandedness = PreferredTrackedHandedness.GetOppositeHandedness();
                        }
                        else
                        {
                            currentTrackedHandedness = Handedness.None;
                        }
                    }

                    target = HandJointService.RequestJointTransform(this.TrackedHandJoint, currentTrackedHandedness);
                }
            }
            else if (TrackedTargetType == TrackedObjectType.CustomOverride)
            {
                target = this.transformOverride;
            }

            TrackTransform(target);
        }

        private void TrackTransform(Transform target)
        {
            if (trackingTarget != null || target == null) return;

            string name = string.Format("SolverHandler Target on {0} with offset {1}, {2}", target.gameObject.name, AdditionalOffset, AdditionalRotation);
            trackingTarget = new GameObject(name);
            trackingTarget.hideFlags = HideFlags.HideInHierarchy;

            trackingTarget.transform.parent = target;
            trackingTarget.transform.localPosition = Vector3.Scale(AdditionalOffset, trackingTarget.transform.localScale);
            trackingTarget.transform.localRotation = Quaternion.Euler(AdditionalRotation);
        }

        private LinePointer GetControllerRay(Handedness handedness)
        {
            return PointerUtils.GetPointer<LinePointer>(handedness);
        }

        /// <summary>
        /// Returns true if the solver handler's transform target is not valid
        /// </summary>
        /// <returns>true if not tracking valid hands and/or target, false otherwise</returns>
        private bool IsInvalidTracking()
        {
            if (trackingTarget == null)
            {
                return true;
            }

            // If we are attached to a pointer (i.e controller ray), 
            // check if pointer's controller is still be tracked
            if (TrackedTargetType == TrackedObjectType.ControllerRay &&
                (controllerRay == null || !controllerRay.IsTracked))
            {
                return true;
            }

            // If we were tracking a particular hand, check that our transform is still valid
            // The HandJointService does not destroy its own hand joint tracked GameObjects even when a hand is no longer tracked
            // Those HandJointService's GameObjects though are the parents of our tracked transform and thus will not be null/destroyed
            if (TrackedTargetType == TrackedObjectType.HandJoint && !currentTrackedHandedness.IsNone())
            {
                bool trackingLeft = HandJointService.IsHandTracked(Handedness.Left);
                bool trackingRight = HandJointService.IsHandTracked(Handedness.Right);

                return (currentTrackedHandedness.IsLeft() && !trackingLeft) ||
                       (currentTrackedHandedness.IsRight() && !trackingRight);
            }

            return false;
        }

        public static bool IsValidHandedness(Handedness hand)
        {
            return hand <= Handedness.Both;
        }

        public static bool IsValidTrackedObjectType(TrackedObjectType type)
        {
            return type == TrackedObjectType.Head || type >= TrackedObjectType.ControllerRay;
        }

        #region Obsolete

        /// <summary>
        /// Tracked object to calculate position and orientation from. If you want to manually override and use a scene object, use the TransformTarget field.
        /// </summary>
        [Obsolete("Use TrackedTargetType instead")]
        public TrackedObjectType TrackedObjectToReference
        {
            get => trackedTargetType;
            set
            {
                if (trackedTargetType != value)
                {
                    trackedTargetType = value;
                    RefreshTrackedObject();
                }
            }
        }

        #endregion
    }
}
