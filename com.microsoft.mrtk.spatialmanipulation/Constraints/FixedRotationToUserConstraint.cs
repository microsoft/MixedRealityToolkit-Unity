// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Component for fixing the rotation of a manipulated object relative to the user.
    /// </summary>
    /// <remarks>
    /// MRTK's constraint system might be redesigned in the near future. When
    /// this occurs, the old constraint components will be deprecated.
    /// </remarks>
    public class FixedRotationToUserConstraint : TransformConstraint
    {
        #region Properties

        /// <inheritdoc />
        public override TransformFlags ConstraintType => TransformFlags.Rotate;

        private Quaternion startObjectRotationCameraSpace;

        #endregion Properties

        #region Public Methods

        /// <inheritdoc />
        public override void OnManipulationStarted(MixedRealityTransform worldPose)
        {
            base.OnManipulationStarted(worldPose);

            startObjectRotationCameraSpace = Quaternion.Inverse(Camera.main.transform.rotation) * worldPose.Rotation;
        }

        /// <summary>
        /// Updates the objects rotation so that the rotation relative to the user
        /// is fixed
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            Vector3 euler = Camera.main.transform.rotation.eulerAngles;
            // don't use roll (feels awkward) - just maintain yaw / pitch angle
            transform.Rotation = Quaternion.Euler(euler.x, euler.y, 0) * startObjectRotationCameraSpace;
        }

        #endregion Public Methods
    }
}