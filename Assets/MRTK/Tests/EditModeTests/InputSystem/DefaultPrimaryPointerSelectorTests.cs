// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.InputSystem
{
    public class DefaultPrimaryPointerSelectorTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        [Test]
        public void Test()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            var selector = new DefaultPrimaryPointerSelector();
            selector.Initialize();

            // No primary pointer
            Assert.IsNull(selector.Update());

            var pointer1 = new TestPointer();
            Assert.IsFalse(pointer1.IsInteractionEnabled);

            selector.RegisterPointer(pointer1);

            // Still no primary pointer because pointer 1 is not interaction enabled
            Assert.IsNull(selector.Update());

            pointer1.IsInteractionEnabled = true;

            // Pointer 1 selected as primary
            Assert.AreSame(pointer1, selector.Update());

            pointer1.IsInteractionEnabled = false;

            // No primary pointer
            Assert.IsNull(selector.Update());

            pointer1.IsInteractionEnabled = true;

            // Pointer 1 selected as primary
            Assert.AreSame(pointer1, selector.Update());

            var pointer2 = new TestPointer();
            pointer2.IsInteractionEnabled = true;
            selector.RegisterPointer(pointer2);

            // Pointer 2 selected as primary because it got enabled last
            Assert.AreSame(pointer2, selector.Update());

            var pointerHandler = selector as IMixedRealityPointerHandler;
            var eventData = new MixedRealityPointerEventData(null);

            // Pointer 1 down
            eventData.Initialize(pointer1, MixedRealityInputAction.None);
            pointerHandler.OnPointerDown(eventData);

            // Pointer 1 selected as primary because it is pressed
            Assert.AreSame(pointer1, selector.Update());

            // Pointer 2 down
            eventData.Initialize(pointer2, MixedRealityInputAction.None);
            pointerHandler.OnPointerDown(eventData);

            // Pointer 1 still primary because it has been pressed longer
            Assert.AreSame(pointer1, selector.Update());

            // Pointer 1 up
            eventData.Initialize(pointer1, MixedRealityInputAction.None);
            pointerHandler.OnPointerUp(eventData);

            // Pointer 2 primary because it is the only one pressed
            Assert.AreSame(pointer2, selector.Update());

            // Pointer 2 up
            eventData.Initialize(pointer2, MixedRealityInputAction.None);
            pointerHandler.OnPointerUp(eventData);

            // Pointer 2 still primary because it is the one released last
            Assert.AreSame(pointer2, selector.Update());

            selector.UnregisterPointer(pointer2);

            // Pointer 1 primary because it is the only one left and is interaction enabled
            Assert.AreSame(pointer1, selector.Update());

            selector.UnregisterPointer(pointer1);

            // No primary pointer
            Assert.IsNull(selector.Update());

            selector.Destroy();
        }
    }
}