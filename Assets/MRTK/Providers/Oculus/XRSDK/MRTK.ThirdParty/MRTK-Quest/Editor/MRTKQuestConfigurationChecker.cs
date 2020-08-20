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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Providers.XRSDK.Oculus.Editor
{
    /// <summary>
    /// Class that checks if the Oculus Integration Assets are present and configures the project if they are.
    /// </summary>
    /// <remarks>
    /// Note that the checks that this class runs are fairly expensive and are only done manually by the user
    /// as part of their setup steps described here:
    /// https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/CrossPlatform/OculusQuestMRTK.html
    /// </remarks>
    static class OculusConfigurationChecker
    {
        // The presence of the OculusProjectConfig.asset is used to determine if the Oculus Integration Assets are in the project.
        private const string OculusIntegrationProjectConfig = "OculusProjectConfig.asset";
        private static readonly string[] Definitions = { "OCULUSINTEGRATION_PRESENT" };

        /// <summary>
        /// Updates the assembly definitions to mark the Oculus Integration Asset as present or not present
        /// </summary>
        /// <returns>true if Assets/Oculus/OculusProjectConfig exists, false otherwise</returns>
        [MenuItem("Mixed Reality Toolkit/Utilities/Oculus/Update Assembly Definitions")]
        private static bool UpdateAssemblyDefinitions()
        {
            FileInfo[] files = FileUtilities.FindFilesInAssets(OculusIntegrationProjectConfig);

            if (files.Length > 0) 
            {
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Android, Definitions);
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
                ConfigureProfilesForHandtracking();
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
        /// Configures the project profiles to support handtracking
        /// </summary>
        static void ConfigureProfilesForHandtracking()
        {
#if OCULUSINTEGRATION_PRESENT
            FileInfo[] files = FileUtilities.FindFilesInAssets(OculusIntegrationProjectConfig);

            // Make this set the application to controllers and hands on first setup
            string configPath = "";
            if (files[0].FullName.Replace("\\", "/").StartsWith(Application.dataPath))
            {
                configPath = "Assets" + files[0].FullName.Substring(Application.dataPath.Length);
            }

            OVRProjectConfig projectConfig = AssetDatabase.LoadAssetAtPath<OVRProjectConfig>(configPath);
            projectConfig.handTrackingSupport = OVRProjectConfig.HandTrackingSupport.ControllersAndHands;
            AssetDatabase.Refresh();
#endif
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
