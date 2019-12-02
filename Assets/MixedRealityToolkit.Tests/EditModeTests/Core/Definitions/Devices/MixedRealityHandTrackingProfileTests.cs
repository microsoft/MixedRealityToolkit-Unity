// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;

namespace Microsoft.MixedReality.Toolkit.Input
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
            MixedRealityHandTrackingProfile profile = new MixedRealityHandTrackingProfile();
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
