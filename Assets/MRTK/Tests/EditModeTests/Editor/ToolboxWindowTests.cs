// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.EditModeTests.Editor
{
    /// <summary>
    /// Set of tests to validate MRTK Toolbox Editor window.
    /// </summary>
    public class ToolboxWindowTests
    {
        /// <summary>
        /// Tests that the MixedRealityToolboxWindow can load without exception and that none of
        /// its internal item contents are null or invalid.
        /// </summary>
        [UnityTest]
        public IEnumerator TestToolboxWindow()
        {
            if (!UnityEngine.Application.isBatchMode)
            {
                MixedRealityToolboxWindow.ShowWindow();

                var window = EditorWindow.GetWindow<MixedRealityToolboxWindow>();

                yield return WaitForWindowLoad();

                Assert.IsNotNull(window.toolBoxCollection);

                foreach (var category in window.ToolboxPrefabs)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(category.CategoryName));
                    foreach (MixedRealityToolboxWindow.ToolboxItem item in category.Items)
                    {
                        ValidateToolboxItem(item);
                    }
                }

                MixedRealityToolboxWindow.HideWindow();
            }
        }

        private static IEnumerator WaitForWindowLoad()
        {
            const int editorWindowWaitFrames = 10;
            for (int i = 0; i < editorWindowWaitFrames; i++)
            {
                yield return null;
            }
        }

        private static void ValidateToolboxItem(MixedRealityToolboxWindow.ToolboxItem item)
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
