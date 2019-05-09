// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    public abstract class TestEventHandler : MonoBehaviour
    {
        private bool wasEventRaised = false;
        public bool WasEventRaised => wasEventRaised;

        protected void SetEventRaised()
        {
            wasEventRaised = true;
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
                SetEventRaised();
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
                SetEventRaised();
            }
        }
    }

    public class EventTester<T> : IDisposable where T : TestEventHandler
    {
        private T eventHandler;
        public T EventHandler => eventHandler;

        public EventTester()
        {
            var gameObject = new GameObject();
            eventHandler = gameObject.AddComponent<T>();
            MixedRealityToolkit.InputSystem.Register(gameObject);
        }

        public void Dispose()
        {
            MixedRealityToolkit.InputSystem.Unregister(eventHandler.gameObject);
            GameObject.DestroyImmediate(eventHandler.gameObject);
        }
    }

    /// <summary>
    /// Utility class that waits until the expected event has been raised or the timeout has expired.
    /// </summary>
    public class WaitForEventOrTimeout<T> : CustomYieldInstruction where T : TestEventHandler
    {
        EventTester<T> tester;
        Stopwatch stopwatch;

        public override bool keepWaiting
        {
            get
            {
                // Event was raised
                if (tester.EventHandler.WasEventRaised)
                {
                    return false;
                }
                // Timeout
                if (TimeSpan.Compare(stopwatch.Elapsed, TimeSpan.Zero) > 0)
                {
                    return false;
                }
                return true;
            }
        }

        public WaitForEventOrTimeout(EventTester<T> tester, float timeout)
        {
            this.tester = tester;
            stopwatch = new Stopwatch();
            stopwatch.Elapsed.Subtract(TimeSpan.FromSeconds((double)timeout));
            stopwatch.Start();
        }
    }
}