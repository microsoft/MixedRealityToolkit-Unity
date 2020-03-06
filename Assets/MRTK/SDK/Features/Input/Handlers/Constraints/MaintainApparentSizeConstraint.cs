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

        public override TransformFlags ConstraintType => TransformFlags.Scale;

        #endregion Properties

        #region Public Methods

        /// <inheritdoc />
        public override void Initialize(MixedRealityPose worldPose)
        {
            base.Initialize(worldPose);

            initialDist = (worldPose.Position - CameraCache.Main.transform.position).magnitude;
            initialScale = TargetTransform.localScale;
        }

        /// <summary>
        /// Constrains scale such that the object's apparent size at manipulation
        /// start does not change when the object is moved towards and away from 
        /// the head.
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            float dist = (transform.Position - CameraCache.Main.transform.position).magnitude;
            transform.Scale = (dist / initialDist) * initialScale;
        }

        #endregion Public Methods
    }
}