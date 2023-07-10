// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Microsoft.MixedReality.Toolkit.Core.Tests.EditMode
{
    internal class MRTK3ProfileEditorTest
    {
        /// <summary>
        /// Verify that the <see cref="MRTK3ProfileEditor"/> window can be opened.
        /// </summary>
        [UnityTest]
        public IEnumerator InspectorWindowSmokeTest()
        {
            var window = SettingsService.OpenProjectSettings("Project/MRTK3");
            Assert.IsNotNull(window, "The MRTK Profile editor window should have been opened");

            var panel = window.rootVisualElement.Query<IMGUIContainer>(className: "settings-panel-imgui-container").First();
            Assert.IsNotNull(panel, "There should have hand a settings panel created for the MRTK Profile.");

            yield return null;
        }
    }
}
