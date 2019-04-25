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
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Input Animation", fileName = "MixedRealityInputAnimation", order = 100)]
    public class InputAnimationAsset : PlayableAsset
    {
        /// </inheritdoc>
        public override double duration => (InputAnimation != null ? InputAnimation.Duration : 0.0);

        /// <summary>
        /// Input animation data.
        /// </summary>
        [SerializeField]
        private InputAnimation inputAnimation = new InputAnimation();
        public InputAnimation InputAnimation
        {
            get
            {
                return inputAnimation;
            }
            set
            {
                inputAnimation = value;
            }
        }

        /// </inheritdoc>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            if (Application.isPlaying)
            {
                var playable = ScriptPlayable<InputPlaybackBehaviour>.Create(graph);
                var behaviour = playable.GetBehaviour();
                behaviour.InputAnimation = InputAnimation;
                return playable;
            }
            return Playable.Null;
        }
    }
}