// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Core
{
    // Tests for the MixedRealityToolkitFiles utility class
    public class MixedRealityToolkitFilesTests
    {
        /// <summary>
        /// Validate that each module has a corresponding found folder (excluding None/AdHocTesting)
        /// </summary>
        [UnityTest]
        public IEnumerator TestGetDirectories()
        {
            yield return RefreshFiles();

            foreach (var moduleType in GetTestModulesTypes())
            {
                var dirs = MixedRealityToolkitFiles.GetDirectories(moduleType);
                Assert.IsNotNull(dirs, $"Directory list was null for module type {moduleType.ToString()}");
                Assert.IsNotEmpty(dirs, $"Directory list was empty for module type {moduleType.ToString()}");
            }
        }

        /// <summary>
        /// Test that the MapModulePath API works for each Module Type
        /// </summary>
        [UnityTest]
        public IEnumerator TestMapModulePath()
        {
            yield return RefreshFiles();

            foreach (var moduleType in GetTestModulesTypes())
            {
                Assert.IsNotNull(MixedRealityToolkitFiles.MapModulePath(moduleType), $"Module Path was null for module type {moduleType.ToString()}");
            }
        }

        /// <summary>
        /// Test the ModuleType.None and that no items are found
        /// </summary>
        [UnityTest]
        public IEnumerator TestNoneDirectory()
        {
            yield return RefreshFiles();

            var dirs = MixedRealityToolkitFiles.GetDirectories(MixedRealityToolkitModuleType.None);
            Assert.IsNull(dirs, $"Directory list should be null for module type {MixedRealityToolkitModuleType.None.ToString()}");
        }

        /// <summary>
        /// Validate that a Non-MRTK folder is recognized still
        /// </summary>
        [UnityTest]
        public IEnumerator TestAdHocDirectory()
        {
            string adhocTesting = MixedRealityToolkitModuleType.AdhocTesting.ToString();
            string adHocFolderPath = Path.Combine(UnityEngine.Application.dataPath, adhocTesting);
            string adHocFolderMetaPath = Path.Combine(UnityEngine.Application.dataPath, adhocTesting + ".meta");
            string adHocSentinelFilePath = Path.Combine(adHocFolderPath, "MRTK." + adhocTesting + ".sentinel");

            try
            {
                Directory.CreateDirectory(adHocFolderPath);
                using (var file = File.Create(adHocSentinelFilePath))
                {
                    yield return RefreshFiles();

                    var moduleType = MixedRealityToolkitModuleType.AdhocTesting;
                    var dirs = MixedRealityToolkitFiles.GetDirectories(moduleType);
                    Assert.IsNotNull(dirs, $"Directory list was null for module type {moduleType.ToString()}");
                    Assert.IsNotEmpty(dirs, $"Directory list was empty for module type {moduleType.ToString()}");
                }
            }
            finally
            {
                // Clean up
                Directory.Delete(adHocFolderPath, true);
                File.Delete(adHocFolderMetaPath);
            }
        }

        /// <summary>
        /// Validates that MixedRealityToolkitFiles is able to reason over MRTK folders when placed in the root Asset directory.
        /// </summary>
        [UnityTest]
        public IEnumerator TestRootAssetFolderResolution()
        {
            yield return RefreshFiles();

            string resolvedPath = MixedRealityToolkitFiles.MapRelativeFilePathToAbsolutePath("Inspectors\\Data\\EditorWindowOptions.json");
            Assert.IsNotNull(resolvedPath);
        }

        /// <summary>
        /// Validates that FileUtilities.FindFilesInAssets can find this test script in the asset database.
        /// </summary>
        [Test]
        public void TestFileUtilitiesFindFilesInAssets()
        {
            FileInfo[] files = FileUtilities.FindFilesInAssets("MixedRealityToolkitFilesTests.cs");
            Assert.IsTrue(files.Length == 1);
        }

        [TearDown]
        public void CleanupTests()
        {
            TestUtilities.EditorTearDownScenes();
        }

        #region Test Helpers

        private static IEnumerator RefreshFiles()
        {
            MixedRealityToolkitFiles.RefreshFolders();
            var task = MixedRealityToolkitFiles.WaitForFolderRefresh();
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Assert.IsTrue(MixedRealityToolkitFiles.AreFoldersAvailable);
        }

        private static IEnumerable<MixedRealityToolkitModuleType> GetTestModulesTypes()
        {
            var excludeTypes = new[] { MixedRealityToolkitModuleType.None, MixedRealityToolkitModuleType.AdhocTesting };

            return Enum.GetValues(typeof(MixedRealityToolkitModuleType))
                .Cast<MixedRealityToolkitModuleType>()
                .Where(t => !excludeTypes.Contains(t));
        }

        #endregion
    }
}
