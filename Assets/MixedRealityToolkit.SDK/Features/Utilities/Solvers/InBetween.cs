// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// InBetween solver positions an object in-between two tracked transforms.
    /// </summary>
    public class InBetween : Solver
    {
        [SerializeField]
        [Tooltip("Distance along the center line the object will be located. 0.5 is halfway, 1.0 is at the second transform, 0.0 is at the first transform.")]
        [Range(0f, 1f)]
        private float partwayOffset = 0.5f;

        /// <summary>
        /// Distance along the center line the object will be located. 0.5 is halfway, 1.0 is at the second transform, 0.0 is at the first transform.
        /// </summary>
        public float PartwayOffset
        {
            get { return partwayOffset; }
            set { partwayOffset = Mathf.Clamp(value, 0.0f, 1.0f); }
        }

        [SerializeField]
        [Tooltip("Tracked object to calculate position and orientation for the second object. If you want to manually override and use a scene object, use the TransformTarget field.")]
        [HideInInspector]
        private TrackedObjectType trackedObjectForSecondTransform = TrackedObjectType.Head;

        /// <summary>
        /// Tracked object to calculate position and orientation for the second object. If you want to manually override and use a scene object, use the TransformTarget field.
        /// </summary>
        public TrackedObjectType TrackedObjectForSecondTransform
        {
            get { return trackedObjectForSecondTransform; }
            set
            {
                trackedObjectForSecondTransform = value;
                if (secondSolverHandler != null)
                {
                    secondSolverHandler.TrackedObjectToReference = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("This transform overrides any Tracked Object as the second point in the In Between.")]
        [HideInInspector]
        private Transform secondTransformOverride = null;

        private SolverHandler secondSolverHandler;

        protected override void OnValidate()
        {
            base.OnValidate();

            UpdateSecondSolverHandler();
        }

        protected void Start()
        {
            // We need to get the secondSolverHandler ready before we tell them both to seek a tracked object.
            secondSolverHandler = gameObject.AddComponent<SolverHandler>();
            secondSolverHandler.UpdateSolvers = false;
            UpdateSecondSolverHandler();
        }

        public override void SolverUpdate()
        {
            if (SolverHandler != null && secondSolverHandler != null)
            {
                if (SolverHandler.TransformTarget != null && secondSolverHandler.TransformTarget != null)
                {
                    AdjustPositionForOffset(SolverHandler.TransformTarget, secondSolverHandler.TransformTarget);
                }
            }
        }

        private void AdjustPositionForOffset(Transform targetTransform, Transform secondTransform)
        {
            if (targetTransform != null && secondTransform != null)
            {
                Vector3 centerline = targetTransform.position - secondTransform.position;
                GoalPosition = secondTransform.position + (centerline * partwayOffset);
                UpdateWorkingPositionToGoal();
            }
        }

        private void UpdateSecondSolverHandler()
        {
            if (secondSolverHandler != null)
            {
                if (secondTransformOverride != null)
                {
                    secondSolverHandler.TransformTarget = secondTransformOverride;
                }
                else
                {
                    secondSolverHandler.TrackedObjectToReference = trackedObjectForSecondTransform;
                }
            }
        }

        /// <summary>
        /// This should only be called from the SolverInBetweenEditor to cause the secondary SolverHandler to reattach this solver.
        /// </summary>
        public void AttachSecondTransformToNewTrackedObject()
        {
            UpdateSecondSolverHandler();
        }
    }
}