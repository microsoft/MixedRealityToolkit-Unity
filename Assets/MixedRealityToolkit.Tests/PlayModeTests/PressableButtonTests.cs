// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class PressableButtonTests
    {
        #region Utilities
        private GameObject InstantiateSceneAndDefaultPressableButton()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            RenderSettings.skybox = null;

            Object pressableButtonPrefab = AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2.prefab", typeof(Object));
            GameObject testButton = Object.Instantiate(pressableButtonPrefab) as GameObject;

            return testButton;
        }

        #endregion

        #region Tests

        [UnityTest]
        public IEnumerator ButtonInstantiate()
        {
            GameObject testButton = InstantiateSceneAndDefaultPressableButton();
            yield return null;
            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }


        [UnityTest]
        public IEnumerator ScaleWorldDistances()
        {
            // instantiate scene and button
            GameObject testButton = InstantiateSceneAndDefaultPressableButton();
            yield return null;
            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            testButton.transform.Translate(new Vector3(10.0f, 5.0f, 20.0f));

            PressableButton.SpaceMode distanceMode = buttonComponent.DistanceSpaceMode;
            // check default value -> default must be using world space to not introduce a breaking change to the button
            Assert.IsTrue(distanceMode == PressableButton.SpaceMode.World, "Pressable button default value is using local space distances which introduces a breaking change for existing projects");

            // make sure there's no scale on our button
            testButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            // set start distance -> default is zero
            buttonComponent.StartPushDistance = 0.00003f;

            // get the buttons default values for the push planes
            float startPushDistance = buttonComponent.StartPushDistance;
            float maxPushDistance = buttonComponent.MaxPushDistance;
            float pressDistance = buttonComponent.PressDistance;
            float releaseDistance = pressDistance - buttonComponent.ReleaseDistanceDelta;

            Vector3 startPushDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(startPushDistance);
            Vector3 maxPushDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(maxPushDistance);
            Vector3 pressDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(pressDistance);
            Vector3 releaseDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(releaseDistance);

            // scale the button in z direction
            // scaling the button while in world space shouldn't influence our button plane distances
            testButton.transform.localScale = new Vector3(1.0f, 1.0f, 2.0f);

            Vector3 startPushDistanceWorldScaled = buttonComponent.GetWorldPositionAlongPushDirection(startPushDistance);
            Vector3 maxPushDistanceWorldScaled = buttonComponent.GetWorldPositionAlongPushDirection(maxPushDistance);
            Vector3 pressDistanceWorldScaled = buttonComponent.GetWorldPositionAlongPushDirection(pressDistance);
            Vector3 releaseDistanceWorldScaled = buttonComponent.GetWorldPositionAlongPushDirection(releaseDistance);

            // compare our distances
            Assert.IsTrue(startPushDistanceWorld == startPushDistanceWorldScaled, "Start Distance was modified while scaling button gameobject");
            Assert.IsTrue(maxPushDistanceWorld == maxPushDistanceWorldScaled, "Max Push Distance was modified while scaling button gameobject");
            Assert.IsTrue(pressDistanceWorld == pressDistanceWorldScaled, "Press Distance was modified while scaling button gameobject");
            Assert.IsTrue(releaseDistanceWorld == releaseDistanceWorldScaled, "Release Distance was modified while scaling button gameobject");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator SwitchWorldToLocalDistanceMode()
        {
            // instantiate scene and button
            GameObject testButton = InstantiateSceneAndDefaultPressableButton();
            yield return null;
            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            PressableButton.SpaceMode distanceMode = buttonComponent.DistanceSpaceMode;
            // check default value -> default must be using world space to not introduce a breaking change to the button
            Assert.IsTrue(distanceMode == PressableButton.SpaceMode.World, "Pressable button default value is using local space distances which introduces a breaking change for existing projects");

            // add scale to our button so we can compare world to local distances
            testButton.transform.localScale = new Vector3(1.0f, 1.0f, 2.0f);

            // set start distance -> default is zero
            buttonComponent.StartPushDistance = 0.00003f;

            // get the buttons default values for the push planes
            float startPushDistance = buttonComponent.StartPushDistance;
            float maxPushDistance = buttonComponent.MaxPushDistance;
            float pressDistance = buttonComponent.PressDistance;
            float releaseDistance = pressDistance - buttonComponent.ReleaseDistanceDelta;

            Vector3 startPushDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(startPushDistance);
            Vector3 maxPushDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(maxPushDistance);
            Vector3 pressDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(pressDistance);
            Vector3 releaseDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(releaseDistance);

            // switch to local space
            buttonComponent.DistanceSpaceMode = PressableButton.SpaceMode.Local;

            float startPushDistanceLocal = buttonComponent.StartPushDistance;
            float maxPushDistanceLocal = buttonComponent.MaxPushDistance;
            float pressDistanceLocal = buttonComponent.PressDistance;
            float releaseDistanceLocal = pressDistanceLocal - buttonComponent.ReleaseDistanceDelta;

            // check if distances have changed
            Assert.IsFalse(startPushDistance == startPushDistanceLocal, "Switching from world to local space distances didn't adjust the plane coords");
            Assert.IsFalse(maxPushDistance == maxPushDistanceLocal, "Switching from world to local space distances didn't adjust the plane coords");
            Assert.IsFalse(pressDistance == pressDistanceLocal, "Switching from world to local space distances didn't adjust the plane coords");
            Assert.IsFalse(releaseDistance == releaseDistanceLocal, "Switching from world to local space distances didn't adjust the plane coords");

            // get world space positions for local distances
            Vector3 startPushDistanceWorldLocal = buttonComponent.GetWorldPositionAlongPushDirection(startPushDistanceLocal);
            Vector3 maxPushDistanceWorldLocal = buttonComponent.GetWorldPositionAlongPushDirection(maxPushDistanceLocal);
            Vector3 pressDistanceWorldLocal = buttonComponent.GetWorldPositionAlongPushDirection(pressDistanceLocal);
            Vector3 releaseDistanceWorldLocal = buttonComponent.GetWorldPositionAlongPushDirection(releaseDistanceLocal);

            // compare world space distances -> local and world space mode should return us the same world space positions 
            Assert.IsTrue(startPushDistanceWorld == startPushDistanceWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");
            Assert.IsTrue(maxPushDistanceWorld == maxPushDistanceWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");
            Assert.IsTrue(pressDistanceWorld == pressDistanceWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");
            Assert.IsTrue(releaseDistanceWorld == releaseDistanceWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");

            // switch back to world space
            buttonComponent.DistanceSpaceMode = PressableButton.SpaceMode.World;

            // distances must match up with original values 
            Assert.IsTrue(startPushDistance == buttonComponent.StartPushDistance, "Conversion from local to world distances didn't return the correct world distances");
            Assert.IsTrue(maxPushDistance == buttonComponent.MaxPushDistance, "Conversion from local to world distances didn't return the correct world distances");
            Assert.IsTrue(pressDistance == buttonComponent.PressDistance, "Conversion from local to world distances didn't return the correct world distances");
            float newReleaseDistance = buttonComponent.PressDistance - buttonComponent.ReleaseDistanceDelta;
            Assert.IsTrue(releaseDistance == newReleaseDistance, "Conversion from local to world distances didn't return the correct world distances");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator ScaleLocalDistances()
        {
            // instantiate scene and button
            GameObject testButton = InstantiateSceneAndDefaultPressableButton();
            yield return null;
            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            testButton.transform.Translate(new Vector3(10.0f, 5.0f, 20.0f));
            PressableButton.SpaceMode distanceMode = buttonComponent.DistanceSpaceMode;
            // check default value -> default must be using world space to not introduce a breaking change to the button
            Assert.IsTrue(distanceMode == PressableButton.SpaceMode.World, "Pressable button default value is using local space distances which introduces a breaking change for existing projects");

            // make sure there's no scale on our button
            testButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            // change into local space distances
            buttonComponent.DistanceSpaceMode = PressableButton.SpaceMode.Local;

            // set start distance -> default is zero
            buttonComponent.StartPushDistance = 0.00003f;

            // get the buttons default values for the push planes
            float startPushDistance = buttonComponent.StartPushDistance;
            float maxPushDistance = buttonComponent.MaxPushDistance;
            float pressDistance = buttonComponent.PressDistance;
            float releaseDistance = pressDistance - buttonComponent.ReleaseDistanceDelta;

            Vector3 startPushDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(startPushDistance);
            Vector3 maxPushDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(maxPushDistance);
            Vector3 pressDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(pressDistance);
            Vector3 releaseDistanceWorld = buttonComponent.GetWorldPositionAlongPushDirection(releaseDistance);

            // scale the button in z direction
            // scaling the button while in local space will alter the world space distances
            Vector3 zScale = new Vector3(1.0f, 1.0f, 2.0f);
            testButton.transform.localScale = zScale;

            Vector3 startPushDistanceWorldScaled = buttonComponent.GetWorldPositionAlongPushDirection(startPushDistance);
            Vector3 maxPushDistanceWorldScaled = buttonComponent.GetWorldPositionAlongPushDirection(maxPushDistance);
            Vector3 pressDistanceWorldScaled = buttonComponent.GetWorldPositionAlongPushDirection(pressDistance);
            Vector3 releaseDistanceWorldScaled = buttonComponent.GetWorldPositionAlongPushDirection(releaseDistance);

            // check if the local plane distances have scaled with the gameobject transform scale
            Vector3 initialPosition = buttonComponent.GetWorldPositionAlongPushDirection(0);
            Assert.IsTrue(((startPushDistanceWorld - initialPosition).Mul(zScale)) == ((startPushDistanceWorldScaled - initialPosition)), "Plane distance didn't scale with object while in local mode"); ;
            Assert.IsTrue(((maxPushDistanceWorld - initialPosition).Mul(zScale)) == ((maxPushDistanceWorldScaled - initialPosition)), "Plane distance didn't scale with object while in local mode"); ;
            Assert.IsTrue(((pressDistanceWorld - initialPosition).Mul(zScale)) == ((pressDistanceWorldScaled - initialPosition)), "Plane distance didn't scale with object while in local mode"); ;
            Assert.IsTrue(((releaseDistanceWorld - initialPosition).Mul(zScale)) == ((releaseDistanceWorldScaled - initialPosition)), "Plane distance didn't scale with object while in local mode"); ;

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }


        #endregion
    }
}


#endif