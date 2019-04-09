// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [System.Serializable]
    public class InputAnimationCurve<T> : ISerializationCallbackReceiver
    {
        internal class Keyframe
        {
            public Keyframe(double time, T value)
            {
                this.time = time;
                this.Value = value;
            }

            internal double time;
            public double Time => time;
            public T Value;
        }

        [NonSerialized]
        private List<Keyframe> keyframes = new List<Keyframe>();
        [SerializeField]
        [HideInInspector]
        internal List<double> serializedFrames = null;
        [SerializeField]
        [HideInInspector]
        internal List<T> serializedValues = null;

        public int KeyframeCount => keyframes.Count;

        private double duration = 0.0;
        public double Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public double GetTime(int index)
        {
            return keyframes[index].time;
        }

        public T GetValue(int index)
        {
            return keyframes[index].Value;
        }

        public void OnBeforeSerialize()
        {
            serializedFrames = new List<double>(keyframes.Count);
            serializedValues = new List<T>();
            serializedValues.Capacity = keyframes.Count;
            for (int i = 0; i < keyframes.Count; ++i)
            {
                serializedFrames.Add(keyframes[i].time);
                serializedValues.Add(keyframes[i].Value);
            }
        }

        public void OnAfterDeserialize()
        {
            keyframes.Clear();
            keyframes.Capacity = serializedFrames.Count;
            for (int i = 0; i < serializedFrames.Count; ++i)
            {
                keyframes.Add(new Keyframe(serializedFrames[i], serializedValues[i]));
            }
            serializedFrames = null;
            serializedValues = null;
        }

        /// <summary>
        /// Find an index i in the sorted keyframes list, such that keyframes[i].Time <= time < keyframes[i+1].Time.
        /// </summary>
        /// <returns>
        /// 0 <= i < keyframeCount if a full interval could be found.
        /// -1 if time is less than the first keyframe time.
        /// keyframeCount-1 if time is greater than the last keyframe time.
        /// </returns>
        /// <remarks>
        /// Uses binary search.
        /// </remarks>
        public int FindKeyframeInterval(double time)
        {
            int lowIdx = -1;
            int highIdx = keyframes.Count;
            while (lowIdx < highIdx - 1)
            {
                int midIdx = (lowIdx + highIdx) >> 1;
                if (time >= keyframes[midIdx].time)
                {
                    lowIdx = midIdx;
                }
                else
                {
                    highIdx = midIdx;
                }
            }
            return lowIdx;
        }

        /// <summary>
        /// Find an index i in the sorted keyframes list, such that keyframes[i].Time <= time < keyframes[i+1].Time.
        /// </summary>
        /// <returns>
        /// 0 <= i < keyframeCount if a full interval could be found.
        /// -1 if time is less than the first keyframe time.
        /// keyframeCount-1 if time is greater than the last keyframe time.
        /// </returns>
        /// <remarks>
        /// Uses binary search.
        /// </remarks>
        public int FindKeyframeInterval(double time, out T low, out double lowTime, out T high, out double highTime)
        {
            int result = FindKeyframeInterval(time);
            if (result >= 0)
            {
                low = keyframes[result].Value;
                lowTime = keyframes[result].Time;
            }
            else
            {
                low = default(T);
                lowTime = 0;
            }
            if (result < keyframes.Count - 1)
            {
                high = keyframes[result + 1].Value;
                highTime = keyframes[result + 1].Time;
            }
            else
            {
                high = default(T);
                highTime = 0;
            }
            return result;
        }

        public T InsertKeyframe(T value, double time)
        {
            // Find the greated index with less-or-equal frame
            int prevIndex = FindKeyframeInterval(time);
            var keyframe = new Keyframe(time, value);
            // Insert after the index
            keyframes.Insert(prevIndex + 1, keyframe);

            return keyframe.Value;
        }

        public void RemoveKeyframeAt(double time)
        {
            keyframes.RemoveAll(k => k.time == time);
        }

        public void RemoveKeyframe(T keyframe)
        {
            keyframes.RemoveAll(k => k.Value.Equals(keyframe));
        }

        public void ClearKeyframes()
        {
            keyframes.Clear();
        }

        public void SetKeyframeTime(double oldTime, double newTime)
        {
            foreach (var keyframe in keyframes.FindAll(k => k.time == oldTime))
            {
                keyframe.time = newTime;
            }

            SortKeyframes();
        }

        private static int KeyframeComp(Keyframe a, Keyframe b)
        {
            if (a.Time < b.Time)
            {
                return -1;
            }
            else if (a.Time > b.Time)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private void SortKeyframes()
        {
            keyframes.Sort(KeyframeComp);
        }
    }

    [System.Serializable]
    public class InputKeyframe
    {
        public SimulatedHandData HandDataLeft = null;
        public SimulatedHandData HandDataRight = null;

        public MixedRealityPose CameraPose = MixedRealityPose.ZeroIdentity;

        public InputKeyframe(SimulatedHandData handDataLeft, SimulatedHandData handDataRight)
        {
            this.HandDataLeft = handDataLeft;
            this.HandDataRight = handDataRight;
        }
    }

    [System.Serializable]
    public class InputAnimation : InputAnimationCurve<InputKeyframe>
    {
        protected static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        public bool Interpolate(double time, SimulatedHandData resultHandDataLeft, SimulatedHandData resultHandDataRight, out MixedRealityPose resultCameraPose)
        {
            // Find [low, high] bracket of keyframes
            FindKeyframeInterval(time, out InputKeyframe low, out double lowTime, out InputKeyframe high, out double highTime);

            if (low != null && high != null)
            {
                float blend = (float)((double)(time - lowTime) / (double)(highTime - lowTime));
                InterpolateHandData(resultHandDataLeft, low.HandDataLeft, high.HandDataLeft, blend);
                InterpolateHandData(resultHandDataRight, low.HandDataRight, high.HandDataRight, blend);
                InterpolateCameraPose(out resultCameraPose, low.CameraPose, high.CameraPose, blend);
                return true;
            }
            else if (low != null)
            {
                resultHandDataLeft.Copy(low.HandDataLeft);
                resultHandDataRight.Copy(low.HandDataRight);
                resultCameraPose = low.CameraPose;
                return true;
            }
            else if (high != null)
            {
                resultHandDataLeft.Copy(high.HandDataLeft);
                resultHandDataRight.Copy(high.HandDataRight);
                resultCameraPose = high.CameraPose;
                return true;
            }
            else
            {
                resultCameraPose = MixedRealityPose.ZeroIdentity;
                return false;
            }
        }

        private void InterpolateHandData(SimulatedHandData result, SimulatedHandData low, SimulatedHandData high, float blend)
        {
            const float pinchThreshold = 0.9f;

            bool isTracked = low.IsTracked && high.IsTracked;
            bool isPinching = (low.IsPinching ? (1.0f - blend) : 0.0f) + (high.IsPinching ? blend : 0.0f) > pinchThreshold;

            result.Update(isTracked, isPinching,
                (Vector3[] jointPositions) =>
                {
                    for (int i = 0; i < jointCount; ++i)
                    {
                        jointPositions[i] = Vector3.Lerp(low.Joints[i], high.Joints[i], blend);
                    }
                });
        }

        private void InterpolateCameraPose(out MixedRealityPose result, MixedRealityPose low, MixedRealityPose high, float blend)
        {
            result = new MixedRealityPose();
            result.Position = Vector3.Lerp(low.Position, high.Position, blend);
            result.Rotation = Quaternion.Slerp(low.Rotation, high.Rotation, blend);
        }
    }
}