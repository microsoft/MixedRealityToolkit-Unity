// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    /// <summary>
    /// Tests to evaluate basic logic of ToggleCollection.
    /// </summary>
    public class ToggleCollectionTests : BaseRuntimeInputTests
    {
        /// <summary>
        /// A dummy interactor used to test basic selection/toggle logic.
        /// </summary>
        private class TestInteractor : XRBaseInteractor { }

        /// <summary>
        /// Makes sure toggles get added automatically at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestAutoAddToggles()
        {
            var parent = new GameObject();

            for (int i = 0; i < 10; i++)
            {
                var toggleObject = new GameObject();
                toggleObject.transform.parent = parent.transform;
                var interactable = toggleObject.AddComponent<StatefulInteractable>();
                interactable.ToggleMode = StatefulInteractable.ToggleType.OneWayToggle;
            }

            var toggleCollection = parent.AddComponent<ToggleCollection>();
            yield return null;

            Assert.IsTrue(toggleCollection.Toggles.Count == 10, "ToggleCollection didn't detect all of the toggles!");
        }

        /// <summary>
        /// Ensures basic toggling behavior works.
        /// </summary>
        [UnityTest]
        public IEnumerator TestToggleLogic()
        {
            var parent = new GameObject();

            for (int i = 0; i < 10; i++)
            {
                var toggleObject = new GameObject();
                toggleObject.transform.parent = parent.transform;
                var interactable = toggleObject.AddComponent<StatefulInteractable>();
                interactable.ToggleMode = StatefulInteractable.ToggleType.OneWayToggle;
            }

            var toggleCollection = parent.AddComponent<ToggleCollection>();
            yield return null;

            Assert.IsTrue(toggleCollection.Toggles.Count == 10, "ToggleCollection didn't detect all of the toggles!");

            var testInteractor = parent.AddComponent<TestInteractor>();

            testInteractor.StartManualInteraction(toggleCollection.Toggles[5] as IXRSelectInteractable);
            yield return null;
            yield return null;
            testInteractor.EndManualInteraction();

            Assert.IsTrue(toggleCollection.Toggles[5].IsToggled, "Interactable didn't get toggled");
            Assert.IsTrue(toggleCollection.CurrentIndex == 5, "ToggleCollection didn't update its CurrentIndex!");

            testInteractor.StartManualInteraction(toggleCollection.Toggles[1] as IXRSelectInteractable);
            yield return null;
            yield return null;
            testInteractor.EndManualInteraction();

            Assert.IsTrue(toggleCollection.CurrentIndex == 1, "ToggleCollection didn't update its CurrentIndex!");
            Assert.IsFalse(toggleCollection.Toggles[5].IsToggled, "Interactable didn't get detoggled");
        }

        /// <summary>
        /// Ensures setting detoggle permissions works correctly
        /// </summary>
        [UnityTest]
        public IEnumerator TestAllowSwitchOff()
        {
            var parent = new GameObject();

            for (int i = 0; i < 10; i++)
            {
                var toggleObject = new GameObject();
                toggleObject.transform.parent = parent.transform;
                var interactable = toggleObject.AddComponent<StatefulInteractable>();

                // Let's set the detoggle-able option. ToggleCollection should set this to OneWay at runtime.
                interactable.ToggleMode = StatefulInteractable.ToggleType.Toggle;
            }

            var toggleCollection = parent.AddComponent<ToggleCollection>();
            Assert.IsFalse(toggleCollection.AllowSwitchOff, "ToggleCollection shouldn't allow switching toggles off by default.");
            yield return null;

            Assert.IsTrue(toggleCollection.Toggles.Count == 10, "ToggleCollection didn't detect all of the toggles!");

            foreach (var toggle in toggleCollection.Toggles)
            {
                Assert.IsTrue(toggle.ToggleMode == StatefulInteractable.ToggleType.OneWayToggle, "ToggleCollection should have set all the toggle types to OneWay at startup.");
            }

            var testInteractor = parent.AddComponent<TestInteractor>();

            testInteractor.StartManualInteraction(toggleCollection.Toggles[5] as IXRSelectInteractable);
            yield return null;
            yield return null;
            testInteractor.EndManualInteraction();

            Assert.IsTrue(toggleCollection.Toggles[5].IsToggled, "Interactable didn't get toggled");
            Assert.IsTrue(toggleCollection.CurrentIndex == 5, "ToggleCollection didn't update its CurrentIndex!");

            // Try to toggle the same one. See if it detoggles (it shouldn't!)
            testInteractor.StartManualInteraction(toggleCollection.Toggles[5] as IXRSelectInteractable);
            yield return null;
            yield return null;
            testInteractor.EndManualInteraction();

            Assert.IsTrue(toggleCollection.Toggles[5].IsToggled, "Interactable got detoggled! ToggleCollection shouldn't let that happen.");
            Assert.IsTrue(toggleCollection.CurrentIndex == 5, "The index changed! That shouldn't happen if detoggling is prohibited.");

            // Now change the setting.
            toggleCollection.AllowSwitchOff = true;

            foreach (var toggle in toggleCollection.Toggles)
            {
                Assert.IsTrue(toggle.ToggleMode == StatefulInteractable.ToggleType.Toggle, "ToggleCollection should have set all the toggle types to Toggle when we set AllowSwitchOff true.");
            }

            // Try to toggle the same one. Now, it should detoggle!
            testInteractor.StartManualInteraction(toggleCollection.Toggles[5] as IXRSelectInteractable);
            yield return null;
            yield return null;
            testInteractor.EndManualInteraction();

            Assert.IsFalse(toggleCollection.Toggles[5].IsToggled, "Interactable didn't get detoggled! ToggleCollection should have allowed it.");
        }
    }
}