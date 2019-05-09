// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A used-defined marker on the input animation timeline.
    /// </summary>
    [Serializable]
    public class InputAnimationMarker
    {
        /// <summary>
        /// Placement of the marker relative to the input animation start time.
        /// </summary>
        public float time = 0.0f;

        /// <summary>
        /// Custom name of the marker.
        /// </summary>
        public string name = "";
    }

    /// <summary>
    /// Contains a set of animation curves that describe motion of camera and hands.
    /// </summary>
    [System.Serializable]
    public class InputAnimation
    {
        protected static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Maximum duration of all animations curves.
        /// </summary>
        [SerializeField]
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

        /// <summary>
        /// Number of markers in the animation.
        /// </summary>
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

        /// <summary>
        /// Add a keyframe for the tracking state of a hand.
        /// </summary>
        public void AddHandStateKey(float time, Handedness handedness, bool isTracked, bool isPinching)
        {
            if (handedness == Handedness.Left)
            {
                AddHandStateKey(time, isTracked, isPinching, handTrackedCurveLeft, handPinchCurveLeft);
            }
            else if (handedness == Handedness.Right)
            {
                AddHandStateKey(time, isTracked, isPinching, handTrackedCurveRight, handPinchCurveRight);
            }
        }

        /// <summary>
        /// Add a keyframe for one hand joint.
        /// </summary>
        public void AddHandJointKey(float time, Handedness handedness, TrackedHandJoint joint, Vector3 jointPosition, float positionThreshold)
        {
            if (handedness == Handedness.Left)
            {
                AddHandJointKey(time, joint, jointPosition, handJointCurvesLeft, positionThreshold);
            }
            else if (handedness == Handedness.Right)
            {
                AddHandJointKey(time, joint, jointPosition, handJointCurvesRight, positionThreshold);
            }
        }

        /// <summary>
        /// Add a keyframe for an entire hand pose containing all the joints.
        /// </summary>
        public void AddHandDataKeys(float time, Handedness handedness, SimulatedHandData data, float positionThreshold, float rotationThreshold)
        {
            if (handedness == Handedness.Left)
            {
                AddHandStateKey(time, data.IsTracked, data.IsPinching, handTrackedCurveLeft, handPinchCurveLeft);
                for (int i = 0; i < jointCount; ++i)
                {
                    AddHandJointKey(time, (TrackedHandJoint)i, data.Joints[i], handJointCurvesLeft, positionThreshold);
                }
            }
            else if (handedness == Handedness.Right)
            {
                AddHandStateKey(time, data.IsTracked, data.IsPinching, handTrackedCurveRight, handPinchCurveRight);
                for (int i = 0; i < jointCount; ++i)
                {
                    AddHandJointKey(time, (TrackedHandJoint)i, data.Joints[i], handJointCurvesRight, positionThreshold);
                }
            }
        }

        /// Add a keyframe for the tracking state of a hand.
        private void AddHandStateKey(float time, bool isTracked, bool isPinching, AnimationCurve trackedCurve, AnimationCurve pinchCurve)
        {
            AddBoolKeyFiltered(trackedCurve, time, isTracked);
            AddBoolKeyFiltered(pinchCurve, time, isPinching);

            duration = Mathf.Max(duration, time);
        }

        /// Add a keyframe for one hand joint.
        private void AddHandJointKey(float time, TrackedHandJoint joint, Vector3 jointPosition, AnimationCurve[] jointCurves, float positionThreshold)
        {
            int curveIndex = (int)joint * 3;
            AddFloatKeyFiltered(jointCurves[curveIndex + 0], time, jointPosition.x, positionThreshold);
            AddFloatKeyFiltered(jointCurves[curveIndex + 1], time, jointPosition.y, positionThreshold);
            AddFloatKeyFiltered(jointCurves[curveIndex + 2], time, jointPosition.z, positionThreshold);

            duration = Mathf.Max(duration, time);
        }

        /// <summary>
        /// Add a keyframe for the camera transform.
        /// </summary>
        public void AddCameraPoseKey(float time, MixedRealityPose cameraPose, float positionThreshold, float rotationThreshold)
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
            int insertAfter = FindKeyframeInterval(curve, time);
            if (insertAfter > 0)
            {
                // Merge the preceding two intervals if difference is small enough
                float value0 = curve.keys[insertAfter - 1].value;
                float value1 = curve.keys[insertAfter].value;
                if (Mathf.Abs(value1 - value0) <= epsilon && Mathf.Abs(value - value1) <= epsilon)
                {
                    curve.RemoveKey(insertAfter);
                }
            }

            // return curve.AddKey(time, value);

            // TODO make use of Bezier interpolation to allow more aggressive compression
            // and use tangents and weights to accurately merge adjacent splines.
            // Use linear interpolation to avoid overshooting from bezier tangents
            var keyframe = new Keyframe(time, value, 0.0f, 0.0f, 0.0f, 0.0f);
            keyframe.weightedMode = WeightedMode.Both;
            return curve.AddKey(keyframe);
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

        /// <summary>
        /// Add a user-defined marker.
        /// </summary>
        public void AddMarker(InputAnimationMarker marker)
        {
            int index = FindMarkerInterval(marker.time) + 1;
            markers.Insert(index, marker);
        }

        /// <summary>
        /// Remove the user-defined marker at the given index.
        /// </summary>
        public void RemoveMarker(int index)
        {
            markers.RemoveAt(index);
        }

        /// <summary>
        /// Change the time of the marker at the given index.
        /// </summary>
        public void SetMarkerTime(int index, float time)
        {
            InputAnimationMarker marker = markers[index];
            markers.RemoveAt(index);

            int newIndex = FindMarkerInterval(time) + 1;
            marker.time = time;
            markers.Insert(newIndex, marker);
        }

        #endif // UNITY_EDITOR

        /// <summary>
        /// Evaluate hand tracking state and joint poses at the given time.
        /// </summary>
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

        /// Evaluate hand tracking state and joint poses at the given time.
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

        /// <summary>
        /// Evaluate the camera transform at the given time.
        /// </summary>
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

        /// <summary>
        /// Get the marker at the given index.
        /// </summary>
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

        /// Sort marker array by time values.
        private void SortMarkers()
        {
            markers.Sort(new CompareMarkers());
        }

        /// <summary>
        /// Serialize animation data into a stream.
        /// </summary>
        public void ToStream(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            var formatter = new BinaryFormatter();

            WriteAnimationCurve(writer, handTrackedCurveLeft);
            WriteAnimationCurve(writer, handTrackedCurveRight);
            WriteAnimationCurve(writer, handPinchCurveLeft);
            WriteAnimationCurve(writer, handPinchCurveRight);
            WriteAnimationCurveArray(writer, handJointCurvesLeft);
            WriteAnimationCurveArray(writer, handJointCurvesRight);
            WriteAnimationCurveArray(writer, cameraPositionCurves);
            WriteAnimationCurveArray(writer, cameraRotationCurves);

            formatter.Serialize(stream, markers);
        }

        /// <summary>
        /// Deserialize animation data from a stream.
        /// </summary>
        public void FromStream(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var formatter = new BinaryFormatter();

            duration = 0.0f;
            ReadAnimationCurve(reader, handTrackedCurveLeft);
            ReadAnimationCurve(reader, handTrackedCurveRight);
            ReadAnimationCurve(reader, handPinchCurveLeft);
            ReadAnimationCurve(reader, handPinchCurveRight);
            ReadAnimationCurveArray(reader, handJointCurvesLeft);
            ReadAnimationCurveArray(reader, handJointCurvesRight);
            ReadAnimationCurveArray(reader, cameraPositionCurves);
            ReadAnimationCurveArray(reader, cameraRotationCurves);

            markers = (List<InputAnimationMarker>)formatter.Deserialize(stream);
        }

        private void WriteAnimationCurve(BinaryWriter writer, AnimationCurve curve)
        {
            InputAnimationUtils.SerializeAnimationCurve(writer, curve);
        }

        private void WriteAnimationCurveArray(BinaryWriter writer, AnimationCurve[] curves)
        {
            foreach (AnimationCurve curve in curves)
            {
                WriteAnimationCurve(writer, curve);
            }
        }

        private void ReadAnimationCurve(BinaryReader reader, AnimationCurve curve)
        {
            InputAnimationUtils.DeserializeAnimationCurve(reader, curve);

            duration = Mathf.Max(duration, curve.Duration());
        }

        private void ReadAnimationCurveArray(BinaryReader reader, AnimationCurve[] curves)
        {
            foreach (AnimationCurve curve in curves)
            {
                ReadAnimationCurve(reader, curve);
            }
        }
    }
}