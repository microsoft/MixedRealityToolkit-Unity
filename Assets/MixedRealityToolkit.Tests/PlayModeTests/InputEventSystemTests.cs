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
        // Can't use NUnit setup routine, because needs to skip several frames while initializing/removing objects.
        public IEnumerator SetupMrtkWithoutCursors()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            IMixedRealityInputSystem inputSystem = null;
            MixedRealityServiceRegistry.TryGetService(out inputSystem);

            Assert.IsNotNull(inputSystem, "Input system must be initialized");

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

            // Event listener collection is filled for backward compatibility
            Assert.AreEqual(inputSystem.EventListeners.Count, 1, "Event listener for old event system API hasn't been registered");
            Assert.Contains(objectBasedListener.gameObject, inputSystem.EventListeners, "Wrong event listener registered for old event system API.");

            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 3, "Input event system doesn't contain expected event handler types.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityPointerHandler), "Input event system doesn't contain IMixedRealityPointerHandler entry.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealitySpeechHandler), "Input event system doesn't contain IMixedRealitySpeechHandler entry.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityBaseInputHandler), "Input event system doesn't contain IMixedRealityBaseInputHandler entry.");

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)].Count, 2, "Input event system doesn't contain expected IMixedRealityPointerHandler handlers.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)], objectBasedListener, "Input event system doesn't contain IMixedRealityPointerHandler for old API handler.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)], handlerBasedListener, "Input event system doesn't contain IMixedRealityPointerHandler for new API handler.");

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)].Count, 2, "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], objectBasedListener, "Input event system doesn't contain IMixedRealitySpeechHandler for old API handler.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], handlerBasedListener, "Input event system doesn't contain IMixedRealitySpeechHandler for new API handler.");

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)].Count, 2, "Input event system doesn't contain expected IMixedRealityBaseInputHandler handlers.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], objectBasedListener, "Input event system doesn't contain IMixedRealityBaseInputHandler for old API handler.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], handlerBasedListener, "Input event system doesn't contain IMixedRealityBaseInputHandler for new API handler.");

            objectBasedListener.enabled = false;

            // This is odd result from disabling just one component, but it's a behavior of old API.
            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 0, "Input event system contains unexpected event handlers.");

            ///
            // Reset registration of handler-based listener and check that it registers itself after that, even though its handlers 
            // have been removed without its intervention.
            handlerBasedListener.enabled = false;
            handlerBasedListener.enabled = true;

            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 2, "Input event system doesn't contain expected event handler types.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealitySpeechHandler), "Input event system doesn't contain IMixedRealitySpeechHandler entry.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityBaseInputHandler), "Input event system doesn't contain IMixedRealityBaseInputHandler entry.");

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)].Count, 1, "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], handlerBasedListener, "Input event system doesn't contain IMixedRealitySpeechHandler for new API handler.");

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)].Count, 1, "Input event system doesn't contain expected IMixedRealityBaseInputHandler handlers.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], handlerBasedListener, "Input event system doesn't contain IMixedRealityBaseInputHandler for new API handler.");

            handlerBasedListener.enabled = false;
            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 0, "Input event system contains unexpected event handlers.");

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
            Assert.AreEqual(inputSystem.EventListeners.Count, 0, "Event listener for old event system API shouldn't be registered");

            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 3, "Input event system doesn't contain expected event handler types.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityPointerHandler), "Input event system doesn't contain IMixedRealityPointerHandler entry.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealitySpeechHandler), "Input event system doesn't contain IMixedRealitySpeechHandler entry.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityBaseInputHandler), "Input event system doesn't contain IMixedRealityBaseInputHandler entry.");

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)].Count, 1, "Input event system doesn't contain expected IMixedRealityPointerHandler handlers.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)], handlerBasedListener1, "Input event system doesn't contain IMixedRealityPointerHandler for all-handlers component.");

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)].Count, 2, "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], handlerBasedListener1, "Input event system doesn't contain IMixedRealitySpeechHandler for all-handlers component.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], handlerBasedListener2, "Input event system doesn't contain IMixedRealitySpeechHandler for speech-handler component.");

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)].Count, 2, "Input event system doesn't contain expected IMixedRealityBaseInputHandler handlers.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], handlerBasedListener1, "Input event system doesn't contain IMixedRealityBaseInputHandler for all-handlers component.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], handlerBasedListener2, "Input event system doesn't contain IMixedRealityBaseInputHandler for speech-handler component.");

            // Disabling one component doesn't influence another one.
            handlerBasedListener1.enabled = false;

            Assert.AreEqual(inputSystem.EventHandlersByType.Count, 2, "Input event system doesn't contain expected event handler types.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealitySpeechHandler), "Input event system doesn't contain IMixedRealitySpeechHandler entry.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType.Keys, typeof(IMixedRealityBaseInputHandler), "Input event system doesn't contain IMixedRealityBaseInputHandler entry.");

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)].Count, 1, "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], handlerBasedListener2, "Input event system doesn't contain IMixedRealitySpeechHandler for speech-handler component.");

            Assert.AreEqual(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)].Count, 1, "Input event system doesn't contain expected IMixedRealityBaseInputHandler handlers.");
            CollectionAssert.Contains(inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], handlerBasedListener2, "Input event system doesn't contain IMixedRealityBaseInputHandler for speech-handler component.");

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

            Assert.AreEqual(objectBasedListener.pointerClickedCount, 1, "Pointer clicked event is not received by old API handler.");
            Assert.Zero(objectBasedListener.pointerDownCount,           "Pointer down event is received by old API handler.");
            Assert.Zero(objectBasedListener.pointerUpCount,             "Pointer up event is received by old API handler.");
            Assert.Zero(objectBasedListener.pointerDraggedCount,        "Pointer dragged event is received by old API handler.");
            Assert.Zero(objectBasedListener.speechCount,                "Speech event is received by old API handler.");
            
            // Wrong behavior, preserved for backward compatibility
            Assert.AreEqual(handlerBasedListener.pointerClickedCount, 1, "Pointer clicked event is not received by new API handler.");
            Assert.Zero(handlerBasedListener.pointerDownCount,           "Pointer down event is received by new  API handler.");
            Assert.Zero(handlerBasedListener.pointerUpCount,             "Pointer up event is received by new API handler.");
            Assert.Zero(handlerBasedListener.pointerDraggedCount,        "Pointer dragged event is received by new API handler.");
            Assert.Zero(handlerBasedListener.speechCount,                "Speech event is received by new API handler.");
            
            Assert.AreEqual(handlerBasedListener1.pointerClickedCount, 1, "Pointer clicked event is not received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerDownCount,           "Pointer down event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerUpCount,             "Pointer up event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerDraggedCount,        "Pointer dragged event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.speechCount,                "Speech event is received by all-handlers component.");

            // No pointer clicked event:
            Assert.Zero(handlerBasedListener2.pointerClickedCount,        "Pointer clicked event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerDownCount,           "Pointer down event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerUpCount,             "Pointer up event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerDraggedCount,        "Pointer dragged event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.speechCount,                "Speech event is received by speech-handler component.");

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
            
            Assert.Zero(objectBasedListener.pointerClickedCount, "Pointer clicked event is received by old API handler.");
            Assert.Zero(objectBasedListener.pointerDownCount,    "Pointer down event is received by old API handler.");
            Assert.Zero(objectBasedListener.pointerUpCount,      "Pointer up event is received by old API handler.");
            Assert.Zero(objectBasedListener.pointerDraggedCount, "Pointer dragged event is received by old API handler.");
            Assert.AreEqual(objectBasedListener.speechCount, 1,  "Speech event is not received by old API handler.");

            Assert.Zero(handlerBasedListener.pointerClickedCount, "Pointer clicked event is received by new API handler.");
            Assert.Zero(handlerBasedListener.pointerDownCount,    "Pointer down event is received by new  API handler.");
            Assert.Zero(handlerBasedListener.pointerUpCount,      "Pointer up event is received by new API handler.");
            Assert.Zero(handlerBasedListener.pointerDraggedCount, "Pointer dragged event is received by new API handler.");
            Assert.AreEqual(handlerBasedListener.speechCount, 1,  "Speech event is not received by new API handler.");
            
            Assert.Zero(handlerBasedListener1.pointerClickedCount, "Pointer clicked event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerDownCount,    "Pointer down event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerUpCount,      "Pointer up event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerDraggedCount, "Pointer dragged event is received by all-handlers component.");
            Assert.AreEqual(handlerBasedListener1.speechCount, 1,  "Speech event is not received by all-handlers component.");

            // No pointer clicked event:
            Assert.Zero(handlerBasedListener2.pointerClickedCount, "Pointer clicked event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerDownCount,    "Pointer down event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerUpCount,      "Pointer up event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerDraggedCount, "Pointer dragged event is received by speech-handler component.");
            Assert.AreEqual(handlerBasedListener2.speechCount, 1,  "Speech event is not received by speech-handler component.");

            /////
            Object.Destroy(object1);
            Object.Destroy(object2);

            yield return null;
        }
    }
}
#endif