using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.HandCoach
{
    /// <summary>
    /// This class provides functionality to rotate the hand hint around a pivot point over time.
    /// </summary>
    public class RotateAroundPoint : MonoBehaviour
    {
        [Tooltip("Parent object centered at rotation center.")]
        [SerializeField]
        private Transform centeredParent = null;

        /// <summary>
        /// Parent object centered at rotation center.
        /// </summary>
        public Transform CenteredParent
        {
            get
            {
                return centeredParent;
            }
            set
            {
                centeredParent = value;
            }
        }

        [Tooltip("Hand hint parent to rotate inverse to centeredParent to keep hand orientation the same.")]
        [SerializeField]
        private Transform inverseParent = null;

        /// <summary>
        /// Hand hint parent to rotate inverse to centeredParent to keep hand orientation the same.
        /// </summary>
        public Transform InverseParent
        {
            get
            {
                return inverseParent;
            }
            set
            {
                inverseParent = value;
            }
        }

        [Tooltip("Point to start movement at.")]
        [SerializeField]
        private Transform pivotPosition = null;

        /// <summary>
        /// Point to start movement at.
        /// </summary>
        public Transform PivotPosition
        {
            get
            {
                return pivotPosition;
            }
            set
            {
                pivotPosition = value;
            }
        }

        [Tooltip("Duration of rotation around the CenteredParent in seconds.")]
        [SerializeField]
        private float duration = 1.38f;

        /// <summary>
        /// Duration of rotation around the CenteredParent in seconds.
        /// </summary>
        public float Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
            }
        }

        [Tooltip("Lerp curve that controls the animation rotation over time.")]
        [SerializeField]
        private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        /// <summary>
        /// Lerp curve that controls the animation rotation over time.
        /// </summary>
        public AnimationCurve AnimationCurve
        {
            get
            {
                return animationCurve;
            }
            set
            {
                animationCurve = value;
            }
        }

        [Tooltip("How many degrees to rotate along each axis.")]
        [SerializeField]
        private Vector3 rotationVector = new Vector3(0f, 90f, 0f);

        /// <summary>
        /// How many degrees to rotate along each axis.
        /// </summary>
        public Vector3 RotationVector
        {
            get
            {
                return rotationVector;
            }
            set
            {
                rotationVector = value;
            }
        }

        private Vector3 centeredParentRotate;

        private Vector3 hintPosition;

        // used to allow position changing in LateUpdate() and position correction in RotateHintSequence()
        private bool updatePosition = true;

        // since this script can attach to an object with an animator, we need to update position in LateUpdate
        public void LateUpdate()
        {
            // set inverseParent position based on DeterminePivot()
            if (updatePosition)
            {
                InverseParent.localPosition = hintPosition;
            }

            // set centeredParent rotation based on RotateToTarget()
            CenteredParent.localEulerAngles = centeredParentRotate;

            // set inverseParent rotation based on RotateToTarget() and parent object
            InverseParent.localEulerAngles = -centeredParentRotate;
        }

        /// <summary>
        /// Start the rotation sequence.
        /// </summary>
        public void RotateToTarget()
        {
            StartCoroutine(RotateHintSequence());
        }

        /// <summary>
        /// Reset position to the PivotPosition.
        /// </summary>
        public IEnumerator ResetAndDeterminePivot()
        {
            // reset rotation and wait a frame to ensure the centeredParent is set in LateUpdate
            centeredParentRotate = new Vector3(0f, 0f, 0f);
            yield return null;

            int index = 0;

            updatePosition = true;
            StartCoroutine(TrackPositionSequence(index));
        }

        // tracks the pivot determined above
        private IEnumerator TrackPositionSequence(int index)
        {
            while (updatePosition)
            {
                hintPosition = PivotPosition.localPosition;
                yield return null;
            }
        }

        // lerp from current position to target position
        private IEnumerator RotateHintSequence()
        {
            updatePosition = false;
            Vector3 centeredOrigin = CenteredParent.localEulerAngles;
            Vector3 targetOrigin = centeredOrigin + RotationVector;

            float t = 0;
            while (t <= Duration)
            {
                centeredParentRotate = Vector3.Lerp(centeredOrigin, targetOrigin, AnimationCurve.Evaluate(t / duration));
                t += Time.deltaTime;
                yield return null;
            }

            // after we reach the target, we still want to be stabilized
            while (!updatePosition)
            {
                centeredParentRotate = targetOrigin;
                yield return null;
            }
        }
    }
}