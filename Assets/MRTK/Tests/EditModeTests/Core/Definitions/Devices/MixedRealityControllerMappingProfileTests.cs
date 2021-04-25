// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.InputSystem
{
    public class MixedRealityControllerMappingProfileTests
    {
        /// <summary>
        /// Verifies that the TestMixedRealityControllerMappingProfile was successfully updated.
        /// </summary>
        [Test]
        public void TestControllerMappingProfileUpdate()
        {
            MixedRealityControllerMapping[] testMappingsChanged = new MixedRealityControllerMapping[]
            {
#if UNITY_2020_1_OR_NEWER
                new MixedRealityControllerMapping(typeof(XRSDK.OpenXR.HPReverbG2Controller), Utilities.Handedness.Left),
                new MixedRealityControllerMapping(typeof(XRSDK.OpenXR.HPReverbG2Controller), Utilities.Handedness.Right)
#else
                new MixedRealityControllerMapping(typeof(OpenVR.Input.ViveWandController), Utilities.Handedness.Left),
                new MixedRealityControllerMapping(typeof(OpenVR.Input.ViveWandController), Utilities.Handedness.Right)
#endif
            };

            testMappingsChanged[0].SetDefaultInteractionMapping();
            testMappingsChanged[1].SetDefaultInteractionMapping();

            testMappingsChanged[0].Interactions[1] = new MixedRealityInteractionMapping(1, "Fake mapping",
                Utilities.AxisType.Digital, DeviceInputType.ButtonNearTouch);

            bool wereMappingsUpdated = false;

            foreach (MixedRealityControllerMapping mapping in testMappingsChanged)
            {
                if (mapping.UpdateInteractionSettingsFromDefault())
                {
                    wereMappingsUpdated = true;
                }
            }

            Assert.IsTrue(wereMappingsUpdated, "No mappings were updated. This test should always need an update.");
        }

        /// <summary>
        /// Verifies that the DefaultMixedRealityControllerMappingProfile didn't need updating.
        /// </summary>
        [Test]
        public void TestNoControllerMappingProfileUpdate()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:MixedRealityControllerMappingProfile DefaultMixedRealityControllerMappingProfile"))
            {
                MixedRealityControllerMappingProfile asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(MixedRealityControllerMappingProfile)) as MixedRealityControllerMappingProfile;

                bool wereMappingsUpdated = false;

                foreach (MixedRealityControllerMapping mapping in asset.MixedRealityControllerMappings)
                {
                    if (mapping.ControllerType.Type == null)
                    {
                        continue;
                    }

                    if (mapping.UpdateInteractionSettingsFromDefault())
                    {
                        wereMappingsUpdated = true;
                    }
                }

                Assert.IsFalse(wereMappingsUpdated, "DefaultMixedRealityControllerMappingProfile needed an update. This should never be checked in needing an update.");
            }
        }
    }
}
