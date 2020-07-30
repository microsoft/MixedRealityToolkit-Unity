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

using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;


namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Tests for the BaseEventSystem
    /// </summary>
    /// <remarks>
    /// Note that the tests below use some of the real sub-classes of BaseEventSystem to get maximal
    /// coverage of the end-to-end. However note that the particular systems used are not important
    /// (i.e. input and diagnostics are used, but any arbitrary systems that can raise events on demand
    /// would work)
    /// </remarks>
    class BaseEventSystemTests
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
        /// Test input system catches exception thrown in global listener responding to input event, and
        /// that the input system is still able to unregister the event listener (i.e. eventExecutionDepth
        /// doesn't get out of sync with reality).
        /// </summary>
        [UnityTest]
        public IEnumerator TestGlobalListenerExceptionThrown()
        {
            var inputSystem = CoreServices.InputSystem;
            var testObject = new GameObject("TestObject");
            var listener = testObject.AddComponent<ExceptionThrowingGlobalListener>();

            yield return null;

            // Emit one event, which should trigger an exception to get logged.
            inputSystem.RaisePointerClicked(inputSystem.GazeProvider.GazePointer, MixedRealityInputAction.None, 1);
            LogAssert.Expect(LogType.Exception, $"Exception: {ExceptionThrowingGlobalListener.ExceptionMessage}");

            // Since the event was just invoked once, the count should just be one.
            Assert.AreEqual(1, listener.pointerClickedCount);

            // Disable the object to force the component to unregister for global events
            testObject.SetActive(false);

            // Emit another event - this shouldn't call the listeners because it should have been unregistered.
            // In particular, pointerClickedCount should still be 1.
            inputSystem.RaisePointerClicked(inputSystem.GazeProvider.GazePointer, MixedRealityInputAction.None, 1);
            Assert.AreEqual(1, listener.pointerClickedCount);

            UnityEngine.Object.Destroy(testObject);

            yield return null;
        }

        /// <summary>
        /// A legacy global listener which will throw an exception in OnPointerClicked, but still
        /// keep tracking of the number of times OnPointerClicked is called (via the base
        /// class pointerClickedCount).
        /// </summary>
        /// <remarks>
        /// Only used for the TestGlobalListenerExceptionThrown test case.
        /// </remarks>
        internal class ExceptionThrowingGlobalListener : TestInputGlobalListener
        {
            public const string ExceptionMessage = "Expected exception in OnPointerClicked";

            public ExceptionThrowingGlobalListener()
            {
                // Important - this causes the registrations to use the legacy global
                // event system, which is what this intends to test.
                useObjectBasedRegistration = true;
            }

            public override void OnPointerClicked(MixedRealityPointerEventData eventData)
            {
                // Invocation of base.OnPointerClicked because it updates
                // pointerClickedCount, which is used to verify that this callback is still
                // getting invoked even as it's throwing exceptions.
                base.OnPointerClicked(eventData);
                throw new System.Exception(ExceptionMessage);
            }
        }

        /// <summary>
        /// Validates that the event systems each maintain their own register/unregister event stacks.
        /// In particular this is made to ensure that we don't regress:
        /// https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6975
        /// </summary>
        /// <remarks>
        /// In particular, this tests a rather tricky issue where the eventExecutionDepth was being
        /// used globally, despite the fact that it was actually a local matter. This is the scenario
        /// that can trigger the old issue:
        ///
        /// Before: An object exists that is a listener of both input events and diagnostic system events.
        /// Note that this means this object is listening to two different BaseEventSystem extending objects.
        ///
        /// 1) An input event gets raised (in this test, arbitrarily chosen to be the pointer clicked event)
        /// 2) In the pointer click event, it raises a diagnostic system event - note that at this point the
        ///    eventExecutionDepth is "2" globally (i.e. there are two active events in flight). However, on
        ///    a per-event system basis, each only has an execution depth of 1 (i.e. they are unrelated events)
        /// 3) In the diagnostic event handler, we unregister for the diagnostic event. This causes us to
        ///    add a postponed action, which will be cleaned up once the execution depth drops to 0.
        /// 4) The diagnostic event execution resolves, dropping the diagnostic event depth to 0. Now, the
        ///    listener gets unregistered (correctly)
        ///    NOTE: This is where the bug in https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6975
        ///    existed - when the eventExecutionDepth was global, the diagnostic handler would never get
        ///    unregistered of the mismatch between the globally tracked state (eventExecutionDepth) and locally
        ///    tracked state (postponed actions)
        /// 5) Input event resolves.
        /// </remarks>
        [UnityTest]
        public IEnumerator TestNestedNonRelatedListenerInvocation()
        {
            var inputSystem = CoreServices.InputSystem;
            var testObject = new GameObject("TestObject");
            var inputListener = testObject.AddComponent<TestNestedInputSystemGlobalListener>();
            var diagnosticsListener = testObject.AddComponent<TestDiagnosticSystemGlobalListener>();

            yield return null;

            // Emits a single event and validates that the pointer click and diagnostic handler are both called.
            inputSystem.RaisePointerClicked(inputSystem.GazeProvider.GazePointer, MixedRealityInputAction.None, 1);
            Assert.AreEqual(1, inputListener.pointerClickedCount);
            Assert.AreEqual(1, diagnosticsListener.eventCount);

            // Emit another event - Note that the diagnosticListener will unregister itself in its handler,
            // so it should only have ever been invoked a single time, but the input listener should have been invoked again.
            inputSystem.RaisePointerClicked(inputSystem.GazeProvider.GazePointer, MixedRealityInputAction.None, 1);
            Assert.AreEqual(2, inputListener.pointerClickedCount);
            Assert.AreEqual(1, diagnosticsListener.eventCount);

            Object.Destroy(testObject);

            yield return null;
        }

        /// <summary>
        /// A test input global listener that will fire off another event (diagnostic system) as part of its
        /// own event handling. This is designed to test "nested global" events.
        /// </summary>
        internal class TestNestedInputSystemGlobalListener : TestInputGlobalListener
        {
            public override void OnPointerClicked(MixedRealityPointerEventData eventData)
            {
                // Invocation of base.OnPointerClicked because it updates pointerClickedCount
                base.OnPointerClicked(eventData);
                MixedRealityDiagnosticsSystem diagnosticsSystem = CoreServices.DiagnosticsSystem as MixedRealityDiagnosticsSystem;
                diagnosticsSystem.RaiseDiagnosticsChanged();
            }
        }

        /// <summary>
        /// A test diagnostic global listener that will unregister for events the moment it gets a callback.
        /// </summary>
        public class TestDiagnosticSystemGlobalListener : MonoBehaviour, IMixedRealityDiagnosticsHandler
        {
            /// <summary>
            /// The number of times OnDiagnosticSettingsChanged has been invoked.
            /// </summary>
            /// <remarks>
            /// Because this object gets unregistered in OnDiagnosticSettingsChanged this should really
            /// only ever have a max value of 1.
            /// </remarks>
            public int eventCount = 0;

            protected virtual async void Start()
            {
                await EnsureDiagnosticSystemValid();

                // We've been destroyed during the await.
                if (this == null)
                {
                    return;
                }

                CoreServices.DiagnosticsSystem.RegisterHandler<IMixedRealityDiagnosticsHandler>(this);
            }

            protected virtual void OnDisable()
            {
                CoreServices.DiagnosticsSystem.UnregisterHandler<IMixedRealityDiagnosticsHandler>(this);
            }

            protected async Task EnsureDiagnosticSystemValid()
            {
                if (CoreServices.DiagnosticsSystem == null)
                {
                    await new WaitUntil(() => CoreServices.DiagnosticsSystem != null);
                }
            }

            public void OnDiagnosticSettingsChanged(DiagnosticsEventData eventData)
            {
                eventCount++;
                // In response to this event, trigger an unregister to occur - this should
                // succeed in unregistering the handler, no matter when this is called (i.e
                // in a nested event loop)
                CoreServices.DiagnosticsSystem.UnregisterHandler<IMixedRealityDiagnosticsHandler>(this);
            }
        }
    }
}
#endif
