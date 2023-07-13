// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for tests. While nice to have, this documentation is not required.
#pragma warning disable 1591

using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Microsoft.MixedReality.Toolkit.Core.Tests.EditMode
{
    internal class MRTK3ProfileEditorTest
    {
        /// <summary>
        /// Verify that the <see cref="MRTK3ProfileEditor"/> window can be opened.
        /// </summary>
        /// <remarks>
        /// This test is disabled when executing in batch mode, since it requires a Unity graphic device.
        /// </remarks>
        [UnityTest]
        public IEnumerator InspectorWindowSmokeTest()
        {
            if (Application.isBatchMode)
            {
                yield return null;
            }
            else
            {
                var window = SettingsService.OpenProjectSettings("Project/MRTK3");
                Assert.IsNotNull(window, "The MRTK Profile editor window should have been opened");

                var panel = window.rootVisualElement.Query<IMGUIContainer>(className: "settings-panel-imgui-container").First();
                Assert.IsNotNull(panel, "There should have hand a settings panel created for the MRTK Profile.");

                yield return null;
            }
        }
    }
}
#pragma warning restore 1591