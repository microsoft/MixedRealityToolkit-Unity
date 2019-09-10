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

        public Transform ConstraintTransform
        {
            get => constraintTransform;
            set => constraintTransform = value;
        }

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

        public override void Initialize(MixedRealityPose worldPose)
        {
            base.Initialize(worldPose);

            distanceAtManipulationStart = Vector3.Distance(worldPose.Position, constraintTransform.position);
        }

        /// <summary>
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityPose pose, ref Vector3 scale)
        {
            Vector3 constraintToPose = pose.Position - constraintTransform.position;
            constraintToPose = constraintToPose.normalized * distanceAtManipulationStart;
            pose.Position = constraintTransform.position + constraintToPose;
        }

        #endregion Public Methods
    }
}