// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    public class InputAnimationMarker
    {
        public float time = 0.0f;
        public string name = "";
    }

    [System.Serializable]
    public class InputAnimation
    {
        protected static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        private float duration = 0.0f;
        public float Duration => duration;

        [SerializeField]
        private AnimationCurve handTrackedCurveLeft;
        [SerializeField]
        private AnimationCurve handTrackedCurveRight;
        [SerializeField]
        private AnimationCurve handPinchCurveLeft;
        [SerializeField]
        private AnimationCurve handPinchCurveRight;
        [SerializeField]
        private AnimationCurve[] handJointCurvesLeft;
        [SerializeField]
        private AnimationCurve[] handJointCurvesRight;
        [SerializeField]
        private AnimationCurve[] cameraPositionCurves;
        [SerializeField]
        private AnimationCurve[] cameraRotationCurves;

        [SerializeField]
        private List<InputAnimationMarker> markers;
        public int markerCount => markers.Count;

        internal class CompareMarkers : IComparer<InputAnimationMarker>
        {
            public int Compare(InputAnimationMarker a, InputAnimationMarker b)
            {
                return a.time.CompareTo(b.time);
            }
        }

        public InputAnimation()
        {
            handTrackedCurveLeft = new AnimationCurve();
            handTrackedCurveRight = new AnimationCurve();
            handPinchCurveLeft = new AnimationCurve();
            handPinchCurveRight = new AnimationCurve();
            handJointCurvesLeft = CreateAnimationCurveArray(3 * jointCount);
            handJointCurvesRight = CreateAnimationCurveArray(3 * jointCount);
            cameraPositionCurves = CreateAnimationCurveArray(3);
            cameraRotationCurves = CreateAnimationCurveArray(4);

            markers = new List<InputAnimationMarker>();
        }

        private AnimationCurve[] CreateAnimationCurveArray(int count)
        {
            var curves = new AnimationCurve[count];
            for (int i = 0; i < count; ++i)
            {
                curves[i] = new AnimationCurve();
            }
            return curves;
        }

        protected AnimationCurve GetHandJointCurve(Handedness handedness, TrackedHandJoint joint, int component)
        {
            const int maxComponent = 3;
            Debug.Assert(component < maxComponent);

            int index = (int)joint * maxComponent + component;

            if (handedness == Handedness.Left)
            {
                return handJointCurvesLeft[index];
            }
            else if (handedness == Handedness.Right)
            {
                return handJointCurvesRight[index];
            }
            return null;
        }

        protected AnimationCurve GetHeadPositionCurve(int component)
        {
            const int maxComponent = 3;
            Debug.Assert(component < maxComponent);

            return cameraPositionCurves[component];
        }

        protected AnimationCurve GetHeadRotationCurve(int component)
        {
            const int maxComponent = 4;
            Debug.Assert(component < maxComponent);

            return cameraRotationCurves[component];
        }

        /// AnimationUtility is only available with UnityEditor
        #if UNITY_EDITOR

        public void AddHandStateKeys(float time, Handedness handedness, bool isTracked, bool isPinching)
        {
            if (handedness == Handedness.Left)
            {
                AddHandStateKeys(time, isTracked, isPinching, handTrackedCurveLeft, handPinchCurveLeft);
            }
            else if (handedness == Handedness.Right)
            {
                AddHandStateKeys(time, isTracked, isPinching, handTrackedCurveRight, handPinchCurveRight);
            }
        }

        public void AddHandJointKeys(float time, Handedness handedness, TrackedHandJoint joint, Vector3 jointPosition, float positionThreshold)
        {
            if (handedness == Handedness.Left)
            {
                AddHandJointKeys(time, joint, jointPosition, handJointCurvesLeft, positionThreshold);
            }
            else if (handedness == Handedness.Right)
            {
                AddHandJointKeys(time, joint, jointPosition, handJointCurvesRight, positionThreshold);
            }
        }

        public void AddHandDataKeys(float time, Handedness handedness, SimulatedHandData data, float positionThreshold, float rotationThreshold)
        {
            if (handedness == Handedness.Left)
            {
                AddHandStateKeys(time, data.IsTracked, data.IsPinching, handTrackedCurveLeft, handPinchCurveLeft);
                for (int i = 0; i < jointCount; ++i)
                {
                    AddHandJointKeys(time, (TrackedHandJoint)i, data.Joints[i], handJointCurvesLeft, positionThreshold);
                }
            }
            else if (handedness == Handedness.Right)
            {
                AddHandStateKeys(time, data.IsTracked, data.IsPinching, handTrackedCurveRight, handPinchCurveRight);
                for (int i = 0; i < jointCount; ++i)
                {
                    AddHandJointKeys(time, (TrackedHandJoint)i, data.Joints[i], handJointCurvesRight, positionThreshold);
                }
            }
        }

        private void AddHandStateKeys(float time, bool isTracked, bool isPinching, AnimationCurve trackedCurve, AnimationCurve pinchCurve)
        {
            AddBoolKeyFiltered(trackedCurve, time, isTracked);
            AddBoolKeyFiltered(pinchCurve, time, isPinching);

            duration = Mathf.Max(duration, time);
        }

        private void AddHandJointKeys(float time, TrackedHandJoint joint, Vector3 jointPosition, AnimationCurve[] jointCurves, float positionThreshold)
        {
            int curveIndex = (int)joint * 3;
            AddFloatKeyFiltered(jointCurves[curveIndex + 0], time, jointPosition.x, positionThreshold);
            AddFloatKeyFiltered(jointCurves[curveIndex + 1], time, jointPosition.y, positionThreshold);
            AddFloatKeyFiltered(jointCurves[curveIndex + 2], time, jointPosition.z, positionThreshold);

            duration = Mathf.Max(duration, time);
        }

        public void AddCameraPoseKeys(float time, MixedRealityPose cameraPose, float positionThreshold, float rotationThreshold)
        {
            AddFloatKeyFiltered(cameraPositionCurves[0], time, cameraPose.Position.x, positionThreshold);
            AddFloatKeyFiltered(cameraPositionCurves[1], time, cameraPose.Position.y, positionThreshold);
            AddFloatKeyFiltered(cameraPositionCurves[2], time, cameraPose.Position.z, positionThreshold);

            AddFloatKeyFiltered(cameraRotationCurves[0], time, cameraPose.Rotation.x, rotationThreshold);
            AddFloatKeyFiltered(cameraRotationCurves[1], time, cameraPose.Rotation.y, rotationThreshold);
            AddFloatKeyFiltered(cameraRotationCurves[2], time, cameraPose.Rotation.z, rotationThreshold);
            AddFloatKeyFiltered(cameraRotationCurves[3], time, cameraPose.Rotation.w, rotationThreshold);

            duration = Mathf.Max(duration, time);
        }

        /// Add a float value to an animation curve
        private static int AddFloatKey(AnimationCurve curve, float time, float value)
        {
            return curve.AddKey(time, value);
        }

        /// Add a float value to an animation curve
        /// Keys are only added if the value changes sufficiently
        private static int AddFloatKeyFiltered(AnimationCurve curve, float time, float value, float epsilon)
        {
            // TODO float filtering is not working well, can cause drifting and ignoring
            // large changes when they accumulate. Disabled for now.
            // int insertAfter = FindKeyframeInterval(curve, time);
            // if (insertAfter >= 0 && Mathf.Abs(curve.keys[insertAfter].value - value) < epsilon)
            // {
            //     // Value unchanged from previous key, ignore
            //     return -1;
            // }

            return curve.AddKey(time, value);
        }

        /// Utility function that creates a non-interpolated keyframe suitable for boolean values
        private static int AddBoolKey(AnimationCurve curve, float time, bool value)
        {
            float fvalue = value ? 1.0f : 0.0f;
            // Set tangents and weights such than the the input value is cut off and out tangent is constant.
            var keyframe = new Keyframe(time, fvalue, 0.0f, 0.0f, 0.0f, 1.0e6f);
            keyframe.weightedMode = WeightedMode.Both;

            return curve.AddKey(keyframe);
        }

        /// Utility function that creates a non-interpolated keyframe suitable for boolean values
        /// Keys are only added if the value changes
        private static int AddBoolKeyFiltered(AnimationCurve curve, float time, bool value)
        {
            float fvalue = value ? 1.0f : 0.0f;
            // Set tangents and weights such than the the input value is cut off and out tangent is constant.
            var keyframe = new Keyframe(time, fvalue, 0.0f, 0.0f, 0.0f, 1.0e6f);
            keyframe.weightedMode = WeightedMode.Both;

            int insertAfter = FindKeyframeInterval(curve, time);
            if (insertAfter >= 0 && curve.keys[insertAfter].value == fvalue)
            {
                // Value unchanged from previous key, ignore
                return -1;
            }

            int insertBefore = insertAfter + 1;
            if (insertBefore < curve.keys.Length && curve.keys[insertBefore].value == fvalue)
            {
                // Same value as next key, replace next key
                return curve.MoveKey(insertBefore, keyframe);
            }

            return curve.AddKey(keyframe);
        }

        /// <summary>
        /// Find an index i in the sorted events list, such that events[i].time <= time < events[i+1].time.
        /// </summary>
        /// <returns>
        /// 0 <= i < eventCount if a full interval could be found.
        /// -1 if time is less than the first event time.
        /// eventCount-1 if time is greater than the last event time.
        /// </returns>
        /// <remarks>
        /// Uses binary search.
        /// </remarks>
        private static int FindKeyframeInterval(AnimationCurve curve, float time)
        {
            var keys = curve.keys;
            int lowIdx = -1;
            int highIdx = keys.Length;
            while (lowIdx < highIdx - 1)
            {
                int midIdx = (lowIdx + highIdx) >> 1;
                if (time >= keys[midIdx].time)
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

        public void AddMarker(InputAnimationMarker marker)
        {
            int index = FindMarkerInterval(marker.time) + 1;
            markers.Insert(index, marker);
        }

        public void RemoveMarker(int index)
        {
            markers.RemoveAt(index);
        }

        public void SetMarkerTime(int index, float time)
        {
            InputAnimationMarker marker = markers[index];
            markers.RemoveAt(index);

            int newIndex = FindMarkerInterval(time) + 1;
            marker.time = time;
            markers.Insert(newIndex, marker);
        }

        #endif // UNITY_EDITOR

        public void EvaluateHandData(float time, Handedness handedness, SimulatedHandData result)
        {
            if (handedness == Handedness.Left)
            {
                EvaluateHandData(time, result, handTrackedCurveLeft, handPinchCurveLeft, handJointCurvesLeft);
            }
            else if (handedness == Handedness.Right)
            {
                EvaluateHandData(time, result, handTrackedCurveRight, handPinchCurveRight, handJointCurvesRight);
            }
        }

        private void EvaluateHandData(float time, SimulatedHandData result, AnimationCurve trackedCurve, AnimationCurve pinchCurve, AnimationCurve[] jointCurves)
        {
            bool isTracked = (trackedCurve.Evaluate(time) > 0.5f);
            bool isPinching = (pinchCurve.Evaluate(time) > 0.5f);
            result.Update(isTracked, isPinching,
                (Vector3[] jointPositions) =>
                {
                    int jointCurve = 0;
                    for (int i = 0; i < jointCount; ++i)
                    {
                        float x = jointCurves[jointCurve++].Evaluate(time);
                        float y = jointCurves[jointCurve++].Evaluate(time);
                        float z = jointCurves[jointCurve++].Evaluate(time);
                        jointPositions[i].Set(x, y, z);
                    }
                });
        }

        public MixedRealityPose EvaluateCameraPose(float time)
        {
            var cameraPose = new MixedRealityPose();

            {
                float x = cameraPositionCurves[0].Evaluate(time);
                float y = cameraPositionCurves[1].Evaluate(time);
                float z = cameraPositionCurves[2].Evaluate(time);
                cameraPose.Position = new Vector3(x, y, z);
            }

            {
                float x = cameraRotationCurves[0].Evaluate(time);
                float y = cameraRotationCurves[1].Evaluate(time);
                float z = cameraRotationCurves[2].Evaluate(time);
                float w = cameraRotationCurves[3].Evaluate(time);
                cameraPose.Rotation = new Quaternion(x, y, z, w);
            }

            return cameraPose;
        }

        public InputAnimationMarker GetMarker(int index)
        {
            return markers[index];
        }

        /// <summary>
        /// Find an index i in the sorted events list, such that events[i].time <= time < events[i+1].time.
        /// </summary>
        /// <returns>
        /// 0 <= i < eventCount if a full interval could be found.
        /// -1 if time is less than the first event time.
        /// eventCount-1 if time is greater than the last event time.
        /// </returns>
        /// <remarks>
        /// Uses binary search.
        /// </remarks>
        public int FindMarkerInterval(float time)
        {
            int lowIdx = -1;
            int highIdx = markers.Count;
            while (lowIdx < highIdx - 1)
            {
                int midIdx = (lowIdx + highIdx) >> 1;
                if (time >= markers[midIdx].time)
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

        private void SortMarkers()
        {
            markers.Sort(new CompareMarkers());
        }
    }
}