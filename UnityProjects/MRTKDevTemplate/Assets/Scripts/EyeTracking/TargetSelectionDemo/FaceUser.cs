// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    using UnityEngine.Serialization;

    /// <summary>
    /// The associated game object will turn depending on the user's 
    /// eye gaze: The currently looked at part will move towards the 
    /// front, facing the user.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/FaceUser")]
    public class FaceUser : MonoBehaviour
    {
        #region Serialized variables
        [Tooltip("Rotation speed factor that will be multiplied with the delta time. Recommended values: 1 or 2.")]
        [SerializeField]
        private float speed = 2f;

        [Tooltip("If the angle between 'Gaze to Target' and 'Camera to Target' is less than this value, do nothing. This is to prevent small jittery rotations.")]
        [SerializeField]
        [Min(0f)]
        private float rotationThresholdInDegrees = 3f;
        #endregion

        private GameObject targetToRotate = null;
        private GameObject objectWithCollider = null;
        private bool finishedReturningToOrigal = true;
        private bool finishedFacingUser = false;
        private bool turnToUser = false;
        private Vector3 originalForwardNormalized = Vector3.zero;

        private void OnEnable()
        {
            Reset();
            InitialSetup();
            turnToUser = true;
        }

        private void OnDisable()
        {
            turnToUser = false;
        }

        private void Reset()
        {
            targetToRotate = null;
            objectWithCollider = null;
            finishedReturningToOrigal = true;
            finishedFacingUser = false;
            turnToUser = false;
            originalForwardNormalized = Vector3.zero;
        }

        /// <summary>
        /// Getting things set up. This includes making sure that relevant objects are defined and parameters are correctly set to start.
        /// </summary>
        private void InitialSetup()
        {
            // Make sure that the target to rotate is set
            if (targetToRotate == null)
            {
                targetToRotate = gameObject;
            }

            // Make also sure that the collider for hit tests is set 
            if (objectWithCollider == null)
            {
                if (!TryGetComponent(out Collider coll))
                {
                    coll = GetComponentInChildren<Collider>();
                }

                if (coll != null)
                {
                    objectWithCollider = GetComponentInChildren<Collider>().gameObject;
                }
            }

            // Let's remember the original orientation of the target to later return to this after a rotation.
            originalForwardNormalized = targetToRotate.transform.forward.normalized;
        }

        private void Update()
        {
            // Update target rotation
            Vector3 TargetToCamera = (Camera.main.transform.position - targetToRotate.transform.position).normalized;
            Vector3 targetForward = -targetToRotate.transform.forward.normalized;

            // If user looks at the game object, slowly turn towards the user
            if (turnToUser && !finishedFacingUser)
            {
                TurnToUser(TargetToCamera, targetForward);
            }
            // If user is not looking at the game object anymore, slowly return to original orientation
            else if (!turnToUser && !finishedReturningToOrigal)
            {
                ReturnToOriginalRotation(targetForward);
            }
        }

        private void TurnToUser(Vector3 targetToCam, Vector3 targetForward)
        {
            // Checking whether to stop rotating once we get close enough to our final destination
            if (Mathf.Abs(Vector3.Angle(targetForward, targetToCam)) < rotationThresholdInDegrees)
            {
                finishedFacingUser = true;
                return;
            }

            // If we haven't reached our destination yet, let's continue rotating towards the user/camera
            Quaternion rotateTowardsCamera = Quaternion.LookRotation(targetToRotate.transform.position - Camera.main.transform.position);
            targetToRotate.transform.rotation = Quaternion.Slerp(targetToRotate.transform.rotation, rotateTowardsCamera, speed * Time.deltaTime);

            // Increase size
            targetToRotate.transform.localScale = targetToRotate.transform.localScale;

            finishedReturningToOrigal = false;
        }

        private void ReturnToOriginalRotation(Vector3 targetForward)
        {
            // Checking whether to stop rotating once we get close enough to our original orientation
            if (Mathf.Abs(Vector3.Angle(targetForward, originalForwardNormalized) - 180f) < rotationThresholdInDegrees)
            {
                finishedReturningToOrigal = true;
                return;
            }

            // Otherwise let's continue rotating towards the original orientation
            Quaternion rotateBackToDefault = Quaternion.LookRotation(originalForwardNormalized);
            targetToRotate.transform.rotation = Quaternion.Slerp(targetToRotate.transform.rotation, rotateBackToDefault, speed * Time.deltaTime);
            finishedFacingUser = false;
        }
    }
}
