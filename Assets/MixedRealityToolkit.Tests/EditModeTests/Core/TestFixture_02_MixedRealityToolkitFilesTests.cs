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

namespace Microsoft.MixedReality.Toolkit.Tests.Core
{
    // Tests for the MixedRealityToolkitFiles utility class
    public class TestFixture_02_MixedRealityToolkitFilesTests
    {
        string[] basePaths = new string[] { "", "C:\\", "C:\\xyz\\", "C:/xyz/" };

        [Test]
        public void Test_01_FindMatchingModule()
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
        public void Test_02_FindMatchingModuleNuget()
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
            TestUtilities.TearDownScenes();
        }
    }
}
