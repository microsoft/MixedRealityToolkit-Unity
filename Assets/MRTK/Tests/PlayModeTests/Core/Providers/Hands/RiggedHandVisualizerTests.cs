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
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class RiggedHandVisualizerTests : BasePlayModeTests
    {
        private const string RiggedHandProfileName = "TestRiggedHandVisualizationConfigurationProfile";

        public override IEnumerator Setup()
        {
            var riggedHandProfile = ScriptableObjectExtensions.GetAllInstances<MixedRealityToolkitConfigurationProfile>()
                           .FirstOrDefault(x => x.name.Equals(RiggedHandProfileName));
            PlayModeTestUtilities.Setup(riggedHandProfile);

            yield return null;
        }

#if UNITY_2019_3_OR_NEWER
        /// <summary>
        /// Run a basic pinch test on the rigged hand to make sure nothing crashes and that the mesh updates appropriately
        /// </summary>
        [UnityTest]
        public IEnumerator TestRiggedHand()
        {
            // Initialize hand
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.zero);

            RiggedHandVisualizer handVisualizer = GameObject.FindObjectOfType<RiggedHandVisualizer>().GetComponent<RiggedHandVisualizer>();

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            Assert.IsTrue(handVisualizer.HandRenderer.sharedMaterial.GetFloat(handVisualizer.PinchStrengthMaterialProperty) < 0.5f);

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Grab);
            Assert.IsTrue(handVisualizer.HandRenderer.sharedMaterial.GetFloat(handVisualizer.PinchStrengthMaterialProperty) > 0.5f);

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            Assert.IsTrue(handVisualizer.HandRenderer.sharedMaterial.GetFloat(handVisualizer.PinchStrengthMaterialProperty) < 0.5f);
        }
#endif
    }
}

#endif