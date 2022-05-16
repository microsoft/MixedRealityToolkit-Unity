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
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Editor
{
    /// <summary>
    /// Class that checks if the Oculus Integration Assets are present and configures the project if they are.
    /// </summary>
    /// <remarks>
    /// <para>Note that the checks that this class runs are fairly expensive and are only done manually by the user
    /// as part of their setup steps described here:
    /// https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/supported-devices/oculus-quest-mrtk </para>
    /// </remarks>
    public static class OculusXRSDKHandtrackingConfigurationChecker
    {
        // The presence of the OculusProjectConfig.asset is used to determine if the Oculus Integration Assets are in the project.
        private const string OculusIntegrationProjectConfig = "OculusProjectConfig.asset";
        private static readonly string[] Definitions = { "OCULUSINTEGRATION_PRESENT" };

        /// <summary>
        /// Integrate MRTK and the Oculus Integration Unity Modules if the Oculus Integration Unity Modules is in the project. If it is not in the project, display a pop up window.
        /// </summary>
        [MenuItem("Mixed Reality/Toolkit/Utilities/Oculus/Integrate Oculus Integration Unity Modules")]
        internal static void IntegrateOculusWithMRTK()
        {
            // Check if Oculus Integration package is present
            bool oculusIntegrationPresent = DetectOculusIntegrationAsset();

            if (!oculusIntegrationPresent)
            {
                EditorUtility.DisplayDialog(
                    "Oculus Integration Package Not Detected",
                    "The Oculus Integration Package could not be found in this project, please import the assets into this project. The assets can be found here: " +
                        "https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022",
                    "OK");
            }

            // Update the ScriptingDefinitions depending on the presence of the Oculus Integration Unity Modules
            ReconcileOculusIntegrationDefine(oculusIntegrationPresent);

            // Update the CSC to filter out warnings emitted by the Oculus Integration Package
            if (oculusIntegrationPresent)
            {
                UpdateCSC();
            }
            ConfigureOculusDeviceManagerDefaults();
        }

        /// <summary>
        /// Separate MRTK and the Oculus Integration Unity Modules and display a prompt for the user to close unity and delete the assets.
        /// </summary>
        [MenuItem("Mixed Reality/Toolkit/Utilities/Oculus/Separate Oculus Integration Unity Modules")]
        internal static void SeparateOculusFromMRTK()
        {
            bool oculusIntegrationPresent = DetectOculusIntegrationAsset();

            // If the user tries to separate the Oculus Integration assets without assets in the project display a message
            if (!oculusIntegrationPresent)
            {
                EditorUtility.DisplayDialog(
                       "MRTK Oculus Removal",
                       "There are no Oculus Integration assets in the project to separate from MRTK",
                       "OK");

                return;
            }

            // Force removal of the ScriptingDefinitions while the Oculus Integration is still in the project
            ReconcileOculusIntegrationDefine(false);

            // Prompt the user to close unity and delete the assets to completely remove.  Closing unity and deleting the assets is optional.
            EditorUtility.DisplayDialog(
                "MRTK Oculus Integration Removal",
                "To complete the removal of the Oculus Integration Unity Modules, close Unity, delete the assets and Library folders, and reopen Unity",
                "OK");
        }

        /// <summary>
        /// Initialize the Oculus Project Config with the appropriate settings to enable handtracking and keyboard support.
        /// </summary>
        [MenuItem("Mixed Reality/Toolkit/Utilities/Oculus/Initialize Oculus Project Config")]
        internal static void InitializeOculusProjectConfig()
        {
#if OCULUSINTEGRATION_PRESENT
            // Updating the oculus project config to allow for handtracking and system keyboard usage
            OVRProjectConfig defaultOculusProjectConfig = OVRProjectConfig.GetProjectConfig();
            if (defaultOculusProjectConfig != null)
            {
                defaultOculusProjectConfig.handTrackingSupport = OVRProjectConfig.HandTrackingSupport.ControllersAndHands;
                defaultOculusProjectConfig.requiresSystemKeyboard = true;

                OVRProjectConfig.CommitProjectConfig(defaultOculusProjectConfig);

                Debug.Log("Enabled Oculus Quest Keyboard and Handtracking in the Oculus Project Config");
            }
#endif
        }

        [MenuItem("Mixed Reality/Toolkit/Utilities/Oculus/Initialize Oculus Project Config", true)]
        private static bool CheckScriptingDefinePresent()
        {
#if OCULUSINTEGRATION_PRESENT
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// Configures the default device manager profile with default prefabs if they are not yet loaded
        /// </summary>
        internal static void ConfigureOculusDeviceManagerDefaults()
        {
            // Updating the device manager profile to point to the right gameobjects
            string[] defaultOvrCameraRigPPrefabGuids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension("MRTK-Quest_OVRCameraRig.prefab"));
            string[] defaultLocalAvatarPrefabGuids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension("MRTK-Quest_LocalAvatar.prefab"));
            GameObject defaultOvrCameraRigPrefab = null;
            GameObject defaultLocalAvatarPrefab = null;

            if (defaultOvrCameraRigPPrefabGuids.Length > 0)
            {
                string ovrCameraRigPrefabPath = AssetDatabase.GUIDToAssetPath(defaultOvrCameraRigPPrefabGuids[0]);
                defaultOvrCameraRigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ovrCameraRigPrefabPath);
            }

            if (defaultLocalAvatarPrefabGuids.Length > 0)
            {
                string localAvatarPrefabPath = AssetDatabase.GUIDToAssetPath(defaultLocalAvatarPrefabGuids[0]);
                defaultLocalAvatarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(localAvatarPrefabPath);
            }

            string[] defaultDeviceManagerProfileGuids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension("DefaultOculusXRSDKDeviceManagerProfile.asset"));
            if (defaultDeviceManagerProfileGuids.Length > 0)
            {
                string deviceManagerProfilePath = AssetDatabase.GUIDToAssetPath(defaultDeviceManagerProfileGuids[0]);
                OculusXRSDKDeviceManagerProfile deviceManagerProfile = AssetDatabase.LoadAssetAtPath<OculusXRSDKDeviceManagerProfile>(deviceManagerProfilePath);

                if (deviceManagerProfile.OVRCameraRigPrefab == null)
                    deviceManagerProfile.OVRCameraRigPrefab = defaultOvrCameraRigPrefab;
                if (deviceManagerProfile.LocalAvatarPrefab == null)
                    deviceManagerProfile.LocalAvatarPrefab = defaultLocalAvatarPrefab;

                EditorUtility.SetDirty(deviceManagerProfile);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Checks if the Oculus Integration Asset as present or not present
        /// </summary>
        /// <returns>true if Assets/Oculus/OculusProjectConfig exists, false otherwise</returns>
        internal static bool DetectOculusIntegrationAsset()
        {
            FileInfo[] files = FileUtilities.FindFilesInAssets(OculusIntegrationProjectConfig);

            return files.Length > 0;
        }

        /// <summary>
        /// Updates the assembly definitions to mark the Oculus Integration Asset as present or not present
        /// </summary>
        internal static void ReconcileOculusIntegrationDefine(bool oculusIntegrationPresent)
        {
            if (oculusIntegrationPresent)
            {
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Android, Definitions);
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
            }
            else
            {
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Android, Definitions);
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
            }
        }

        /// <summary>
        /// Adds warnings to the nowarn line in the csc.rsp file located at the root of assets.  Warning 618 is added to the nowarn line because if
        /// the MRTK source is from the repo, warnings are converted to errors. Warnings are not converted to errors if the MRTK source is from the unity packages.
        /// Warning 618 is logged when building the project as of Oculus Integration 39.0 https://developer.oculus.com/downloads/package/unity-integration/39.0
        /// 618 is the obsolete property/function warning
        /// </summary>
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
                "618"
            };

            if (!File.Exists(cscFilePath))
            {
                return;
            }

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

            Debug.Log("csc file updated to filter out warnings");
        }
    }
}
