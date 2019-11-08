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

namespace Microsoft.MixedReality.Toolkit.Tests.Core
{
    // Tests for the MixedRealityToolkitFiles utility class
    public class MixedRealityToolkitFilesTests
    {
        [SetUp]
        public void Setup()
        {
            RefreshFiles();
        }

        /// <summary>
        /// Validate that each module has a corresponding found folder (excluding None/AdHocTesting)
        /// </summary>
        [Test]
        public void TestGetDirectories()
        {
            foreach (var moduleType in GetTestModulesTypes())
            {
                var dirs = MixedRealityToolkitFiles.GetDirectories(moduleType);
                Assert.IsNotNull(dirs, $"Directory list was null for module type {moduleType.ToString()}");
                Assert.IsNotEmpty(dirs, $"Directory list was empty for module type {moduleType.ToString()}");
            }
        }

        [Test]
        public void TestMapModulePath()
        {
            foreach (var moduleType in GetTestModulesTypes())
            {
                Assert.IsNotNull(MixedRealityToolkitFiles.MapModulePath(moduleType), $"Module Path was null for module type {moduleType.ToString()}");
            }
        }

        /// <summary>
        /// Test the ModuleType.None and that no items are found
        /// </summary>
        [Test]
        public void TestNoneDirectory()
        {
            var dirs = MixedRealityToolkitFiles.GetDirectories(MixedRealityToolkitModuleType.None);
            Assert.IsNull(dirs, $"Directory list should be null for module type {MixedRealityToolkitModuleType.None.ToString()}");
        }

        /// <summary>
        /// Validate that a Non-MRTK folder is recognized still
        /// </summary>
        [Test]
        public void TestAdHocDirectory()
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
                    const int WAIT_TIMEOUT = 5000;// miliseconds
                    Assert.IsTrue(MixedRealityToolkitFiles.RefreshFoldersAsync().Wait(WAIT_TIMEOUT));

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
        [Test]
        public void TestRootAssetFolderResolution()
        {
            string resolvedPath = MixedRealityToolkitFiles.MapRelativeFilePathToAbsolutePath("Inspectors\\Data\\EditorWindowOptions.json");
            Assert.IsNotNull(resolvedPath);
        }

        [TearDown]
        public void CleanupTests()
        {
            TestUtilities.EditorTearDownScenes();
        }

        #region Test Helpers

        private static void RefreshFiles()
        {
            MixedRealityToolkitFiles.RefreshFolders();

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
