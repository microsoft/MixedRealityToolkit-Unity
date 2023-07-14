// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Component for setting the min/max scale values for <see cref="ObjectManipulator"/>
    /// or <see cref="BoundsControl"/> components.
    /// </summary>
    /// <remarks>
    /// The constraint system might be reworked in the future. In such a case, these existing components will be deprecated.
    /// </remarks>
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

        /// <inheritdoc />
        public override TransformFlags ConstraintType => TransformFlags.Scale;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Clamps the transform scale to the scale limits set by <see cref="MinimumScale"/> and <see cref="MaximumScale"/>.
        /// </summary>
        /// <remarks> 
        /// No one component of the returned vector will be greater than the max scale, and
        /// mo one component of the returned vector will be less than the min scale.
        /// </remarks>
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