// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Component for setting the min/max scale values for ManipulationHandler
    /// or BoundingBox
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

        public override void Start()
        {
            base.Start();

            if (ConstraintTransform == null)
            {
                ConstraintTransform = CameraCache.Main.transform;
            }
        }

        #endregion MonoBehaviour Methods

        #region Public Methods

        /// <inheritdoc />
        public override void Initialize(MixedRealityPose worldPose)
        {
            base.Initialize(worldPose);

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
    }
}