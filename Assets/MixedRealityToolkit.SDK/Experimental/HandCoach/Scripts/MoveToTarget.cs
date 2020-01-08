using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// This class provides functionality to move the hand hint from a tracking position to a target position over time.
    /// </summary>
    public class MoveToTarget : MonoBehaviour
    {
        /// <summary>
        /// Object to track.
        /// </summary>
        [SerializeField]
        private GameObject m_trackingObject = null;

        /// <summary>
        /// Target to move to.
        /// </summary>
        [SerializeField]
        private GameObject m_targetObject = null;

        /// <summary>
        /// Shared parent between tracking and target objects used for relative local positions.
        /// </summary>
        [SerializeField]
        private GameObject m_rootObject = null;

        /// <summary>
        /// Duration of move from tracking object to target object in seconds.
        /// </summary>
        [SerializeField]
        private float m_duration = 1.38f;

        /// <summary>
        /// Tunable offset to get the GameObject to arrive at the right target position.
        /// </summary>
        [SerializeField]
        private Vector3 m_targetOffset = new Vector3(0.05f, -0.1f, -0.2f);

        /// <summary>
        /// Lerp curve.
        /// </summary>
        [SerializeField]
        private AnimationCurve m_animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        // The local position of this gameObject relative to the root object
        private Vector3 m_relativePositionTrackingToRoot;

        // The local position of m_targetObject relative to the root object
        private Vector3 m_relativeTargetPositionToRoot;

        // bool to determine when to stop the follow sequence
        private bool m_followingTargetObject;

        // Since this script can attach to an object with an animator, we need to update position in LateUpdate
        private void LateUpdate()
        {
            if (m_targetObject != null && m_rootObject != null)
            {
                m_relativeTargetPositionToRoot = GetRelativeLocalPosition(m_targetObject, m_rootObject) + m_targetOffset;
                transform.parent.localPosition = m_relativePositionTrackingToRoot;
            }
        }

        /// <summary>
        /// Starts coroutine to lerp from current position to target position
        /// </summary>
        public void MoveToTargetPosition()
        {
            if (m_relativeTargetPositionToRoot != Vector3.zero)
            {
                m_followingTargetObject = false;
                StartCoroutine(MoveHintSequence());
            }
        }

        private IEnumerator MoveHintSequence()
        {
            Vector3 origin = m_relativePositionTrackingToRoot;

            float t = 0;
            while (t <= m_duration)
            {
                m_relativePositionTrackingToRoot = Vector3.Lerp(origin, m_relativeTargetPositionToRoot, m_animationCurve.Evaluate(t / m_duration));
                t += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Set the target object to move to.
        /// </summary>
        public void SetHintTarget(GameObject target)
        {
            m_targetObject = target;
        }

        /// <summary>
        /// Starts coroutine to follow the target object.
        /// </summary>
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
                m_relativePositionTrackingToRoot = GetRelativeLocalPosition(m_trackingObject, m_rootObject);
                yield return null;
            }
        }

        private Vector3 GetRelativeLocalPosition(GameObject input, GameObject root)
        {
            return input.transform.position - root.transform.position;
        }
    }
}
