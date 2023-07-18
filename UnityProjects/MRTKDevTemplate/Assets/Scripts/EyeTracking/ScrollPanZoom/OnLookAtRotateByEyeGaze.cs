// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Examples
{
    using Input;
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    /// <summary>
    /// The associated game object will turn depending on which part of the object is looked at: 
    /// The currently looked at part will move towards the front facing the user.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/OnLookAtRotateByEyeGaze")]
    public class OnLookAtRotateByEyeGaze : StatefulInteractable
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
        private float rotationThresholdInDegrees = 5.0f;

        [Tooltip("Minimum horizontal rotation angle. This is to limit the rotation in different directions.")]
        [SerializeField]
        private float minRotationX = -10.0f;

        [Tooltip("Maximum horizontal rotation angle. This is to limit the rotation in different directions.")]
        [SerializeField]
        private float maxRotationX = 10.0f;

        [Tooltip("Minimal vertical rotation angle. This is to limit the rotation in different directions.")]
        [SerializeField]
        private float minRotationY = -180.0f;

        [Tooltip("Maximum vertical rotation angle. This is to limit the rotation in different directions.")]
        [SerializeField]
        private float maxRotationY = 180.0f;
        #endregion

        /// <summary>
        /// Updates the rotation of the GameObject based on the current eye gaze position.
        /// </summary>
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {   
            // Dynamic is effectively just your normal Update().
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                foreach (var interactor in interactorsHovering)
                {
                    if (interactor is FuzzyGazeInteractor gaze)
                    {
                        Vector3 targetToHit = (gameObject.transform.position - gaze.PreciseHitResult.raycastHit.point).normalized;
                        Vector3 targetToCamera = (gameObject.transform.position - Camera.main.transform.position).normalized;

                        float angle1x = Mathf.Atan2(targetToHit.y, targetToHit.z);
                        float angle1y = Mathf.Atan2(targetToHit.x * Mathf.Cos(angle1x), targetToHit.z);
                        float angle1z = Mathf.Atan2(Mathf.Cos(angle1x), Mathf.Sin(angle1x) * Mathf.Sin(angle1y));

                        float angle2x = Mathf.Atan2(targetToCamera.y, targetToCamera.z);
                        float angle2y = Mathf.Atan2(targetToCamera.x * Mathf.Cos(angle2x), targetToCamera.z);

                        if (angle1y > 0f && angle1z < 0f)
                        {
                            angle1y -= Mathf.PI;
                        }
                        else if (angle1y < 0f && angle1z < 0f)
                        {
                            angle1y += Mathf.PI;
                        }

                        float rotationX = angle1x - angle2x;
                        float rotationY = angle1y - angle2y;
                        float newRotationX = transform.eulerAngles.x, newRotY = transform.eulerAngles.y;

                        // Restrict the rotation to a given angle range for x.
                        if (Mathf.Abs(rotationX) > (Mathf.Deg2Rad * rotationThresholdInDegrees))
                        {
                            float stepx = speedX * (inverseX ? -1f : 1f) * rotationX;
                            newRotationX = ClampAngleInDegree(transform.eulerAngles.x + stepx, minRotationX, maxRotationX);
                        }

                        // Restrict the rotation to a given angle range for y.
                        if (Mathf.Abs(rotationY) > (Mathf.Deg2Rad * rotationThresholdInDegrees))
                        {
                            float stepy = speedY * (inverseY ? -1f : 1f) * rotationY;
                            newRotY = ClampAngleInDegree(transform.eulerAngles.y + stepy, minRotationY, maxRotationY);
                        }

                        // Assign the computed Euler angles.
                        transform.eulerAngles = new Vector3(newRotationX, newRotY, transform.eulerAngles.z);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Clamps angle within the range of a given min and max value and maps it to the range of -180 to +180.
        /// </summary>
        private static float ClampAngleInDegree(float angleInDegree, float minAngleInDegree, float maxAngleInDegree)
        {
            // Angle is not constricted
            if (Mathf.Approximately(minAngleInDegree, -180f) && Mathf.Approximately(maxAngleInDegree, 180f)) 
            {
                return angleInDegree;
            }

            // Wrap around angle to stay within [-180, 180] degrees
            if (angleInDegree > 180f)
            {
                angleInDegree -= 360f;
            }

            if (angleInDegree < -180f)
            {
                angleInDegree += 360f;
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
