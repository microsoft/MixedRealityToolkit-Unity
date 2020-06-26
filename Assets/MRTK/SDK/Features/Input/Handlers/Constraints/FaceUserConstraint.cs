// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Component for fixing the rotation of a manipulated object such that
    /// it always faces or faces away from the user
    /// </summary>
    public class FaceUserConstraint : TransformConstraint
    {
        #region Properties

        [SerializeField]
        [Tooltip("Option to use this constraint to face away from the user")]
        private bool faceAway = false;

        /// <summary>
        /// If true, this will constrain rotation to face away from the user
        /// </summary>
        public bool FaceAway
        {
            get => faceAway;
            set => faceAway = value;
        }

        public override TransformFlags ConstraintType => TransformFlags.Rotate;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Updates an rotation so that its facing the camera
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            Vector3 directionToTarget = transform.Position - CameraCache.Main.transform.position;
            transform.Rotation = Quaternion.LookRotation(faceAway ? directionToTarget : -directionToTarget);
        }

        #endregion Public Methods
    }
}