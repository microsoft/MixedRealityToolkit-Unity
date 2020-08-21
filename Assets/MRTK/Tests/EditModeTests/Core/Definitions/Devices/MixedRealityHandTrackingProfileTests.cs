// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.InputSystem
{
    class MixedRealityHandTrackingProfileTests
    {
        /// <summary>
        /// Verifies that calling MixedRealityHandTrackingProfile.EnableHandJointVisualization and
        /// MixedRealityHandTrackingProfile.EnableHandMeshVisualization will only affect the visualization
        /// mode of the currently active application mode (i.e. editor or player).
        /// </summary>
        [Test]
        public void TestEditorOnlyChanges()
        {
            MixedRealityHandTrackingProfile profile = ScriptableObject.CreateInstance<MixedRealityHandTrackingProfile>();
            profile.HandJointVisualizationModes = SupportedApplicationModes.Editor | SupportedApplicationModes.Player;
            profile.HandMeshVisualizationModes = SupportedApplicationModes.Editor | SupportedApplicationModes.Player;

            // Since these tests run in the Unity editor, setting these to false should only clear the
            // SupportedApplicationModes.Editor from each of the Hand*VisualizationModes properties.
            profile.EnableHandJointVisualization = false;
            profile.EnableHandMeshVisualization = false;

            Assert.AreEqual(SupportedApplicationModes.Player, profile.HandJointVisualizationModes);
            Assert.AreEqual(SupportedApplicationModes.Player, profile.HandMeshVisualizationModes);
        }
    }
}
