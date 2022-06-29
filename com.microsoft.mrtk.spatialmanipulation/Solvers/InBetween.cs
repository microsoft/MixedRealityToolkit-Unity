// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// InBetween solver positions an object in-between two tracked transforms.
    /// </summary>
    [AddComponentMenu("MRTK/Spatial Manipulation/Solvers/In Between")]
    public class InBetween : Solver
    {
        [SerializeField]
        [Tooltip("Distance along the center line the object will be located. 0.5 is halfway, 1.0 is at the first transform, 0.0 is at the second transform.")]
        [Range(0f, 1f)]
        private float partwayOffset = 0.5f;

        /// <summary>
        /// Distance along the center line the object will be located. 0.5 is halfway, 1.0 is at the first transform, 0.0 is at the second transform.
        /// </summary>
        public float PartwayOffset
        {
            get { return partwayOffset; }
            set { partwayOffset = Mathf.Clamp(value, 0.0f, 1.0f); }
        }

        [SerializeField]
        [Tooltip("Tracked object to calculate position and orientation for the second object. If you want to manually override and use a scene object, use the TransformTarget field.")]
        private TrackedObjectType secondTrackedObjectType = TrackedObjectType.Head;

        /// <summary>
        /// Tracked object to calculate position and orientation for the second object. If you want to manually override and use a scene object, use the TransformTarget field.
        /// </summary>
        public TrackedObjectType SecondTrackedObjectType
        {
            get { return secondTrackedObjectType; }
            set
            {
                if (secondTrackedObjectType != value)
                {
                    secondTrackedObjectType = value;
                    UpdateSecondSolverHandler();
                }
            }
        }

        [SerializeField]
        [Tooltip("If tracking hands or motion controllers, determines which hand(s) are valid attachments")]
        private Handedness secondTrackedHandedness = Handedness.Left | Handedness.Right;

        /// <summary>
        /// If tracking hands or motion controllers, determines which hand(s) are valid attachments.
        /// </summary>
        /// <remarks>
        /// Only None, Left, Right, and Both are valid values
        /// </remarks>
        public Handedness SecondTrackedHandedness
        {
            get => secondTrackedHandedness;
            set
            {
                if (secondTrackedHandedness != value && SolverHandler.IsValidHandedness(value))
                {
                    secondTrackedHandedness = value;
                    UpdateSecondSolverHandler();
                }
            }
        }

        [SerializeField]
        [Tooltip("When SecondTrackedTargetType is set to hand joint, use this specific joint to calculate position and orientation")]
        private TrackedHandJoint secondTrackedHandJoint = TrackedHandJoint.Palm;

        /// <summary>
        /// When SecondTrackedTargetType is set to hand joint, use this specific joint to calculate position and orientation
        /// </summary>
        public TrackedHandJoint SecondTrackedHandJoint
        {
            get => secondTrackedHandJoint;
            set
            {
                if (secondTrackedHandJoint != value)
                {
                    secondTrackedHandJoint = value;
                    UpdateSecondSolverHandler();
                }
            }
        }


        [SerializeField]
        [Tooltip("This transform overrides any Tracked Object as the second point for the In Between.")]
        private Transform secondTransformOverride = null;

        /// <summary>
        /// This transform overrides any Tracked Object as the second point for the In Between
        /// </summary>
        public Transform SecondTransformOverride
        {
            get { return secondTransformOverride; }
            set
            {
                if (secondTransformOverride != value)
                {
                    secondTransformOverride = value;
                    UpdateSecondSolverHandler();
                }
            }
        }

        private SolverHandler secondSolverHandler;

        protected override void Start()
        {
            base.Start();

            // We need to get the secondSolverHandler ready before we tell them both to seek a tracked object.
            secondSolverHandler = gameObject.AddComponent<SolverHandler>();
            secondSolverHandler.LeftInteractor = SolverHandler.LeftInteractor;
            secondSolverHandler.RightInteractor = SolverHandler.RightInteractor;
            secondSolverHandler.UpdateSolvers = false;

            UpdateSecondSolverHandler();
        }

        private static readonly ProfilerMarker SolverUpdatePerfMarker =
            new ProfilerMarker("[MRTK] InBetween.SolverUpdate");

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            using (SolverUpdatePerfMarker.Auto())
            {
                if (SolverHandler != null && secondSolverHandler != null)
                {
                    if (SolverHandler.TransformTarget != null && secondSolverHandler.TransformTarget != null)
                    {
                        AdjustPositionForOffset(SolverHandler.TransformTarget, secondSolverHandler.TransformTarget);
                    }
                }
            }
        }

        private void AdjustPositionForOffset(Transform targetTransform, Transform secondTransform)
        {
            if (targetTransform != null && secondTransform != null)
            {
                Vector3 centerline = targetTransform.position - secondTransform.position;
                GoalPosition = secondTransform.position + (centerline * partwayOffset);
            }
        }

        private void UpdateSecondSolverHandler()
        {
            if (secondSolverHandler != null)
            {
                secondSolverHandler.TrackedTargetType = secondTrackedObjectType;

                if (secondTrackedObjectType == TrackedObjectType.ControllerRay)
                {
                    secondSolverHandler.TrackedHandedness = secondTrackedHandedness;
                }
                else if (secondTrackedObjectType == TrackedObjectType.HandJoint)
                {
                    secondSolverHandler.TrackedHandedness = secondTrackedHandedness;
                    secondSolverHandler.TrackedHandJoint = secondTrackedHandJoint;
                }
                else if (secondTrackedObjectType == TrackedObjectType.CustomOverride && secondTransformOverride != null)
                {
                    secondSolverHandler.TransformOverride = secondTransformOverride;
                }
            }
        }
    }
}