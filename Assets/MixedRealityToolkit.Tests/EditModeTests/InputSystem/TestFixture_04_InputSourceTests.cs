// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Services;
using Microsoft.MixedReality.Toolkit.Services.InputSimulation;
using Microsoft.MixedReality.Toolkit.Services.InputSystem;
using Microsoft.MixedReality.Toolkit.Tests.Services;
using NUnit.Framework;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    public abstract class TestEventHandler : MonoBehaviour
    {
        public bool IsEventRaised = false;
    }

    public class EventTester<T> : IDisposable where T : TestEventHandler
    {
        public T EventHandler;

        public EventTester()
        {
            var gameObject = new GameObject();
            EventHandler = gameObject.AddComponent<T>();
            MixedRealityToolkit.InputSystem.Register(gameObject);
        }

        public void Dispose()
        {
            MixedRealityToolkit.InputSystem.Unregister(EventHandler.gameObject);
            GameObject.DestroyImmediate(EventHandler.gameObject);
        }

        public bool TestEventRaised(float timeout)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.Elapsed.Seconds < timeout)
            {
                if (EventHandler.IsEventRaised)
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Controller properties to examine when testing.
    /// </summary>
    public class ControllerTester
    {
        public Handedness handedness;

        public ControllerTester(Handedness handedness = Handedness.Any)
        {
            this.handedness = handedness;
        }

        public bool IsValid(IMixedRealityController controller)
        {
            // All controller handedness values must be included
            if (controller.ControllerHandedness != (controller.ControllerHandedness & handedness))
            {
                return false;
            }
            return true;
        }
    }

    public class SourceDetectedHandler : TestEventHandler, IMixedRealitySourceStateHandler
    {
        public readonly ControllerTester ControllerTester = new ControllerTester();

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            if (ControllerTester.IsValid(eventData.Controller))
            {
                IsEventRaised = true;
            }
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {}
    }

    public class SourceLostHandler : TestEventHandler, IMixedRealitySourceStateHandler
    {
        public readonly ControllerTester ControllerTester = new ControllerTester();

        public void OnSourceDetected(SourceStateEventData eventData)
        {}

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (ControllerTester.IsValid(eventData.Controller))
            {
                IsEventRaised = true;
            }
        }
    }

    public class TestFixture_04_InputSourceTests
    {
        public float SourceEventTimeout = 0.1f;

        [Test]
        public void Test01_TestSourceDetected()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(true);

            var inputSimService = MixedRealityToolkit.Instance.GetService<InputSimulationService>();
            inputSimService.Update();

            // using (var tester = new EventTester<SourceDetectedHandler>())
            // {
            //     tester.EventHandler.ControllerTester.handedness = Handedness.Left;

            //     inputSimService.HandStateLeft.IsTracked = true;
            //     inputSimService.Update();
            //     Assert.True(tester.TestEventRaised(SourceEventTimeout));
            // }

            // using (var tester = new EventTester<SourceLostHandler>())
            // {
            //     tester.EventHandler.ControllerTester.handedness = Handedness.Left;

            //     inputSimService.HandStateLeft.IsTracked = false;
            //     inputSimService.Update();
            //     Assert.True(tester.TestEventRaised(SourceEventTimeout));
            // }

            // using (var tester = new EventTester<SourceDetectedHandler>())
            // {
            //     tester.EventHandler.ControllerTester.handedness = Handedness.Right;

            //     inputSimService.HandStateRight.IsTracked = true;
            //     inputSimService.Update();
            //     Assert.True(tester.TestEventRaised(SourceEventTimeout));
            // }

            // using (var tester = new EventTester<SourceLostHandler>())
            // {
            //     tester.EventHandler.ControllerTester.handedness = Handedness.Right;

            //     inputSimService.HandStateRight.IsTracked = false;
            //     inputSimService.Update();
            //     Assert.True(tester.TestEventRaised(SourceEventTimeout));
            // }
        }

        private void TestDetectedControllers(Handedness handedness)
        {
            foreach (var detectedController in MixedRealityToolkit.InputSystem.DetectedControllers)
            {
                Assert.AreEqual(handedness, detectedController.ControllerHandedness);

                // detectedController.InputSource.GetType();
            }
        }

        [Test]
        public void Test02_TestSourceRemoved()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}