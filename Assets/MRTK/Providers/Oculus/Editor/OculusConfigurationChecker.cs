using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Oculus
{
    /// <summary>
    /// Class that checks if the Oculus Integration Assets are present and configures the project if they are.
    /// </summary>
    [InitializeOnLoad]
    static class OculusConfigurationChecker
    {
        // The presence of the OculusProjectConfig.asset is used to determine if the Oculus Integration Assets are in the project.
        private const string OculusIntegrationProjectConfig = "OculusProjectConfig.asset";
        private static readonly string[] Definitions = { "OCULUS_HANDTRACKING_PRESENT" };

        // True if the Oculus Integration Assets are in the project.
        private static bool isOculusInProject = false;

        // The current supported Oculus Integration Assets version numbers.
        private static string[] leapCoreAssetsVersionsSupported = new string[] { "1.7.0"};

        // The current Oculus Integration Assets version in this project
        private static string currentLeapCoreAssetsVersion = "";

        // The path difference between the root of assets and the root of the Oculus Integration Assets.
        private static string pathDifference = "";

        static OculusConfigurationChecker()
        {
            // Check if leap core is in the project
            isOculusInProject = ReconcileOculusIntegrationDefine();

            ConfigureOculusIntegeration(isOculusInProject);
        }

        //seems to work fine as is
        private static bool ReconcileOculusIntegrationDefine()
        {
            FileInfo[] files = FileUtilities.FindFilesInAssets(OculusIntegrationProjectConfig);

            if (files.Length > 0)
            {
                // Make this set the application to controllers and hands
                // something vaguely like this.
                OVRProjectConfig projectConfig = AssetDatabase.LoadAssetAtPath(files[0].DirectoryName + "/" + OculusIntegrationProjectConfig, typeof(OVRProjectConfig)) as OVRProjectConfig;
                projectConfig.handTrackingSupport = OVRProjectConfig.HandTrackingSupport.ControllersAndHands;
                AssetDatabase.Refresh();
                return true;
            }
            else
            {
                return false;
            }
        }


        private static void ConfigureOculusIntegeration(bool isOculusInProject)
        {
            FileInfo[] leapDataProviderAsmDefFile = FileUtilities.FindFilesInAssets("Microsoft.MixedReality.Toolkit.Providers.LeapMotion.asmdef");

            // When MRTK is used through NuGet compiled assemblies, there will not be an asmdef file in the assets directory to configure.
            if (leapDataProviderAsmDefFile.Length == 0)
            {
                return;
            }

            AssemblyDefinition leapDataProviderAsmDef = AssemblyDefinition.Load(leapDataProviderAsmDefFile[0].FullName);

            List<string> references = leapDataProviderAsmDef.References.ToList();

            if (isOculusInProject && !references.Contains("OculusIntegration"))
            {
                //// Get the location of the Leap Core Assets relative to the root directory
                //pathDifference = GetPathDifference();

                // Make sure the Leap Core Assets imported are version 4.4.0
                bool isOculusIntegrationVersionSupported = OculusIntegrationVersionCheck();

                if (isOculusIntegrationVersionSupported)
                {
                    //Need to figure out if I need to modify asmdefs
                    //AddAndUpdateAsmDefs();
                    //AddLeapEditorAsmDefs();
                    // Refresh the database because tests were removed and 10 asmdefs were added
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("MRTK only supports the Oculus Integration Version 1.7.0, the Oculus Integration imported is not Version 1.7.0");
                }
            }

            if (!isOculusInProject && references.Contains("OculusIntegration"))
            {
                references.Remove("OculusIntegration");
                leapDataProviderAsmDef.References = references.ToArray();
                leapDataProviderAsmDef.Save(leapDataProviderAsmDefFile[0].FullName);
            }
        }

        /// <summary>
        /// The oculus integration unitypackage doesn't actually say what version number it is ;-;
        /// </summary>
        /// <returns>True, if the Leap Motion Core Assets version imported is supported</returns>
        private static bool OculusIntegrationVersionCheck()
        {
            return true;
        }

        /// <summary>
        /// Get the difference between the root of assets and the location of the leap core assets.  If the leap core assets 
        /// are at the root of assets, there is no path difference.
        /// </summary>
        /// <returns>Returns an empty string if the leap core assets are at the root of assets, otherwise return the path difference</returns>
        private static string GetPathDifference()
        {
            // The file LeapXRServiceProvider.cs is used as a location anchor instead of the LeapMotion directory
            // to avoid a potential incorrect location return if there is a folder named LeapMotion prior to the leap 
            // core assets import 
            FileInfo[] leapPathLocationAnchor = FileUtilities.FindFilesInAssets(OculusIntegrationProjectConfig);
            string leapFilePath = leapPathLocationAnchor[0].FullName;

            List<string> leapPath = leapFilePath.Split(Path.DirectorySeparatorChar).ToList();

            // Remove the last 3 elements of leap path (/Core/Scripts/LeapXRService.cs) from the list to get the root of the leap core assets
            leapPath.RemoveRange(leapPath.Count - 3, 3);

            List<string> unityDataPath = Application.dataPath.Split('/').ToList();
            unityDataPath.Add("OculusIntegration");

            // Get the difference between the root of assets and the root of leap core assets
            IEnumerable<string> difference = leapPath.Except(unityDataPath);

            return string.Join("/", difference);
        }

        /// <summary>
        /// Adds warnings to the nowarn line in the csc.rsp file located at the root of assets.  Warning 618 and 649 are added to the nowarn line because if
        /// the MRTK source is from the repo, warnings are converted to errors. Warnings are not converted to errors if the MRTK source is from the unity packages.
        /// Warning 618 and 649 are logged when the Leap Motion Core Assets are imported into the project, 618 is the obsolete warning and 649 is a null on start warning.
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

            Debug.Log($"Saving {cscFilePath}");
        }

#if UNITY_2018       
        /// <summary>
        /// Force Leap Motion integration after the Leap Motion Core Assets import.  In Unity 2018.4, the configuration checker sometimes does not update after the 
        /// Leap Motion Core Assets import, this case only occurs if the MRTK source is from the unity packages. If the integration of leap and MRTK has not occurred, users can 
        /// select the Configure Leap Motion menu option to force integration. 
        /// </summary>
        [MenuItem("Mixed Reality Toolkit/Utilities/Leap Motion/Configure Leap Motion")]
        static void ForceLeapMotionConfiguration()
        {
            // Check if leap core is in the project
            isLeapInProject = ReconcileLeapMotionDefine();

            ConfigureLeapMotion(isLeapInProject);
        }
#endif
    }
}
