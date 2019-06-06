// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

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

        public class PoseCurves
        {
            readonly public AnimationCurve PositionX = new AnimationCurve();
            readonly public AnimationCurve PositionY = new AnimationCurve();
            readonly public AnimationCurve PositionZ = new AnimationCurve();
            readonly public AnimationCurve RotationX = new AnimationCurve();
            readonly public AnimationCurve RotationY = new AnimationCurve();
            readonly public AnimationCurve RotationZ = new AnimationCurve();
            readonly public AnimationCurve RotationW = new AnimationCurve();
        }

        [SerializeField]
        private AnimationCurve handTrackedCurveLeft;
        [SerializeField]
        private AnimationCurve handTrackedCurveRight;
        [SerializeField]
        private AnimationCurve handPinchCurveLeft;
        [SerializeField]
        private AnimationCurve handPinchCurveRight;
        [SerializeField]
        private Dictionary<TrackedHandJoint, PoseCurves> handJointCurvesLeft;
        [SerializeField]
        private Dictionary<TrackedHandJoint, PoseCurves> handJointCurvesRight;
        [SerializeField]
        private PoseCurves cameraCurves;
        public PoseCurves CameraCurves => cameraCurves;

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
            handJointCurvesLeft = new Dictionary<TrackedHandJoint, PoseCurves>();
            handJointCurvesRight = new Dictionary<TrackedHandJoint, PoseCurves>();
            cameraCurves = new PoseCurves();

            markers = new List<InputAnimationMarker>();
        }

        public bool TryGetHandJointCurves(Handedness handedness, TrackedHandJoint joint, out PoseCurves curves)
        {
            if (handedness == Handedness.Left)
            {
                return handJointCurvesLeft.TryGetValue(joint, out curves);
            }
            else if (handedness == Handedness.Right)
            {
                return handJointCurvesRight.TryGetValue(joint, out curves);
            }
            curves = new PoseCurves();
            return false;
        }

        public PoseCurves CreateHandJointCurves(Handedness handedness, TrackedHandJoint joint)
        {
            if (handedness == Handedness.Left)
            {
                if (!handJointCurvesLeft.TryGetValue(joint, out PoseCurves curves))
                {
                    curves = new PoseCurves();
                    handJointCurvesLeft.Add(joint, curves);
                }
                return curves;
            }
            else if (handedness == Handedness.Right)
            {
                if (!handJointCurvesRight.TryGetValue(joint, out PoseCurves curves))
                {
                    curves = new PoseCurves();
                    handJointCurvesRight.Add(joint, curves);
                }
                return curves;
            }
            return null;
        }

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
        public void AddHandJointKey(float time, Handedness handedness, TrackedHandJoint joint, MixedRealityPose jointPose, float positionThreshold, float rotationThreshold)
        {
            if (handedness == Handedness.Left)
            {
                AddHandJointKey(time, joint, jointPose, handJointCurvesLeft, positionThreshold, rotationThreshold);
            }
            else if (handedness == Handedness.Right)
            {
                AddHandJointKey(time, joint, jointPose, handJointCurvesRight, positionThreshold, rotationThreshold);
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
        private void AddHandJointKey(float time, TrackedHandJoint joint, MixedRealityPose jointPose, Dictionary<TrackedHandJoint, PoseCurves> jointCurves, float positionThreshold, float rotationThreshold)
        {
            if (!jointCurves.TryGetValue(joint, out PoseCurves curves))
            {
                curves = new PoseCurves();
                jointCurves.Add(joint, curves);
            }

            AddPoseKeyFiltered(curves, time, jointPose, positionThreshold, rotationThreshold);

            duration = Mathf.Max(duration, time);
        }

        /// <summary>
        /// Add a keyframe for the camera transform.
        /// </summary>
        public void AddCameraPoseKey(float time, MixedRealityPose cameraPose, float positionThreshold, float rotationThreshold)
        {
            AddPoseKeyFiltered(cameraCurves, time, cameraPose, positionThreshold, rotationThreshold);

            duration = Mathf.Max(duration, time);
        }

        private static void AddPoseKey(PoseCurves curves, float time, MixedRealityPose pose)
        {
            AddFloatKey(curves.PositionX, time, pose.Position.x);
            AddFloatKey(curves.PositionY, time, pose.Position.y);
            AddFloatKey(curves.PositionZ, time, pose.Position.z);

            AddFloatKey(curves.RotationX, time, pose.Rotation.x);
            AddFloatKey(curves.RotationY, time, pose.Rotation.y);
            AddFloatKey(curves.RotationZ, time, pose.Rotation.z);
            AddFloatKey(curves.RotationW, time, pose.Rotation.w);
        }

        private static void AddPoseKeyFiltered(PoseCurves curves, float time, MixedRealityPose pose, float positionThreshold, float rotationThreshold)
        {
            AddFloatKeyFiltered(curves.PositionX, time, pose.Position.x, positionThreshold);
            AddFloatKeyFiltered(curves.PositionY, time, pose.Position.y, positionThreshold);
            AddFloatKeyFiltered(curves.PositionZ, time, pose.Position.z, positionThreshold);

            AddFloatKeyFiltered(curves.RotationX, time, pose.Rotation.x, rotationThreshold);
            AddFloatKeyFiltered(curves.RotationY, time, pose.Rotation.y, rotationThreshold);
            AddFloatKeyFiltered(curves.RotationZ, time, pose.Rotation.z, rotationThreshold);
            AddFloatKeyFiltered(curves.RotationW, time, pose.Rotation.w, rotationThreshold);
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

        public void CutoffBeforeTime(float time)
        {
            foreach (var curve in AnimationCurves)
            {
                CutoffBeforeTime(curve, time);
            }
        }

        private void CutoffBeforeTime(AnimationCurve curve, float time)
        {
            // Keep the keyframe before the cutoff time to ensure correct value at the beginning
            int idx0 = FindKeyframeInterval(curve, time);
            if (idx0 > 0)
            {
                Keyframe[] newKeys = new Keyframe[curve.keys.Length - idx0];
                for (int i = 0; i < newKeys.Length; ++i)
                {
                    newKeys[i] = curve.keys[idx0 + i];
                }
                curve.keys = newKeys;
            }
        }

        public void Clear()
        {
            foreach (var curve in AnimationCurves)
            {
                curve.keys = new Keyframe[0];
            }
        }

        internal class AnimationCurveEnumerable : IEnumerable<AnimationCurve>
        {
            private InputAnimation animation;

            public AnimationCurveEnumerable(InputAnimation animation)
            {
                this.animation = animation;
            }

            public IEnumerator<AnimationCurve> GetEnumerator()
            {
                yield return animation.handTrackedCurveLeft;
                yield return animation.handTrackedCurveRight;
                yield return animation.handPinchCurveLeft;
                yield return animation.handPinchCurveRight;
                foreach (var curves in animation.handJointCurvesLeft.Values)
                {
                    yield return curves.PositionX;
                    yield return curves.PositionY;
                    yield return curves.PositionZ;
                    yield return curves.RotationX;
                    yield return curves.RotationY;
                    yield return curves.RotationZ;
                    yield return curves.RotationW;
                }
                foreach (var curves in animation.handJointCurvesRight.Values)
                {
                    yield return curves.PositionX;
                    yield return curves.PositionY;
                    yield return curves.PositionZ;
                    yield return curves.RotationX;
                    yield return curves.RotationY;
                    yield return curves.RotationZ;
                    yield return curves.RotationW;
                }
                yield return animation.cameraCurves.PositionX;
                yield return animation.cameraCurves.PositionY;
                yield return animation.cameraCurves.PositionZ;
                yield return animation.cameraCurves.RotationX;
                yield return animation.cameraCurves.RotationY;
                yield return animation.cameraCurves.RotationZ;
                yield return animation.cameraCurves.RotationW;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private IEnumerable<AnimationCurve> AnimationCurves => new AnimationCurveEnumerable(this);

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

        /// <summary>
        /// Evaluate hand tracking state at the given time.
        /// </summary>
        public void EvaluateHandState(float time, Handedness handedness, out bool isTracked, out bool isPinching)
        {
            if (handedness == Handedness.Left)
            {
                EvaluateHandState(time, handTrackedCurveLeft, handPinchCurveLeft, out isTracked, out isPinching);
            }
            else if (handedness == Handedness.Right)
            {
                EvaluateHandState(time, handTrackedCurveRight, handPinchCurveRight, out isTracked, out isPinching);
            }
            else
            {
                isTracked = false;
                isPinching = false;
            }
        }

        /// Evaluate hand tracking state at the given time.
        private void EvaluateHandState(float time, AnimationCurve trackedCurve, AnimationCurve pinchCurve, out bool isTracked, out bool isPinching)
        {
            isTracked = (trackedCurve.Evaluate(time) > 0.5f);
            isPinching = (pinchCurve.Evaluate(time) > 0.5f);
        }

        /// <summary>
        /// Evaluate joint pose at the given time.
        /// </summary>
        public MixedRealityPose EvaluateHandJoint(float time, Handedness handedness, TrackedHandJoint joint)
        {
            if (handedness == Handedness.Left)
            {
                return EvaluateHandJoint(time, joint, handJointCurvesLeft);
            }
            else if (handedness == Handedness.Right)
            {
                return EvaluateHandJoint(time, joint, handJointCurvesRight);
            }
            else
            {
                return MixedRealityPose.ZeroIdentity;
            }
        }

        /// Evaluate joint pose at the given time.
        private MixedRealityPose EvaluateHandJoint(float time, TrackedHandJoint joint, Dictionary<TrackedHandJoint, PoseCurves> jointCurves)
        {
            if (jointCurves.TryGetValue(joint, out PoseCurves curves))
            {
                return EvaluatePose(curves, time);
            }
            else
            {
                return MixedRealityPose.ZeroIdentity;
            }
        }

        /// <summary>
        /// Evaluate the camera transform at the given time.
        /// </summary>
        public MixedRealityPose EvaluateCameraPose(float time)
        {
            return EvaluatePose(cameraCurves, time);
        }

        private static MixedRealityPose EvaluatePose(PoseCurves curves, float time)
        {
            float px = curves.PositionX.Evaluate(time);
            float py = curves.PositionY.Evaluate(time);
            float pz = curves.PositionZ.Evaluate(time);
            float rx = curves.RotationX.Evaluate(time);
            float ry = curves.RotationY.Evaluate(time);
            float rz = curves.RotationZ.Evaluate(time);
            float rw = curves.RotationW.Evaluate(time);

            var pose = new MixedRealityPose();
            pose.Position = new Vector3(px, py, pz);
            pose.Rotation = new Quaternion(rx, ry, rz, rw);
            pose.Rotation.Normalize();
            return pose;
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

        private void ComputeDuration()
        {
            duration = 0.0f;
            foreach (var curve in AnimationCurves)
            {
                float curveDuration = (curve.length > 0 ? curve.keys[curve.length - 1].time : 0.0f);
                duration = Mathf.Max(duration, curveDuration);
            }
        }

        /// <summary>
        /// Serialize animation data into a stream.
        /// </summary>
        public void ToStream(Stream stream)
        {
            PoseCurves defaultCurves = new PoseCurves();

            var writer = new BinaryWriter(stream);

            PoseCurvesToStream(writer, cameraCurves);

            InputAnimationSerializationUtils.WriteBoolCurve(writer, handTrackedCurveLeft);
            InputAnimationSerializationUtils.WriteBoolCurve(writer, handTrackedCurveRight);
            InputAnimationSerializationUtils.WriteBoolCurve(writer, handPinchCurveLeft);
            InputAnimationSerializationUtils.WriteBoolCurve(writer, handPinchCurveRight);

            for (int i = 0; i < jointCount; ++i)
            {
                if (!handJointCurvesLeft.TryGetValue((TrackedHandJoint)i, out PoseCurves curves))
                {
                    curves = defaultCurves;
                }
                PoseCurvesToStream(writer, curves);
            }
            for (int i = 0; i < jointCount; ++i)
            {
                if (!handJointCurvesRight.TryGetValue((TrackedHandJoint)i, out PoseCurves curves))
                {
                    curves = defaultCurves;
                }
                PoseCurvesToStream(writer, curves);
            }

            InputAnimationSerializationUtils.WriteMarkerList(writer, markers);
        }

        /// <summary>
        /// Deserialize animation data from a stream.
        /// </summary>
        public void FromStream(Stream stream)
        {
            var reader = new BinaryReader(stream);

            PoseCurvesFromStream(reader, cameraCurves);

            InputAnimationSerializationUtils.ReadBoolCurve(reader, handTrackedCurveLeft);
            InputAnimationSerializationUtils.ReadBoolCurve(reader, handTrackedCurveRight);
            InputAnimationSerializationUtils.ReadBoolCurve(reader, handPinchCurveLeft);
            InputAnimationSerializationUtils.ReadBoolCurve(reader, handPinchCurveRight);

            for (int i = 0; i < jointCount; ++i)
            {
                if (!handJointCurvesLeft.TryGetValue((TrackedHandJoint)i, out PoseCurves curves))
                {
                    curves = new PoseCurves();
                    handJointCurvesLeft.Add((TrackedHandJoint)i, curves);
                }
                PoseCurvesFromStream(reader, curves);
            }
            for (int i = 0; i < jointCount; ++i)
            {
                if (!handJointCurvesRight.TryGetValue((TrackedHandJoint)i, out PoseCurves curves))
                {
                    curves = new PoseCurves();
                    handJointCurvesRight.Add((TrackedHandJoint)i, curves);
                }
                PoseCurvesFromStream(reader, curves);
            }

            InputAnimationSerializationUtils.ReadMarkerList(reader, markers);

            ComputeDuration();
        }

        private static void PoseCurvesToStream(BinaryWriter writer, PoseCurves curves)
        {
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.PositionX);
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.PositionY);
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.PositionZ);

            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.RotationX);
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.RotationY);
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.RotationZ);
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.RotationW);
        }

        private static void PoseCurvesFromStream(BinaryReader reader, PoseCurves curves)
        {
            InputAnimationSerializationUtils.ReadFloatCurve(reader, curves.PositionX);
            InputAnimationSerializationUtils.ReadFloatCurve(reader, curves.PositionY);
            InputAnimationSerializationUtils.ReadFloatCurve(reader, curves.PositionZ);

            InputAnimationSerializationUtils.ReadFloatCurve(reader, curves.RotationX);
            InputAnimationSerializationUtils.ReadFloatCurve(reader, curves.RotationY);
            InputAnimationSerializationUtils.ReadFloatCurve(reader, curves.RotationZ);
            InputAnimationSerializationUtils.ReadFloatCurve(reader, curves.RotationW);
        }
    }
}