using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.HandCoach
{
    /// <summary>
    /// This class provides functionality to move the hand hint from a tracking position to a target position over time.
    /// </summary>
    public class MoveToTarget : MonoBehaviour
    {
        [Tooltip("Object to track.")]
        [SerializeField]
        private GameObject trackingObject = null;

        /// <summary>
        /// Object to track.
        /// </summary>
        public GameObject TrackingObject
        {
            get
            {
                return trackingObject;
            }
            set
            {
                trackingObject = value;
            }
        }

        [Tooltip("Target to move to.")]
        [SerializeField]
        private GameObject targetObject = null;

        /// <summary>
        /// Target to move to.
        /// </summary>
        public GameObject TargetObject
        {
            get
            {
                return targetObject;
            }
            set
            {
                targetObject = value;
            }
        }


        [Tooltip("Shared parent between tracking and target objects used for relative local positions.")]
        [SerializeField]
        private GameObject rootObject = null;

        /// <summary>
        /// Shared parent between tracking and target objects used for relative local positions.
        /// </summary>
        public GameObject RootObject
        {
            get
            {
                return rootObject;
            }
            set
            {
                rootObject = value;
            }
        }

        [Tooltip("Duration of move from tracking object to target object in seconds.")]
        [SerializeField]
        private float duration = 1.38f;

        /// <summary>
        /// Duration of move from tracking object to target object in seconds.
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

        [Tooltip("Tunable offset to get the GameObject to arrive at the right target position.")]
        [SerializeField]
        private Vector3 targetOffset = new Vector3(0f, 0f, 0f);

        /// <summary>
        /// Tunable offset to get the GameObject to arrive at the right target position.
        /// </summary>
        public Vector3 TargetOffset
        {
            get
            {
                return targetOffset;
            }
            set
            {
                targetOffset = value;
            }
        }

        [Tooltip("Lerp curve that controls the animation position over time from the trackingObject to the targetObject.")]
        [SerializeField]
        private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        /// <summary>
        /// Lerp curve that controls the animation position over time from the trackingObject to the targetObject.
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

        // The local position of this gameObject relative to the root object
        private Vector3 relativePositionTrackingToRoot;

        // The local position of targetObject relative to the root object
        private Vector3 relativeTargetPositionToRoot;

        // bool to determine when to stop the follow sequence
        private bool followingTargetObject;

        // Since this script can attach to an object with an animator, we need to update position in LateUpdate
        private void LateUpdate()
        {
            if (TargetObject != null && RootObject != null)
            {
                relativeTargetPositionToRoot = GetRelativeLocalPosition(TargetObject, RootObject) + TargetOffset;
                transform.parent.localPosition = relativePositionTrackingToRoot;
            }
        }

        /// <summary>
        /// Starts coroutine to lerp from current position to target position
        /// </summary>
        public void MoveToTargetPosition()
        {
            if (relativeTargetPositionToRoot != Vector3.zero)
            {
                followingTargetObject = false;
                StartCoroutine(MoveHintSequence());
            }
        }

        private IEnumerator MoveHintSequence()
        {
            Vector3 origin = relativePositionTrackingToRoot;

            float t = 0;
            while (t <= Duration)
            {
                relativePositionTrackingToRoot = Vector3.Lerp(origin, relativeTargetPositionToRoot, AnimationCurve.Evaluate(t / Duration));
                t += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Starts coroutine to follow the target object.
        /// </summary>
        public void Follow()
        {
            if (TrackingObject != null && RootObject != null)
            {
                followingTargetObject = true;
                StartCoroutine(FollowSequence());
            }
        }

        private IEnumerator FollowSequence()
        {
            while (followingTargetObject)
            {
                relativePositionTrackingToRoot = GetRelativeLocalPosition(TrackingObject, RootObject);
                yield return null;
            }
        }

        private Vector3 GetRelativeLocalPosition(GameObject input, GameObject root)
        {
            return input.transform.position - root.transform.position;
        }
    }
}
