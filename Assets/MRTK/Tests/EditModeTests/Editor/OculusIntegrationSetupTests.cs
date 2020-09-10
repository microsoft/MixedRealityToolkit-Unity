// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Providers.XRSDK.Oculus.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.EditModeTests.Editor
{
    /// <summary>
    /// Set of tests to validate MRTK Toolbox Editor window.
    /// </summary>
    public class OculusIntegrationSetupTests
    {
        /// <summary>
        /// Tests that the MixedRealityToolboxWindow can load without exception and that none of
        /// its internal item contents are null or invalid.
        /// </summary>
        [UnityTest]
        public IEnumerator TestOculusIntegrationSetup()
        {
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Assets\\OculusProjectConfig.asset");

            // Create a dummy asset for the OculusProjectConfig
            Material DummyAsset = new Material(Shader.Find("Specular"));
            AssetDatabase.CreateAsset(DummyAsset, "Assets/OculusProjectConfig.asset");

            // Configure the oculus settings successfully
            OculusXRSDKHandtrackingConfigurationChecker.ConfigureOculusIntegration();

            // Check that the definitions and amdefs are set up correctly when Oculus Integration is present
            Assert.IsTrue(OculusXRSDKHandtrackingConfigurationChecker.ReconcileOculusIntegrationDefine());
            ValidateAsmdefs(true);

            // Remove the dummy asset and clean up the project to prevent errors
            AssetDatabase.DeleteAsset("Assets/OculusProjectConfig.asset");
            OculusXRSDKHandtrackingConfigurationChecker.ConfigureOculusIntegration();

            // Check that the definitions and amdefs are set up correctly when Oculus Integration is not present
            Assert.IsFalse(OculusXRSDKHandtrackingConfigurationChecker.ReconcileOculusIntegrationDefine());
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
    }
}
