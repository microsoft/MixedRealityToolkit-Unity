// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// The associated game object will turn depending on the user's 
    /// eye gaze: The currently looked at part will move towards the 
    /// front, facing the user.
    /// </summary>
    public class FaceUserEx : MonoBehaviour
    {
        #region Serialized variables
        [Tooltip("Rotation speed factor that will be multiplied with the delta time. Recommended values: 1 or 2.")]
        [SerializeField]
        private float Speed = 2f;

        [Tooltip("If the angle between 'Gaze to Target' and 'Camera to Target' is less than this value, do nothing. This is to prevent small jittery rotations.")]
        [SerializeField]
        private float RotationThreshInDegrees = 3f;
        #endregion

        private GameObject _targetToRotate = null;
        private GameObject _objectWithCollider = null;
        private bool _finishedReturningToOrig = true;
        private bool _finishedFacingUser = false;
        private bool _turnToUser = false;
        private Vector3 _origForwardNormalized = Vector3.zero;

        private void OnEnable()
        {
            Reset();
            InitialSetup();
            _turnToUser = true;
        }

        private void OnDisable()
        {
            _turnToUser = false;
        }

        private void Reset()
        {
            _targetToRotate = null;
            _objectWithCollider = null;
            _finishedReturningToOrig = true;
            _finishedFacingUser = false;
            _turnToUser = false;
            _origForwardNormalized = Vector3.zero;
        }

        /// <summary>
        /// Getting things set up. This includes making sure that relevant objects are defined and parameters are correctly set to start.
        /// </summary>
        private void InitialSetup()
        {
            // Make sure that the target to rotate is set
            if (_targetToRotate == null)
            {
                _targetToRotate = gameObject;
            }

            // Make also sure that the collider for hit tests is set 
            if (_objectWithCollider == null)
            {
                Collider coll;
                if (!TryGetComponent(out coll))
                {
                    coll = GetComponentInChildren<Collider>();
                }

                if (coll != null)
                {
                    _objectWithCollider = GetComponentInChildren<Collider>().gameObject;
                }
            }

            // Let's remember the original orientation of the target to later return to this after a rotation.
            _origForwardNormalized = _targetToRotate.transform.forward.normalized;
        }

        // Update is called once per frame
        public void Update()
        {
            // Update target rotation
            Vector3 TargetToCam = (Camera.main.transform.position - _targetToRotate.transform.position).normalized;
            Vector3 TargetForw = -_targetToRotate.transform.forward.normalized;

            // If user looks at the game object, slowly turn towards the user
            if (_turnToUser && (!_finishedFacingUser))
            {
                TurnToUser(TargetToCam, TargetForw);
            }
            // If user is not looking at the game object anymore, slowly return to original orientation
            else if ((!_turnToUser) && (!_finishedReturningToOrig))
            {
                ReturnToOriginalRotation(TargetForw);
            }
        }

        private void TurnToUser(Vector3 targetToCam, Vector3 targetForward)
        {
            // Checking whether to stop rotating once we get close enough to our final destination
            if (Mathf.Abs(Vector3.Angle(targetForward, targetToCam)) < RotationThreshInDegrees)
            {
                _finishedFacingUser = true;
                return;
            }

            // If we haven't reached our destination yet, let's continue rotating towards the user/camera
            Quaternion rotateTowardsCamera = Quaternion.LookRotation(_targetToRotate.transform.position - Camera.main.transform.position);
            _targetToRotate.transform.rotation = Quaternion.Slerp(_targetToRotate.transform.rotation, rotateTowardsCamera, Speed * Time.deltaTime);

            // Increase size
            _targetToRotate.transform.localScale = _targetToRotate.transform.localScale;

            _finishedReturningToOrig = false;
        }

        private void ReturnToOriginalRotation(Vector3 targetForward)
        {
            // Checking whether to stop rotating once we get close enough to our original orientation
            if (Mathf.Abs(Vector3.Angle(targetForward, _origForwardNormalized) - 180f) < RotationThreshInDegrees)
            {
                _finishedReturningToOrig = true;
                return;
            }

            // Otherwise let's continue rotating towards the original orientation
            Quaternion rotateBackToDefault = Quaternion.LookRotation(_origForwardNormalized);
            _targetToRotate.transform.rotation = Quaternion.Slerp(_targetToRotate.transform.rotation, rotateBackToDefault, Speed * Time.deltaTime);
            _finishedFacingUser = false;
        }
    }
}
