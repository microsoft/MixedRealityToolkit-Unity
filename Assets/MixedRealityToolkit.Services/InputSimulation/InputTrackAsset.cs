// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Settings for behaviour of the playable during play mode.
    /// </summary>
    [System.Serializable]
    public class InputAnimationRecordingSettings
    {
        /// Minimum time between keyframes.
        public float epsilonTime = 0.1f;
        /// Minimum movement of hand joints to record a keyframe.
        public float epsilonJointPositions = 0.01f;
        /// Minimum movement of the camera to record a keyframe.
        public float epsilonCameraPosition = 0.05f;
        /// Minimum rotation angle of the camera to record a keyframe.
        public float epsilonCameraRotation = Mathf.Deg2Rad * 2.0f;
    }

    /// <summary>
    /// Utility component to record an InputAnimation.
    /// </summary>
    internal class InputAnimationRecorder : MonoBehaviour
    {
        public InputAnimationRecordingSettings settings = null;

        public InputAnimation inputAnimation;

        private double currentTime;

        public void Awake()
        {
            inputAnimation = new InputAnimation();
            currentTime = 0.0;
        }

        public void Update()
        {
            currentTime += Time.deltaTime;
            InputAnimationUtils.RecordKeyframeFiltered(
                inputAnimation,
                currentTime,
                settings.epsilonTime,
                settings.epsilonJointPositions,
                settings.epsilonCameraPosition,
                settings.epsilonCameraRotation);
        }
    }

    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input Track Asset", fileName = "InputTrackAsset", order = 100)]
    [TrackClipTypeAttribute(typeof(InputAnimationAsset))]
    public class InputTrackAsset : TrackAsset
    {
        /// <summary>
        /// Settings for recording new input animation.
        /// </summary>
        public InputAnimationRecordingSettings RecordingSettings;

        /// <summary>
        /// Input animation clip currently being recorded.
        /// </summary>
        internal InputAnimationRecorder recorder = null;

        public bool IsRecording => (recorder != null);

        public bool StartRecording()
        {
            var mrtk = MixedRealityToolkit.Instance;
            if (!mrtk)
            {
                return false;
            }

            recorder = mrtk.gameObject.AddComponent<InputAnimationRecorder>();
            recorder.settings = RecordingSettings;

            return true;
        }

        public void StopRecording()
        {
            var clip = CreateClip<InputAnimationAsset>();
            var asset = clip.asset as InputAnimationAsset;
            asset.InputAnimation = recorder.inputAnimation;

            clip.duration = asset.duration;

            GameObject.Destroy(recorder);
        }
    }
}