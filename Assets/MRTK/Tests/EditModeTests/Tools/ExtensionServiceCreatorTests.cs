// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.Editor
{
    public class ExtensionServiceCreatorTests
    {

        private const string DefaultGeneratedFolder = "MixedRealityToolkit.Generated";
        private const string DefaultExtensionFolder = "MixedRealityToolkit.Generated/ExtensionFolder";
        private bool GeneratedFolderExisted = false;
        private bool ExtensionFolderExisted = false;
        
        [TearDown]
        public void TearDown()
        {
            // Only do extension folder cleanup if it hadn't already existed (i.e. to avoid
            // destroying local contributor state).
            if (!ExtensionFolderExisted)
            {
                // If the generated folder also didn't exist prior to the test running, then
                // we need to clean it up (to fully clean up state created by the test)
                if (!GeneratedFolderExisted)
                {
                    DeleteFolderAndMeta(DefaultGeneratedFolder);
                }
                else
                {
                    // Otherwise, the generated folder already existed, we just need to clean
                    // up the extensions folder that we created.
                    DeleteFolderAndMeta(DefaultExtensionFolder);
                }

                AssetDatabase.Refresh();
            }
        }

        [Test]
        public void TestCreateDefaultFolder()
        {
            GeneratedFolderExisted = AssetDatabase.IsValidFolder(Path.Combine("Assets", DefaultGeneratedFolder));
            ExtensionFolderExisted = AssetDatabase.IsValidFolder(Path.Combine("Assets", DefaultExtensionFolder));
            
            // This test intentionally no-ops in the case that the extension folder already exists - we
            // don't to destroy local state, or risk bugs in moving that temporarily. This test is designed
            // to work on a fresh clone (i.e. on CI).
            if (ExtensionFolderExisted)
            {
                return;
            }
            
            ExtensionServiceCreator creator = new ExtensionServiceCreator();
            creator.ValidateAssets(new List<string>());
            Assert.IsNotNull(creator.ServiceFolderObject);
        }

        private static void DeleteFolderAndMeta(string folder)
        {
            string resolvedFolder = Path.Combine(Application.dataPath, folder);
            string resolvedFolderMeta = resolvedFolder + ".meta";
            try
            {
                Directory.Delete(resolvedFolder, true);
                File.Delete(resolvedFolderMeta);
            }
            catch (Exception)
            {
                // It's possible that these things could have been deleted outside of the test
                // process, so don't fail the test if that happened.
            }
        }
    }
}