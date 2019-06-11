// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class InputEventSystemTests
    {
        // Can't use NUnit setup routine, because need to skip several frames while initializing/removing objects.
        public IEnumerator SetupMrtkWithoutCursors()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            IMixedRealityInputSystem inputSystem = null;
            MixedRealityServiceRegistry.TryGetService(out inputSystem);

            Assert.IsNotNull(inputSystem);

            // Let input system to register all cursors and managers.
            yield return null;

            // Switch off / Destroy all input components, which listen to global events
            Object.Destroy(inputSystem.GazeProvider.GazeCursor as Behaviour);
            inputSystem.GazeProvider.Enabled = false;

            var diagnosticsVoiceControls = Object.FindObjectsOfType<DiagnosticsSystemVoiceControls>();
            foreach (var diagnosticsComponent in diagnosticsVoiceControls)
            {
                diagnosticsComponent.enabled = false;
            }

            // Let objects be destroyed
            yield return null;

            // Check that input system is clean
            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 0, "Input event system handler registry is not empty in the beginning of the test.");

            yield return null;
        }

        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator Test01_TestObjectBasedEventRegistration()
        {
            yield return SetupMrtkWithoutCursors();

            IMixedRealityInputSystem inputSystem = null;
            MixedRealityServiceRegistry.TryGetService(out inputSystem);

            var object1 = new GameObject("Object");

            // Second handler weirdly depends on the first one due to event registration working on the entire object
            var objectBasedListener = object1.AddComponent<TestInputGlobalListenerObjectBased>();
            var handlerBasedListener = object1.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            yield return null;

            // Event listener is registered for backward compatibility
            Assert.AreEqual(inputSystem.EventListeners.Count, 1, "Event listener for old event system API hasn't been registered");
            Assert.Contains(objectBasedListener.gameObject, inputSystem.EventListeners, "Wrong event listener registered for old event system API.");

            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 3);
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityPointerHandler));
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealitySpeechHandler));
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityBaseInputHandler));

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)].Count, 2);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)], objectBasedListener);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)], handlerBasedListener);

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)].Count, 2);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], objectBasedListener);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], handlerBasedListener);

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)].Count, 2);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], objectBasedListener);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], handlerBasedListener);

            objectBasedListener.enabled = false;

            // This is odd result from disabling just one component, but it's a behavior of old API.
            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 0);

            ///
            // Reset registration of handler-based listener and check that it registers itself after that, even though its handlers 
            // have been removed without its intervention.
            handlerBasedListener.enabled = false;
            handlerBasedListener.enabled = true;

            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 2);
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealitySpeechHandler));
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityBaseInputHandler));

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)].Count, 1);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], handlerBasedListener);

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)].Count, 1);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], handlerBasedListener);

            handlerBasedListener.enabled = false;
            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 0);

            Object.Destroy(object1);
            yield return null;
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator Test02_TestHandlerBasedEventRegistration()
        {
            yield return SetupMrtkWithoutCursors();

            IMixedRealityInputSystem inputSystem = null;
            MixedRealityServiceRegistry.TryGetService(out inputSystem);

            var object1 = new GameObject("Object");

            // These 2 handlers are independent
            var handlerBasedListener1 = object1.AddComponent<TestInputGlobalListenerHandlerBasedAllHandlers>();
            var handlerBasedListener2 = object1.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            yield return null;

            // No event listener registration in this test
            Assert.AreEqual(inputSystem.EventListeners.Count, 0, "Event listener for old event system API hasn't been registered");

            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 3);
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityPointerHandler));
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealitySpeechHandler));
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityBaseInputHandler));

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)].Count, 1);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)], handlerBasedListener1);

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)].Count, 2);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], handlerBasedListener1);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], handlerBasedListener2);

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)].Count, 2);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], handlerBasedListener1);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], handlerBasedListener2);

            // Disabling one component doesn't influence another one.
            handlerBasedListener1.enabled = false;

            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 2);
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealitySpeechHandler));
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityBaseInputHandler));

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)].Count, 1);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], handlerBasedListener2);

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)].Count, 1);
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], handlerBasedListener2);

            ///
            Object.Destroy(object1);
            yield return null;
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator Test03_TestPointerEventCallsForGlobalHandlers()
        {
            // We need Gaze Cursor in this test to use it as source to emit events.
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            IMixedRealityInputSystem inputSystem = null;
            MixedRealityServiceRegistry.TryGetService(out inputSystem);

            var object1 = new GameObject("Object 1");

            // Second handler depends on the first one
            var objectBasedListener = object1.AddComponent<TestInputGlobalListenerObjectBased>();
            var handlerBasedListener = object1.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            var object2 = new GameObject("Object 2");

            // These 2 handlers are independent
            var handlerBasedListener1 = object2.AddComponent<TestInputGlobalListenerHandlerBasedAllHandlers>();
            var handlerBasedListener2 = object2.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            yield return null;

            // Emit pointer event, which should be received by global handler, poor speech handler, which is on the same object as global,
            // and a handler listening to all events.
            inputSystem.RaisePointerClicked(inputSystem.GazeProvider.GazePointer, MixedRealityInputAction.None, 1);

            Assert.AreEqual(objectBasedListener.pointerClickedCount, 1);
            Assert.Zero(objectBasedListener.pointerDownCount);
            Assert.Zero(objectBasedListener.pointerUpCount);
            Assert.Zero(objectBasedListener.pointerDraggedCount);
            Assert.Zero(objectBasedListener.speechCount);

            Assert.AreEqual(handlerBasedListener.pointerClickedCount, 1);
            Assert.Zero(handlerBasedListener.pointerDownCount);
            Assert.Zero(handlerBasedListener.pointerUpCount);
            Assert.Zero(handlerBasedListener.pointerDraggedCount);
            Assert.Zero(handlerBasedListener.speechCount);

            Assert.AreEqual(handlerBasedListener1.pointerClickedCount, 1);
            Assert.Zero(handlerBasedListener1.pointerDownCount);
            Assert.Zero(handlerBasedListener1.pointerUpCount);
            Assert.Zero(handlerBasedListener1.pointerDraggedCount);
            Assert.Zero(handlerBasedListener1.speechCount);

            // No pointer clicked event:
            Assert.Zero(handlerBasedListener2.pointerClickedCount);
            Assert.Zero(handlerBasedListener2.pointerDownCount);
            Assert.Zero(handlerBasedListener2.pointerUpCount);
            Assert.Zero(handlerBasedListener2.pointerDraggedCount);
            Assert.Zero(handlerBasedListener2.speechCount);

            /////
            Object.Destroy(object1);
            Object.Destroy(object2);

            yield return null;
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator Test04_TestSpeechEventCallsForGlobalHandlers()
        {
            // We need Gaze Cursor in this test to use it as source to emit events.
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            IMixedRealityInputSystem inputSystem = null;
            MixedRealityServiceRegistry.TryGetService(out inputSystem);

            var object1 = new GameObject("Object 1");

            // These 2 handlers are independent
            var objectBasedListener = object1.AddComponent<TestInputGlobalListenerObjectBased>();
            var handlerBasedListener = object1.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            var object2 = new GameObject("Object 2");

            // These 2 handlers are independent
            var handlerBasedListener1 = object2.AddComponent<TestInputGlobalListenerHandlerBasedAllHandlers>();
            var handlerBasedListener2 = object2.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            yield return null;

            //////
            // Emit speech event, which should be received by all handlers.
            var gazeInputSource = inputSystem.DetectedInputSources.Where(x => x.SourceName.Equals("Gaze")).First();
            inputSystem.RaiseSpeechCommandRecognized(gazeInputSource, RecognitionConfidenceLevel.High, new System.TimeSpan(), System.DateTime.Now, new SpeechCommands("menu", KeyCode.Alpha1, MixedRealityInputAction.None));

            Assert.Zero(objectBasedListener.pointerClickedCount);
            Assert.Zero(objectBasedListener.pointerDownCount);
            Assert.Zero(objectBasedListener.pointerUpCount);
            Assert.Zero(objectBasedListener.pointerDraggedCount);
            Assert.AreEqual(objectBasedListener.speechCount, 1);

            Assert.Zero(handlerBasedListener.pointerClickedCount);
            Assert.Zero(handlerBasedListener.pointerDownCount);
            Assert.Zero(handlerBasedListener.pointerUpCount);
            Assert.Zero(handlerBasedListener.pointerDraggedCount);
            Assert.AreEqual(handlerBasedListener.speechCount, 1);

            Assert.Zero(handlerBasedListener1.pointerClickedCount);
            Assert.Zero(handlerBasedListener1.pointerDownCount);
            Assert.Zero(handlerBasedListener1.pointerUpCount);
            Assert.Zero(handlerBasedListener1.pointerDraggedCount);
            Assert.AreEqual(handlerBasedListener1.speechCount, 1);

            // No pointer clicked event:
            Assert.Zero(handlerBasedListener2.pointerClickedCount);
            Assert.Zero(handlerBasedListener2.pointerDownCount);
            Assert.Zero(handlerBasedListener2.pointerUpCount);
            Assert.Zero(handlerBasedListener2.pointerDraggedCount);
            Assert.AreEqual(handlerBasedListener2.speechCount, 1);

            /////
            Object.Destroy(object1);
            Object.Destroy(object2);

            yield return null;
        }
    }
}
#endif