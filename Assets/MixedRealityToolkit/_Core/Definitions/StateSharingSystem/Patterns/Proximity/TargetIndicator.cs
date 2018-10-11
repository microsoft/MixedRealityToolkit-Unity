using UnityEngine;
using System.Collections.Generic;
using System;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Patterns.Proximity
{
    public class TargetIndicator : MonoBehaviour, ITargetIndicator
    {
        public Action OnFeedbackPulse { get; set; }

        public bool FeedbackEnabled { get { return feedbackEnabled; } set { feedbackEnabled = value; } }
        public Transform SearchOrigin { get { return searchOrigin; } set { searchOrigin = value; } }
        public bool HasActiveTarget { get { return targets.Count > 0 && targets[0].Active; } }
        public int MaxActiveTargets { get { return maxActiveTargets; } set { maxActiveTargets = value; } }
        public float MaxActiveDistance { get { return maxActiveDistance; } set { maxActiveDistance = value; } }

        [Header("Target settings")]
        [SerializeField]
        private int maxActiveTargets = 3;
        [SerializeField]
        private float maxActiveDistance = 5f;
        [SerializeField]
        private float feedbackPulseTimeMultiplier = 1.5f;
        [SerializeField]
        private AnimationCurve feedbackPulseCurve;
        [SerializeField]
        private Transform searchOrigin;
        [SerializeField]
        private List<TargetIndicatorInfo> targets = new List<TargetIndicatorInfo>();

        private float timeLastFeedbackPulse;
        private int numActiveTargets;
        private int closestTargetIndex;
        private bool feedbackEnabled;
        private bool targetsSetThisFrame;

        public TargetIndicatorInfo ClosestTarget { get { return targets[0]; } }

        public IEnumerable<TargetIndicatorInfo> ActiveTargets
        {
            get
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (i >= maxActiveTargets)
                        yield break;

                    TargetIndicatorInfo info = targets[i];
                    if (info.Target == null || !info.Target.gameObject.activeSelf)
                        continue;

                    yield return info;
                }
                yield break;
            }
        }

        public void SetTargets(IEnumerable<Transform> newTargets)
        {
            targets.Clear();
            foreach (Transform targetTransform in newTargets)
            {
                numActiveTargets = 0;

                TargetIndicatorInfo newTargetInfo = new TargetIndicatorInfo();
                newTargetInfo.Distance = Vector3.Distance(searchOrigin.position, targetTransform.position);
                newTargetInfo.Target = targetTransform;
                targets.Add(newTargetInfo);
            }

            targetsSetThisFrame = true;

            UpdateTargets();
        }

        public void ClearTargets()
        {
            targets.Clear();
            targetsSetThisFrame = true;
            UpdateTargets();
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                if (targetsSetThisFrame)
                {
                    targetsSetThisFrame = false;
                    return;
                }

                UpdateTargets();
            }
        }

        private void UpdateTargets()
        { 
            numActiveTargets = 0;
            closestTargetIndex = -1;
            float closestDistanceSoFar = Mathf.Infinity;

            targets.Sort(delegate (TargetIndicatorInfo t1, TargetIndicatorInfo t2)
            { return (t1.Distance.CompareTo(t2.Distance)); });

            for (int i = 0; i < targets.Count; i++)
            {
                TargetIndicatorInfo info = targets[i];
                if (info.Target == null || !info.Target.gameObject.activeSelf)
                    continue;

                info.Distance = Vector3.Distance(info.Target.position, searchOrigin.position);
                info.Active = (info.Distance < MaxActiveDistance);

                if (info.Distance < closestDistanceSoFar)
                {
                    closestDistanceSoFar = info.Distance;
                    closestTargetIndex = i;
                }

                info.LocalDirection = (info.Target.position - searchOrigin.position).normalized;
                targets[i] = info;

                if (info.Active)
                   numActiveTargets++;
            }

            if (feedbackEnabled && closestTargetIndex >= 0)
            {
                TargetIndicatorInfo info = targets[closestTargetIndex];
                float normalizedDistance = info.Distance / MaxActiveDistance;
                if (Time.time > timeLastFeedbackPulse + (feedbackPulseCurve.Evaluate(normalizedDistance) * feedbackPulseTimeMultiplier))
                {
                    timeLastFeedbackPulse = Time.time;
                    if (Application.isPlaying)
                    {
                        if (OnFeedbackPulse != null)
                            OnFeedbackPulse();
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                UpdateTargets();

                foreach (TargetIndicatorInfo info in ActiveTargets)
                { 
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(info.Target.position, searchOrigin.position);
                }

                if (closestTargetIndex >= 0)
                {
                    TargetIndicatorInfo info = targets[closestTargetIndex];
                    float normalizedDistance = info.Distance / MaxActiveDistance;
                    Gizmos.color = Color.Lerp(Color.red, Color.blue, normalizedDistance);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    Gizmos.color = Color.Lerp(Color.white, Color.red, Time.time - timeLastFeedbackPulse);
                    Gizmos.DrawSphere(info.Target.position, 0.5f);
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(searchOrigin.position, searchOrigin.position + info.LocalDirection * info.Distance / 2);
                }
            }
        }
    }
}