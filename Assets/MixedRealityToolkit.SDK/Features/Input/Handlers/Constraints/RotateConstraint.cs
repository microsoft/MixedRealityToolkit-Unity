// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Component for limiting the rotation axes for ManipulationHandler
    /// or BoundingBox
    /// </summary>
    public class RotateConstraint : TransformConstraint
    {
        #region Properties

        [SerializeField]
        [EnumFlags]
        [Tooltip("Constrain rotation about an axis")]
        private AxisFlags constraintOnRotation = 0;

        /// <summary>
        /// Constrain rotation about an axis
        /// </summary>
        public AxisFlags ConstraintOnRotation
        {
            get => constraintOnRotation;
            set => constraintOnRotation = value;
        }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Removes rotation about given axis if its flag is found
        /// in ConstraintOnRotation
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityPose pose, ref Vector3 scale)
        {
            Quaternion rotation = pose.Rotation * Quaternion.Inverse(worldPoseOnManipulationStart.Rotation);
            Vector3 eulers = rotation.eulerAngles;
            if (constraintOnRotation.HasFlag(AxisFlags.XAxis))
            {
                eulers.x = 0;
            }
            if (constraintOnRotation.HasFlag(AxisFlags.YAxis))
            {
                eulers.y = 0;
            }
            if (constraintOnRotation.HasFlag(AxisFlags.ZAxis))
            {
                eulers.z = 0;
            }
            pose.Rotation = Quaternion.Euler(eulers) * worldPoseOnManipulationStart.Rotation;
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}