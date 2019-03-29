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

    // [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input Test Asset", fileName = "InputTestAsset", order = 100)]
    public class InputTestAsset : PlayableAsset
    {
        /// </inheritdoc>
        public override double duration => (InputAnimation != null) ? Time.fixedUnscaledDeltaTime * InputAnimation.keyframeCount : 0.0;

        /// <summary>
        /// Settings for behaviour of the playable during play mode.
        /// </summary>
        public InputAnimationRecordingSettings RecordingSettings;

        /// <summary>
        /// Test the tested object against expected values.
        /// </summary>
        public bool TestsEnabled = true;

        /// <summary>
        /// The object whose properties will be recorded and tested on playback.
        /// </summary>
        public ExposedReference<GameObject> TestedObject;

        /// <summary>
        /// Controller input animation data.
        /// </summary>
        [SerializeField]
        private InputTestAnimation inputAnimation = new InputTestAnimation();
        public InputTestAnimation InputAnimation => inputAnimation;
        /// <summary>
        /// Recorded values of the tested object that are expected during playback.
        /// </summary>
        [SerializeField]
        private InputTestExpectedValueMap expectedValues = new InputTestExpectedValueMap();
        public InputTestExpectedValueMap ExpectedValues => expectedValues;

        /// Use input recording behavior when playable is created.
        private bool useInputRecording = false;
        /// Use test recording behavior when playable is created.
        private bool useTestRecording = false;

        /// </inheritdoc>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            if (useInputRecording && Application.isPlaying)
            {
                var playable = ScriptPlayable<InputAnimationRecordingBehaviour>.Create(graph);
                var behaviour = playable.GetBehaviour();
                behaviour.InputTest = this;
                behaviour.Settings = RecordingSettings;

                // Only create recording playable once
                useInputRecording = false;
                useTestRecording = false;

                return playable;
            }
            else if (useTestRecording && Application.isPlaying)
            {
                var playable = ScriptPlayable<InputTestRecordingBehaviour>.Create(graph);
                var behaviour = playable.GetBehaviour();
                behaviour.InputTest = this;
                behaviour.TestedObject = TestedObject.Resolve(graph.GetResolver());

                // Only create recording playable once
                useInputRecording = false;
                useTestRecording = false;

                return playable;
            }
            else if (TestsEnabled)
            {
                var playable = ScriptPlayable<InputTestingBehaviour>.Create(graph);
                var behaviour = playable.GetBehaviour();
                behaviour.InputAnimation = InputAnimation;
                behaviour.ExpectedValues = ExpectedValues;
                behaviour.TestedObject = TestedObject.Resolve(graph.GetResolver());
                return playable;
            }
            else
            {
                var playable = ScriptPlayable<InputPlaybackBehaviour>.Create(graph);
                var behaviour = playable.GetBehaviour();
                behaviour.InputAnimation = InputAnimation;
                return playable;
            }
        }

        public void EnableInputRecording()
        {
            useInputRecording = true;
        }

        public void EnableTestRecording()
        {
            useTestRecording = true;
        }
    }
}