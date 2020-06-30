// Copyright,orld (c.Right) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Component for limiting the translation axes for ObjectManipulator
    /// or BoundsControl
    /// </summary>
    public class MoveAxisConstraint : TransformConstraint
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
        private bool useLocalSpaceForConstraint = false;

        /// <summary>
        /// Relative to rotation at manipulation start or world
        /// </summary>
        public bool UseLocalSpaceForConstraint
        {
            get => useLocalSpaceForConstraint;
            set => useLocalSpaceForConstraint = value;
        }

        public override TransformFlags ConstraintType => TransformFlags.Move;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Removes movement along a given axis if its flag is found
        /// in ConstraintOnMovement
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            Quaternion inverseRotation = Quaternion.Inverse(worldPoseOnManipulationStart.Rotation);
            Vector3 position = transform.Position;
            if (constraintOnMovement.HasFlag(AxisFlags.XAxis))
            {
                if (useLocalSpaceForConstraint)
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
                if (useLocalSpaceForConstraint)
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
                if (useLocalSpaceForConstraint)
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
            transform.Position = position;
        }

        #endregion Public Methods
    }
}