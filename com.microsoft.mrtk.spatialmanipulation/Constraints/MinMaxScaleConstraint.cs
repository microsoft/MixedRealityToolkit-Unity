// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Component for setting the min/max scale values for ObjectManipulator
    /// or BoundsControl
    /// We're looking to rework this system in the future. These existing components will be deprecated then.
    /// </summary>
    [AddComponentMenu("MRTK/Spatial Manipulation/Min Max Scale Constraint")]
    public class MinMaxScaleConstraint : TransformConstraint
    {
        #region Properties

        [SerializeField]
        [Tooltip("Minimum scaling allowed")]
        private Vector3 minimumScale = Vector3.one * 0.2f;

        /// <summary>
        /// Minimum scale allowed
        /// </summary>
        public Vector3 MinimumScale
        {
            get => minimumScale;
            set => minimumScale = value;
        }

        [SerializeField]
        [Tooltip("Maximum scaling allowed")]
        private Vector3 maximumScale = Vector3.one * 2f;

        /// <summary>
        /// Maximum scale allowed
        /// </summary>
        public Vector3 MaximumScale
        {
            get => maximumScale;
            set => maximumScale = value;
        }

        [SerializeField]
        [Tooltip("Min/max scaling relative to initial scale if true")]
        private bool relativeToInitialState = true;

        /// <summary>
        /// Min/max scaling relative to initial scale if true
        /// </summary>
        public bool RelativeToInitialState
        {
            get => relativeToInitialState;
            set => relativeToInitialState = value;
        }

        public override TransformFlags ConstraintType => TransformFlags.Scale;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Clamps the transform scale to the scale limits set by <see cref="SetScaleLimits"/> such that:
        /// - No one component of the returned vector will be greater than the max scale.
        /// - No one component of the returned vector will be less than the min scale.
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            Vector3 min = minimumScale;
            Vector3 max = maximumScale;

            if (relativeToInitialState)
            {
                min = InitialWorldPose.Scale.Mul(min);
                max = InitialWorldPose.Scale.Mul(max);
            }

            transform.Scale = Vector3.Max(min, Vector3.Min(max, transform.Scale));
        }

        #endregion Public Methods
    }
}