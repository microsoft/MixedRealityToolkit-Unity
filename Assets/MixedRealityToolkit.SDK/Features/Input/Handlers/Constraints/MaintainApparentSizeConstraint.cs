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
    public class MaintainApparentSizeConstraint : TransformConstraint
    {
        #region Properties

        private float initialDist;
        private Vector3 initialScale;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Intended to be called on manipulation started
        /// </summary>
        public override void Initialize(MixedRealityPose worldPose)
        {
            base.Initialize(worldPose);

            initialDist = (worldPose.Position - CameraCache.Main.transform.position).magnitude;
            initialScale = TargetTransform.localScale;
        }

        /// <summary>
        /// Clamps the transform scale to the scale limits set by <see cref="SetScaleLimits"/> such that:
        /// - No one component of the returned vector will be greater than the max scale.
        /// - No one component of the returned vector will be less than the min scale.
        /// - The returned vector's direction will be the same as the given vector
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityPose pose, ref Vector3 scale)
        {
            float dist = (pose.Position - CameraCache.Main.transform.position).magnitude;
            scale = (dist / initialDist) * initialScale;
        }

        #endregion Public Methods
    }
}