// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;


#if UNITY_2019_3_OR_NEWER
using Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Editor;

namespace Microsoft.MixedReality.Toolkit.Tests.EditModeTests.Editor
{
    /// <summary>
    /// Tests which ensure that our Oculus Integration setup process works.
    /// </summary>
    public class OculusIntegrationSetupTests
    {
        [SetUp]
        public void SetUp()
        {
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Assets\\OculusProjectConfig.asset");

            // Create a dummy asset for the OculusProjectConfig
            Material DummyAsset = new Material(Shader.Find("Specular"));
            AssetDatabase.CreateAsset(DummyAsset, "Assets/OculusProjectConfig.asset");
        }

        /// <summary>
        /// Tests that the appropriate definitions and asmdef references are configured based on the presence of 
        /// the OculusProjectConfig.asset file
        /// </summary>
        [UnityTest]
        public IEnumerator TestOculusIntegrationSetup()
        {
            // Configure the oculus settings successfully
            OculusXRSDKHandtrackingConfigurationChecker.IntegrateOculusWithMRTK();

            // Check that the definitions and amdefs are set up correctly when Oculus Integration is present
            Assert.IsTrue(OculusXRSDKHandtrackingConfigurationChecker.DetectOculusIntegrationDefine());
            ValidateAsmdefs(true);

            // Remove the dummy asset and clean up the project to prevent errors
            TearDown();
            OculusXRSDKHandtrackingConfigurationChecker.ReconcileOculusIntegrationDefine(false);
            OculusXRSDKHandtrackingConfigurationChecker.ConfigureOculusIntegration(false);

            // Check that the definitions and amdefs are set up correctly when Oculus Integration is not present
            Assert.IsFalse(OculusXRSDKHandtrackingConfigurationChecker.DetectOculusIntegrationDefine());
            ValidateAsmdefs(false);

            yield return null;
        }

        private void ValidateAsmdefs(bool OculusIntegrationPresent)
        {
            FileInfo[] oculusXRSDKAsmDefFile = FileUtilities.FindFilesInAssets("Microsoft.MixedReality.Toolkit.Providers.XRSDK.Oculus.asmdef");
            FileInfo[] oculusXRSDKHandtrackingUtilsAsmDefFile = FileUtilities.FindFilesInAssets("Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Handtracking.Utilities.asmdef");
            FileInfo[] oculusXRSDKHandtrackingEditorAsmDefFile = FileUtilities.FindFilesInAssets("Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Handtracking.Editor.asmdef");

            List<FileInfo[]> oculusAsmDefFiles = new List<FileInfo[]>() { oculusXRSDKAsmDefFile, oculusXRSDKHandtrackingUtilsAsmDefFile, oculusXRSDKHandtrackingEditorAsmDefFile };

            foreach (FileInfo[] oculusAsmDefFile in oculusAsmDefFiles)
            {
                if (oculusAsmDefFile.Length == 0)
                {
                    return;
                }

                AssemblyDefinition oculusAsmDef = AssemblyDefinition.Load(oculusAsmDefFile[0].FullName);

                List<string> references = oculusAsmDef.References.ToList();

                if (OculusIntegrationPresent)
                {
                    if (oculusAsmDefFile == oculusXRSDKAsmDefFile || oculusAsmDefFile == oculusXRSDKHandtrackingUtilsAsmDefFile)
                    {
                        Assert.IsTrue(references.Contains("Oculus.VR"));
                    }
                    if (oculusAsmDefFile == oculusXRSDKHandtrackingEditorAsmDefFile)
                    {
                        Assert.IsTrue(references.Contains("Oculus.VR.Editor"));
                    }
                }
                else
                {
                    if (oculusAsmDefFile == oculusXRSDKAsmDefFile || oculusAsmDefFile == oculusXRSDKHandtrackingUtilsAsmDefFile)
                    {
                        Assert.IsFalse(references.Contains("Oculus.VR"));
                    }
                    if (oculusAsmDefFile == oculusXRSDKHandtrackingEditorAsmDefFile)
                    {
                        Assert.IsFalse(references.Contains("Oculus.VR.Editor"));
                    }
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset("Assets/OculusProjectConfig.asset");
            AssetDatabase.Refresh();
        }
    }
}
#endif