﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    public class TestFixture_01_ArticulatedHands
    {
        // [UnityTest]
        // public IEnumerator Test01_TestSimpleSphere()
        // {
        //     TestUtilities.InitializeMixedRealityToolkitScene(true);

        //     var playspace = MixedRealityToolkit.Instance.MixedRealityPlayspace;
        //     playspace.transform.position = new Vector3(1.0f, 1.5f, -2.0f);
        //     playspace.transform.LookAt(Vector3.zero);

        //     var inputSimService = MixedRealityToolkit.Instance.GetService<InputSimulationService>();
        //     // Disable keyboard and mouse input
        //     inputSimService.UserInputEnabled = false;
        //     inputSimService.Update();
        //     var handLeft = inputSimService.HandStateLeft;
        //     var handRight = inputSimService.HandStateRight;

        //     handLeft.IsTracked = true;
        //     handRight.IsTracked = true;

        //     handLeft.SetViewportPosition(new Vector3(0.3f, 0.5f, 0.5f));
        //     handRight.SetViewportPosition(new Vector3(0.7f, 0.5f, 0.5f));

        //     var testObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //     testObject.transform.localScale = Vector3.one * 0.1f;

        //     var stopwatch = new System.Diagnostics.Stopwatch();
        //     stopwatch.Start();
        //     while (stopwatch.Elapsed.TotalSeconds < 10.0f)
        //     {
        //         float t = (float)stopwatch.Elapsed.TotalSeconds;
        //         var pi2 = 2.0f*Mathf.PI;

        //         if (t > 5.0f)
        //         {
        //             handRight.Gesture = SimulatedHandPose.GestureId.ThumbsUp;
        //         }
        //         handLeft.Gesture = (t % 1.5f < 0.5f) ? SimulatedHandPose.GestureId.Open : SimulatedHandPose.GestureId.Grab;

        //         handLeft.SetViewportPosition(new Vector3(0.3f, 0.5f, 0.5f + 0.2f * Mathf.Sin(t * pi2)));
        //         handRight.SetViewportPosition(new Vector3(0.7f, 0.5f, 0.5f));

        //         var a = 0.5f * t;
        //         var b = 0.75f * t;
        //         var c = 0.9f * t;
        //         testObject.transform.position = new Vector3(
        //             Mathf.Sin((a + 0.0f) * pi2),
        //             Mathf.Sin((b + 0.333f) * pi2),
        //             Mathf.Sin((c + 0.666f) * pi2)
        //         );
        //         yield return new WaitForFixedUpdate();
        //     }
        //     stopwatch.Stop();
        // }

        [UnityTest]
        public IEnumerator Test02_TestControllerSequence()
        {
            var loadOp = TestUtilities.LoadTestSceneAsync("TestInputSystemGGV");
            while (loadOp.MoveNext())
            {
                yield return new WaitForFixedUpdate();
            }

            var playOp = TestUtilities.RunPlayableGraphAsync();
            while (playOp.MoveNext())
            {
                // INSERT TEST CONDITIONS HERE

                yield return new WaitForFixedUpdate();
            }
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}