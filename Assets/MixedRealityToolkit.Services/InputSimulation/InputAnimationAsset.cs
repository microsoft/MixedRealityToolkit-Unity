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
    /// Utility component to record an InputAnimation.
    /// </summary>
    internal class InputAnimationRecorder : MonoBehaviour
    {
        public InputRecordingSettings settings = null;

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
                settings.EpsilonTime,
                settings.EpsilonJointPositions,
                settings.EpsilonCameraPosition,
                settings.EpsilonCameraRotation);
        }
    }

    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input Animation Asset", fileName = "InputAnimationAsset", order = 100)]
    public class InputAnimationAsset : PlayableAsset
    {
        /// </inheritdoc>
        public override double duration => (InputAnimation != null ? InputAnimation.Duration : 0.0);

        /// <summary>
        /// Input animation data.
        /// </summary>
        [SerializeField]
        private InputAnimation inputAnimation = new InputAnimation();
        public InputAnimation InputAnimation => inputAnimation;

        /// Utility class that records input animation.
        internal InputAnimationRecorder recorder = null;

        /// <summary>
        /// True if new input animation is being recorded.
        /// </summary>
        public bool IsRecording => (recorder != null);

        /// <summary>
        /// New input animation data that is currently being recorded.
        /// </summary>
        public InputAnimation RecordingInputAnimation => (recorder ? recorder.inputAnimation : null);

        /// </inheritdoc>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            if (Application.isPlaying && !IsRecording)
            {
                var playable = ScriptPlayable<InputPlaybackBehaviour>.Create(graph);
                var behaviour = playable.GetBehaviour();
                behaviour.InputAnimation = InputAnimation;
                return playable;
            }
            return Playable.Null;
        }

        public bool StartRecording()
        {
            var mrtk = MixedRealityToolkit.Instance;
            if (!mrtk)
            {
                return false;
            }

            var inputSimService = mrtk.GetService<IInputSimulationService>();
            if (inputSimService == null)
            {
                return false;
            }

            var profile = inputSimService.InputSimulationProfile;
            if (!profile)
            {
                return false;
            }

            recorder = mrtk.gameObject.AddComponent<InputAnimationRecorder>();
            recorder.settings = profile.RecordingSettings;

            return true;
        }

        public void StopRecording()
        {
            inputAnimation = recorder.inputAnimation;

            GameObject.Destroy(recorder);
        }
    }
}