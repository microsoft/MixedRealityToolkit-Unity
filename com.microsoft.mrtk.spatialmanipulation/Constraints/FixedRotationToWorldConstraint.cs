// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Component for fixing the rotation of a manipulated object relative to the world.
    /// </summary>
    /// <remarks>
    /// MRTK's constraint system might be redesigned in the near future. When
    /// this occurs, the old constraint components will be deprecated.
    /// </remarks>
    public class FixedRotationToWorldConstraint : TransformConstraint
    {
        #region Properties

        /// <inheritdoc />
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