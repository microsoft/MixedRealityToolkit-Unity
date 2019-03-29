// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class InputAnimationRecordingBehaviour : PlayableBehaviour
    {
        /// <summary>
        /// Settings for behaviour of the playable during play mode.
        /// </summary>
        public InputAnimationRecordingSettings Settings;

        public InputTestAsset InputTest = null;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // Only supported during play mode
            if (!Application.isPlaying)
            {
                return;
            }
            if (InputTest == null)
            {
                return;
            }

            double currentTime = playable.GetTime();
            InputTestAnimationUtils.RecordInputTestKeyframeFiltered(
                InputTest.InputAnimation,
                currentTime,
                Settings.epsilonTime,
                Settings.epsilonJointPositions,
                Settings.epsilonCameraPosition,
                Settings.epsilonCameraRotation);

            EditorUtility.SetDirty(InputTest);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (Application.isPlaying)
            {
                AssetDatabase.SaveAssets();
            }
        }
    }
}