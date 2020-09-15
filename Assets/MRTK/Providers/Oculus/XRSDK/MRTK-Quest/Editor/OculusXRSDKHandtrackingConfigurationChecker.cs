// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿
//------------------------------------------------------------------------------ -
//MRTK - Quest
//https ://github.com/provencher/MRTK-Quest
//------------------------------------------------------------------------------ -
//
//MIT License
//
//Copyright(c) 2020 Eric Provencher, Roger Liu
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files(the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions :
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------ -


using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.EditModeTests")]
namespace Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Editor
{
    /// <summary>
    /// Class that checks if the Oculus Integration Assets are present and configures the project if they are.
    /// </summary>
    /// <remarks>
    /// Note that the checks that this class runs are fairly expensive and are only done manually by the user
    /// as part of their setup steps described here:
    /// https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/CrossPlatform/OculusQuestMRTK.html
    /// </remarks>
    public static class OculusXRSDKHandtrackingConfigurationChecker
    {
        // The presence of the OculusProjectConfig.asset is used to determine if the Oculus Integration Assets are in the project.
        private const string OculusIntegrationProjectConfig = "OculusProjectConfig.asset";
        private static readonly string[] Definitions = { "OCULUSINTEGRATION_PRESENT" };

        /// <summary>
        /// Detects if the Oculus Integration package is present and updates the AsmDefs with the appropriate definitions and references.
        /// </summary>
        [MenuItem("Mixed Reality Toolkit/Utilities/Oculus/Integrate Oculus Integration Unity Modules")]
        internal static void ConfigureOculusIntegration()
        {
            bool OculusIntegrationPresent = ReconcileOculusIntegrationDefine();

            // Updating asmdefs
            FileInfo[] oculusXRSDKAsmDefFile = FileUtilities.FindFilesInAssets("Microsoft.MixedReality.Toolkit.Providers.XRSDK.Oculus.asmdef");
            FileInfo[] oculusXRSDKHandtrackingUtilsAsmDefFile = FileUtilities.FindFilesInAssets("Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Handtracking.Utilities.asmdef");
            FileInfo[] oculusXRSDKHandtrackingEditorAsmDefFile = FileUtilities.FindFilesInAssets("Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Handtracking.Editor.asmdef");

            List<FileInfo[]> oculusAsmDefFiles = new List<FileInfo[]>() { oculusXRSDKAsmDefFile, oculusXRSDKHandtrackingUtilsAsmDefFile, oculusXRSDKHandtrackingEditorAsmDefFile };

            foreach (FileInfo[] oculusAsmDefFile in oculusAsmDefFiles)
            {
                // When MRTK is used through NuGet compiled assemblies, there will not be an asmdef file in the assets directory to configure.
                if (oculusAsmDefFile.Length == 0)
                {
                    return;
                }

                AssemblyDefinition oculusAsmDef = AssemblyDefinition.Load(oculusAsmDefFile[0].FullName);

                List<string> references = oculusAsmDef.References.ToList();

                if (!OculusIntegrationPresent)
                {
                    Debug.Log("Oculus Integration package not detected, removing references from asmdefs");
                    if (references.Contains("Unity.XR.Oculus"))
                    {
                        references.Remove("Unity.XR.Oculus");
                    }
                    if (references.Contains("Oculus.VR"))
                    {
                        references.Remove("Oculus.VR");
                    }
                    if (references.Contains("Oculus.VR.Editor"))
                    {
                        references.Remove("Oculus.VR.Editor");
                    }
                    oculusAsmDef.References = references.ToArray();
                }
                else
                {
                    if (oculusAsmDefFile == oculusXRSDKAsmDefFile)
                    {
                        oculusAsmDef.AddReference("Unity.XR.Oculus");
                    }
                    if (oculusAsmDefFile == oculusXRSDKAsmDefFile || oculusAsmDefFile == oculusXRSDKHandtrackingUtilsAsmDefFile)
                    {
                        oculusAsmDef.AddReference("Oculus.VR");
                    }
                    if (oculusAsmDefFile == oculusXRSDKHandtrackingEditorAsmDefFile)
                    {
                        oculusAsmDef.AddReference("Oculus.VR.Editor");
                    }
                }
                oculusAsmDef.Save(oculusAsmDefFile[0].FullName);
            }

            // Updating the device manager profile to point to the right gameobjects
            string[] ovrCameraRigPPrefabGuids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension("MRTK-Quest_OVRCameraRig.prefab"));
            string[] localAvatarPrefabGuids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension("MRTK-Quest_LocalAvatar.prefab"));
            GameObject ovrCameraRigPrefab = null;
            GameObject localAvatarPrefab = null;

            if (ovrCameraRigPPrefabGuids.Length > 0)
            {
                string ovrCameraRigPrefabPath = AssetDatabase.GUIDToAssetPath(ovrCameraRigPPrefabGuids[0]);
                ovrCameraRigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ovrCameraRigPrefabPath);
            }

            if (localAvatarPrefabGuids.Length > 0)
            {
                string localAvatarPrefabPath = AssetDatabase.GUIDToAssetPath(localAvatarPrefabGuids[0]);
                localAvatarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(localAvatarPrefabPath);
            }

            string[] deviceManagerProfileGuids = AssetDatabase.FindAssets("t:OculusXRSDKDeviceManagerProfile");
            if(deviceManagerProfileGuids.Length > 0)
            {
                string deviceManagerProfilePath = AssetDatabase.GUIDToAssetPath(deviceManagerProfileGuids[0]);
                OculusXRSDKDeviceManagerProfile deviceManagerProfile = AssetDatabase.LoadAssetAtPath<OculusXRSDKDeviceManagerProfile>(deviceManagerProfilePath);
                if (OculusIntegrationPresent)
                {
                    deviceManagerProfile.OVRCameraRigPrefab = ovrCameraRigPrefab;
                    deviceManagerProfile.LocalAvatarPrefab = localAvatarPrefab;
                }
                else
                {
                    deviceManagerProfile.OVRCameraRigPrefab = null;
                    deviceManagerProfile.LocalAvatarPrefab = null;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Updates the assembly definitions to mark the Oculus Integration Asset as present or not present
        /// </summary>
        /// <returns>true if Assets/Oculus/OculusProjectConfig exists, false otherwise</returns>
        internal static bool ReconcileOculusIntegrationDefine()
        {
            FileInfo[] files = FileUtilities.FindFilesInAssets(OculusIntegrationProjectConfig);

            if (files.Length > 0)
            {
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Android, Definitions);
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
                return true;
            }
            else
            {
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Android, Definitions);
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
                return false;
            }
        }

        /// <summary>
        /// Adds warnings to the nowarn line in the csc.rsp file located at the root of assets.  Warning 618 and 649 are added to the nowarn line because if
        /// the MRTK source is from the repo, warnings are converted to errors. Warnings are not converted to errors if the MRTK source is from the unity packages.
        /// Warning 618 and 649 are logged when Oculus Integration is imported into the project, 618 is the obsolete warning and 649 is a null on start warning.
        /// </summary>
        [MenuItem("Mixed Reality Toolkit/Utilities/Oculus/Configure CSC File for Oculus")]
        static void UpdateCSC()
        {
            // The csc file will always be in the root of assets
            string cscFilePath = Path.Combine(Application.dataPath, "csc.rsp");

            // Each line of the csc file
            List<string> cscFileLines = new List<string>();

            // List of the warning numbers after "-nowarn: " in the csc file
            List<string> warningNumbers = new List<string>();

            // List of new warning numbers to add to the csc file
            List<string> warningNumbersToAdd = new List<string>()
            {
                "618",
                "649"
            };

            using (StreamReader streamReader = new StreamReader(cscFilePath))
            {
                while (streamReader.Peek() > -1)
                {
                    string cscFileLine = streamReader.ReadLine();

                    if (cscFileLine.Contains("-nowarn"))
                    {
                        string[] currentWarningNumbers = cscFileLine.Split(',', ':');
                        warningNumbers = currentWarningNumbers.ToList();

                        // Remove "nowarn" from the warningNumbers list
                        warningNumbers.Remove("-nowarn");

                        foreach (string warningNumberToAdd in warningNumbersToAdd)
                        {
                            // Add the new warning numbers if they are not already in the file
                            if (!warningNumbers.Contains(warningNumberToAdd))
                            {
                                warningNumbers.Add(warningNumberToAdd);
                            }
                        }

                        cscFileLines.Add(string.Join(",", warningNumbers));
                    }
                    else
                    {
                        cscFileLines.Add(cscFileLine);
                    }
                }
            }

            using (StreamWriter streamWriter = new StreamWriter(cscFilePath))
            {
                foreach (string cscLine in cscFileLines)
                {
                    if (cscLine.StartsWith("1701"))
                    {
                        string warningNumbersJoined = string.Join(",", warningNumbers);
                        streamWriter.WriteLine(string.Concat("-nowarn:", warningNumbersJoined));
                    }
                    else
                    {
                        streamWriter.WriteLine(cscLine);
                    }
                }
            }
        }
    }
}
