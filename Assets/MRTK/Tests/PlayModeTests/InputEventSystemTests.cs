// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // type shortcuts
    using Handle = BaseEventSystem.EventHandlerEntry;
    using HandleList = List<BaseEventSystem.EventHandlerEntry>;

    class InputEventSystemTests
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            PlayModeTestUtilities.Setup();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayModeTestUtilities.TearDown();
            yield return null;
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator TestObjectBasedEventRegistration()
        {
            // Need to remove cursors and other global event handlers
            yield return PlayModeTestUtilities.SetupMrtkWithoutGlobalInputHandlers();

            BaseEventSystem inputSystem = (BaseEventSystem)CoreServices.InputSystem;

            var object1 = new GameObject("Object");

            // Second handler weirdly depends on the first one due to event registration working on the entire object
            var objectBasedListener = object1.AddComponent<TestInputGlobalListenerObjectBased>();
            var handlerBasedListener = object1.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            yield return null;

            LogAssert.Expect(LogType.Error, new Regex("Detected simultaneous usage of IMixedRealityEventSystem.Register and IMixedRealityEventSystem.RegisterHandler"));

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
                new HandleList { new Handle(handlerBasedListener, true) },
                inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)],
                "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");

            CollectionAssert.AreEquivalent(
                new HandleList { new Handle(handlerBasedListener, true) },
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
                new HandleList { new Handle(handlerBasedListener, false) },
                inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)],
                "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");

            CollectionAssert.AreEquivalent(
                new HandleList { new Handle(handlerBasedListener, false) },
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

            BaseEventSystem inputSystem = (BaseEventSystem)CoreServices.InputSystem;

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
                new HandleList { new Handle(handlerBasedListener1) },
                inputSystem.EventHandlersByType[typeof(IMixedRealityPointerHandler)],
                "Input event system doesn't contain expected IMixedRealityPointerHandler handlers.");

            CollectionAssert.AreEquivalent(
                new HandleList { new Handle(handlerBasedListener1), new Handle(handlerBasedListener2) },
                inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)],
                "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");

            CollectionAssert.AreEquivalent(
                new HandleList { new Handle(handlerBasedListener1), new Handle(handlerBasedListener2) },
                inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)],
                "Input event system doesn't contain expected IMixedRealityBaseInputHandler handlers.");

            // Disabling one component doesn't influence another one.
            handlerBasedListener1.enabled = false;

            CollectionAssert.AreEquivalent(
                new List<System.Type> { typeof(IMixedRealitySpeechHandler), typeof(IMixedRealityBaseInputHandler) },
                inputSystem.EventHandlersByType.Keys,
                "Input event system doesn't contain expected event handler types.");

            CollectionAssert.AreEquivalent(
                new HandleList { new Handle(handlerBasedListener2) },
                inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)],
                "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");

            CollectionAssert.AreEquivalent(
                new HandleList { new Handle(handlerBasedListener2) },
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

            LogAssert.Expect(LogType.Error, new Regex("Detected simultaneous usage of IMixedRealityEventSystem.Register and IMixedRealityEventSystem.RegisterHandler"));

            // Emit pointer event, which should be received by global handler, poor speech handler, which is on the same object as global,
            // and a handler listening to all events.
            inputSystem.RaisePointerClicked(inputSystem.GazeProvider.GazePointer, MixedRealityInputAction.None, 1);

            Assert.AreEqual(objectBasedListener.pointerClickedCount, 1, "Pointer clicked event is not received by old API handler.");
            Assert.Zero(objectBasedListener.pointerDownCount, "Pointer down event is received by old API handler.");
            Assert.Zero(objectBasedListener.pointerUpCount, "Pointer up event is received by old API handler.");
            Assert.Zero(objectBasedListener.pointerDraggedCount, "Pointer dragged event is received by old API handler.");
            Assert.Zero(objectBasedListener.speechCommandsReceived.Count(), "Speech event is received by old API handler.");

            // Wrong behavior, preserved for backward compatibility
            Assert.AreEqual(handlerBasedListener.pointerClickedCount, 1, "Pointer clicked event is not received by new API handler.");
            Assert.Zero(handlerBasedListener.pointerDownCount, "Pointer down event is received by new  API handler.");
            Assert.Zero(handlerBasedListener.pointerUpCount, "Pointer up event is received by new API handler.");
            Assert.Zero(handlerBasedListener.pointerDraggedCount, "Pointer dragged event is received by new API handler.");
            Assert.Zero(handlerBasedListener.speechCommandsReceived.Count(), "Speech event is received by new API handler.");

            Assert.AreEqual(handlerBasedListener1.pointerClickedCount, 1, "Pointer clicked event is not received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerDownCount, "Pointer down event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerUpCount, "Pointer up event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerDraggedCount, "Pointer dragged event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.speechCommandsReceived.Count(), "Speech event is received by all-handlers component.");

            // No pointer clicked event:
            Assert.Zero(handlerBasedListener2.pointerClickedCount, "Pointer clicked event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerDownCount, "Pointer down event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerUpCount, "Pointer up event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerDraggedCount, "Pointer dragged event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.speechCommandsReceived.Count(), "Speech event is received by speech-handler component.");

            Object.Destroy(object1);
            Object.Destroy(object2);

            yield return null;
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator TestSpeechEventCallsForGlobalHandlers()
        {
            var object1 = new GameObject("Object 1");

            // These 2 handlers are independent
            var objectBasedListener = object1.AddComponent<TestInputGlobalListenerObjectBased>();
            var handlerBasedListener = object1.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            var object2 = new GameObject("Object 2");

            // These 2 handlers are independent
            var handlerBasedListener1 = object2.AddComponent<TestInputGlobalListenerHandlerBasedAllHandlers>();
            var handlerBasedListener2 = object2.AddComponent<TestInputGlobalListenerHandlerBasedSpeechHandler>();

            yield return null;

            LogAssert.Expect(LogType.Error, new Regex("Detected simultaneous usage of IMixedRealityEventSystem.Register and IMixedRealityEventSystem.RegisterHandler"));

            // Emit speech events, which should be received by all handlers.
            var gazeInputSource = CoreServices.InputSystem.DetectedInputSources.Where(x => x.SourceName.Equals("Gaze")).First();
            SpeechCommands[] commandList = CoreServices.InputSystem.InputSystemProfile.SpeechCommandsProfile.SpeechCommands;
            foreach (SpeechCommands command in commandList)
            {
                CoreServices.InputSystem.RaiseSpeechCommandRecognized(gazeInputSource, RecognitionConfidenceLevel.High, new System.TimeSpan(), System.DateTime.Now, new SpeechCommands(command.Keyword, command.KeyCode, MixedRealityInputAction.None));
            }

            Assert.Zero(objectBasedListener.pointerClickedCount, "Pointer clicked event is received by old API handler.");
            Assert.Zero(objectBasedListener.pointerDownCount, "Pointer down event is received by old API handler.");
            Assert.Zero(objectBasedListener.pointerUpCount, "Pointer up event is received by old API handler.");
            Assert.Zero(objectBasedListener.pointerDraggedCount, "Pointer dragged event is received by old API handler.");
            Assert.True(objectBasedListener.speechCommandsReceived.SequenceEqual(commandList.Select(x => x.Keyword)), "Speech events were not received correctly by old API handler.");

            Assert.Zero(handlerBasedListener.pointerClickedCount, "Pointer clicked event is received by new API handler.");
            Assert.Zero(handlerBasedListener.pointerDownCount, "Pointer down event is received by new  API handler.");
            Assert.Zero(handlerBasedListener.pointerUpCount, "Pointer up event is received by new API handler.");
            Assert.Zero(handlerBasedListener.pointerDraggedCount, "Pointer dragged event is received by new API handler.");
            Assert.True(handlerBasedListener.speechCommandsReceived.SequenceEqual(commandList.Select(x => x.Keyword)), "Speech events were not received correctly by new API handler.");

            Assert.Zero(handlerBasedListener1.pointerClickedCount, "Pointer clicked event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerDownCount, "Pointer down event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerUpCount, "Pointer up event is received by all-handlers component.");
            Assert.Zero(handlerBasedListener1.pointerDraggedCount, "Pointer dragged event is received by all-handlers component.");
            Assert.True(handlerBasedListener1.speechCommandsReceived.SequenceEqual(commandList.Select(x => x.Keyword)), "Speech events were not received correctly by all-handlers component");

            // No pointer clicked event:
            Assert.Zero(handlerBasedListener2.pointerClickedCount, "Pointer clicked event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerDownCount, "Pointer down event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerUpCount, "Pointer up event is received by speech-handler component.");
            Assert.Zero(handlerBasedListener2.pointerDraggedCount, "Pointer dragged event is received by speech-handler component.");
            Assert.True(handlerBasedListener2.speechCommandsReceived.SequenceEqual(commandList.Select(x => x.Keyword)), "Speech events were not received correctly by speech-handler component.");

            // Checks that the appropriate speech commands were received
            Assert.True(objectBasedListener.speechCommandsReceived.SequenceEqual(commandList.Select(x => x.Keyword)), "Speech events were not received correctly.");

            Object.Destroy(object1);
            Object.Destroy(object2);

            yield return null;
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator TestInputSystemGlobalHandlerListener()
        {
            // Need to remove cursors and other global event handlers
            yield return PlayModeTestUtilities.SetupMrtkWithoutGlobalInputHandlers();

            BaseEventSystem inputSystem = (BaseEventSystem)CoreServices.InputSystem;

            var object1 = new GameObject("Object");

            var listener = object1.AddComponent<TestInputGlobalHandlerListener>();

            yield return null;

            // No event listener registration in this test
            CollectionAssert.IsEmpty(inputSystem.EventListeners, "Event listener for old event system API shouldn't be registered");

            CollectionAssert.AreEquivalent(
                new List<System.Type> {
                    typeof(IMixedRealityHandJointHandler),
                    typeof(IMixedRealitySpeechHandler),
                    typeof(IMixedRealityBaseInputHandler),
                    typeof(IMixedRealityInputHandler<float>)
                },
                inputSystem.EventHandlersByType.Keys,
                "Input event system doesn't contain expected event handler types.");

            CollectionAssert.AreEquivalent(
                new HandleList { new Handle(listener) },
                inputSystem.EventHandlersByType[typeof(IMixedRealityHandJointHandler)],
                "Input event system doesn't contain expected IMixedRealityHandJointHandler handlers.");

            CollectionAssert.AreEquivalent(
                new HandleList { new Handle(listener) },
                inputSystem.EventHandlersByType[typeof(IMixedRealitySpeechHandler)],
                "Input event system doesn't contain expected IMixedRealitySpeechHandler handlers.");

            CollectionAssert.AreEquivalent(
                new HandleList { new Handle(listener) },
                inputSystem.EventHandlersByType[typeof(IMixedRealityBaseInputHandler)],
                "Input event system doesn't contain expected IMixedRealityBaseInputHandler handlers.");

            CollectionAssert.AreEquivalent(
                new HandleList { new Handle(listener) },
                inputSystem.EventHandlersByType[typeof(IMixedRealityInputHandler<float>)],
                "Input event system doesn't contain expected IMixedRealityInputHandler<float> handlers.");

            Object.Destroy(object1);
            yield return null;
        }

        /// <summary>
        /// Test input system catches exception thrown in global listener responding to input event and that
        /// the input system does not crash accordingly as well
        /// </summary>
        [UnityTest]
        public IEnumerator TestGlobalListenerExceptionThrown()
        {
            var inputSystem = CoreServices.InputSystem;

            var object1 = new GameObject("Object");
            var listener = object1.AddComponent<TestInputGlobalListenerException>();

            yield return null;

            // Emit pointer event, which should be received by global handler
            inputSystem.RaisePointerClicked(inputSystem.GazeProvider.GazePointer, MixedRealityInputAction.None, 1);

            LogAssert.Expect(LogType.Exception, $"Exception: {TestInputGlobalListenerException.ExceptionMessage}");

            Object.Destroy(object1);

            yield return null;
        }
    }
}
#endif
