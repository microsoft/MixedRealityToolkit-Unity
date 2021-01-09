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

using Microsoft.MixedReality.Toolkit.UI.HandCoach;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class HandCoachTests
    {
        private const string handCoachRightGuid = "2225b28a6968ba04594a7564e934a679";
        private static readonly string handCoachRightPath = AssetDatabase.GUIDToAssetPath(handCoachRightGuid);

        [UnitySetUp]
        public IEnumerator Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayModeTestUtilities.TearDown();
            yield return null;
        }

        #region Tests
        /// <summary>
        /// Tests that the hand coach is disabled when the hand is brought up
        /// at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandCoachActivation()
        {
            InstantiateHandCoach(out HandInteractionHint handCoach);
            handCoach.AnimationState = "HandFlip_R";
            handCoach.HideIfHandTracked = true;
            handCoach.HintDisplayDelay = 0.0f;

            float timingLeeway = 0.1f;
            float fadeOutTime = 1.0f;

            handCoach.transform.position = Vector3.forward;

            handCoach.StartHintLoop();
            yield return new WaitForSeconds(timingLeeway);

            Assert.IsTrue(handCoach.VisualsRoot.activeSelf);

            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            yield return null;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService, ArticulatedHandPose.GestureId.Open, Vector3.forward);
            
            yield return new WaitForSeconds(fadeOutTime);

            Assert.IsFalse(handCoach.VisualsRoot.activeSelf);

        }

        /// <summary>
        /// Tests that the hand coach lifetime management works as expected
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandCoachLifetime()
        {
            InstantiateHandCoach(out HandInteractionHint handCoach);
            string animationState = "NearSelect_R";
            float hintDelay = 0.4f;
            float repeatDelay = 0.5f;
            float timingLeeway = 0.1f;

            handCoach.AnimationState = animationState;
            handCoach.HideIfHandTracked = false;
            handCoach.HintDisplayDelay = hintDelay;
            handCoach.RepeatDelay = repeatDelay;
            handCoach.Repeats = 1;

            handCoach.transform.position = Vector3.forward;

            handCoach.StartHintLoop();
            Assert.IsFalse(handCoach.VisualsRoot.activeSelf);

            yield return new WaitForSeconds(hintDelay + timingLeeway);
            Assert.IsTrue(handCoach.VisualsRoot.activeSelf);

            yield return new WaitForSeconds(handCoach.GetAnimationDuration(animationState));
            Assert.IsFalse(handCoach.VisualsRoot.activeSelf);

            yield return new WaitForSeconds(repeatDelay);
            Assert.IsTrue(handCoach.VisualsRoot.activeSelf);

            yield return new WaitForSeconds(handCoach.GetAnimationDuration(animationState));
            Assert.IsFalse(handCoach.VisualsRoot.activeSelf);
        }

        /// <summary>
        /// Tests that the hand coach lifetime management works as expected when a hand is tracked
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandCoachLifetimeWhenTracked()
        {
            InstantiateHandCoach(out HandInteractionHint handCoach);
            string animationState = "NearSelect_R";
            float hintDelay = 0.4f;
            float repeatDelay = 0.5f;
            float timingLeeway = 0.1f;

            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            yield return null;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService, ArticulatedHandPose.GestureId.Open, Vector3.forward);


            handCoach.AnimationState = animationState;
            handCoach.HideIfHandTracked = false;
            handCoach.TrackedHandHintDisplayDelay = hintDelay;
            handCoach.RepeatDelay = repeatDelay;
            handCoach.Repeats = 1;

            handCoach.transform.position = Vector3.forward;

            handCoach.StartHintLoop();
            Assert.IsFalse(handCoach.VisualsRoot.activeSelf);

            yield return new WaitForSeconds(hintDelay + timingLeeway);
            Assert.IsTrue(handCoach.VisualsRoot.activeSelf);

            yield return new WaitForSeconds(handCoach.GetAnimationDuration(animationState));
            Assert.IsFalse(handCoach.VisualsRoot.activeSelf);

            yield return new WaitForSeconds(repeatDelay);
            Assert.IsTrue(handCoach.VisualsRoot.activeSelf);

            yield return new WaitForSeconds(handCoach.GetAnimationDuration(animationState));
            Assert.IsFalse(handCoach.VisualsRoot.activeSelf);
        }
        #endregion Tests

        #region Private methods
        /// <summary>
        /// Instantiates the default interactable button.
        /// </summary>
        private void InstantiateHandCoach(out HandInteractionHint handCoach)
        {
            // Load HandCoach prefab.
            GameObject handCoachPrefab = AssetDatabase.LoadAssetAtPath(handCoachRightPath, typeof(Object)) as GameObject;
            handCoachPrefab.GetComponentInChildren<HandInteractionHint>().AutoActivate = false;

            GameObject handCoachGameObject = Object.Instantiate(handCoachPrefab) as GameObject;
            Assert.IsNotNull(handCoachGameObject);

            handCoach = handCoachGameObject.GetComponentInChildren<HandInteractionHint>();
            Assert.IsNotNull(handCoach);
        }
        #endregion Private methods
    }
}
#endif