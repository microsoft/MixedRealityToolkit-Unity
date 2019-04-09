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
        public override double duration => (InputAnimation != null && InputAnimation.keyframeCount > 0) ? InputAnimation.GetTime(InputAnimation.keyframeCount - 1) : 0.0;

        /// <summary>
        /// Controller input animation data.
        /// </summary>
        [SerializeField]
        private InputAnimation inputAnimation = new InputAnimation();
        public InputAnimation InputAnimation
        {
            get { return inputAnimation; }
            set { inputAnimation = value; }
        }

        /// <summary>
        /// Input animation clip currently being recorded.
        /// </summary>
        internal InputAnimationRecorder recorder = null;

        public bool IsRecording => (recorder != null);

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

            var inputSimService = mrtk.GetService<InputSimulationService>();
            if (inputSimService == null)
            {
                return false;
            }

            var profile = inputSimService.GetInputSimulationProfile();
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