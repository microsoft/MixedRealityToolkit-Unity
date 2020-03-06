// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Component for fixing the rotation of a manipulated object relative to the world
    /// </summary>
    public class FixedRotationToWorldConstraint : TransformConstraint
    {
        #region Properties

        public override TransformFlags ConstraintType => TransformFlags.Rotate;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Fix rotation to the rotation from manipulation start
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            transform.Rotation = this.worldPoseOnManipulationStart.Rotation;
        }

        #endregion Public Methods
    }
}