// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Devices.Hands;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Services.InputSimulation
{
    public class InputPlaybackBehaviour : PlayableBehaviour
    {
        public InputTestAnimation InputAnimation = null;

        public override void OnPlayableDestroy(Playable playable)
        {
            if (Application.isPlaying)
            {
                var inputSimService = MixedRealityToolkit.Instance.GetService<InputSimulationService>();
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
            InputTestAnimationUtils.ApplyInputTestAnimation(InputAnimation, currentTime);
        }
    }
}