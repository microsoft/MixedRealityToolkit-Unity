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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class InputEventSystemTests
    {
        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator TestObjectBasedEventRegistration()
        {
            // Need to remove cursors and other global event handlers
            yield return PlayModeTestUtilities.SetupMrtkWithoutGlobalInputHandlers();

            IMixedRealityInputSystem iInputSystem = null;
            MixedRealityServiceRegistry.TryGetService(out iInputSystem);

            BaseEventSystem inputSystem = (BaseEventSystem)iInputSystem;

            var object1 = new GameObject("Object");

            // Second handler weirdly depends on the first one due to event registration working on the entire object
            var objectBasedListener = object1.AddComponent<TestInputGlobalListenerObjectBased>();
            var handlerBasedListener = object1.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            yield return null;

            // Event listener collection is filled for backward compatibility
            CollectionAssert.AreEquivalent(
                new List<GameObject> { objectBasedListener.gameObject },
                inputSystem.EventListeners,
                "Event listener for old event system API hasn't been registered correctly.");

            CollectionAssert.AreEquivalent(
                new List<System.Type> { typeof(IMixedRealitySpeechHandler), typeof(IMixedRealityBaseInputHandler) },
                inputSystem.EventHandlersByType.Keys,
                "Input event system doesn't contain expected event handler types.");

            CollectionAssert.AreEquivalent(
                new List<IEventSystemHandler> { handlerBasedListener },
                inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)],
                "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");

            CollectionAssert.AreEquivalent(
                new List<IEventSystemHandler> { handlerBasedListener },
                inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)],
                "Input event system doesn't contain expected IMixedRealityBaseInputHandler handlers.");

            // Make sure that disabling global listener doesn't remove the new API one.
            objectBasedListener.enabled = false;

            CollectionAssert.IsEmpty(inputSystem.EventListeners, "Event listener for old event system API shouldn't be registered");

            CollectionAssert.AreEquivalent(
                new List<System.Type> { typeof(IMixedRealitySpeechHandler), typeof(IMixedRealityBaseInputHandler) },
                inputSystem.EventHandlersByType.Keys,
                "Input event system doesn't contain expected event handler types.");

            CollectionAssert.AreEquivalent(
                new List<IEventSystemHandler> { handlerBasedListener },
                inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)],
                "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");

            CollectionAssert.AreEquivalent(
                new List<IEventSystemHandler> { handlerBasedListener },
                inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)],
                "Input event system doesn't contain expected IMixedRealityBaseInputHandler handlers.");

            handlerBasedListener.enabled = false;
            CollectionAssert.IsEmpty(inputSystem.EventHandlersByType, "Input event system contains unexpected event handlers.");

            Object.Destroy(object1);
            yield return null;
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandlerBasedEventRegistration()
        {
            // Need to remove cursors and other global event handlers
            yield return PlayModeTestUtilities.SetupMrtkWithoutGlobalInputHandlers();

            IMixedRealityInputSystem iInputSystem = null;
            MixedRealityServiceRegistry.TryGetService(out iInputSystem);

            BaseEventSystem inputSystem = (BaseEventSystem)iInputSystem;

            var object1 = new GameObject("Object");

            // These 2 handlers are independent
            // 1st is Pointer + Speech
            // 2nd is Speech only
            var handlerBasedListener1 = object1.AddComponent<TestInputGlobalListenerHandlerBasedAllHandlers>();
            var handlerBasedListener2 = object1.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            yield return null;

            // No event listener registration in this test
            CollectionAssert.IsEmpty(inputSystem.EventListeners, "Event listener for old event system API shouldn't be registered");

            CollectionAssert.AreEquivalent(
                new List<System.Type> { typeof(IMixedRealityPointerHandler), typeof(IMixedRealitySpeechHandler), typeof(IMixedRealityBaseInputHandler) },
                inputSystem.EventHandlersByType.Keys,
                "Input event system doesn't contain expected event handler types.");

            CollectionAssert.AreEquivalent(
                new List<IEventSystemHandler> { handlerBasedListener1 },
                inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)],
                "Input event system doesn't contain expected IMixedRealityPointerHandler handlers.");

            CollectionAssert.AreEquivalent(
                new List<IEventSystemHandler> { handlerBasedListener1, handlerBasedListener2 },
                inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)],
                "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");

            CollectionAssert.AreEquivalent(
                new List<IEventSystemHandler> { handlerBasedListener1, handlerBasedListener2 },
                inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)],
                "Input event system doesn't contain expected IMixedRealityBaseInputHandler handlers.");

            // Disabling one component doesn't influence another one.
            handlerBasedListener1.enabled = false;

            CollectionAssert.AreEquivalent(
                new List<System.Type> { typeof(IMixedRealitySpeechHandler), typeof(IMixedRealityBaseInputHandler) }, 
                inputSystem.EventHandlersByType.Keys, 
                "Input event system doesn't contain expected event handler types.");

            CollectionAssert.AreEquivalent(
                new List<IEventSystemHandler> { handlerBasedListener2 }, 
                inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)], 
                "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");

            CollectionAssert.AreEquivalent(
                new List<IEventSystemHandler> { handlerBasedListener2 }, 
                inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)], 
                "Input event system doesn't contain expected IMixedRealityBaseInputHandler handlers.");

            Object.Destroy(object1);
            yield return null;
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator TestPointerEventCallsForGlobalHandlers()
        {
            // We need Gaze Cursor in this test to use it as source to emit events.
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            IMixedRealityInputSystem inputSystem = null;
            MixedRealityServiceRegistry.TryGetService(out inputSystem);

            var object1 = new GameObject("Object 1");

            // Second handler depends on the first one as they sit on the same object
            var objectBasedListener = object1.AddComponent<TestInputGlobalListenerObjectBased>();
            var handlerBasedListener = object1.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            var object2 = new GameObject("Object 2");

            // These 2 handlers are independent
            // 1st is Pointer + Speech
            // 2nd is Speech only
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

            Object.Destroy(object1);
            Object.Destroy(object2);

            yield return null;
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator TestSpeechEventCallsForGlobalHandlers()
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

            Object.Destroy(object1);
            Object.Destroy(object2);

            yield return null;
        }
    }
}
#endif