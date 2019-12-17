#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using System.Collections;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Utility class that provides helpful methods for generating and assembling buttons to be used in tests
    /// Also provides functions to test/simulate pressing buttons etc
    /// </summary>
    public static class TestButtonUtilities
    {
        public const float ButtonPressAnimationDelay = 0.25f;
        public const float ButtonReleaseAnimationDelay = 0.25f;
        private const int MoveHandNumSteps = 32;
        private static readonly Vector3 ButtonTranslateOffset = new Vector3(0.05f, 0f, 0.51f);

        /// <summary>
        /// TODO: Troy - add comments
        /// </summary>
        public static GameObject InstantiateInteractableFromPath(Vector3 position, Quaternion rotation, string path)
        {
            // Load interactable prefab
            Object interactablePrefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            GameObject result = Object.Instantiate(interactablePrefab) as GameObject;
            Assert.IsNotNull(result);

            // Move the object into position
            result.transform.position = position;
            result.transform.rotation = rotation;
            return result;
        }

        /// <summary>
        /// Instantiates Hololens Pressable Button from different Prefabs
        /// </summary>
        public static void InstantiatePressableButtonPrefab(Vector3 position, Quaternion rotation, 
            string prefabPath, string translateTargetPath, out Interactable interactable, out Transform translateTargetTransform)
        {
            // Load interactable prefab
            var interactableObject = InstantiateInteractableFromPath(position, rotation, prefabPath);
            interactable = interactableObject.GetComponent<Interactable>();
            Assert.IsNotNull(interactable);

            // Find the target object for the interactable transformation
            translateTargetTransform = interactableObject.transform.Find(translateTargetPath);

            Assert.IsNotNull(translateTargetTransform, $"Object {translateTargetPath} could not be found under Button instantiated from {prefabPath}.");
        }

        /// <summary>
        /// TODO: Troy - add comments
        /// </summary>
        public static IEnumerator CheckButtonTranslation(Vector3 targetStartPosition, Transform translateTarget, bool shouldTranslate = true)
        {
            bool wasTranslated = false;
            float pressEndTime = Time.time + ButtonPressAnimationDelay;
            while (Time.time < pressEndTime)
            {   // If the transform is moved at any point during this interval, we were successful
                yield return new WaitForFixedUpdate();
                wasTranslated |= targetStartPosition != translateTarget.localPosition;
            }

            Assert.AreEqual(shouldTranslate, wasTranslated, "Transform target object did or did not translate properly by action.");
        }

        /// <summary>
        /// TODO: Troy - add comments
        /// </summary>
        public static IEnumerator TestClickPushButton(Transform button, Vector3 targetStartPosition, Transform translateTargetObject, bool shouldClick = true)
        {
            yield return MoveHandToButton(button);

            yield return CheckButtonTranslation(targetStartPosition, translateTargetObject, shouldClick);

            yield return MoveHandAwayFromButton(button);

            yield return new WaitForSeconds(ButtonReleaseAnimationDelay);
        }

        /// <summary>
        /// TODO: Troy - add comments
        /// </summary>
        public static IEnumerator MoveHandToButton(Transform button)
        {
            // TODO: Troy - Fix this as not like before?
            Vector3 p1 = button.transform.position - ButtonTranslateOffset;
            Vector3 p2 = button.transform.position;

            // Move the hand towards
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHandFromTo(p1, p2, MoveHandNumSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
        }

        /// <summary>
        /// TODO: Troy - add comments
        /// </summary>
        public static IEnumerator MoveHandAwayFromButton(Transform button)
        {
            Vector3 p2 = button.transform.position;
            Vector3 p3 = button.transform.position - ButtonTranslateOffset;

            // Move the hand back
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            yield return PlayModeTestUtilities.MoveHandFromTo(p2, p3, MoveHandNumSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);
        }
    }
}

#endif