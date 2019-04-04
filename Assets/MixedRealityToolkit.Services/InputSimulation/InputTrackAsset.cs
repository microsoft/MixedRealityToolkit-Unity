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
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input Track Asset", fileName = "InputTrackAsset", order = 100)]
    [TrackClipTypeAttribute(typeof(InputAnimationAsset))]
    public class InputTrackAsset : TrackAsset
    {
        /// <summary>
        /// Settings for behaviour of the playable during play mode.
        /// </summary>
        public InputAnimationRecordingSettings RecordingSettings;

        /// <summary>
        /// Input animation clip currently being recorded.
        /// </summary>
        public InputAnimationAsset recordingClip = null;

        // /// Use input recording behavior when playable is created.
        // private bool useInputRecording = false;

        // /// </inheritdoc>
        // public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        // {
        //     if (Application.isPlaying)
        //     {
        //         if (useInputRecording)
        //         {
        //             var playable = ScriptPlayable<InputRecordingBehaviour>.Create(graph);
        //             var behaviour = playable.GetBehaviour();
        //             behaviour.Asset = this;
        //             behaviour.Settings = RecordingSettings;

        //             // Only create recording playable once
        //             useInputRecording = false;

        //             return playable;
        //         }
        //         else if (InputAnimation.keyframeCount > 0)
        //         {
        //             var playable = ScriptPlayable<InputPlaybackBehaviour>.Create(graph);
        //             var behaviour = playable.GetBehaviour();
        //             behaviour.InputAnimation = InputAnimation;
        //             return playable;
        //         }
        //     }
        //     return Playable.Null;
        // }

        // public void EnableInputRecording()
        // {
        //     useInputRecording = true;
        // }
    }
}