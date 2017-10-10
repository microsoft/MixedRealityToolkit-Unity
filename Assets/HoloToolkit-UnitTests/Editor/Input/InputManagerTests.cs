// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.Tests
{
    [TestFixture]
    public class InputManagerTests
    {
        private List<GameObject> receivedEventSources;

        [SetUp]
        public void SetUpTests()
        {
            TestUtils.ClearScene();
            receivedEventSources = new List<GameObject>();
            //Create a main camera and add input manager, event system and gaze manager to it
            var inputManagerContainer = TestUtils.CreateMainCamera().gameObject;
            inputManagerContainer.AddComponent<InputManager>();
            inputManagerContainer.AddComponent<GazeManager>();
            inputManagerContainer.AddComponent<FocusManager>();
            inputManagerContainer.AddComponent<EventSystem>();

            inputManagerContainer.transform.position = inputManagerContainer.transform.forward * -5;
            //call awake and start 
            inputManagerContainer.CallInitialization();
        }

        [Test]
        public void DisableInputManagerRefCountNoEventCaught()
        {
            CreateGlobalTestHandler().CallInitialization();

            InputManager.Instance.PushInputDisable();
            FireTestEvent();
            InputManager.Instance.PopInputDisable();

            Assert.That(receivedEventSources, Is.Empty);
        }

        [Test]
        public void DisableInputManagerScriptNoEventCaught()
        {
            CreateGlobalTestHandler().CallInitialization();

            InputManager.Instance.enabled = false;
            FireTestEvent();
            InputManager.Instance.enabled = true;

            Assert.That(receivedEventSources, Is.Empty);
        }

        [Test]
        public void CatchSingleGlobalEvent()
        {
            CreateGlobalTestHandler().CallInitialization();

            FireTestEvent();

            Assert.That(receivedEventSources, Is.Not.Empty);
        }

        [Test]
        public void CatchSingleGlobalEventCheckSource()
        {
            var globalHandler = CreateGlobalTestHandler().CallInitialization();

            FireTestEvent();

            Assert.That(receivedEventSources[0], Is.EqualTo(globalHandler));
        }

        [Test]
        public void CatchDoubleGlobalEvent()
        {
            CreateGlobalTestHandler().CallInitialization();
            CreateGlobalTestHandler().CallInitialization();

            FireTestEvent();

            Assert.That(receivedEventSources.Count, Is.EqualTo(2));
        }

        [Test]
        public void CatchDoubleGlobalEventCheckSource()
        {
            var globalHandler1 = CreateGlobalTestHandler().CallInitialization();
            var globalHandler2 = CreateGlobalTestHandler().CallInitialization();

            FireTestEvent();

            Assert.That(receivedEventSources.Contains(globalHandler1), Is.True);
            Assert.That(receivedEventSources.Contains(globalHandler2), Is.True);
        }

        [Test]
        public void CatchFocusedEvent()
        {
            var focusedHandler = CreateTestHandler().CallInitialization();

            InputManager.Instance.OverrideFocusedObject = focusedHandler;
            FireTestEvent();

            Assert.That(receivedEventSources, Is.Not.Empty);
        }

        [Test]
        public void CatchModalEvent()
        {
            var modalHandler = CreateTestHandler().CallInitialization();

            InputManager.Instance.PushModalInputHandler(modalHandler);
            FireTestEvent();
            InputManager.Instance.PopModalInputHandler();

            Assert.That(receivedEventSources, Is.Not.Empty);
        }

        [Test]
        public void CatchOneModalEventAndCheckSource()
        {
            var modalHandler1 = CreateTestHandler().CallInitialization();
            var modalHandler2 = CreateTestHandler().CallInitialization();

            InputManager.Instance.PushModalInputHandler(modalHandler1);
            InputManager.Instance.PushModalInputHandler(modalHandler2);
            FireTestEvent();
            InputManager.Instance.PopModalInputHandler();
            InputManager.Instance.PopModalInputHandler();

            Assert.That(receivedEventSources.Count, Is.EqualTo(1));
            Assert.That(receivedEventSources[0], Is.EqualTo(modalHandler2));
        }

        [Test]
        public void CatchFallbackEvent()
        {
            var fallbackHandler = CreateTestHandler().CallInitialization();

            InputManager.Instance.PushFallbackInputHandler(fallbackHandler);
            FireTestEvent();
            InputManager.Instance.PopFallbackInputHandler();

            Assert.That(receivedEventSources, Is.Not.Empty);
        }

        [Test]
        public void CatchModalEventOverFocusedEvent()
        {
            var focusedHandler = CreateTestHandler().CallInitialization();
            var modalHandler = CreateTestHandler().CallInitialization();

            InputManager.Instance.OverrideFocusedObject = focusedHandler;
            InputManager.Instance.PushModalInputHandler(modalHandler);
            FireTestEvent();
            InputManager.Instance.PopModalInputHandler();

            Assert.That(receivedEventSources.Count, Is.EqualTo(1));
            Assert.That(receivedEventSources[0], Is.EqualTo(modalHandler));
        }

        [Test]
        public void CatchModalFocusedChildEvent()
        {
            var focusedHandler = CreateTestHandler().CallInitialization();
            var modalHandler = CreateTestHandler().CallInitialization();
            focusedHandler.transform.SetParent(modalHandler.transform);

            InputManager.Instance.OverrideFocusedObject = focusedHandler;
            InputManager.Instance.PushModalInputHandler(modalHandler);
            FireTestEvent();
            InputManager.Instance.PopModalInputHandler();

            Assert.That(receivedEventSources.Count, Is.EqualTo(1));
            Assert.That(receivedEventSources[0], Is.EqualTo(focusedHandler));
        }

        [Test]
        public void CatchMultipleEventSources()
        {
            var globalHandler1 = CreateGlobalTestHandler().CallInitialization();
            var globalHandler2 = CreateGlobalTestHandler().CallInitialization();
            var focusedHandler = CreateTestHandler().CallInitialization();
            var modalHandler = CreateTestHandler().CallInitialization();
            var fallbackHandler = CreateTestHandler().CallInitialization();

            InputManager.Instance.OverrideFocusedObject = focusedHandler;
            InputManager.Instance.PushFallbackInputHandler(fallbackHandler);

            FireTestEvent();
            Assert.That(receivedEventSources, Is.EquivalentTo(new List<GameObject> { globalHandler1, globalHandler2, focusedHandler }));

            InputManager.Instance.PushModalInputHandler(modalHandler);
            FireTestEvent();
            Assert.That(receivedEventSources, Is.EquivalentTo(new List<GameObject> { globalHandler1, globalHandler2, modalHandler }));

            modalHandler.SetActive(false);
            FireTestEvent();
            Assert.That(receivedEventSources, Is.EquivalentTo(new List<GameObject> { globalHandler1, globalHandler2, focusedHandler }));

            focusedHandler.SetActive(false);
            FireTestEvent();
            Assert.That(receivedEventSources, Is.EquivalentTo(new List<GameObject> { globalHandler1, globalHandler2, fallbackHandler }));

            InputManager.Instance.PopModalInputHandler();
            InputManager.Instance.PopFallbackInputHandler();
        }

        [Test]
        public void FocusChangeFullIntegration()
        {
            var handler = CreateCubeTestHandler().CallInitialization();

            GazeManager.Instance.gameObject.CallUpdate();

            Assert.That(receivedEventSources.Count, Is.EqualTo(1));
            Assert.That(receivedEventSources[0], Is.EqualTo(handler));
        }


        private GameObject CreateTestHandler()
        {
            return SetTestHandler(new GameObject());
        }

        private GameObject CreateCubeTestHandler()
        {
            return SetTestHandler(GameObject.CreatePrimitive(PrimitiveType.Cube));
        }

        private GameObject SetTestHandler(GameObject gameObject)
        {
            gameObject.AddComponent<TestEventHandler>().EventFiredCallback = OnEventFired;
            return gameObject;
        }


        private GameObject CreateGlobalTestHandler()
        {
            var testHandler = CreateTestHandler();
            testHandler.GetComponent<TestEventHandler>().IsGlobal = true;
            testHandler.AddComponent<SetGlobalListener>();
            return testHandler;
        }

        private void FireTestEvent()
        {
            receivedEventSources = new List<GameObject>();
            InputManager.Instance.HandleEvent(new BaseEventData(EventSystem.current), TestEventHandler.OnTestHandler);
        }

        private void OnEventFired(GameObject source, BaseEventData baseEventData)
        {
            receivedEventSources.Add(source);
        }
    }
}
