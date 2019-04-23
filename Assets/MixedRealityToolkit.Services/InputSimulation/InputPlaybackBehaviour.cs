// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class InputPlaybackBehaviour : PlayableBehaviour
    {
        public InputAnimation InputAnimation = null;

        public override void OnPlayableDestroy(Playable playable)
        {
            if (Application.isPlaying)
            {
                var inputSimService = MixedRealityToolkit.Instance.GetService<IInputSimulationService>();
                if (inputSimService != null)
                {
                    inputSimService.UserInputEnabled = true;
                }
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // Only supported during play mode
            if (!Application.isPlaying)
            {
                return;
            }
            if (InputAnimation == null)
            {
                return;
            }

            double currentTime = playable.GetTime();
            InputAnimationUtils.ApplyInputAnimation(InputAnimation, currentTime);
        }
    }
}