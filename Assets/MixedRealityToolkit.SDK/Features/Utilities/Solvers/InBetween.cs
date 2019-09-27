// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// InBetween solver positions an object in-between two tracked transforms.
    /// </summary>
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
        [HideInInspector]
        [FormerlySerializedAs("trackedObjectForSecondTransform")]
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

        /// <summary>
        /// Tracked object to calculate position and orientation for the second object. If you want to manually override and use a scene object, use the TransformTarget field.
        /// </summary>
        [Obsolete("Use SecondTrackedObjectType property instead")]
        public TrackedObjectType TrackedObjectForSecondTransform
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
        [Tooltip("This transform overrides any Tracked Object as the second point for the In Between.")]
        [HideInInspector]
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

        protected void OnValidate()
        {
            UpdateSecondSolverHandler();
        }

        protected override void Start()
        {
            base.Start();

            // We need to get the secondSolverHandler ready before we tell them both to seek a tracked object.
            secondSolverHandler = gameObject.AddComponent<SolverHandler>();
            secondSolverHandler.UpdateSolvers = false;

            UpdateSecondSolverHandler();
        }

        /// <inheritdoc />
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
            }
        }

        private void UpdateSecondSolverHandler()
        {
            if (secondSolverHandler != null)
            {
                secondSolverHandler.TrackedTargetType = secondTrackedObjectType;

                if (secondTrackedObjectType == TrackedObjectType.CustomOverride && secondTransformOverride != null)
                {
                    secondSolverHandler.TransformOverride = secondTransformOverride;
                }
            }
        }
    }
}