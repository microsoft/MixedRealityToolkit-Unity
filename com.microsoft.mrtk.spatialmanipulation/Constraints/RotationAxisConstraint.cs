// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Component for limiting the rotation axes for ObjectManipulator
    /// or BoundsControl
    /// We're looking to rework this system in the future. These existing components will be deprecated then.
    /// </summary>
    public class RotationAxisConstraint : TransformConstraint
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

        [SerializeField]
        [Tooltip("Check if object rotation should be in local space of object being manipulated instead of world space.")]
        private bool useLocalSpaceForConstraint = false;

        /// <summary>
        /// Gets or sets whether the constraints should be applied in local space of the object being manipulated or world space.
        /// </summary>
        public bool UseLocalSpaceForConstraint
        {
            get => useLocalSpaceForConstraint;
            set => useLocalSpaceForConstraint = value;
        }

        public override TransformFlags ConstraintType => TransformFlags.Rotate;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Removes rotation about given axis if its flag is found
        /// in ConstraintOnRotation
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            Quaternion rotation = transform.Rotation * Quaternion.Inverse(WorldPoseOnManipulationStart.Rotation);
            Vector3 eulers = rotation.eulerAngles;
            if (constraintOnRotation.IsMaskSet(AxisFlags.XAxis))
            {
                eulers.x = 0;
            }
            if (constraintOnRotation.IsMaskSet(AxisFlags.YAxis))
            {
                eulers.y = 0;
            }
            if (constraintOnRotation.IsMaskSet(AxisFlags.ZAxis))
            {
                eulers.z = 0;
            }

            transform.Rotation = useLocalSpaceForConstraint
                ? WorldPoseOnManipulationStart.Rotation * Quaternion.Euler(eulers)
                : Quaternion.Euler(eulers) * WorldPoseOnManipulationStart.Rotation;
        }

        #endregion Public Methods
    }
}