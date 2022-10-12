// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Component for setting the min/max scale values for ObjectManipulator
    /// or BoundsControl
    /// We're looking to rework this system in the future. These existing components will be deprecated then.
    /// </summary>
    public class FixedDistanceConstraint : TransformConstraint
    {
        #region Properties

        [SerializeField]
        [Tooltip("Transform to fix distance to. Defaults to the head.")]
        private Transform constraintTransform = null;

        /// <summary>
        /// Transform to fix distance to. Defaults to the head.
        /// </summary>
        public Transform ConstraintTransform
        {
            get => constraintTransform;
            set => constraintTransform = value;
        }

        public override TransformFlags ConstraintType => TransformFlags.Move;

        private float distanceAtManipulationStart;

        #endregion Properties

        #region MonoBehaviour Methods

        public void Start()
        {
            EnsureConstraintTransform();
        }

        #endregion MonoBehaviour Methods

        #region Public Methods

        /// <inheritdoc />
        public override void OnManipulationStarted(MixedRealityTransform worldPose)
        {
            base.OnManipulationStarted(worldPose);
            EnsureConstraintTransform();
            distanceAtManipulationStart = Vector3.Distance(worldPose.Position, constraintTransform.position);
        }

        /// <summary>
        /// Constrains position such that the distance between pose and
        /// the ConstraintTransform does not change from manipulation start
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            Vector3 constraintToPose = transform.Position - constraintTransform.position;
            constraintToPose = constraintToPose.normalized * distanceAtManipulationStart;
            transform.Position = constraintTransform.position + constraintToPose;
        }

        #endregion Public Methods

        private void EnsureConstraintTransform()
        {
            if (ConstraintTransform == null)
            {
                ConstraintTransform = Camera.main.transform;
            }
        }
    }
}
