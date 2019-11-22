// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Component for limiting the rotation of a manipulated object relative to the user
    /// or BoundingBox
    /// </summary>
    public class FixedRotationToUserConstraint : TransformConstraint
    {
        #region Properties

        public override TransformFlags ConstraintType => TransformFlags.Rotate;

        private Quaternion startObjectRotationCameraSpace;

        #endregion Properties

        #region Public Methods

        /// <inheritdoc />
        public override void Initialize(MixedRealityPose worldPose)
        {
            base.Initialize(worldPose);

            startObjectRotationCameraSpace = Quaternion.Inverse(CameraCache.Main.transform.rotation) * worldPose.Rotation;
        }

        /// <summary>
        /// Removes rotation about given axis if its flag is found
        /// in ConstraintOnRotation
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            Vector3 euler = CameraCache.Main.transform.rotation.eulerAngles;
            // don't use roll (feels awkward) - just maintain yaw / pitch angle
            transform.Rotation = Quaternion.Euler(euler.x, euler.y, 0) * startObjectRotationCameraSpace;
        }

        #endregion Public Methods
    }
}