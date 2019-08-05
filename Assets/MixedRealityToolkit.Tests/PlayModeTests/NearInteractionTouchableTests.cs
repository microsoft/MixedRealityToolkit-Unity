// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class NearInteractionTouchableTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            PlayModeTestUtilities.EnsureInputModule();

            inputSim = PlayModeTestUtilities.GetInputSimulationService();
            Assert.NotNull(inputSim);

            Assert.IsTrue(MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem));
            Assert.NotNull(inputSystem);

            GameObject lightObj = new GameObject("Light");
            lightObj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, new Vector3(1, -4, 2));
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;

            var shader = Shader.Find("Mixed Reality Toolkit/Standard");

            idleMaterial = new Material(shader);
            idleMaterial.color = Color.yellow;

            pokeMaterial = new Material(shader);
            pokeMaterial.color = Color.green;
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// Test creating adding a NearInteractionTouchable to GameObject programmatically.
        /// Should be able to run scene without getting any exceptions.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator NearInteractionTouchableInstantiate()
        {
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;

            var touchable = testObject.AddComponent<NearInteractionTouchable>();
            // Wait for two frames to make sure we don't get null pointer exception.
            yield return null;
            yield return null;

            GameObject.Destroy(testObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        private const int numSteps = 30;
        // Scale larger than bounds vector to test bounds checks
        private float objectScale = 0.4f;
        private Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
        private Vector3 objectPosition = new Vector3(0f, 0f, 1f);
        private Vector3 rightPosition = new Vector3(1f, 0f, 1f);
        private Vector3 backPosition = new Vector3(0f, 0f, 2f);
        private Vector3 outOfBoundsOffset = new Vector3(0.15f, 0f, 0f);
        private Vector3 touchNormal = new Vector3(0, 0, -1);

        private Material idleMaterial;
        private Material pokeMaterial;

        // 96 float triplets, random points inside a unit sphere
        const int numRandomPoints = 96;
        private static readonly float[] randomPoints = new float[]
        {
            0.235f,0.517f,-0.594f,0.783f,0.248f,-0.513f,0.242f,-0.805f,-0.405f,0.172f,-0.533f,-0.359f,-0.629f,0.133f,-0.091f,-0.385f,-0.109f,0.171f,-0.282f,-0.001f,0.837f,0.225f,-0.676f,0.581f,-0.923f,-0.137f,0.057f,0.682f,-0.355f,0.182f,-0.409f,-0.411f,0.402f,-0.762f,-0.089f,-0.400f,
            0.747f,-0.140f,-0.639f,-0.622f,0.128f,0.485f,-0.460f,-0.232f,0.475f,0.088f,0.645f,-0.257f,0.251f,-0.516f,-0.273f,0.794f,-0.394f,0.168f,0.733f,-0.297f,-0.234f,0.313f,0.460f,0.454f,0.102f,-0.445f,0.289f,0.236f,0.333f,-0.287f,0.560f,-0.715f,0.194f,0.161f,0.705f,0.303f,
            -0.182f,-0.506f,-0.115f,0.442f,-0.568f,-0.019f,0.457f,0.186f,0.291f,0.844f,0.175f,0.322f,0.612f,0.116f,0.169f,-0.437f,0.137f,-0.594f,-0.651f,0.558f,-0.358f,0.222f,-0.355f,-0.564f,-0.340f,-0.772f,0.514f,-0.072f,-0.002f,0.687f,0.551f,0.219f,-0.498f,0.027f,-0.164f,0.750f,
            0.334f,-0.593f,-0.374f,-0.407f,-0.868f,-0.212f,-0.387f,0.734f,-0.152f,-0.326f,0.242f,0.680f,-0.659f,0.307f,0.362f,0.108f,0.816f,0.107f,0.357f,-0.089f,-0.712f,0.065f,-0.766f,0.602f,-0.233f,-0.691f,-0.390f,0.824f,0.061f,0.519f,-0.276f,-0.168f,0.584f,0.076f,-0.206f,0.706f,
            -0.084f,0.434f,-0.760f,0.752f,-0.408f,-0.482f,0.206f,0.132f,0.483f,-0.093f,0.096f,0.591f,0.557f,-0.310f,0.569f,0.824f,0.457f,0.059f,-0.658f,-0.354f,0.604f,0.257f,0.029f,-0.793f,-0.026f,-0.744f,0.064f,-0.144f,-0.590f,-0.754f,0.261f,-0.490f,-0.025f,-0.625f,0.524f,0.051f,
            -0.102f,0.072f,0.793f,0.124f,-0.136f,-0.672f,0.278f,-0.764f,-0.351f,-0.728f,-0.431f,-0.324f,0.270f,-0.421f,-0.434f,-0.985f,-0.037f,-0.150f,0.167f,0.157f,-0.803f,-0.757f,-0.078f,0.122f,-0.223f,-0.570f,0.021f,0.203f,0.634f,-0.743f,0.154f,0.581f,-0.147f,0.108f,0.486f,-0.658f,
            -0.106f,0.490f,0.121f,0.130f,-0.422f,-0.746f,0.058f,0.345f,0.865f,-0.773f,0.397f,0.495f,0.076f,0.633f,-0.423f,0.333f,-0.175f,0.192f,0.130f,0.405f,-0.409f,0.145f,-0.668f,-0.003f,0.704f,0.191f,0.597f,0.203f,0.540f,0.306f,-0.369f,0.808f,-0.354f,-0.406f,-0.586f,-0.039f,
            -0.461f,-0.653f,-0.124f,-0.137f,-0.367f,-0.386f,-0.653f,0.305f,0.010f,0.550f,-0.114f,0.445f,0.362f,0.279f,0.126f,-0.206f,-0.226f,-0.664f,-0.052f,-0.334f,-0.870f,0.503f,0.589f,0.456f,0.766f,-0.091f,0.575f,-0.175f,-0.142f,-0.308f,-0.228f,-0.364f,0.215f,-0.738f,-0.494f,-0.325f,
        };

        private static Vector3 GetRandomPoint(int i)
        {
            int idx = i % numRandomPoints;
            return new Vector3(randomPoints[3*idx + 0], randomPoints[3*idx + 1], randomPoints[3*idx + 2]);
        }

        private InputSimulationService inputSim;
        IMixedRealityInputSystem inputSystem;

        private T CreateTouchable<T>(float scale) where T : BaseNearInteractionTouchable
        {
            // Set up cube with touchable
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = new Vector3(1.0f, 1.0f, 0.1f) * scale;
            testObject.transform.position = objectPosition;

            testObject.GetComponent<Renderer>().material = idleMaterial;

            var touchable = testObject.AddComponent<T>();

            return touchable;
        }

        private TouchEventCatcher CreateEventCatcher(BaseNearInteractionTouchable touchable)
        {
            var catcher = TouchEventCatcher.Create(touchable.gameObject);

            catcher.OnTouchStartedEvent.AddListener(() =>
            {
                touchable.GetComponent<Renderer>().material = pokeMaterial;
            });
            catcher.OnTouchCompletedEvent.AddListener(() =>
            {
                touchable.GetComponent<Renderer>().material = idleMaterial;
            });

            return catcher;
        }

        /// <summary>
        /// Test creates an object with NearInteractionTouchable
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator NearInteractionTouchableVariant()
        {
            var touchable = CreateTouchable<NearInteractionTouchable>(objectScale);
            touchable.SetLocalForward(touchNormal);
            touchable.Bounds = new Vector2(0.5f, 0.5f);

            yield return new WaitForFixedUpdate();
            yield return null;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSim);

            using (var catcher = CreateEventCatcher(touchable))
            {
                // Touch started and completed when entering and exiting
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(0, catcher.EventsCompleted);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(1, catcher.EventsCompleted);

                // Touch started and completed when entering and exiting behind the plane
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(1, catcher.EventsCompleted);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, backPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);

                // No touch when moving at behind the plane
                yield return PlayModeTestUtilities.MoveHandFromTo(backPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);

                // No touch when moving outside the bounds
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition + outOfBoundsOffset, objectPosition + outOfBoundsOffset, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition + outOfBoundsOffset, rightPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);
            }

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSim);

            UnityEngine.Object.Destroy(touchable.gameObject);
        }

        /// <summary>
        /// Test creates an object with NearInteractionTouchableVolume
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator NearInteractionTouchableVolumeVariant()
        {
            var touchable = CreateTouchable<NearInteractionTouchableVolume>(objectScale);

            yield return new WaitForFixedUpdate();
            yield return null;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSim);

            using (var catcher = CreateEventCatcher(touchable))
            {
                // Touch started and completed when entering and exiting the collider
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(0, catcher.EventsCompleted);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(1, catcher.EventsCompleted);

                // No touch when moving outside the collider
                yield return PlayModeTestUtilities.MoveHandFromTo(backPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(1, catcher.EventsCompleted);

                // Touch when moving off-center
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition + outOfBoundsOffset, objectPosition + outOfBoundsOffset, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition + outOfBoundsOffset, rightPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);
            }

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSim);

            UnityEngine.Object.Destroy(touchable.gameObject);
        }

        private static void TestEvents(TouchEventCatcher[] catchers, int[] eventsStarted, int[] eventsCompleted)
        {
            Assert.AreEqual(catchers.Length, eventsCompleted.Length);
            Assert.AreEqual(catchers.Length, eventsStarted.Length);
            for (int i = 0; i < catchers.Length; ++i)
            {
                Assert.AreEqual(catchers[i].EventsStarted, eventsStarted[i]);
                Assert.AreEqual(catchers[i].EventsCompleted, eventsCompleted[i]);
            }
        }

        // This test is temporarily removed for the following reasons:
        // - This test was moving the hand through a number of stacked touchables. Previously
        //   moving the hand through these touchables gave touch events first for touchables[0],
        //   and then touchables[3] twice. These events weren't necessarily expected to occur
        //   in this manner, but because that was the behaviour when the test was written, we
        //   were verifying that this behaviour did not change. However, with the new changes
        //   to the hand gesture JSON files, it became clear that this test was brittle
        // - The PokePointer ray starts a distance behind the pointer. This means that in cases
        //   where you have touchables stacked on top of each other, like in this test, the
        //   pointer target will be behind the hand.
        // - The PokePointer.closestProximityTouchable however is the object with the shortest
        //   distance to the pointer, using NearInteractionTouchable.DistanceToTouchable to
        //   calculate distance. This calculation gets the distance from the pointer to the
        //   touchable surface, but if the pointer is not in front of or behind the surface, it
        //   will return float.PositiveInfinity.
        // - In order for RaiseOnTouchStarted to be called, the closestProximityTouchable needs
        //   to be part of the same GameObject as the pointer target. But for the reasons
        //   outlined above, in this test, the pointer target is most often well behind the
        //   hand, whereas the closestProximityTouchable is nearer the hand. Because they are
        //   not on the same object, we get no touch down events for touchables excluding the
        //   touchables[0], where these would be on the same object, and touchables[3].
        // - So why touchables[3]? Well the touchables in this test had random x,y offsets that
        //   were consistent across runs. When the hand in this test moved towards the right, it
        //   would by chance no longer be in front of or behind the touchables closer to it, and
        //   so NearInteractionTouchable.DistanceToTouchable would return infinity for these
        //   touchables. However, the hand would still be in front of touchables[3], which
        //   happened to be the pointer target when the hand started moving to the right. And so
        //   we would get touch down for touchables[3] even though the hand was not close to it.
        // - With the minor changes to the hand gestures, the precise series of events that led
        //   to touchables[3] getting a touch down were no longer occuring, and so the test
        //   would fail.
        // - Because stacked touchables are broken, and because this test was only passing
        //   before because of strange behaviour, I have commented out this test, until we have
        //   fixed touchables and we have a way to test this properly.
        // - Here is a link to a PR that aims to fix many aspects of touch, but cannot be
        //   completed before GA: 
        //   https://github.com/microsoft/MixedRealityToolkit-Unity/pull/5264 

#if false
        /// <summary>
        /// Test scene query with stacked touchables.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator NearInteractionTouchableStack()
        {
            int numTouchables = 10;
            var touchables = new NearInteractionTouchable[numTouchables];
            var catchers = new TouchEventCatcher[numTouchables];
            for (int i = 0; i < numTouchables; ++i)
            {
                Vector3 r = GetRandomPoint(i);

                touchables[i] = CreateTouchable<NearInteractionTouchable>(0.15f);
                touchables[i].SetLocalForward(touchNormal);
                touchables[i].Bounds = new Vector2(0.5f, 0.5f);
                touchables[i].transform.position = objectPosition + new Vector3(0.02f * r.x, 0.015f * r.y, 0.1f * i - 0.5f);

                catchers[i] = CreateEventCatcher(touchables[i]);
            }

            yield return new WaitForFixedUpdate();
            yield return null;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSim);

            yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
            // No. 0 is touched initially
            TestEvents(catchers, new int [] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int [] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
            // Only No. 3 gets touched when moving through the row, because No. 0 is still active while inside the poke threshold
            TestEvents(catchers, new int [] { 1, 0, 0, 1, 0, 0, 0, 0, 0, 0 }, new int [] { 1, 0, 0, 1, 0, 0, 0, 0, 0, 0 });
            yield return PlayModeTestUtilities.MoveHandFromTo(rightPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
            // No. 3 touched a second time
            TestEvents(catchers, new int [] { 1, 0, 0, 2, 0, 0, 0, 0, 0, 0 }, new int [] { 1, 0, 0, 2, 0, 0, 0, 0, 0, 0 });

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSim);

            foreach (var touchable in touchables)
            {
                UnityEngine.Object.Destroy(touchable.gameObject);
            }
        }
#endif

        /// <summary>
        /// Test buffer saturation for the overlap query
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator NearInteractionTouchableOverlapQuerySaturation()
        {
            // Use all the points
            int numTouchables = numRandomPoints;
            var touchables = new NearInteractionTouchable[numTouchables];
            var catchers = new TouchEventCatcher[numTouchables];

            // Spread out touchables over a radius, decrease over time to increase density and fill the buffer
            float radiusStart = 1.0f;
            float radiusEnd = 0.01f;

            for (int i = 0; i < numTouchables; ++i)
            {
                Vector3 r = GetRandomPoint(i);

                touchables[i] = CreateTouchable<NearInteractionTouchable>(0.15f);
                touchables[i].SetLocalForward(touchNormal);
                touchables[i].Bounds = new Vector2(0.5f, 0.5f);
                touchables[i].transform.position = objectPosition + r * radiusStart;

                catchers[i] = CreateEventCatcher(touchables[i]);
            }

            yield return new WaitForFixedUpdate();
            yield return null;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSim);
            yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, 1, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);

            for (int i = 0; i < numSteps; ++i)
            {
                float scale = radiusStart + (radiusEnd - radiusStart) * (float)(i + 1) / (float)numSteps;
                for (int j = 0; j < numTouchables; ++j)
                {
                    Vector3 r = GetRandomPoint(j + 10);
                    touchables[j].transform.position = objectPosition + r * scale;
                }
                yield return null;
            }

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSim);

            foreach (var touchable in touchables)
            {
                UnityEngine.Object.Destroy(touchable.gameObject);
            }
        }

        /// <summary>
        /// Test Unity UI button
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator NearInteractionTouchableUnityUiButton()
        {

            var canvas = UnityUiUtilities.CreateCanvas(0.002f);

            var button = UnityUiUtilities.CreateButton(Color.gray, Color.blue, Color.green);
            button.transform.SetParent(canvas.transform, false);
            var text = UnityUiUtilities.CreateText("test");
            text.transform.SetParent(button.transform);

            canvas.transform.position = objectPosition;

            yield return new WaitForFixedUpdate();
            yield return null;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSim);

            using (var catcher = new UnityButtonEventCatcher(button))
            {
                // Touch started and completed when entering and exiting
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(0, catcher.Click);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, initialHandPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.Click);
            }

            UnityEngine.Object.Destroy(canvas.gameObject);
        }

        /// <summary>
        /// Test Unity UI toggle button
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator NearInteractionTouchableUnityUiToggle()
        {

            var canvas = UnityUiUtilities.CreateCanvas(0.002f);

            var toggle = UnityUiUtilities.CreateToggle(Color.gray, Color.blue, Color.green);
            toggle.transform.SetParent(canvas.transform, false);
            var text = UnityUiUtilities.CreateText("test");
            text.transform.SetParent(toggle.transform);

            canvas.transform.position = objectPosition;

            yield return new WaitForFixedUpdate();
            yield return null;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSim);

            using (var catcher = new UnityToggleEventCatcher(toggle))
            {
                // Turn on the toggle after exiting
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.IsFalse(catcher.IsOn);
                Assert.AreEqual(0, catcher.Changed);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, initialHandPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.IsTrue(catcher.IsOn);
                Assert.AreEqual(1, catcher.Changed);

                // Turn off the toggle after exiting
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.IsTrue(catcher.IsOn);
                Assert.AreEqual(1, catcher.Changed);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, initialHandPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.IsFalse(catcher.IsOn);
                Assert.AreEqual(2, catcher.Changed);
            }

            UnityEngine.Object.Destroy(canvas.gameObject);
        }

    }
}
#endif
