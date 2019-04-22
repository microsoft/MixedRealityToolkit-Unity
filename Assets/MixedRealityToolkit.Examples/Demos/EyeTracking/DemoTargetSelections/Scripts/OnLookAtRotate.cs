// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// The associated GameObject will rotate when being looked at based on a given direction.
    /// </summary>
    [RequireComponent(typeof(EyeTrackingTarget))]
    public class OnLookAtRotate : BaseEyeFocusHandler
    {
        #region Serialized variables

        [Tooltip("Euler angles by which the object should be rotated by.")]
        [SerializeField]
        private Vector3 RotateByEulerAngles = Vector3.zero;

        [Tooltip("Rotation speed factor.")]
        [SerializeField]
        private float speed = 1f;

        #endregion

        protected override void OnEyeFocusStay()
        {
            // Update target rotation
            RotateHitTarget();
        }

        /// <summary>
        /// Rotate game object based on specified rotation speed and Euler angles.
        /// </summary>
        private void RotateHitTarget()
        {
            transform.eulerAngles = transform.eulerAngles + RotateByEulerAngles * speed;
        }
    }
}