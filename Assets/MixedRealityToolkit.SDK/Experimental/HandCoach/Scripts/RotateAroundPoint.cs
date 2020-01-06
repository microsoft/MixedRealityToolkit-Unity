using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class RotateAroundPoint : MonoBehaviour
    {
        [Tooltip("Parent object centered at rotation center")]
        [SerializeField]
        private Transform m_centeredParent = null;

        [Tooltip("Hand hint parent to rotate inverse to centeredParent")]
        [SerializeField]
        private Transform m_inverseParent = null;

        [Tooltip("Point to start movement at")]
        [SerializeField]
        private Transform m_pivotPosition = null;

        [Tooltip("Duration of move from tracking object to target object")]
        [SerializeField]
        private float m_duration = 1.38f;

        [Tooltip("Lerp curve")]
        [SerializeField]
        private AnimationCurve m_animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("How many degrees to rotate")]
        [SerializeField]
        private float m_degreesToMove = 0f;

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

        public void RotateToTarget()
        {
            StartCoroutine(RotateHintSequence());
        }

        private IEnumerator ResetAndDeterminePivot()
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
            Vector3 targetOrigin = centeredOrigin + new Vector3(0, m_degreesToMove, 0);

            //Vector3 hintStart = m_boudingBox.transform.localEulerAngles;

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