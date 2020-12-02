// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Container used to efficiently store a sequence of input animation keyframes while recording
    /// </summary>
    public class InputRecordingBuffer : IEnumerable<InputRecordingBuffer.Keyframe>
    {
        /// <summary>
        /// The input state for a single frame
        /// </summary>
        public class Keyframe
        {
            public float Time { get; set; }
            public bool LeftTracked { get; set; }
            public bool RightTracked { get; set; }
            public bool LeftPinch { get; set; }
            public bool RightPinch { get; set; }
            public MixedRealityPose CameraPose { get; set; }
            public Ray GazeRay { get; set; }
            public Dictionary<TrackedHandJoint, MixedRealityPose> LeftJoints { get; set; }
            public Dictionary<TrackedHandJoint, MixedRealityPose> RightJoints { get; set; }

            public Keyframe(float time)
            {
                Time = time;
                LeftJoints = new Dictionary<TrackedHandJoint, MixedRealityPose>();
                RightJoints = new Dictionary<TrackedHandJoint, MixedRealityPose>();
            }
        }

        /// <summary>
        /// The time of the first keyframe in the buffer
        /// </summary>
        public float StartTime => keyframes.Peek().Time;
        
        private Keyframe currentKeyframe;
        private Queue<Keyframe> keyframes;

        /// <summary>
        /// Default constructor
        /// </summary>
        public InputRecordingBuffer() => keyframes = new Queue<Keyframe>();

        /// <summary>
        /// Removes all keyframes from the buffer
        /// </summary>
        public void Clear()
        {
            keyframes.Clear();
            currentKeyframe = null;
        }

        /// <summary>
        /// Removes all keyframes before a given time
        /// </summary>
        public void RemoveBeforeTime(float time)
        {
            while (keyframes.Count > 0 && keyframes.Peek().Time < time)
            {
                keyframes.Dequeue();
            }
        }

        /// <summary>
        /// Sets the camera pose to be stored in the newest keyframe
        /// </summary>
        public void SetCameraPose(MixedRealityPose pose) => currentKeyframe.CameraPose = pose;

        /// <summary>
        /// Sets the eye gaze ray to be stored in the newest keyframe
        /// </summary>
        public void SetGazeRay(Ray ray) => currentKeyframe.GazeRay = ray;

        /// <summary>
        /// Sets the state of a given hand to be stored in the newest keyframe
        /// </summary>
        public void SetHandState(Handedness handedness, bool tracked, bool pinching)
        {
            if (handedness == Handedness.Left)
            {
                currentKeyframe.LeftTracked = tracked;
                currentKeyframe.LeftPinch = pinching;
            }
            else
            {
                currentKeyframe.RightTracked = tracked;
                currentKeyframe.RightPinch = pinching;
            }
        }

        /// <summary>
        /// Sets the pose of a given joint to be stored in the newest keyframe
        /// </summary>
        public void SetJointPose(Handedness handedness, TrackedHandJoint joint, MixedRealityPose pose)
        {
            if (handedness == Handedness.Left)
            {
                currentKeyframe.LeftJoints.Add(joint, pose);
            }
            else
            {
                currentKeyframe.RightJoints.Add(joint, pose);
            }
        }

        /// <summary>
        /// Creates a new, empty keyframe with a given time at the end of the buffer
        /// </summary>
        /// <returns>The index of the new keyframe</returns>
        public int NewKeyframe(float time)
        {
            currentKeyframe = new Keyframe(time);
            keyframes.Enqueue(currentKeyframe);

            return keyframes.Count - 1;
        }

        /// <summary>
        /// Returns a sequence of all keyframes in the buffer
        /// </summary>
        public IEnumerator<Keyframe> GetEnumerator() => keyframes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}