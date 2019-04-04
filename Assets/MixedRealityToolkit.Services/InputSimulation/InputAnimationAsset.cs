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
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input Animation Asset", fileName = "InputAnimationAsset", order = 100)]
    public class InputAnimationAsset : ScriptableObject, IPlayableAsset
    {
        /// </inheritdoc>
        public double duration => (InputAnimation != null && InputAnimation.keyframeCount > 0) ? InputAnimation.GetTime(InputAnimation.keyframeCount - 1) : 0.0;

        public IEnumerable<PlayableBinding> outputs => null;

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

        /// </inheritdoc>
        public Playable CreatePlayable(PlayableGraph graph, GameObject owner)
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