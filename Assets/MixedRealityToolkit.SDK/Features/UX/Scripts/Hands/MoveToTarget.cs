using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class MoveToTarget : MonoBehaviour
    {
        [Tooltip("Object to track")]
        [SerializeField]
        private GameObject m_trackingObject = null;

        [Tooltip("Target to move to")]
        [SerializeField]
        private GameObject m_targetObject = null;

        [Tooltip("Shared parent between tracking and target objects used for relative local positions")]
        [SerializeField]
        private GameObject m_rootObject = null;

        [Tooltip("Duration of move from tracking object to target object")]
        [SerializeField]
        private float m_duration = 1.38f;

        [Tooltip("Tunable offset to get the GameObject to arrive at the right target position")]
        [SerializeField]
        private Vector3 m_targetOffset = new Vector3(0.05f, -0.1f, -0.2f);

        [Tooltip("Lerp curve")]
        [SerializeField]
        private AnimationCurve m_animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        // The local position of this gameObject
        private Vector3 m_position;

        // The local position of m_targetObject
        private Vector3 m_targetPosition;

        // The offset from this gameObject's position to the m_trackingObject's position
        private Vector3 m_trackingOffset;

        // bool to determine when to stop the follow sequence
        private bool m_followingTargetObject;

        private void Awake()
        {
            if (m_trackingObject != null)
            {
                m_trackingOffset = new Vector3(0,0,0); // GetRelativeLocalPosition(m_trackingObject, m_rootObject) - gameObject.transform.parent.localPosition;
            }
        }

        // Since this script can attach to an object with an animator, we need to update position in LateUpdate
        private void LateUpdate()
        {
            if (m_targetObject != null && m_rootObject != null)
            {
                m_targetPosition = GetRelativeLocalPosition(m_targetObject, m_rootObject) + m_targetOffset;
                transform.parent.localPosition = m_position;
            }
        }

        // Starts coroutine to lerp from current position to target position
        public void MoveToTargetPosition()
        {
            if (m_targetPosition != null)
            {
                m_followingTargetObject = false;
                StartCoroutine(MoveHintSequence());
            }
        }

        private IEnumerator MoveHintSequence()
        {
            Vector3 origin = m_position;

            float t = 0;
            while (t <= m_duration)
            {
                m_position = Vector3.Lerp(origin, m_targetPosition, m_animationCurve.Evaluate(t / m_duration));
                t += Time.deltaTime;
                yield return null;
            }
        }

        public void SetHintTarget(GameObject target)
        {
            m_targetObject = target;
        }

        // Starts coroutine to follow the target object
        public void Follow()
        {
            if (m_trackingObject != null && m_rootObject != null)
            {
                m_followingTargetObject = true;
                StartCoroutine(FollowSequence());
            }
        }

        private IEnumerator FollowSequence()
        {
            while (m_followingTargetObject)
            {
                m_position = GetRelativeLocalPosition(m_trackingObject, m_rootObject) - m_trackingOffset;
                yield return null;
            }
        }

        private Vector3 GetRelativeLocalPosition(GameObject input, GameObject root)
        {
            Vector3 sum = new Vector3();
            Transform currentLevel = input.transform;
            while (currentLevel.gameObject != root)
            {
                // first rotate the current sum using the current level's rotation
                sum = currentLevel.localRotation * sum;
                // then get the scaled value
                sum = Vector3.Scale(sum, currentLevel.localScale);
                // now add the current level's local position
                sum += currentLevel.localPosition;
                currentLevel = currentLevel.parent;
            }
            return sum;
        }
    }
}
