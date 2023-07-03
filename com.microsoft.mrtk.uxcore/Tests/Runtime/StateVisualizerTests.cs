// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TestTools;

using HandshapeId = Microsoft.MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    /// <summary>
    /// Tests to evaluate the functionality of the StateVisualizer modular visual driver.
    /// </summary>
    public class StateVisualizerTests : BaseRuntimeInputTests
    {
        [UnityTest]
        /// <summary>
        /// Tests to see if StateVisualizer can correctly evaluate/execute the SetTargetActiveEffect.
        /// Adds/removes the SetTargetActiveEffect from a few different states to make sure everything works.
        /// </summary>
        public IEnumerator TestSetTargetsActiveEffect()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            StatefulInteractable interactable = cube.AddComponent<StatefulInteractable>() as StatefulInteractable;
            interactable.DisableInteractorType(typeof(IPokeInteractor));
            cube.AddComponent<Animator>();
            StateVisualizer sv = cube.AddComponent<StateVisualizer>() as StateVisualizer;

            cube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.1f, 0.1f, 1));
            cube.transform.localScale = Vector3.one * 0.1f;

            GameObject cubeToToggle = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Attach a toggle effect to the Select state.
            SetTargetsActiveEffect toggleEffect = new SetTargetsActiveEffect(new List<GameObject>() { cubeToToggle });
            sv.AddEffect("Select", toggleEffect);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(cubeToToggle.activeSelf, "The cube should be immediately toggled off when the effect is added.");

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser(0.5f));
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.MoveTo(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(cubeToToggle.activeSelf, "Nothing should have happened on hover.");
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(cubeToToggle.activeSelf, "The toggle effect should have turned on the cube on selection.");
            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(cubeToToggle.activeSelf, "The toggle effect should have turned off the cube on deselection.");

            // Detach the effect from the state.
            sv.RemoveEffect("Select", toggleEffect);
            yield return rightHand.MoveTo(Vector3.zero);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.MoveTo(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(cubeToToggle.activeSelf, "Nothing should have happened on hover.");
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(cubeToToggle.activeSelf, "The toggle effect should have been removed from the StateVisualizer!");
            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Attach the same toggle effect to the ActiveHover state instead.
            sv.AddEffect("ActiveHover", toggleEffect);
            yield return rightHand.MoveTo(Vector3.zero);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.MoveTo(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(cubeToToggle.activeSelf, "The toggle effect should have turned on the cube on active hover.");
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(cubeToToggle.activeSelf, "The cube should have stayed toggled.");
            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Detach the effect and attach it to the Toggle state instead.
            sv.RemoveEffect("ActiveHover", toggleEffect);
            sv.AddEffect("Toggle", toggleEffect);
            yield return rightHand.MoveTo(Vector3.zero);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Have to turn on toggle mode on the interactable, or else toggles won't toggle :)
            interactable.ToggleMode = StatefulInteractable.ToggleType.Toggle;

            yield return rightHand.MoveTo(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(cubeToToggle.activeSelf, "Nothing should have happened on hover.");
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(cubeToToggle.activeSelf, "Nothing should have happened on selection.");
            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(cubeToToggle.activeSelf, "The toggle effect should have turned on the cube on IsToggled = true.");

            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(cubeToToggle.activeSelf, "Cube should be toggled off again.");
        }

        /// <summary>
        /// This test effect scales the object up by the parameter value.
        /// </summary>
        private class CustomTestEffect : IEffect
        {
            public Playable Playable => Playable.Null;
            private Vector3 initialLocalScale;
            private GameObject owner;

            // Lets us check the parameter passed in for testing.
            public float LastSetParameter = 0;

            public void Setup(PlayableGraph graph, GameObject owner)
            {
                initialLocalScale = owner.transform.localScale;
                this.owner = owner;
            }

            public bool Evaluate(float parameter)
            {
                owner.transform.localScale = initialLocalScale * (1.0f + parameter);
                LastSetParameter = parameter;
                return true;
            }
        }

        [UnityTest]
        /// <summary>
        /// Tests to see whether StateVisualizer can properly evaluate/execute a custom effect,
        /// defined above (CustomTestEffect).
        /// </summary>
        public IEnumerator TestCustomEffect()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            StatefulInteractable interactable = cube.AddComponent<StatefulInteractable>() as StatefulInteractable;
            interactable.DisableInteractorType(typeof(IPokeInteractor));
            cube.AddComponent<Animator>();
            StateVisualizer sv = cube.AddComponent<StateVisualizer>() as StateVisualizer;

            cube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.1f, 0.1f, 1));
            cube.transform.localScale = Vector3.one * 0.1f;

            // Attach a toggle effect to the Select state.
            CustomTestEffect customEffect = new CustomTestEffect();
            sv.AddEffect("Select", customEffect);

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser(0.5f));
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.MoveTo(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.AreEqual(cube.transform.localScale.z, 0.1f, 0.00001f, "Nothing should have happened on hover.");
            Assert.AreEqual(customEffect.LastSetParameter, 0.0f, 0.00001f, "The custom effect should have received the parameter value of 0.0f.");
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(cube.transform.localScale.z, 0.2f, 0.00001f, "The custom effect should have made the cube grow in size on selection.");
            Assert.AreEqual(customEffect.LastSetParameter, 1.0f, 0.00001f, "The custom effect should have received the parameter value of 1.0f.");
            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(cube.transform.localScale.z, 0.1f, 0.00001f, "The custom effect should have returned the cube to its original size on deselection.");
            Assert.AreEqual(customEffect.LastSetParameter, 0.0f, 0.00001f, "The custom effect should have received the parameter value of 0.0f.");
        }

        [UnityTest]
        /// <summary>
        /// Makes sure StateVisualizer is going to sleep/waking up correctly to save resources.
        /// </summary>
        public IEnumerator TestSleepWakeBehavior()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            StatefulInteractable interactable = cube.AddComponent<StatefulInteractable>() as StatefulInteractable;
            interactable.DisableInteractorType(typeof(IPokeInteractor));
            cube.AddComponent<Animator>();
            StateVisualizer sv = cube.AddComponent<StateVisualizer>() as StateVisualizer;

            cube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.5f, 0.5f, 1));
            cube.transform.localScale = Vector3.one * 0.1f;

            // Attach a test effect to the Select state.
            CustomTestEffect customEffect = new CustomTestEffect();
            sv.AddEffect("Select", customEffect);

            Assert.IsTrue(sv.Animator.enabled, "The animator should be enabled by default.");

            // Wait for the keepAliveTime
            yield return new WaitForSeconds(0.11f);

            Assert.IsFalse(sv.Animator.enabled, "The animator should be disabled after the keepAliveTime has elapsed.");

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser(0.5f));
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.MoveTo(cube.transform.position);
            yield return new WaitForSeconds(1.0f);

            Assert.IsTrue(sv.Animator.enabled, "The animator should have woken up when hovered.");

            yield return new WaitForSeconds(0.5f);

            Assert.IsTrue(sv.Animator.enabled, "The animator should remain awake throughout a hover.");

            yield return rightHand.MoveTo(Vector3.zero);

            Assert.IsTrue(sv.Animator.enabled, "The animator should still be enabled after a short delay.");

            // Wait for the rest of the keepAliveTime
            yield return new WaitForSeconds(0.5f);

            Assert.IsFalse(sv.Animator.enabled, "The animator should be disabled after the keepAliveTime has elapsed.");
        }

        /// <summary>
        /// This test effect does not report that it is "done" until the parameter returns to zero.
        /// Used to test the "keep awake" behavior.
        /// </summary>
        private class TestEffectThatJustKeepsGoing : IEffect
        {
            public Playable Playable => Playable.Null;

            public void Setup(PlayableGraph graph, GameObject owner) { }

            public bool Evaluate(float parameter)
            {
                // This effect will only be done once the parameter returns to zero.
                // StateVisualizer will stay awake, waiting for this to be done.
                return parameter == 0.0f;
            }
        }

        [UnityTest]
        /// <summary>
        /// Uses TestEffectThatJustKeepsGoing to see if the StateVisualizer will stay awake
        /// waiting for the effect to be done.
        /// </summary>
        public IEnumerator TestWaitForLongRunningEffect()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            StatefulInteractable interactable = cube.AddComponent<StatefulInteractable>() as StatefulInteractable;
            interactable.DisableInteractorType(typeof(IPokeInteractor));
            cube.AddComponent<Animator>();
            StateVisualizer sv = cube.AddComponent<StateVisualizer>() as StateVisualizer;

            cube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.1f, 0.1f, 1));
            cube.transform.localScale = Vector3.one * 0.1f;

            // Attach a test effect to the Select state.
            TestEffectThatJustKeepsGoing customEffect = new TestEffectThatJustKeepsGoing();
            sv.AddEffect("Select", customEffect);

            Assert.IsTrue(sv.Animator.enabled, "The animator should be enabled by default.");

            // Wait for the keepAliveTime
            yield return new WaitForSeconds(0.12f);

            Assert.IsFalse(sv.Animator.enabled, "The animator should be disabled after the keepAliveTime has elapsed.");

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser(0.5f));
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.MoveTo(cube.transform.position);
            yield return new WaitForSeconds(1.0f);

            Assert.IsTrue(sv.Animator.enabled, "The animator should have woken up when hovered.");

            yield return new WaitForSeconds(0.5f);

            Assert.IsTrue(sv.Animator.enabled, "The animator should remain awake throughout a hover.");

            yield return rightHand.MoveTo(Vector3.zero);

            Assert.IsTrue(sv.Animator.enabled, "The animator should still be enabled after a short delay.");

            // Wait for the keepAliveTime
            yield return new WaitForSeconds(0.1f);

            Assert.IsFalse(sv.Animator.enabled, "The animator should be disabled after the keepAliveTime has elapsed.");

            yield return rightHand.MoveTo(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactable.isSelected && interactable.IsGrabSelected, "Interactable wasn't selected");
            Assert.IsTrue(sv.Animator.enabled, "The animator should have woken up when selected.");

            // Wait for far longer than the keepAliveTime.
            // The StateVisualizer should remain awake, waiting for our TestEffectThatJustKeepsGoing to be done.
            // However, our test effect will never be done, until we un-hover.
            yield return new WaitForSeconds(0.5f);

            Assert.IsTrue(sv.Animator.enabled, "StateVisualizer did not wait for the effect to be done!");

            yield return rightHand.MoveTo(Vector3.zero);

            // Wait for longer than the keepAliveTime. The hand is still selecting, and so the stateviz shouldn't go back to sleep.
            yield return new WaitForSeconds(0.5f);

            Assert.IsTrue(sv.Animator.enabled, "The animator should remain awake throughout a select.");

            yield return rightHand.SetHandshape(HandshapeId.Open);

            // Wait for longer than the keepAliveTime. The effect should report "done" and the StateVisualizer should go back to sleep.
            yield return new WaitForSeconds(0.25f);

            Assert.IsFalse(sv.Animator.enabled, "StateVisualizer did not go back to sleep!");
        }

        [UnityTest]
        /// <summary>
        /// Makes sure an Animator component is added when necessary.
        /// </summary>
        public IEnumerator TestAnimatorMissing()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<StatefulInteractable>();
            cube.AddComponent<StateVisualizer>();

            yield return null;

            Assert.IsTrue(cube.GetComponent<Animator>() != null, "An animator wasn't automatically added when it was missing!");
        }
    }
}
