// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class InputTestingBehaviour : InputPlaybackBehaviour
    {
        public Dictionary<InputTestPropertyKey, InputTestCurve<object>> ExpectedValues = null;
        public GameObject TestedObject = null;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // Only supported during play mode
            if (!Application.isPlaying)
            {
                return;
            }

            base.ProcessFrame(playable, info, playerData);
        }
    }
}