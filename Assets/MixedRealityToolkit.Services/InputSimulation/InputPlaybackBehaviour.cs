// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Input
{
    // /// <summary>
    // /// Uses input animation data to simulate camera and hand movement via the input simulation service.
    // /// </summary>
    // public class InputPlaybackBehaviour : PlayableBehaviour
    // {
    //     public InputAnimation InputAnimation = null;

    //     public override void OnPlayableDestroy(Playable playable)
    //     {
    //         if (Application.isPlaying)
    //         {
    //             var inputSimService = MixedRealityToolkit.Instance.GetService<IInputSimulationService>();
    //             if (inputSimService != null)
    //             {
    //                 inputSimService.UserInputEnabled = true;
    //             }
    //         }
    //     }

    //     public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    //     {
    //         // Only supported during play mode
    //         if (!Application.isPlaying)
    //         {
    //             return;
    //         }
    //         if (InputAnimation == null)
    //         {
    //             return;
    //         }

    //         float currentTime = (float)playable.GetTime();
    //         // InputAnimationUtils.ApplyInputAnimation(InputAnimation, currentTime);
    //     }

    //     // /// <summary>
    //     // /// Evaluate input animation and apply it using the input simulation service.
    //     // /// </summary>
    //     // public static void ApplyInputAnimation(InputAnimation animation, float time)
    //     // {
    //     //     if (InputSimService != null)
    //     //     {
    //     //         InputSimService.UserInputEnabled = false;

    //     //         animation.EvaluateHandData(time, Handedness.Left, InputSimService.HandDataLeft);
    //     //         animation.EvaluateHandData(time, Handedness.Right, InputSimService.HandDataRight);

    //     //         if (CameraCache.Main)
    //     //         {
    //     //             MixedRealityPose cameraPose = animation.EvaluateCameraPose(time);
    //     //             CameraCache.Main.transform.SetPositionAndRotation(cameraPose.Position, cameraPose.Rotation);
    //     //         }
    //     //     }
    //     // }
    // }
}