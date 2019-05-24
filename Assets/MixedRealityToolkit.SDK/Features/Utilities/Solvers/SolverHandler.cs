// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// This class handles the solver components that are attached to this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>
    /// </summary>
    public class SolverHandler : ControllerFinder
    {
        [SerializeField]
        [Tooltip("Tracked object to calculate position and orientation from. If you want to manually override and use a scene object, use the TransformTarget field.")]
        private TrackedObjectType trackedObjectToReference = TrackedObjectType.Head;

        /// <summary>
        /// Tracked object to calculate position and orientation from. If you want to manually override and use a scene object, use the TransformTarget field.
        /// </summary>
        public TrackedObjectType TrackedObjectToReference
        {
            get { return trackedObjectToReference; }
            set
            {
                if (trackedObjectToReference != value)
                {
                    trackedObjectToReference = value;
                    RefreshTrackedObject();
                }
            }
        }

        [SerializeField]
        [Tooltip("When using the hand to calculate position and orientation, use this specific joint.")]
        private TrackedHandJoint trackedHandJoint = TrackedHandJoint.Palm;

        /// <summary>
        /// When using the hand to calculate position and orientation, use this specific joint.
        /// </summary>
        public TrackedHandJoint TrackedHandJoint
        {
            get { return trackedHandJoint; }
            set
            {
                trackedHandJoint = value;
                RefreshTrackedObject();
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
            get { return additionalOffset; }
            set
            {
                additionalOffset = value;
                transformTarget = MakeOffsetTransform(transformTarget);
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
            get { return additionalRotation; }
            set
            {
                additionalRotation = value;
                transformTarget = MakeOffsetTransform(transformTarget);
            }
        }

        [SerializeField]
        [Tooltip("Manual override for TrackedObjectToReference if you want to use a scene object. Leave empty if you want to use head, motion-tracked controllers, or motion-tracked hands.")]
        private Transform transformTarget;

        /// <summary>
        /// The target transform that the solvers will act upon.
        /// </summary>
        public Transform TransformTarget
        {
            get { return transformTarget; }
            set { transformTarget = value; }
        }

        [SerializeField]
        [Tooltip("Whether or not this SolverHandler calls SolverUpdate() every frame. Only one SolverHandler should manage SolverUpdate(). This setting does not affect whether the Target Transform of this SolverHandler gets updated or not.")]
        private bool updateSolvers = true;

        /// <summary>
        /// Whether or not this SolverHandler calls SolverUpdate() every frame. Only one SolverHandler should manage SolverUpdate(). This setting does not affect whether the Target Transform of this SolverHandler gets updated or not.
        /// </summary>
        public bool UpdateSolvers
        {
            get { return updateSolvers; }
            set { updateSolvers = value; }
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

        private bool RequiresOffset => AdditionalOffset.sqrMagnitude != 0 || AdditionalRotation.sqrMagnitude != 0;

        protected readonly List<Solver> solvers = new List<Solver>();

        private float lastUpdateTime;

        private GameObject transformWithOffset;

        private IMixedRealityHandJointService HandJointService => handJointService ?? (handJointService = (InputSystem as IMixedRealityDataProviderAccess)?.GetDataProvider<IMixedRealityHandJointService>());
        private IMixedRealityHandJointService handJointService = null;

        #region MonoBehaviour Implementation

        private void Awake()
        {
            GoalScale = Vector3.one;
            AltScale = new Vector3Smoothed(Vector3.one, 0.1f);
            DeltaTime = Time.deltaTime;
            lastUpdateTime = Time.realtimeSinceStartup;

            solvers.AddRange(GetComponents<Solver>());
        }

        private void Start()
        {
            // TransformTarget overrides TrackedObjectToReference
            if (!transformTarget)
            {
                AttachToNewTrackedObject();
            }
        }

        private void Update()
        {
            DeltaTime = Time.realtimeSinceStartup - lastUpdateTime;
            lastUpdateTime = Time.realtimeSinceStartup;
        }

        private void LateUpdate()
        {
            if (UpdateSolvers)
            {
                for (int i = 0; i < solvers.Count; ++i)
                {
                    Solver solver = solvers[i];

                    if (solver.enabled)
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

        protected override void OnControllerFound()
        {
            if (!transformTarget)
            {
                TrackTransform(ControllerTransform);
            }
        }

        protected override void OnControllerLost()
        {
            DetachFromCurrentTrackedObject();
        }

        /// <summary>
        /// Clears the transform target and attaches to the current <see cref="TrackedObjectToReference"/>.
        /// </summary>
        public void RefreshTrackedObject()
        {
            DetachFromCurrentTrackedObject();
            AttachToNewTrackedObject();
        }

        protected virtual void DetachFromCurrentTrackedObject()
        {
            transformTarget = null;

            if (transformWithOffset != null)
            {
                Destroy(transformWithOffset);
                transformWithOffset = null;
            }
        }

        protected virtual void AttachToNewTrackedObject()
        {
            switch (TrackedObjectToReference)
            {
                case TrackedObjectType.Head:
                    // No need to search for a controller if we've already attached to the head.
                    Handedness = Handedness.None;
                    TrackTransform(CameraCache.Main.transform);
                    break;
                case TrackedObjectType.MotionControllerLeft:
                    Handedness = Handedness.Left;
                    break;
                case TrackedObjectType.MotionControllerRight:
                    Handedness = Handedness.Right;
                    break;
                case TrackedObjectType.HandJointLeft:
                    // Set to None, so the underlying ControllerFinder doesn't attach to a controller.
                    // TODO: Make this more generic / configurable for hands vs controllers. Also resolve the duplicate Handedness variables.
                    Handedness = Handedness.None;
                    TrackTransform(RequestEnableHandJoint(Handedness.Left));
                    break;
                case TrackedObjectType.HandJointRight:
                    Handedness = Handedness.None;
                    TrackTransform(RequestEnableHandJoint(Handedness.Right));
                    break;
            }
        }

        private void TrackTransform(Transform newTrackedTransform)
        {
            transformTarget = RequiresOffset ? MakeOffsetTransform(newTrackedTransform) : newTrackedTransform;
        }

        public Transform RequestEnableHandJoint(Handedness handedness)
        {
            return HandJointService?.RequestJointTransform(trackedHandJoint, handedness);
        }

        private Transform MakeOffsetTransform(Transform parentTransform)
        {
            if (transformWithOffset == null)
            {
                transformWithOffset = new GameObject();
                transformWithOffset.transform.parent = parentTransform;
            }

            transformWithOffset.transform.localPosition = Vector3.Scale(AdditionalOffset, transformWithOffset.transform.localScale);
            transformWithOffset.transform.localRotation = Quaternion.Euler(AdditionalRotation);
            transformWithOffset.name = string.Format("{0} on {1} with offset {2}, {3}", gameObject.name, TrackedObjectToReference.ToString(), AdditionalOffset, AdditionalRotation);
            return transformWithOffset.transform;
        }
    }
}