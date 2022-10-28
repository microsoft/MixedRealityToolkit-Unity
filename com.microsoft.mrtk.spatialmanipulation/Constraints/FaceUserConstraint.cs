// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Component for fixing the rotation of a manipulated object such that
    /// it always faces or faces away from the user
    /// We're looking to rework this system in the future. These existing components will be deprecated then.
    /// </summary>
    [AddComponentMenu("MRTK/Spatial Manipulation/Face User Constraint")]
    public class FaceUserConstraint : TransformConstraint
    {
        #region Properties

        [SerializeField]
        [Tooltip("Option to use this constraint to face away from the user")]
        private bool faceAway = false;

        [SerializeField]
        [Tooltip("Option to gravity align this constraint")]
        private bool gravityAlign = false;

        /// <summary>
        /// If true, this will constrain rotation to face away from the user
        /// </summary>
        public bool FaceAway
        {
            get => faceAway;
            set => faceAway = value;
        }

        /// <summary>
        /// If true, this will gravity align the rotation
        /// </summary>
        public bool GravityAlign
        {
            get => gravityAlign;
            set => gravityAlign = value;
        }

        public override TransformFlags ConstraintType => TransformFlags.Rotate;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Updates an rotation so that its facing the camera
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            Vector3 directionToTarget = transform.Position - Camera.main.transform.position;
            directionToTarget = gravityAlign ? Vector3.ProjectOnPlane(directionToTarget, Vector3.up) : directionToTarget;
            transform.Rotation = Quaternion.LookRotation(faceAway ? directionToTarget : -directionToTarget);
        }

        #endregion Public Methods
    }
}
