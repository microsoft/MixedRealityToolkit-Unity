#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Utility class that provides helpful methods for generating and assembling buttons to be used in tests
    /// Also provides functions to test/simulate pressing buttons etc
    /// </summary>
    public static class TestButtonUtilities
    {
        public enum DefaultButtonType
        {
            DefaultPushButton,
            DefaultHL2Button,
            DefaultHL2ToggleButton
        };

        /// <summary>
        /// Asset path to default model push button prefab asset
        /// </summary>
        public static readonly string DefaultInteractablePrefabAssetPath = AssetDatabase.GUIDToAssetPath(DefaultInteractablePrefabAssetGuid);

        /// <summary>
        /// Asset path to default HoloLens 2 Pressable Button prefab asset
        /// </summary>
        public static readonly string PressableHoloLens2PrefabPath = AssetDatabase.GUIDToAssetPath(PressableHoloLens2PrefabGuid);

        /// <summary>
        /// Asset path to default HoloLens 2 Toggle Pressable Button prefab asset
        /// </summary>
        public static readonly string PressableHoloLens2TogglePrefabPath = AssetDatabase.GUIDToAssetPath(PressableHoloLens2TogglePrefabGuid);

        /// <summary>
        /// Amount of time for press action on button
        /// </summary>
        public const float ButtonPressAnimationDelay = 0.25f;

        /// <summary>
        /// Amount of time for release action on button
        /// </summary>
        public const float ButtonReleaseAnimationDelay = 0.25f;

        /// <summary>
        /// Default position of new interactable buttons instantiated
        /// </summary>
        public static readonly Vector3 DefaultPosition = new Vector3(0.0f, 0.0f, 0.5f);

        /// <summary>
        /// Default rotation applied to new interactable button instantiated
        /// </summary>
        public static readonly Quaternion DefaultRotation = Quaternion.LookRotation(Vector3.up);

        private const int MoveHandNumSteps = 32;
        private static readonly Vector3 ButtonTranslateOffset = new Vector3(0.05f, 0f, 0.51f);

        private static readonly string[] AssetPaths = { DefaultInteractablePrefabAssetPath, PressableHoloLens2PrefabPath, PressableHoloLens2TogglePrefabPath };
        private static readonly string[] TranslateTargetPaths = { "Cylinder", "CompressableButtonVisuals/FrontPlate", "CompressableButtonVisuals/FrontPlate" };
        private static readonly Quaternion[] DefaultRotations = { DefaultRotation, Quaternion.LookRotation(Vector3.forward), Quaternion.LookRotation(Vector3.forward) };

        // Examples/Demos/UX/Interactables/Prefabs/Model_PushButton.prefab
        private const string DefaultInteractablePrefabAssetGuid = "29a6f5316e0868e47adff5eee8945193";

        // SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2.prefab
        private const string PressableHoloLens2PrefabGuid = "3f1f46cbecbe08e46a303ccfdb5b498a";

        // SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2Toggle.prefab
        private const string PressableHoloLens2TogglePrefabGuid = "64790b91b91094d49942373c4e83c237";

        /// <summary>
        /// Instantiate and configure one of the <see cref="DefaultButtonType"/> types.
        /// Returns reference to <see cref="Microsoft.MixedReality.Toolkit.UI.Interactable"/> component and Transform to movable object that transforms on press 
        /// </summary>
        /// <remarks>
        /// The button will be instantiated relative to the current MRTKPlayspace. This means its world transform
        /// will be the position and rotation baked into the prefab, applied relative to the MRTKPlayspace. 
        /// See <see cref="InstantiateInteractableFromPath(Vector3, Quaternion, string)"/>
        /// </remarks>
        public static void InstantiateDefaultButton(DefaultButtonType buttonType, out Interactable interactable, out Transform translateTargetObject)
        {
            InstantiatePressableButtonPrefab(
                DefaultPosition,
                DefaultRotations[(int)buttonType],
                AssetPaths[(int)buttonType],
                TranslateTargetPaths[(int)buttonType],
                out interactable,
                out translateTargetObject);
        }

        /// <summary>
        /// Instantiates <see cref="Microsoft.MixedReality.Toolkit.UI.Interactable"/> prefab from provided asset database path at given position and rotation
        /// </summary>
        /// <remarks>
        /// The button will be instantiated relative to the current MRTKPlayspace. This means its world transform
        /// will be the position and rotation baked into the prefab, applied relative to the MRTKPlayspace. 
        /// </remarks>
        public static GameObject InstantiateInteractableFromPath(Vector3 position, Quaternion rotation, string path)
        {
            // Load interactable prefab
            Object interactablePrefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            GameObject result = Object.Instantiate(interactablePrefab) as GameObject;
            Assert.IsNotNull(result);

            // Move the object into position
            Pose worldFromLocal = TestUtilities.PlaceRelativeToPlayspace(position, rotation);

            result.transform.position = worldFromLocal.position;
            result.transform.rotation = worldFromLocal.rotation;
            return result;
        }

        /// <summary>
        /// Instantiates Pressable Button based on list of arguments provided
        /// </summary>
        /// <remarks>
        /// The button will be instantiated relative to the current MRTKPlayspace. This means its world transform
        /// will be the position and rotation baked into the prefab, applied relative to the MRTKPlayspace. 
        /// See <see cref="InstantiateInteractableFromPath(Vector3, Quaternion, string)"/>
        /// </remarks>
        public static void InstantiatePressableButtonPrefab(Vector3 position, Quaternion rotation,
            string prefabPath, string translateTargetPath,
            out Interactable interactable, out Transform translateTargetTransform)
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
        /// Confirms that provided Transform translates as expected due to a press currently happening
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
        /// Execute and test end-to-end press of a button with right hand poke pointer
        /// </summary>
        /// <param name="button">Position of button to push hand to/through</param>
        /// <param name="targetStartPosition">The start position of the button component that will translate on press</param>
        /// <param name="translateTargetObject">Transform of the button component that will translate on press</param>
        /// <param name="shouldClick">Should we expect a click (i.e translation) from the button. The button may be disabled</param>
        public static IEnumerator TestClickPushButton(Transform button, Vector3 targetStartPosition, Transform translateTargetObject, bool shouldClick = true)
        {
            yield return MoveHandToButton(button);

            yield return CheckButtonTranslation(targetStartPosition, translateTargetObject, shouldClick);

            yield return MoveHandAwayFromButton(button);

            yield return new WaitForSeconds(ButtonReleaseAnimationDelay);
        }

        /// <summary>
        /// Move the right hand from in front of the button through to the button
        /// </summary>
        public static IEnumerator MoveHandToButton(Transform button)
        {
            Vector3 p1 = button.transform.position - button.TransformDirection(ButtonTranslateOffset);
            Vector3 p2 = button.transform.position;

            yield return MoveHand(p1, p2);
        }

        /// <summary>
        /// Move the right from in the button to just in front of the button
        /// </summary>
        public static IEnumerator MoveHandAwayFromButton(Transform button)
        {
            Vector3 p2 = button.transform.position;
            Vector3 p3 = button.transform.position - button.TransformDirection(ButtonTranslateOffset);

            yield return MoveHand(p2, p3);
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, PlayModeTestUtilities.GetInputSimulationService());
        }

        /// <summary>
        /// Move the right hand from point 1 to point 2
        /// </summary>
        public static IEnumerator MoveHand(Vector3 p1, Vector3 p2)
        {
            // Move the hand towards
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHand(p1, p2, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
        }

    }
}

#endif