// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
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
        private float Speed = 2f;

        [Tooltip("If the angle between 'Gaze to Target' and 'Camera to Target' is less than this value, do nothing. This is to prevent small jittery rotations.")]
        [SerializeField]
        private float RotationThreshInDegrees = 3f;
        #endregion

        private GameObject targetToRotate = null;
        private GameObject objectWithCollider = null;
        private bool finished_returningToOrig = true;
        private bool finished_facingUser = false;
        private Vector3 origForwardNormalized = Vector3.zero;
        private bool turnToUser = false;

        private void Start()
        {
            InitialSetup();
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
                Collider coll;
                coll = GetComponent<Collider>();
                if (coll == null)
                {
                    coll = GetComponentInChildren<Collider>();
                }

                if (coll != null)
                {
                    objectWithCollider = GetComponentInChildren<Collider>().gameObject;
                }
            }

            // Let's remember the original orientation of the target to later return to this after a rotation.
            origForwardNormalized = targetToRotate.transform.forward.normalized;

            // Assuming that the game object is not moving in the scene graph, let's look up its full name once at the start
            // colliderFullName = Utils.General.GetFullName(ObjectWithCollider);
        }

        // Update is called once per frame
        public void Update()
        {
            // Update target rotation
            Vector3 TargetToCam = (CameraCache.Main.transform.position - targetToRotate.transform.position).normalized;
            Vector3 TargetForw = -targetToRotate.transform.forward.normalized;

            // If user looks at the game object, slowly turn towards the user
            if (turnToUser && (!finished_facingUser))
            {
                TurnToUser(TargetToCam, TargetForw);
            }
            // If user is not looking at the game object anymore, slowly return to original orientation
            else if ((!turnToUser) && (!finished_returningToOrig))
            {
                ReturnToOriginalRotation(TargetToCam, TargetForw);
            }
        }

        private void TurnToUser(Vector3 TargetToCam, Vector3 TargetForw)
        {
            // Checking whether to stop rotating once we get close enough to our final destination
            if (Mathf.Abs(Vector3.Angle(TargetForw, TargetToCam)) < RotationThreshInDegrees)
            {
                finished_facingUser = true;
                return;
            }

            // If we haven't reached our destination yet, let's continue rotating towards the user/camera
            Quaternion rotateTowardsCamera = Quaternion.LookRotation(targetToRotate.transform.position - CameraCache.Main.transform.position);
            targetToRotate.transform.rotation = Quaternion.Slerp(targetToRotate.transform.rotation, rotateTowardsCamera, Speed * Time.deltaTime);

            // Increase size
            targetToRotate.transform.localScale = targetToRotate.transform.localScale;

            finished_returningToOrig = false;
        }

        private void ReturnToOriginalRotation(Vector3 TargetToCam, Vector3 TargetForw)
        {
            // Checking whether to stop rotating once we get close enough to our original orientation
            if (Mathf.Abs(Vector3.Angle(TargetForw, origForwardNormalized) - 180) < RotationThreshInDegrees)
            {
                finished_returningToOrig = true;
                return;
            }

            // Otherwise let's continue rotating towards the original orientation
            Quaternion rotateBackToDefault = Quaternion.LookRotation(origForwardNormalized);
            targetToRotate.transform.rotation = Quaternion.Slerp(targetToRotate.transform.rotation, rotateBackToDefault, Speed * Time.deltaTime);
            finished_facingUser = false;
        }

        public void Engage()
        {
            turnToUser = true;
        }

        public void Disengage()
        {
            turnToUser = false;
        }
    }
}