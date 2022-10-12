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
    public class MaintainApparentSizeConstraint : TransformConstraint
    {
        #region Properties

        private float initialDist;

        public override TransformFlags ConstraintType => TransformFlags.Scale;

        #endregion Properties

        #region Public Methods

        /// <inheritdoc />
        public override void OnManipulationStarted(MixedRealityTransform worldPose)
        {
            base.OnManipulationStarted(worldPose);

            initialDist = (worldPose.Position - Camera.main.transform.position).magnitude;
        }

        /// <summary>
        /// Constrains scale such that the object's apparent size at manipulation
        /// start does not change when the object is moved towards and away from 
        /// the head.
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            float dist = (transform.Position - Camera.main.transform.position).magnitude;
            transform.Scale = (dist / initialDist) * WorldPoseOnManipulationStart.Scale;
        }

        #endregion Public Methods
    }
}
