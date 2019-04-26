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

        public void AddHandJointKeys(float time, Handedness handedness, TrackedHandJoint joint, Vector3 jointPosition)
        {
            if (handedness == Handedness.Left)
            {
                AddHandJointKeys(time, joint, jointPosition, handJointCurvesLeft);
            }
            else if (handedness == Handedness.Right)
            {
                AddHandJointKeys(time, joint, jointPosition, handJointCurvesRight);
            }
        }

        public void AddHandDataKeys(float time, Handedness handedness, SimulatedHandData data)
        {
            if (handedness == Handedness.Left)
            {
                AddHandStateKeys(time, data.IsTracked, data.IsPinching, handTrackedCurveLeft, handPinchCurveLeft);
                for (int i = 0; i < jointCount; ++i)
                {
                    AddHandJointKeys(time, (TrackedHandJoint)i, data.Joints[i], handJointCurvesLeft);
                }
            }
            else if (handedness == Handedness.Right)
            {
                AddHandStateKeys(time, data.IsTracked, data.IsPinching, handTrackedCurveRight, handPinchCurveRight);
                for (int i = 0; i < jointCount; ++i)
                {
                    AddHandJointKeys(time, (TrackedHandJoint)i, data.Joints[i], handJointCurvesRight);
                }
            }
        }

        private void AddHandStateKeys(float time, bool isTracked, bool isPinching, AnimationCurve trackedCurve, AnimationCurve pinchCurve)
        {
            int key;

            key = trackedCurve.AddKey(time, isTracked ? 1.0f : 0.0f);
            AnimationUtility.SetKeyBroken(trackedCurve, key, true);

            key = pinchCurve.AddKey(time, isPinching ? 1.0f : 0.0f);
            AnimationUtility.SetKeyBroken(pinchCurve, key, true);

            duration = Mathf.Max(duration, time);
        }

        private void AddHandJointKeys(float time, TrackedHandJoint joint, Vector3 jointPosition, AnimationCurve[] jointCurves)
        {
            int curveIndex = (int)joint * 3;
            jointCurves[curveIndex + 0].AddKey(time, jointPosition.x);
            jointCurves[curveIndex + 1].AddKey(time, jointPosition.y);
            jointCurves[curveIndex + 2].AddKey(time, jointPosition.z);

            duration = Mathf.Max(duration, time);
        }

        public void AddCameraPoseKeys(float time, MixedRealityPose cameraPose)
        {
            cameraPositionCurves[0].AddKey(time, cameraPose.Position.x);
            cameraPositionCurves[1].AddKey(time, cameraPose.Position.y);
            cameraPositionCurves[2].AddKey(time, cameraPose.Position.z);

            cameraRotationCurves[0].AddKey(time, cameraPose.Rotation.x);
            cameraRotationCurves[1].AddKey(time, cameraPose.Rotation.y);
            cameraRotationCurves[2].AddKey(time, cameraPose.Rotation.z);
            cameraRotationCurves[3].AddKey(time, cameraPose.Rotation.w);

            duration = Mathf.Max(duration, time);
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
            bool isTracked = (trackedCurve.Evaluate(time) > 0.0f);
            bool isPinching = (pinchCurve.Evaluate(time) > 0.0f);
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