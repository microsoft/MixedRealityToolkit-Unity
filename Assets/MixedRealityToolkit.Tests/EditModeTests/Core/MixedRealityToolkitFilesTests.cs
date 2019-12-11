// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using NUnit.Framework;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Tests.Core
{
    // Tests for the MixedRealityToolkitFiles utility class
    public class MixedRealityToolkitFilesTests
    {
        [Test]
        public void TestGetDirectories()
        {
            MixedRealityToolkitModuleType[] moduleTypes = new MixedRealityToolkitModuleType[]
            {
                MixedRealityToolkitModuleType.Core,
                MixedRealityToolkitModuleType.Providers,
                MixedRealityToolkitModuleType.Services,
                MixedRealityToolkitModuleType.SDK,
                MixedRealityToolkitModuleType.Examples,
                MixedRealityToolkitModuleType.Tests,
                MixedRealityToolkitModuleType.Extensions,
                MixedRealityToolkitModuleType.Tools,
            };

            MixedRealityToolkitFiles.RefreshFolders();
            foreach (var moduleType in moduleTypes)
            {
                // Validate that each module has a corresponding found folder
                Assert.IsTrue(MixedRealityToolkitFiles.GetDirectories(moduleType).Any());
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
    }
}
