// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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

        /// <summary>
        /// Get animation curves for the pose of the given hand joint, if they exist.
        /// </summary>
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
            curves = null;
            return false;
        }

        /// <summary>
        /// Make sure the pose animation curves for the given hand joint exist.
        /// </summary>
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

        /// Add a pose keyframe to an animation curve.
        /// Keys are only added if the value changes sufficiently.
        private static void AddPoseKeyFiltered(PoseCurves curves, float time, MixedRealityPose pose, float positionThreshold, float rotationThreshold)
        {
            AddPositionKeyFiltered(curves.PositionX, curves.PositionY, curves.PositionZ, time, pose.Position, positionThreshold);
            AddRotationKeyFiltered(curves.RotationX, curves.RotationY, curves.RotationZ, curves.RotationW, time, pose.Rotation, rotationThreshold);
        }

        // Add a vector keyframe to animation curve if the threshold distance to the previous value is exceeded.
        // Otherwise replace the last keyframe instead of adding a new one.
        private static void AddPositionKeyFiltered(AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float time, Vector3 position, float threshold)
        {
            float sqrThreshold = threshold * threshold;

            int iX = FindKeyframeInterval(curveX, time);
            int iY = FindKeyframeInterval(curveY, time);
            int iZ = FindKeyframeInterval(curveZ, time);
            if (iX > 0 && iY > 0 && iZ > 0)
            {
                Vector3 v0 = new Vector3(curveX.keys[iX - 1].value, curveY.keys[iY - 1].value, curveZ.keys[iZ - 1].value);
                Vector3 v1 = new Vector3(curveX.keys[iX].value, curveY.keys[iY].value, curveZ.keys[iZ].value);

                // Merge the preceding two intervals if difference is small enough
                if ((v1 - v0).sqrMagnitude <= sqrThreshold && (position - v1).sqrMagnitude <= sqrThreshold)
                {
                    curveX.RemoveKey(iX);
                    curveY.RemoveKey(iY);
                    curveZ.RemoveKey(iZ);
                }
            }

            AddFloatKey(curveX, time, position.x);
            AddFloatKey(curveY, time, position.y);
            AddFloatKey(curveZ, time, position.z);
        }

        // Add a quaternion keyframe to animation curve if the threshold angular distance to the previous value is exceeded.
        // Otherwise replace the last keyframe instead of adding a new one.
        private static void AddRotationKeyFiltered(AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, AnimationCurve curveW, float time, Quaternion rotation, float threshold)
        {
            float sqrThreshold = threshold * threshold;

            int iX = FindKeyframeInterval(curveX, time);
            int iY = FindKeyframeInterval(curveY, time);
            int iZ = FindKeyframeInterval(curveZ, time);
            int iW = FindKeyframeInterval(curveW, time);
            if (iX > 0 && iY > 0 && iZ > 0 && iW > 0)
            {
                Quaternion q0 = new Quaternion(curveX.keys[iX - 1].value, curveY.keys[iY - 1].value, curveZ.keys[iZ - 1].value, curveW.keys[iW - 1].value);
                Quaternion q1 = new Quaternion(curveX.keys[iX].value, curveY.keys[iY].value, curveZ.keys[iZ].value, curveW.keys[iW].value);

                // Merge the preceding two intervals if difference is small enough
                (q0 * Quaternion.Inverse(q1)).ToAngleAxis(out float angle0, out Vector3 axis0);
                (rotation * Quaternion.Inverse(q0)).ToAngleAxis(out float angle1, out Vector3 axis1);
                if (angle0 <= sqrThreshold && angle1 <= sqrThreshold)
                {
                    curveX.RemoveKey(iX);
                    curveY.RemoveKey(iY);
                    curveZ.RemoveKey(iZ);
                    curveW.RemoveKey(iW);
                }
            }

            AddFloatKey(curveX, time, rotation.x);
            AddFloatKey(curveY, time, rotation.y);
            AddFloatKey(curveZ, time, rotation.z);
            AddFloatKey(curveW, time, rotation.w);
        }

        /// Add a float value to an animation curve.
        /// Returns the index of the newly added keyframe.
        private static int AddFloatKey(AnimationCurve curve, float time, float value)
        {
            // Use linear interpolation by setting tangents and weights to zero.
            var keyframe = new Keyframe(time, value, 0.0f, 0.0f, 0.0f, 0.0f);
            keyframe.weightedMode = WeightedMode.Both;
            return curve.AddKey(keyframe);
        }

        /// Arbitrarily large weight for representing a boolean value in float curves.
        const float boolOutWeight = 1.0e6f;

        /// Utility function that creates a non-interpolated keyframe suitable for boolean values.
        /// Returns the index of the newly added keyframe.
        private static int AddBoolKey(AnimationCurve curve, float time, bool value)
        {
            float fvalue = value ? 1.0f : 0.0f;
            // Set tangents and weights such than the the input value is cut off and out tangent is constant.
            var keyframe = new Keyframe(time, fvalue, 0.0f, 0.0f, 0.0f, boolOutWeight);
            keyframe.weightedMode = WeightedMode.Both;

            return curve.AddKey(keyframe);
        }

        /// Utility function that creates a non-interpolated keyframe suitable for boolean values.
        /// Keys are only added if the value changes.
        /// Returns the index of the newly added keyframe, or -1 if no keyframe has been added.
        private static int AddBoolKeyFiltered(AnimationCurve curve, float time, bool value)
        {
            float fvalue = value ? 1.0f : 0.0f;
            // Set tangents and weights such than the the input value is cut off and out tangent is constant.
            var keyframe = new Keyframe(time, fvalue, 0.0f, 0.0f, 0.0f, boolOutWeight);
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
        /// Remove all keyframes from all animation curves with time values before the given cutoff time.
        /// </summary>
        /// <remarks>
        /// If keyframes exists before the cutoff time then one preceding keyframe will be retained,
        /// so that interpolation at the cutoff time yields the same result.
        /// </remarks>
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

        /// <summary>
        /// Remove all keyframes from all animation curves.
        /// </summary>
        public void Clear()
        {
            foreach (var curve in AnimationCurves)
            {
                curve.keys = new Keyframe[0];
            }
        }

        // Helper class to enumerate all curves in the input animation
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
        /// Find an index i in the sorted events list, such that events[i].time &lt;= time &lt; events[i+1].time.
        /// </summary>
        /// <returns>
        /// 0 &lt;= i &lt; eventCount if a full interval could be found.
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
        public void ToStream(Stream stream, float startTime)
        {
            PoseCurves defaultCurves = new PoseCurves();

            var writer = new BinaryWriter(stream);

            InputAnimationSerializationUtils.WriteHeader(writer);

            PoseCurvesToStream(writer, cameraCurves, startTime);

            InputAnimationSerializationUtils.WriteBoolCurve(writer, handTrackedCurveLeft, startTime);
            InputAnimationSerializationUtils.WriteBoolCurve(writer, handTrackedCurveRight, startTime);
            InputAnimationSerializationUtils.WriteBoolCurve(writer, handPinchCurveLeft, startTime);
            InputAnimationSerializationUtils.WriteBoolCurve(writer, handPinchCurveRight, startTime);

            for (int i = 0; i < jointCount; ++i)
            {
                if (!handJointCurvesLeft.TryGetValue((TrackedHandJoint)i, out PoseCurves curves))
                {
                    curves = defaultCurves;
                }
                PoseCurvesToStream(writer, curves, startTime);
            }
            for (int i = 0; i < jointCount; ++i)
            {
                if (!handJointCurvesRight.TryGetValue((TrackedHandJoint)i, out PoseCurves curves))
                {
                    curves = defaultCurves;
                }
                PoseCurvesToStream(writer, curves, startTime);
            }

            InputAnimationSerializationUtils.WriteMarkerList(writer, markers, startTime);
        }

        /// <summary>
        /// Deserialize animation data from a stream.
        /// </summary>
        public void FromStream(Stream stream)
        {
            var reader = new BinaryReader(stream);

            InputAnimationSerializationUtils.ReadHeader(reader, out int versionMajor, out int versionMinor);
            if (versionMajor != 1 || versionMinor != 0)
            {
                Debug.LogError("Only version 1.0 of input animation file format is supported.");
                return;
            }

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

        private static void PoseCurvesToStream(BinaryWriter writer, PoseCurves curves, float startTime)
        {
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.PositionX, startTime);
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.PositionY, startTime);
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.PositionZ, startTime);

            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.RotationX, startTime);
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.RotationY, startTime);
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.RotationZ, startTime);
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curves.RotationW, startTime);
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