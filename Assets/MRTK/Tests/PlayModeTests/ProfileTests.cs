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

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ProfileTests
    {
        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        private const string HoloLens1ProfileName = "DefaultHoloLens1ConfigurationProfile";

        /// <summary>
        /// Test that the default HoloLens 2 profile acts as expected (when hands are up, we see a hand ray).
        /// </summary>
        [UnityTest]
        public IEnumerator TestDefaultProfile()
        {
            PlayModeTestUtilities.Setup();

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward);


            var allPointers = GetAllPointers();
            // https://nunit.org/docs/2.5.5/collectionConstraints.html
            Assert.That(allPointers, Has.No.InstanceOf(typeof(GGVPointer)));
            Assert.That(allPointers, Has.Some.InstanceOf(typeof(ShellHandRayPointer)));
        }

        /// <summary>
        /// Test that the Hands Free Input simulation is enabled in editor in the default profile when user input is enabled
        /// </summary>
        [UnityTest]
        public IEnumerator TestEditorProfile()
        {
            PlayModeTestUtilities.Setup();

            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            inputSimulationService.UserInputEnabled = true;

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var allPointers = GetAllPointers();
            // https://nunit.org/docs/2.5.5/collectionConstraints.html
            Assert.That(allPointers, Has.Some.InstanceOf(typeof(GGVPointer)));
            Assert.That(allPointers, Has.No.InstanceOf(typeof(ShellHandRayPointer)));
        }


        /// <summary>
        /// Test that HoloLens 1 profile acts as expected (e.g. when hands are up there are no hand rays)
        /// </summary>
        [UnityTest]
        public IEnumerator TestHL1Profile()
        {
            var hl1Profile = ScriptableObjectExtensions.GetAllInstances<MixedRealityToolkitConfigurationProfile>()
                .FirstOrDefault(x => x.name.Equals(HoloLens1ProfileName));
            TestUtilities.InitializeMixedRealityToolkit(hl1Profile);

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward);

            var allPointers = GetAllPointers();
            // https://nunit.org/docs/2.5.5/collectionConstraints.html
            Assert.That(allPointers, Has.Some.InstanceOf(typeof(GGVPointer)));
            Assert.That(allPointers, Has.No.InstanceOf(typeof(ShellHandRayPointer)));
        }

        private List<IMixedRealityPointer> GetAllPointers()
        {
            var allPointers = new List<IMixedRealityPointer>();

            HashSet<IMixedRealityInputSource> inputSources = CoreServices.InputSystem?.DetectedInputSources;

            if (inputSources != null)
            {
                foreach (var inputSource in inputSources)
                {
                    allPointers.AddRange(inputSource.Pointers);
                }
            }

            return allPointers;
        }

        /// <summary>
        /// Tests that the camera system initializes with the floor height specified in the profile.
        /// </summary>
        /// <returns>enumerator for Unity</returns>
        [UnityTest]
        public IEnumerator TestProfileContentOffset()
        {
            float contentOffset = 3.0f;

            var hl1Profile = ScriptableObjectExtensions.GetAllInstances<MixedRealityToolkitConfigurationProfile>()
                .FirstOrDefault(x => x.name.Equals(HoloLens1ProfileName));

            // keep the old floor height and experience scale to reset it later
            ExperienceScale originalExperienceScale = hl1Profile.ExperienceSettingsProfile.TargetExperienceScale;
            float oldContentOffset = hl1Profile.ExperienceSettingsProfile.ContentOffset;

            hl1Profile.ExperienceSettingsProfile.TargetExperienceScale = ExperienceScale.Room;
            hl1Profile.ExperienceSettingsProfile.ContentOffset = contentOffset;
            TestUtilities.InitializeMixedRealityToolkit(hl1Profile);

            TestUtilities.InitializeCamera();
            yield return new WaitForSeconds(0.5f);

            MixedRealitySceneContent sceneContent = GameObject.Find("MixedRealitySceneContent").GetComponent<MixedRealitySceneContent>();

            TestUtilities.AssertAboutEqual(TestUtilities.PositionRelativeToPlayspace(Vector3.zero), Vector3.zero, "The playspace was not set to the origin");
            TestUtilities.AssertAboutEqual(sceneContent.transform.position, Vector3.up * contentOffset, "The floor height was not set correctly");

            // be sure to set the profile's ContentOffset back to it's original value afterwards
            hl1Profile.ExperienceSettingsProfile.TargetExperienceScale = originalExperienceScale;
            hl1Profile.ExperienceSettingsProfile.ContentOffset = oldContentOffset;
            TestUtilities.InitializeMixedRealityToolkit(hl1Profile);
        }
    }
}

#endif
