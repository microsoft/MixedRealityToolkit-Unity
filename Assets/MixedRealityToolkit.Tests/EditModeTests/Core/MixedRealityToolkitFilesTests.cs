// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Tests.Core
{
    // Tests for the MixedRealityToolkitFiles utility class
    public class MixedRealityToolkitFilesTests
    {
        string[] basePaths = new string[] { "", "C:\\", "C:\\xyz\\", "C:/xyz/" };
        bool adhocTestDirectoryCreated = false;

        [Test]
        public void FindMatchingModule()
        {
            TestInvalidPath("");

            // Test invalid base name
            TestInvalidPath("aaa");
            TestInvalidPath(".SDK");
            TestInvalidPath("aaa.SDK");

            // Test missing chars
            TestInvalidPath("MixedRealityToolki.SDK");
            TestInvalidPath("MixedRealityToolkit.SD");
            TestInvalidPath("ixedRealityToolkit.SDK");
            TestInvalidPath("MixedRealityToolkit.DK");

            // Test missing dots
            TestInvalidPath("SDK");
            TestInvalidPath("MixedRealityToolkitSDK");

            // Test valid paths
            TestValidPath("MixedRealityToolkit", MixedRealityToolkitModuleType.Core);
            TestValidPath("MixedRealityToolkit.SDK", MixedRealityToolkitModuleType.SDK);

            // Test that all modules can be found and internal module map is complete.
            // Check that MixedRealityToolkitFiles.moduleNameMap has all entries if this fails!
            var modules = Enum.GetValues(typeof(MixedRealityToolkitModuleType));
            var moduleNames = Enum.GetNames(typeof(MixedRealityToolkitModuleType));
            for (int i = 0; i < modules.Length; ++i)
            {
                var module = (MixedRealityToolkitModuleType)modules.GetValue(i);
                if (module != MixedRealityToolkitModuleType.Core)
                {
                    TestValidPath($"MixedRealityToolkit.{moduleNames[i]}", module);
                }
            }
        }

        [Test]
        public void FindMatchingModuleNuget()
        {
            // Test invalid base name
            TestInvalidPath("aaa.1.23-45/MRTK");
            TestInvalidPath(".1.23-45.SDK/MRTK");
            TestInvalidPath("aaa.1.23-45.SDK/MRTK");

            // Test missing chars
            TestInvalidPath("Microsoft.MixedReality.Toolki.SDK.1.23-45/MRTK");
            TestInvalidPath("Microsoft.MixedReality.Toolkit.SD.1.23-45/MRTK");
            TestInvalidPath("Microsoft.MixedReality.Toolkit.SDK.1.23-45/MRT");

            TestInvalidPath("icrosoft.MixedReality.Toolkit.SDK.1.23-45/MRTK");
            TestInvalidPath("Microsoft.MixedReality.Toolkit.DK.1.23-45/MRTK");
            TestInvalidPath("Microsoft.MixedReality.Toolkit.SDK.1.23-45/RTK");

            // Test missing dots
            TestInvalidPath("Microsoft.MixedReality.Toolkit.SDK1.23-45/MRTK");
            TestInvalidPath("Microsoft.MixedReality.ToolkitSDK.1.23-45/MRTK");
            TestInvalidPath("Microsoft.MixedReality.ToolkitSDK1.23-45/MRTK");

            // Test missing version
            TestInvalidPath("Microsoft.MixedReality.Toolkit.SDK/MRTK");
            // Test missing MRTK suffix
            TestInvalidPath("Microsoft.MixedReality.Toolkit.SDK.1.23-45");

            // Test valid paths
            TestValidPath("Microsoft.MixedReality.Toolkit.1.23-45/MRTK", MixedRealityToolkitModuleType.Core);
            TestValidPath("Microsoft.MixedReality.Toolkit.SDK.1.23-45/MRTK", MixedRealityToolkitModuleType.SDK);

            // Test that all modules can be found and internal module map is complete.
            // Check that MixedRealityToolkitFiles.moduleNameMap has all entries if this fails!
            var modules = Enum.GetValues(typeof(MixedRealityToolkitModuleType));
            var moduleNames = Enum.GetNames(typeof(MixedRealityToolkitModuleType));
            for (int i = 0; i < modules.Length; ++i)
            {
                var module = (MixedRealityToolkitModuleType)modules.GetValue(i);
                if (module != MixedRealityToolkitModuleType.Core)
                {
                    TestValidPath($"Microsoft.MixedReality.Toolkit.{moduleNames[i]}.1.23-45/MRTK", (MixedRealityToolkitModuleType)modules.GetValue(i));
                }
            }
        }

        /// <summary>
        /// This test validates that the MixedRealityToolkitFiles class is able to reason over MRTK folders even
        /// when they aren't in the root Asset directory.
        /// </summary>
        /// <remarks>
        /// Note that this test will create a folder called lib/mrtk/MixedRealityToolkit.AdhocTesting/empty
        /// in order to do this.
        /// </remarks>
        [Test]
        public void TestNonRootAssetFolderResolution()
        {
            CreateAdhocTestingPackageDirectory();
            MixedRealityToolkitFiles.RefreshFolders();
            string resolvedPath =
                MixedRealityToolkitFiles.MapRelativeFolderPathToAbsolutePath(MixedRealityToolkitModuleType.AdhocTesting,
                    "empty");
            string expectedPath = Path.Combine(Application.dataPath, "lib/mrtk/MixedRealityToolkit.AdhocTesting/empty");
        }

        /// <summary>
        /// Validates that MixedRealityToolkitFiles is able to reason over MRTK folders when placed in the root Asset directory.
        /// </summary>
        public void TestRootAssetFolderResolution()
        {
            string resolvedPath = MixedRealityToolkitFiles.MapRelativeFilePathToAbsolutePath("Inspectors/Data/EditorWindowOptions.json");
            string expectedPath = Path.Combine(Application.dataPath, "MixedRealityToolkit/Inspectors/Data/EditorWindowOptions.json");
            Assert.AreEqual(expectedPath, resolvedPath);
        }

        public void TestInvalidPath(string path)
        {
            foreach (string basePath in basePaths)
            {
                string fullPath = Path.Combine(basePath, path);
                Assert.False(MixedRealityToolkitFiles.FindMatchingModule(fullPath, out MixedRealityToolkitModuleType module));
            }
        }

        public void TestValidPath(string path, MixedRealityToolkitModuleType expectedModule)
        {
            foreach (string basePath in basePaths)
            {
                string fullPath = Path.Combine(basePath, path);
                Assert.True(MixedRealityToolkitFiles.FindMatchingModule(fullPath, out MixedRealityToolkitModuleType module));
                Assert.AreEqual(module, expectedModule);
            }
        }

        [TearDown]
        public void CleanupTests()
        {
            TestUtilities.EditorTearDownScenes();
            if (adhocTestDirectoryCreated)
            {
                File.Delete(Path.Combine(Application.dataPath, "lib.meta"));
                RecursiveFolderCleanup(new DirectoryInfo(Path.Combine(Application.dataPath, "lib")));
                adhocTestDirectoryCreated = false;
            }
        }

        /// <summary>
        /// Creates a directory under the Assets path that looks like:
        /// Assets/lib/mrtk/MixedRealityToolkit.Adhoc/empty
        /// </summary>
        /// <remarks>
        /// This must be cleaned up in CleanupTests.
        /// </remarks>
        private void CreateAdhocTestingPackageDirectory()
        {
            adhocTestDirectoryCreated = true;
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "lib"));
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "lib/mrtk"));
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "lib/mrtk/MixedRealityToolkit.AdhocTesting"));
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "lib/mrtk/MixedRealityToolkit.AdhocTesting/empty"));
        }

        private static void RecursiveFolderCleanup(DirectoryInfo folder)
        {
            foreach (DirectoryInfo subFolder in folder.GetDirectories())
            {
                RecursiveFolderCleanup(subFolder);
            }

            FileInfo[] fileList = folder.GetFiles("*");
            DirectoryInfo[] folderList = folder.GetDirectories();
            foreach (FileInfo file in fileList)
            {
                if (file.Extension.Equals(".meta"))
                {
                    string nameCheck = file.FullName.Remove(file.FullName.Length - 5);
                    if (!fileList.Concat<FileSystemInfo>(folderList).Any(t => nameCheck.Equals(t.FullName)))
                    {
                        file.Delete();
                    }
                }
            }

            if (folder.GetDirectories().Length == 0 && folder.GetFiles().Length == 0)
            {
                folder.Delete();
            }
        }
    }
}
