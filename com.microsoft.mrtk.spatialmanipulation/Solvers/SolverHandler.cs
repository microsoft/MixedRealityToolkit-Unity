// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// This class handles the solver components that are attached to this
    /// <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>
    /// </summary>
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/solvers/solver")]
    [AddComponentMenu("MRTK/Spatial Manipulation/Solvers/Solver Handler")]
    public class SolverHandler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The interactor used when solving for the left hand / controller.")]
        private XRBaseInteractor leftInteractor = null;

        /// <summary>
        /// The interactor used when solving for the left hand / controller.
        /// </summary>
        public XRBaseInteractor LeftInteractor
        {
            get => leftInteractor;
            set => leftInteractor = value;
        }

        [SerializeField]
        [Tooltip("The interactor used when solving for the right hand / controller.")]
        private XRBaseInteractor rightInteractor = null;

        /// <summary>
        /// The interactor used when solving for the left hand / controller.
        /// </summary>
        public XRBaseInteractor RightInteractor
        {
            get => rightInteractor;
            set => rightInteractor = value;
        }

        [SerializeField]
        [Tooltip("Tracked object from which to calculate position and orientation. To manually override and use a scene object, use the TransformTarget field.")]
        private TrackedObjectType trackedTargetType = TrackedObjectType.Head;

        /// <summary>
        /// Tracked object from which to calculate position and orientation.
        /// To manually override and use a scene object, use the TransformTarget field.
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
        [FormerlySerializedAs("trackedHandness")]
        private Handedness trackedHandedness = Handedness.Both;

        /// <summary>
        /// If tracking hands or motion controllers, determines which hand(s) are valid attachments.
        /// </summary>
        /// <remarks>
        /// Only None, Left, Right, and Both are valid values
        /// </remarks>
        public Handedness TrackedHandedness
        {
            get => trackedHandedness;
            set
            {
                if (trackedHandedness != value && IsValidHandedness(value))
                {
                    trackedHandedness = value;
                    RefreshTrackedObject();
                }
            }
        }

        [SerializeField]
        [Tooltip("When TrackedTargetType is set to hand joint, use this specific joint to calculate position and orientation")]
        private TrackedHandJoint trackedHandJoint = TrackedHandJoint.Palm;

        /// <summary>
        /// When TrackedTargetType is set to hand joint, use this specific joint to calculate position and orientation
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
        [Tooltip("Additional offset of the tracked object on which to base the solver.")]
        private Vector3 additionalOffset;

        /// <summary>
        /// Add an additional offset of the tracked object on which to base the solver on.
        /// </summary>
        /// <remarks>
        /// AdditionalOffset can be helpful when tracking something like a halo position above your head
        /// or an object to the side of a hand / controller.
        /// </remarks>
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
        [Tooltip("Additional rotation to be added to the tracked object.")]
        private Vector3 additionalRotation;

        /// <summary>
        /// Additional rotation to be added to of the tracked object.
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
        [Tooltip("Indicated whether or not this SolverHandler calls SolverUpdate() every frame.")]
        private bool updateSolvers = true;

        /// <summary>
        /// Indicated whether or not this SolverHandler calls SolverUpdate() every frame.
        /// </summary>
        /// <remarks>
        /// Only one SolverHandler should manage SolverUpdate().
        /// This setting does not affect whether or not the Target
        /// Transform of this SolverHandler gets updated or not.
        /// </remarks>
        public bool UpdateSolvers
        {
            get => updateSolvers;
            set => updateSolvers = value;
        }

        protected List<Solver> solvers = new List<Solver>();

        /// <summary>
        /// List of solvers that this handler will manage and update.
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
        /// The position to which the solver is trying to move.
        /// </summary>
        public Vector3 GoalPosition { get; set; }

        /// <summary>
        /// The rotation to which the solver is trying to rotate.
        /// </summary>
        public Quaternion GoalRotation { get; set; }

        /// <summary>
        /// The scale to which the solver is trying to scale.
        /// </summary>
        public Vector3 GoalScale { get; set; }

        /// <summary>
        /// Alternate scale.
        /// </summary>
        public Vector3Smoothed AltScale { get; set; }

        /// <summary>
        /// The timestamp the solvers will use for calculations.
        /// </summary>
        public float DeltaTime { get; set; }

        /// <summary>
        /// The target transform upon which the solvers will act.
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
        /// Currently tracked hand or motion controller, if applicable.
        /// </summary>
        /// <remarks>
        /// The allowable <see cref="Handedness"/> values are Left, Right, or None
        /// </remarks>
        public Handedness CurrentTrackedHandedness => currentTrackedHandedness;

        // Stores controller side to favor if TrackedHandedness is set to both left and right.
        protected Handedness preferredTrackedHandedness = Handedness.Left;

        /// <summary>
        /// Controller side to favor and pick first if the <see cref="Handedness"/>
        /// is set to both left and right.
        /// </summary>
        /// <remarks>
        /// Allowed <see cref="Handedness"/> values are Left or Right. Both hands can't be preferred simultaneously.
        /// </remarks>
        public Handedness PreferredTrackedHandedness
        {
            get => preferredTrackedHandedness;
            set
            {
                if ((value == Handedness.Left || value == Handedness.Right)
                    && preferredTrackedHandedness != value)
                {
                    preferredTrackedHandedness = value;
                }
            }
        }

        // Hidden GameObject managed by this component and attached as a child to the tracked target type (i.e head, hand etc)
        private GameObject trackingTarget;

        private XRBaseInteractor controllerInteractor;

        private float lastUpdateTime;

        private const string TrackingTargetName = "SolverHandler Tracking Target";

        #region MonoBehaviour Implementation

        private void Awake()
        {
            GoalScale = Vector3.one;
            AltScale = new Vector3Smoothed(Vector3.one, 0.1f);
            DeltaTime = Time.deltaTime;
            lastUpdateTime = Time.realtimeSinceStartup;

            if (!IsValidHandedness(trackedHandedness))
            {
                Debug.LogError("Using invalid SolverHandler.TrackedHandness value. Defaulting to Handedness.Both");
                TrackedHandedness = Handedness.Both;
            }

            if (!IsValidTrackedObjectType(trackedTargetType))
            {
                Debug.LogError("Using an unsupported SolverHandler.TrackedTargetType. Defaulting to type Head");
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

            // MRTK2 had a system that tracked Unity transforms for all hand joints.
            // We don't have that (yet?) in v3, so SolverHandler manages it itself.
            if (TrackedTargetType == TrackedObjectType.HandJoint)
            {
                UpdateCachedHandJointTransform();
            }

            DeltaTime = Time.realtimeSinceStartup - lastUpdateTime;
            lastUpdateTime = Time.realtimeSinceStartup;
        }

        private void LateUpdate()
        {
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
            if (trackingTarget != null)
            {
                Destroy(trackingTarget);
            }
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

        // Used to cache and reduce allocs for getting the solver components on this GameObject
        private List<Solver> inspectorOrderedSolvers = new List<Solver>();

        /// <summary>
        /// Adds <paramref name="solver"/> to the list of <see cref="Solvers"/> guaranteeing inspector ordering.
        /// </summary>
        internal void RegisterSolver(Solver solver)
        {
            if (!solvers.Contains(solver))
            {
                // Make sure we only process solvers which are located on the same GameObject.
                GetComponents(inspectorOrderedSolvers);
                if (inspectorOrderedSolvers.Contains(solver))
                {
                    solvers.Add(solver);
                    // Ensure that the solvers list obeys inspector ordering afterwards
                    solvers = inspectorOrderedSolvers.Intersect(solvers).ToList();
                }
            }
        }

        /// <summary>
        /// Removes <paramref name="solver"/> from the list of <see cref="Solvers"/>.
        /// </summary>
        internal void UnregisterSolver(Solver solver)
        {
            solvers.Remove(solver);
        }

        protected virtual void DetachFromCurrentTrackedObject()
        {
            if (trackingTarget != null)
            {
                trackingTarget.transform.parent = null;
            }
        }

        private Transform cachedHandJointTransform = null;

        private static readonly ProfilerMarker AttachToNewTrackedObjectPerfMarker =
            new ProfilerMarker("[MRTK] SolverHandler.AttachToNewTrackedObject");

        protected virtual void AttachToNewTrackedObject()
        {
            using (AttachToNewTrackedObjectPerfMarker.Auto())
            {
                currentTrackedHandedness = Handedness.None;
                controllerInteractor = null;

                Transform target = null;
                if (TrackedTargetType == TrackedObjectType.Head)
                {
                    target = Camera.main.transform;
                }
                else if (TrackedTargetType == TrackedObjectType.ControllerRay)
                {
                    currentTrackedHandedness = TrackedHandedness;
                    if ((currentTrackedHandedness & Handedness.Both) == Handedness.Both)
                    {
                        currentTrackedHandedness = PreferredTrackedHandedness;
                        controllerInteractor = GetControllerInteractor(currentTrackedHandedness);

                        if (controllerInteractor == null || !controllerInteractor.isHoverActive)
                        {
                            // If no interactor found, try again on the opposite hand
                            currentTrackedHandedness = currentTrackedHandedness.GetOppositeHandedness();
                            controllerInteractor = GetControllerInteractor(currentTrackedHandedness);
                        }
                    }
                    else
                    {
                        controllerInteractor = GetControllerInteractor(currentTrackedHandedness);
                    }

                    if (controllerInteractor != null)
                    {
                        target = controllerInteractor.transform;
                    }
                    else
                    {
                        currentTrackedHandedness = Handedness.None;
                    }
                }
                else if (TrackedTargetType == TrackedObjectType.HandJoint)
                {
                    if (XRSubsystemHelpers.HandsAggregator != null)
                    {
                        currentTrackedHandedness = TrackedHandedness;
                        if ((currentTrackedHandedness & Handedness.Both) == Handedness.Both)
                        {
                            if (IsHandTracked(PreferredTrackedHandedness))
                            {
                                currentTrackedHandedness = PreferredTrackedHandedness;
                            }
                            else if (IsHandTracked(PreferredTrackedHandedness.GetOppositeHandedness()))
                            {
                                currentTrackedHandedness = PreferredTrackedHandedness.GetOppositeHandedness();
                            }
                            else
                            {
                                currentTrackedHandedness = Handedness.None;
                            }
                        }

                        if (UpdateCachedHandJointTransform())
                        {
                            target = cachedHandJointTransform;
                        }
                    }
                }
                else if (TrackedTargetType == TrackedObjectType.CustomOverride)
                {
                    target = this.transformOverride;
                }

                TrackTransform(target);
            }
        }

        private static readonly ProfilerMarker UpdateCachedHandJointTransformPerfMarker =
            new ProfilerMarker("[MRTK] SolverHandler.UpdateCachedHandJointTransform");

        /// <summary>
        /// Update the cached transform's position to match that of the current track joint. 
        /// </summary>
        /// <returns>
        /// True, if the tracked joint is found and cached transform is updated. False, otherwise.
        /// </returns>
        private bool UpdateCachedHandJointTransform()
        {
            bool updated = false;
            using (UpdateCachedHandJointTransformPerfMarker.Auto())
            {
                XRNode? handNode = currentTrackedHandedness.ToXRNode();

                if (handNode.HasValue &&
                    XRSubsystemHelpers.HandsAggregator != null &&
                    XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint, handNode.Value, out HandJointPose jointPos))
                {
                    if (cachedHandJointTransform == null)
                    {
                        cachedHandJointTransform = new GameObject("SolverHandler HandJoint Tracker").transform;
                    }

                    cachedHandJointTransform.SetPositionAndRotation(jointPos.Position, jointPos.Rotation);
                    updated = true;
                }
            }
            return updated;
        }

        private void TrackTransform(Transform target)
        {
            if (trackingTarget == null)
            {
                trackingTarget = new GameObject(TrackingTargetName);
                trackingTarget.hideFlags = HideFlags.HideInHierarchy;
            }

            if (target != null)
            {
                trackingTarget.transform.parent = target;
                trackingTarget.transform.localPosition = Vector3.Scale(AdditionalOffset, trackingTarget.transform.localScale);
                trackingTarget.transform.localRotation = Quaternion.Euler(AdditionalRotation);
            }

        }

        /// <summary>
        /// Gets the current hand's interactor.
        /// </summary>
        /// <param name="handedness">
        /// The <see cref="Handedness"/> of the controller for which the interactor
        /// is being requested.
        /// </param>
        /// <remarks>
        /// If <see cref="Handedness"/> is set to both left and right, the right interactor will be returned.
        /// </remarks>
        /// <returns>
        /// The associated interactor, attached to the controller with the
        /// specified <see cref="Handedness"/>, or null.
        /// </returns>
        private XRBaseInteractor GetControllerInteractor(Handedness handedness)
        {
            if (handedness == Handedness.None || !IsValidHandedness(handedness)) { return null; }

            return (handedness == Handedness.Left) ? LeftInteractor : RightInteractor;
        }

        /// <summary>
        /// Returns true if the solver handler's transform target is not valid
        /// </summary>
        /// <returns>true if not tracking valid hands and/or target, false otherwise</returns>
        private bool IsInvalidTracking()
        {
            if (trackingTarget == null || trackingTarget.transform.parent == null)
            {
                return true;
            }

            if (TrackedTargetType == TrackedObjectType.ControllerRay &&
                (controllerInteractor == null || !controllerInteractor.isHoverActive))
            {
                return true;
            }

            // If we were tracking a particular hand, check that our transform is still valid
            if (TrackedTargetType == TrackedObjectType.HandJoint && currentTrackedHandedness != Handedness.None)
            {
                bool trackingLeft = currentTrackedHandedness.IsMatch(Handedness.Left) && IsHandTracked(Handedness.Left);
                bool trackingRight = currentTrackedHandedness.IsMatch(Handedness.Right) && IsHandTracked(Handedness.Right);
                return !trackingLeft && !trackingRight;
            }

            return false;
        }

        /// <summary>
        /// Determines if the specified hand is being tracked by the subsystem.
        /// </summary>
        /// <param name="hand">The <see cref="Handedness"/> of the hand being queried.</param>
        /// <remarks>
        /// Currently, this returns true always.
        /// </remarks>
        /// <returns>
        /// True if the hand is tracked, or false.
        /// </returns>
        private bool IsHandTracked(Handedness hand)
        {
            // Early out if the hand isn't a valid XRNode
            // (i.e., Handedness.None or Handedness.Both)
            XRNode? node = hand.ToXRNode();
            if (!node.HasValue) { return false; }
            return XRSubsystemHelpers.HandsAggregator != null &&
                   XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, node.Value, out HandJointPose pose);
        }

        /// <summary>
        /// Determines whether or not the specified handedness is valid for solver scenarios.
        /// </summary>
        /// <param name="hand">The <see cref="Handedness"/> to be validated.</param>
        /// <returns>
        /// True if the <see cref="Handedness"/> value is valid for solver scenarios, or false.
        /// </returns>
        public static bool IsValidHandedness(Handedness hand)
        {
            return hand.IsMatch(Handedness.Both) || hand == Handedness.None;
        }

        /// <summary>
        /// Determines if the tracked object type is valid for solver scenarios.
        /// </summary>
        /// <param name="type">
        /// The <see cref="TrackedObjectType"/> to be validated.
        /// </param>
        /// <returns>
        /// True if the <see cref="TrackedObjectType"/> is valid for solver scenarios, or false.
        /// </returns>
        private static bool IsValidTrackedObjectType(TrackedObjectType type)
        {
            return type == TrackedObjectType.Head || type >= TrackedObjectType.ControllerRay;
        }
    }
}
