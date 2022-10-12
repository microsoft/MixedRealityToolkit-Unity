// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Component for fixing the rotation of a manipulated object relative to the world
    /// We're looking to rework this system in the future. These existing components will be deprecated then.
    /// </summary>
    public class FixedRotationToWorldConstraint : TransformConstraint
    {
        #region Properties

        public override TransformFlags ConstraintType => TransformFlags.Rotate;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Fix rotation to the rotation from manipulation start
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            transform.Rotation = this.WorldPoseOnManipulationStart.Rotation;
        }

        #endregion Public Methods
    }
}