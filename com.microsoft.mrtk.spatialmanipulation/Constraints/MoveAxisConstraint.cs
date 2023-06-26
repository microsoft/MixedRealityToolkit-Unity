// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Component for limiting the translation axes for ObjectManipulator
    /// or BoundsControl.
    /// </summary>
    /// <remarks>
    /// MRTK's constraint system might be redesigned in the near future. When
    /// this occurs, the old constraint components will be deprecated.
    /// </remarks>
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

        /// <inheritdoc />
        public override TransformFlags ConstraintType => TransformFlags.Move;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Removes movement along a given axis if its flag is found
        /// in ConstraintOnMovement
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            Quaternion inverseRotation = Quaternion.Inverse(WorldPoseOnManipulationStart.Rotation);
            Vector3 position = transform.Position;
            if (constraintOnMovement.IsMaskSet(AxisFlags.XAxis))
            {
                if (useLocalSpaceForConstraint)
                {
                    position = inverseRotation * position;
                    position.x = (inverseRotation * WorldPoseOnManipulationStart.Position).x;
                    position = WorldPoseOnManipulationStart.Rotation * position;
                }
                else
                {
                    position.x = WorldPoseOnManipulationStart.Position.x;
                }
            }
            if (constraintOnMovement.IsMaskSet(AxisFlags.YAxis))
            {
                if (useLocalSpaceForConstraint)
                {
                    position = inverseRotation * position;
                    position.y = (inverseRotation * WorldPoseOnManipulationStart.Position).y;
                    position = WorldPoseOnManipulationStart.Rotation * position;
                }
                else
                {
                    position.y = WorldPoseOnManipulationStart.Position.y;
                }
            }
            if (constraintOnMovement.IsMaskSet(AxisFlags.ZAxis))
            {
                if (useLocalSpaceForConstraint)
                {
                    position = inverseRotation * position;
                    position.z = (inverseRotation * WorldPoseOnManipulationStart.Position).z;
                    position = WorldPoseOnManipulationStart.Rotation * position;
                }
                else
                {
                    position.z = WorldPoseOnManipulationStart.Position.z;
                }
            }
            transform.Position = position;
        }

        #endregion Public Methods
    }
}