// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Editor;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using static Microsoft.MixedReality.Toolkit.Editor.MixedRealityToolboxWindow;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Editor
{
    /// <summary>
    /// Set of tests to validate MRTK Toolbox Editor window
    /// </summary>
    public class ToolboxWindowTests
    {
        /// <summary>
        /// Tests that the MixedRealityToolboxWindow can load without exception and that none of 
        /// it's internal item contents are null or invalid
        /// </summary>
        [UnityTest]
        public IEnumerator TestToolboxWindow()
        {
            MixedRealityToolboxWindow.ShowWindow();

            var window = MixedRealityToolboxWindow.GetWindow<MixedRealityToolboxWindow>();

            yield return WaitForWindowLoad();

            Assert.IsNotNull(window.toolBoxCollection);

            foreach (var category in window.ToolboxPrefabs)
            {
                Assert.IsFalse(string.IsNullOrEmpty(category.CategoryName));
                foreach (var item in category.Items)
                {
                    ValidateToolboxItem(item);
                }
            }

            MixedRealityToolboxWindow.HideWindow();
        }

        private static IEnumerator WaitForWindowLoad()
        {
            const int editorWindowWaitFrames = 10;
            for (int i = 0; i < editorWindowWaitFrames; i++)
            {
                yield return null;
            }
        }

        private static void ValidateToolboxItem(ToolboxItem item)
        {
            Assert.IsNotNull(item);
            Assert.IsFalse(string.IsNullOrEmpty(item.Name));
            Assert.IsFalse(string.IsNullOrEmpty(item.AssetPath), item.Name);
            Assert.IsFalse(string.IsNullOrEmpty(item.IconPath));
            Assert.IsNotNull(item.Prefab);
            Assert.IsNotNull(item.Icon);
        }
    }
}
