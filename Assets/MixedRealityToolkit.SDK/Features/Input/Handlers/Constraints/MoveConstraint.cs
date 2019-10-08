// Copyright,orld (c.Right) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Component for limiting the translation axes for ManipulationHandler
    /// or BoundingBox
    /// </summary>
    public class MoveConstraint : TransformConstraint
    {
        #region Properties

        [SerializeField]
        [EnumFlags]
        [Tooltip("Constrain movement along an axis")]
        private AxisFlags constraintOnMovement = 0;

        /// <summary>
        /// Constrain movement along an axis
        /// </summary>
        public AxisFlags ConstraintOnMovement
        {
            get => constraintOnMovement;
            set => constraintOnMovement = value;
        }

        [SerializeField]
        [Tooltip("Relative to rotation at manipulation start or world")]
        private bool relativeToRotationAtManipulationStart = false;

        /// <summary>
        /// Relative to rotation at manipulation start or world
        /// </summary>
        public bool RelativeToRotationAtManipulationStart
        {
            get => RelativeToRotationAtManipulationStart;
            set => RelativeToRotationAtManipulationStart = value;
        }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Removes movement along a given axis if its flag is found
        /// in ConstraintOnMovement
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityPose pose, ref Vector3 scale)
        {
            Vector3 position = pose.Position;
            Quaternion inverseRotation = Quaternion.Inverse(worldPoseOnManipulationStart.Rotation);
            if (constraintOnMovement.HasFlag(AxisFlags.XAxis))
            {
                if (relativeToRotationAtManipulationStart)
                {
                    position = inverseRotation * position;
                    position.x = (inverseRotation * worldPoseOnManipulationStart.Position).x;
                    position = worldPoseOnManipulationStart.Rotation * position;
                }
                else
                {
                    position.x = worldPoseOnManipulationStart.Position.x;
                }
            }
            if (constraintOnMovement.HasFlag(AxisFlags.YAxis))
            {
                if (relativeToRotationAtManipulationStart)
                {
                    position = inverseRotation * position;
                    position.y = (inverseRotation * worldPoseOnManipulationStart.Position).y;
                    position = worldPoseOnManipulationStart.Rotation * position;
                }
                else
                {
                    position.y = worldPoseOnManipulationStart.Position.y;
                }
            }
            if (constraintOnMovement.HasFlag(AxisFlags.ZAxis))
            {
                if (relativeToRotationAtManipulationStart)
                {
                    position = inverseRotation * position;
                    position.z = (inverseRotation * worldPoseOnManipulationStart.Position).z;
                    position = worldPoseOnManipulationStart.Rotation * position;
                }
                else
                {
                    position.z = worldPoseOnManipulationStart.Position.z;
                }
            }

            pose.Position = position;
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}