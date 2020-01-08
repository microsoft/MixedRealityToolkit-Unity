using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// This class provides functionality to rotate the hand hint around a pivot point over time.
    /// </summary>
    public class RotateAroundPoint : MonoBehaviour
    {
        /// <summary>
        /// Parent object centered at rotation center.
        /// </summary>
        [SerializeField]
        private Transform m_centeredParent = null;

        /// <summary>
        /// Hand hint parent to rotate inverse to centeredParent to keep hand orientation the same.
        /// </summary>
        [SerializeField]
        private Transform m_inverseParent = null;

        /// <summary>
        /// Point to start movement at.
        /// </summary>
        [SerializeField]
        private Transform m_pivotPosition = null;

        /// <summary>
        /// Duration of rotation around the CenteredParent.
        /// </summary>
        [SerializeField]
        private float m_duration = 1.38f;

        /// <summary>
        /// Lerp curve.
        /// </summary>
        [SerializeField]
        private AnimationCurve m_animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        /// <summary>
        /// How many degrees to rotate along each axis.
        /// </summary>
        public Vector3 RotationVector = new Vector3(0f, 90f, 0f);

        private Vector3 m_centeredParentRotate;

        private Vector3 m_hintPosition;

        // used to allow position changing in LateUpdate() and position correction in RotateHintSequence()
        private bool m_updatePosition = true;

        // since this script can attach to an object with an animator, we need to update position in LateUpdate
        public void LateUpdate()
        {
            // set inverseParent position based on DeterminePivot()
            if (m_updatePosition)
            {
                m_inverseParent.localPosition = m_hintPosition;
            }

            // set centeredParent rotation based on RotateToTarget()
            m_centeredParent.localEulerAngles = m_centeredParentRotate;

            // set inverseParent rotation based on RotateToTarget() and parent object
            m_inverseParent.localEulerAngles = -m_centeredParentRotate;
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
            // reset rotation and wait a frame to ensure the m_centeredParent is set in LateUpdate
            m_centeredParentRotate = new Vector3(0f, 0f, 0f);
            yield return null;

            int index = 0;

            m_updatePosition = true;
            StartCoroutine(TrackPositionSequence(index));
        }

        // tracks the pivot determined above
        private IEnumerator TrackPositionSequence(int index)
        {
            while (m_updatePosition)
            {
                m_hintPosition = m_pivotPosition.localPosition;
                yield return null;
            }
        }

        // lerp from current position to target position
        private IEnumerator RotateHintSequence()
        {
            m_updatePosition = false;
            Vector3 centeredOrigin = m_centeredParent.localEulerAngles;
            Vector3 targetOrigin = centeredOrigin + RotationVector;

            float t = 0;
            while (t <= m_duration)
            {
                m_centeredParentRotate = Vector3.Lerp(centeredOrigin, targetOrigin, m_animationCurve.Evaluate(t / m_duration));
                t += Time.deltaTime;
                yield return null;
            }

            // after we reach the target, we still want to be stabilized
            while (!m_updatePosition)
            {
                m_centeredParentRotate = targetOrigin;
                yield return null;
            }
        }
    }
}