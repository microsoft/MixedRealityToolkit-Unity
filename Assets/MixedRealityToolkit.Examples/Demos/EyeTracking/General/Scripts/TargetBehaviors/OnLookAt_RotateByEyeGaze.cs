// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// The associated game object will turn depending on which part of the object is looked at: 
    /// The currently looked at part will move towards the front facing the user.
    /// </summary>
    [RequireComponent(typeof(EyeTrackingTarget))]
    public class OnLookAt_RotateByEyeGaze : BaseEyeFocusHandler
    {
        #region Serialized variables
        [Tooltip("Horizontal rotation speed.")]
        [SerializeField]
        private float speedX = 0.0f;

        [Tooltip("Vertical rotation speed.")]
        [SerializeField]
        private float speedY = 4.0f;

        [Tooltip("To inverse the horizontal rotation direction.")]
        [SerializeField]
        private bool inverseX = false;

        [Tooltip("To inverse the vertical rotation direction.")]
        [SerializeField]
        private bool inverseY = false;

        [Tooltip("If the angle between 'Gaze to Target' and 'Camera to Target' is less than this value, do nothing. This is to prevent small jittery rotations.")]
        [SerializeField]
        private float rotationThreshInDegrees = 5.0f;

        [Tooltip("Minimum horizontal rotation angle. This is to limit the rotation in different directions.")]
        [SerializeField]
        private float minRotX = -10.0f;

        [Tooltip("Maximum horizontal rotation angle. This is to limit the rotation in different directions.")]
        [SerializeField]
        private float maxRotX = 10.0f;

        [Tooltip("Minimal vertical rotation angle. This is to limit the rotation in different directions.")]
        [SerializeField]
        private float minRotY = -180.0f;

        [Tooltip("Maximum vertical rotation angle. This is to limit the rotation in different directions.")]
        [SerializeField]
        private float maxRotY = 180.0f;
        #endregion

        protected override void OnEyeFocusStay()
        {
            // Update target rotation
            RotateHitTarget();
        }

        private void RotateHitTarget()
        {
            Vector3 TargetToHit = (this.gameObject.transform.position - MixedRealityToolkit.InputSystem.EyeGazeProvider.HitPosition).normalized;
            Vector3 TargetToCam = (this.gameObject.transform.position - CameraCache.Main.transform.position).normalized;

            float angle1x, angle1y, angle1z, angle2x, angle2y;

            angle1x = Mathf.Atan2(TargetToHit.y, TargetToHit.z);
            angle1y = Mathf.Atan2(TargetToHit.x * Mathf.Cos(angle1x), TargetToHit.z);
            angle1z = Mathf.Atan2(Mathf.Cos(angle1x), Mathf.Sin(angle1x) * Mathf.Sin(angle1y));

            angle2x = Mathf.Atan2(TargetToCam.y, TargetToCam.z);
            angle2y = Mathf.Atan2(TargetToCam.x * Mathf.Cos(angle2x), TargetToCam.z);

            if ((angle1y > 0) && (angle1z < 0))
            {
                angle1y = angle1y - Mathf.PI;
            }
            else if ((angle1y < 0) && (angle1z < 0))
            {
                angle1y = angle1y + Mathf.PI;
            }

            float rotx, roty;
            rotx = angle1x - angle2x;
            roty = angle1y - angle2y;
            float newRotX = transform.eulerAngles.x, newRotY = transform.eulerAngles.y;

            // Restrict the rotation to a given angle range for x.
            if (Mathf.Abs(rotx) > (Mathf.Deg2Rad * rotationThreshInDegrees))
            {
                float stepx = speedX * ((inverseX) ? -1 : 1) * rotx;
                newRotX = ClampAngleInDegree(transform.eulerAngles.x + stepx, minRotX, maxRotX);
            }

            // Restrict the rotation to a given angle range for y.
            if (Mathf.Abs(roty) > (Mathf.Deg2Rad * rotationThreshInDegrees))
            {
                float stepy = speedY * ((inverseY) ? -1 : 1) * roty;
                newRotY = ClampAngleInDegree(transform.eulerAngles.y + stepy, minRotY, maxRotY);
            }

            // Assign the computed Euler angles.
            transform.eulerAngles = new Vector3(newRotX, newRotY, transform.eulerAngles.z);
        }

        /// <summary>
        /// Clamps angle within the range of a given min and max value and maps it to the range of -180 to +180.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="min"></param>
        /// <param name="maxAngleInDegree"></param>
        /// <returns></returns>
        private float ClampAngleInDegree(float angleInDegree, float minAngleInDegree, float maxAngleInDegree)
        {
            // Angle is not constricted
            if ((minAngleInDegree == -180f) && (maxAngleInDegree == 180f))
            {
                return angleInDegree;
            }

            // Wrap around angle to stay within [-180, 180] degrees
            if (angleInDegree > 180)
            {
                angleInDegree -= 360;
            }

            if (angleInDegree < -180)
            {
                angleInDegree += 360;
            }

            // Final checks on min and max range
            if (angleInDegree > maxAngleInDegree)
            {
                return maxAngleInDegree;
            }

            if (angleInDegree < minAngleInDegree)
            {
                return minAngleInDegree;
            }

            return angleInDegree;
        }
    }
}