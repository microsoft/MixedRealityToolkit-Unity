// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.LeapMotion
{
    /// <summary>
    /// Class that checks if the Leap Motion Core assets are present and configures the project if they are.
    /// </summary>

    static class LeapMotionConfigurationChecker
    {
        // The presence of the LeapXRServiceProvider.cs is used to determine if the Leap Motion Core Assets are in the project.
        private const string trackedLeapFileName = "LeapXRServiceProvider.cs";
        private static readonly string[] Definitions = { "LEAPMOTIONCORE_PRESENT" };

        // True if the Leap Motion Core Assets are in the project.
        private static bool isLeapInProject = false;

        // Does MRTK recognize the Leap Motion Unity Modules?
        // The assets can be in the project but MRTK might not recognize their presence because 
        // the user has not selected the integration menu item.
        private static bool isLeapRecognizedByMRTK = false;

        // The current supported Leap Core Assets version numbers.
        private static string[] leapCoreAssetsVersionsSupported = new string[] { "4.5.0", "4.5.1", "4.6.0", "4.7.0", "4.7.1", "4.8.0", "4.9.1" };

        // The current Leap Core Assets version in this project
        private static string currentLeapCoreAssetsVersion = "";

        // The path difference between the root of assets and the root of the Leap Motion Core Assets.
        private static string pathDifference = "";

        // The Leap Unity Modules version 4.7.1 already contains a LeapMotion.asmdef file at this path
        private static string leapAsmDefPath_471 = "LeapMotion/Core/Scripts/LeapMotion.asmdef";

        // This path is used to determine if the Leap Motion Unity Modules is version 4.7.0
        private static string leapTestsPath_470 = "LeapMotion/Core/Editor/Tests";

        // This path is used to determine if the Leap Motion Unity Modules is version 4.6.0 or 4.5.1
        private static string leapXRPath_460 = "LeapMotion/Core/Scripts/XR/LeapXRPinchLocomotion.cs";

        // Array of paths to Leap Motion testing directories that will be removed from the project.
        // Make sure each test directory ends with '/'
        // These paths only need to be deleted if the Leap Core Assets version is 4.4.0
        private static readonly string[] pathsToDelete = new string[]
        {
            "LeapMotion/Core/Editor/Tests/",
            "LeapMotion/Core/Plugins/LeapCSharp/Editor/Tests/",
            "LeapMotion/Core/Scripts/Algorithms/Editor/Tests/",
            "LeapMotion/Core/Scripts/DataStructures/Editor/Tests/",
            "LeapMotion/Core/Scripts/Encoding/Editor/",
            "LeapMotion/Core/Scripts/Query/Editor/",
            "LeapMotion/Core/Scripts/Utils/Editor/BitConverterNonAllocTests.cs",
            "LeapMotion/Core/Scripts/Utils/Editor/ListAndArrayExtensionTests.cs",
            "LeapMotion/Core/Scripts/Utils/Editor/TransformUtilTests.cs",
            "LeapMotion/Core/Scripts/Utils/Editor/UtilsTests.cs",
        };

        // Dictionary of names and references of new asmdefs that will be added to the Leap Motion Core Assets.
        private static readonly Dictionary<string, string[]> leapEditorDirectories = new Dictionary<string, string[]>
        {
            { "LeapMotion.Core.Editor", new string[] { "LeapMotion" } },
            { "LeapMotion.Core.Scripts.Animation.Editor", new string[] { "LeapMotion", "LeapMotion.Core.Editor", "LeapMotion.Core.Scripts.Utils.Editor" } },
            { "LeapMotion.Core.Scripts.Attachments.Editor", new string[] { "LeapMotion", "LeapMotion.Core.Editor" } },
            { "LeapMotion.Core.Scripts.Attributes.Editor", new string[] { "LeapMotion" } },
            { "LeapMotion.Core.Scripts.DataStructures.Editor", new string[] { "LeapMotion" } },
            { "LeapMotion.Core.Scripts.EditorTools.Editor", new string[] { "LeapMotion", "LeapMotion.Core.Scripts.Utils.Editor" } },
            { "LeapMotion.Core.Scripts.Utils.Editor", new string[] { "LeapMotion", "LeapMotion.Core.Editor" } },
            { "LeapMotion.Core.Scripts.XR.Editor", new string[] { "LeapMotion", "LeapMotion.Core.Editor" } },
            { "LeapMotion.Core.Tests.Editor", new string[] { "LeapMotion" } }
        };

        /// <summary>
        /// Ensures that the appropriate symbolic constant is defined based on the presence of the Leap Motion Core Assets.
        /// </summary>
        /// <returns>If the define was added or the define has already been added, return true</returns>
        private static bool ReconcileLeapMotionDefine()
        {
            FileInfo[] files = FileUtilities.FindFilesInAssets(trackedLeapFileName);

            if (files.Length > 0)
            {
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.WSA, Definitions);

                isLeapInProject = true;
                isLeapRecognizedByMRTK = true;

                return true;
            }
            else
            {
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.WSA, Definitions);
                isLeapRecognizedByMRTK = false;

                return false;
            }
        }

        /// <summary>
        /// Configure the Leap Motion Core assets if they are in the project.  First remove testing folders, add LeapMotion.asmdef at the
        /// root of the core assets, and add the leap editor asmdefs.  If the core assets are not in the project, make sure the reference
        /// in the Microsoft.MixedReality.Toolkit.Providers.LeapMotion.asmdef does not contain a ref to LeapMotion.
        /// </summary>
        /// <param name="isLeapInProject">Bool that determines if the Leap Motion Core assets are in the project</param>
        private static void ConfigureLeapMotion(bool isLeapInProject)
        {
            FileInfo[] leapDataProviderAsmDefFile = FileUtilities.FindFilesInAssets("MRTK.LeapMotion.asmdef");

            // When MRTK is used through NuGet compiled assemblies, there will not be an asmdef file in the assets directory to configure.
            if (leapDataProviderAsmDefFile.Length == 0)
            {
                return;
            }

            if (isLeapInProject)
            {
                // Get the location of the Leap Core Assets relative to the root directory
                pathDifference = GetPathDifference();

                // Make sure the Leap Core Assets version is supported 
                bool isLeapCoreAssetsVersionSupported = LeapCoreAssetsVersionSupport();

                if (isLeapCoreAssetsVersionSupported)
                {
                    if (currentLeapCoreAssetsVersion == "4.7.1")
                    {
                        Debug.Log($"Integrating the Leap Motion Unity Modules Version {currentLeapCoreAssetsVersion} or 4.8.0 with MRTK");
                    }
                    else
                    {
                        Debug.Log($"Integrating the Leap Motion Unity Modules Version {currentLeapCoreAssetsVersion} with MRTK");
                    }

                    RemoveTestingFolders();
                    AddAndUpdateAsmDefs();
                    AddLeapEditorAsmDefs();

                    // Refresh the database because tests were removed and 10 asmdefs were added
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("The Leap Motion Unity Modules version imported is not currently supported by MRTK, compatible versions are listed in the Leap Motion MRTK documentation: " +
                        "https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/supported-devices/leap-motion-mrtk");
                }
            }
        }

        /// <summary>
        /// Checks if the Leap Motion Core Assets version is supported.
        /// </summary>
        /// <returns>True, if the Leap Motion Core Assets version imported is supported</returns>
        private static bool LeapCoreAssetsVersionSupport()
        {
            string versionLeapPath = Path.Combine(Application.dataPath, pathDifference, "LeapMotion", "Core", "Version.txt");

            using (StreamReader streamReader = new StreamReader(versionLeapPath))
            {
                while (streamReader.Peek() > -1)
                {
                    string line = streamReader.ReadLine();

                    foreach (string versionNumberSupported in leapCoreAssetsVersionsSupported)
                    {
                        // If the leap core assets version number is supported
                        if (line.Contains(versionNumberSupported))
                        {
                            currentLeapCoreAssetsVersion = versionNumberSupported;

                            // The Leap Motion Unity modules Version.txt has remained 4.5.1 across versions 4.6.0, 4.7.0, 4.7.1, and 4.8.0 check for the presence
                            // of certain paths to infer the version number.
                            if (currentLeapCoreAssetsVersion == "4.5.1")
                            {
                                // This path is only present in 4.7.1
                                string leap471Path = Path.Combine(Application.dataPath, pathDifference, leapAsmDefPath_471);

                                // This path is present in versions 4.7.0 and 4.7.1
                                string testDirectoryPath = Path.Combine(Application.dataPath, pathDifference, leapTestsPath_470);

                                // This path is present in 4.6.0 and not 4.5.1
                                string xrPath = Path.Combine(Application.dataPath, pathDifference, leapXRPath_460);

                                if (File.Exists(leap471Path))
                                {
                                    // The Leap Motion Unity modules Core package version 4.7.1 is identical to the version 4.8.0 
                                    // Core package. Due to the lack of differences between the two versions, the modules will be marked as 4.7.1 even
                                    // if they are version 4.8.0.
                                    currentLeapCoreAssetsVersion = "4.7.1";
                                }
                                else if (!File.Exists(leap471Path) && Directory.Exists(testDirectoryPath))
                                {
                                    currentLeapCoreAssetsVersion = "4.7.0";
                                }
                                else if (!File.Exists(leap471Path) && !Directory.Exists(testDirectoryPath) && File.Exists(xrPath))
                                {
                                    currentLeapCoreAssetsVersion = "4.6.0";
                                }
                            }

                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// The Leap Core Assets currently contain multiple folders with tests in them.  An issue has been filed in the Unity
        /// Modules repo: https://github.com/leapmotion/UnityModules/issues/1097.  The issue with the multiple test folders is when an 
        /// asmdef is placed at the root of the core assets, each folder containing tests needs another separate asmdef.  This method
        /// is used to avoid adding an additional 8 asmdefs to the project, by removing the folders and files that are tests in the 
        /// Leap Core Assets.
        /// </summary>
        private static void RemoveTestingFolders()
        {
            // If one of the leap test directories exists, then we assume the rest have not been deleted
            if (Directory.Exists(Path.Combine(Application.dataPath, pathDifference, pathsToDelete[0])))
            {
                foreach (string path in pathsToDelete)
                {
                    // Get the full path including the path difference in case the core assets are not imported to the root of the project
                    string fullPath = Path.Combine(Application.dataPath, pathDifference, path);

                    // If we are deleting a specific file, then we also need to remove the meta associated with the file
                    if (File.Exists(fullPath) && fullPath.Contains(".cs"))
                    {
                        // Delete the test files
                        FileUtil.DeleteFileOrDirectory(fullPath);

                        // Also delete the meta files
                        FileUtil.DeleteFileOrDirectory(fullPath + ".meta");
                    }

                    if (Directory.Exists(fullPath))
                    {
                        // Delete the test directories
                        FileUtil.DeleteFileOrDirectory(fullPath);

                        // Delete the test directories meta files
                        FileUtil.DeleteFileOrDirectory(fullPath.TrimEnd('/') + ".meta");
                    }
                }
            }
        }

        /// <summary>
        /// Adds an asmdef at the root of the LeapMotion Core Assets once they are imported into the project and adds the newly created LeapMotion.asmdef
        /// as a reference for the existing leap data provider asmdef.
        /// </summary>
        private static void AddAndUpdateAsmDefs()
        {
            string leapCoreAsmDefPath = Path.Combine(Application.dataPath, pathDifference, "LeapMotion", "LeapMotion.asmdef");

            // If the Leap Unity Modules version is 4.7.1, the LeapMotion.asmdef file does not need to be created
            if (currentLeapCoreAssetsVersion == "4.7.1" || currentLeapCoreAssetsVersion == "4.9.1")
            {
                return;
            }

            // If the asmdef has already been created then do not create another one
            if (!File.Exists(leapCoreAsmDefPath))
            {
                // Create the asmdef that will be placed in the Leap Core Assets when they are imported
                // A new asmdef needs to be created in order to reference it in the MRTK/Providers/LeapMotion/Microsoft.MixedReality.Toolkit.Providers.LeapMotion.asmdef file
                AssemblyDefinition leapAsmDef = new AssemblyDefinition
                {
                    Name = "LeapMotion",
                    AllowUnsafeCode = true,
                    References = new string[] { },
                    IncludePlatforms = new string[] { "Editor", "WindowsStandalone32", "WindowsStandalone64" }
                };

                // An assembly definition was added to the Leap Core Assets in version 4.5.1
                // The LeapMotion.LeapCSharp assembly definition is added as a reference at the root of the Core Assets
                if (currentLeapCoreAssetsVersion == "4.5.1" || currentLeapCoreAssetsVersion == "4.6.0" || currentLeapCoreAssetsVersion == "4.7.0")
                {
                    leapAsmDef.AddReference("LeapMotion.LeapCSharp");

                    // If the unity modules version is 4.6.0 or 4.7.0 then add SpatialTracking as a reference
#if UNITY_2019_3_OR_NEWER
                    leapAsmDef.AddReference("UnityEngine.SpatialTracking");
#endif
                }

                leapAsmDef.Save(leapCoreAsmDefPath);
            }
        }

        /// <summary>
        /// Add asmdefs to the editor directories in the leap core assets.
        /// </summary>
        private static void AddLeapEditorAsmDefs()
        {
            if (FileUtilities.FindFilesInAssets("LeapMotion.Core.Editor.asmdef").Length == 0)
            {
                foreach (KeyValuePair<string, string[]> leapAsmDef in leapEditorDirectories)
                {
                    // Convert asmdef name to a path
                    string leapAsmDefPath = leapAsmDef.Key.Replace('.', '/');

                    string leapAsmDefFilename = string.Concat(leapAsmDef.Key, ".asmdef");

                    // Path for the asmdef including the filename
                    string fullLeapAsmDefFilePath = Path.Combine(Application.dataPath, pathDifference, leapAsmDefPath, leapAsmDefFilename);

                    // Path for the asmdef NOT including the filename
                    string fullLeapAsmDefDirectoryPath = Path.Combine(Application.dataPath, pathDifference, leapAsmDefPath);

                    // Make sure the directory exists within the leap core assets before we add the asmdef
                    // The leap core assets version 4.5.0 contains the LeapMotion/Core/Tests/Editor directory while 4.4.0 does not.
                    if (!File.Exists(fullLeapAsmDefFilePath) && Directory.Exists(fullLeapAsmDefDirectoryPath))
                    {
                        // Create and save the new asmdef
                        AssemblyDefinition leapEditorAsmDef = new AssemblyDefinition
                        {
                            Name = leapAsmDef.Key,
                            References = leapAsmDef.Value,
                            IncludePlatforms = new string[] { "Editor" }
                        };

                        // Add the LeapMotion.LeapCSharp assembly definition to the leap motion tests assembly definition
                        if ((currentLeapCoreAssetsVersion == "4.5.1" || currentLeapCoreAssetsVersion == "4.6.0" || currentLeapCoreAssetsVersion == "4.7.0" || currentLeapCoreAssetsVersion == "4.7.1" || currentLeapCoreAssetsVersion == "4.9.1") && (leapAsmDef.Key == "LeapMotion.Core.Tests.Editor" || leapAsmDef.Key == "LeapMotion.Core.Editor"))
                        {
                            leapEditorAsmDef.AddReference("LeapMotion.LeapCSharp");
                        }

#if !UNITY_2019_3_OR_NEWER
                        // In Unity 2018.4, directories that contain tests need to have a test assembly.
                        // An asmdef is added to a leap directory that contains tests for the leap core assets 4.5.0.
                        if (leapEditorAsmDef.Name.Contains("Tests"))
                        {
                            leapEditorAsmDef.OptionalUnityReferences = new string[] { "TestAssemblies" };
                        }
#endif

                        leapEditorAsmDef.Save(fullLeapAsmDefFilePath);
                    }
                }
            }
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
            FileInfo[] leapPathLocationAnchor = FileUtilities.FindFilesInAssets(trackedLeapFileName);
            string leapFilePath = leapPathLocationAnchor[0].FullName;

            List<string> leapPath = leapFilePath.Split(Path.DirectorySeparatorChar).ToList();

            // Remove the last 3 elements of leap path (/Core/Scripts/LeapXRService.cs) from the list to get the root of the leap core assets
            leapPath.RemoveRange(leapPath.Count - 3, 3);

            List<string> unityDataPath = Application.dataPath.Split('/').ToList();
            unityDataPath.Add("LeapMotion");

            // Get the difference between the root of assets and the root of leap core assets
            IEnumerable<string> difference = leapPath.Except(unityDataPath);

            return string.Join("/", difference);
        }

        /// <summary>
        /// Adds warnings to the nowarn line in the csc.rsp file located at the root of assets.  Warning 618 and 649 are added to the nowarn line because if
        /// the MRTK source is from the repo, warnings are converted to errors. Warnings are not converted to errors if the MRTK source is from the unity packages.
        /// Warning 618 and 649 are logged when the Leap Motion Core Assets are imported into the project, 618 is the obsolete warning and 649 is a null on start warning.
        /// </summary>
        /// <remarks>Updating the CSC file was only required for the 4.4.0 Leap Assets and only version 4.5.0 and up is supported moving forward</remarks>        
        [Obsolete("Updating the CSC file was only required for the 4.4.0 Leap Assets and only version 4.5.0 and up is supported moving forward")]
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


        /// <summary>
        /// Integrate MRTK and the Leap Motion Unity Modules if the Leap Motion Unity Modules are in the project. If they are not in the project, display a pop up window.
        /// </summary>
        [MenuItem("Mixed Reality/Toolkit/Utilities/Leap Motion/Integrate Leap Motion Unity Modules")]
        public static void IntegrateLeapMotionWithMRTK()
        {
            // Check if leap unity modules are in the project
            isLeapInProject = ReconcileLeapMotionDefine();

            if (!isLeapInProject)
            {
                EditorUtility.DisplayDialog(
                    "Leap Motion Unity Modules Not Found",
                    "The Leap Motion Unity Modules could not be found in this project, please import the assets into this project. The assets can be found here: " +
                        "https://developer.leapmotion.com/unity",
                    "OK");
            }

            ConfigureLeapMotion(isLeapInProject);
        }

        /// <summary>
        /// Separate MRTK and the Leap Motion Unity Modules and display a prompt for the user to close unity and delete the assets.
        /// </summary>
        [MenuItem("Mixed Reality/Toolkit/Utilities/Leap Motion/Separate Leap Motion Unity Modules")]
        public static void SeparateLeapMotion()
        {

            // Force removal of the Scripting Definitions while the Leap Assets are still in the project
            ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
            ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.WSA, Definitions);

            isLeapRecognizedByMRTK = false;

            // Prompt the user to close unity and delete the assets to completely remove.  Closing unity and deleting the assets is optional.
            EditorUtility.DisplayDialog(
                "MRTK Leap Motion Removal",
                "The Leap Motion Modules are now safe to delete from the project. " +
                "Close Unity, delete the Leap assets in the file explorer, and reopen Unity",
                "OK");

        }

        /// <summary>
        /// Check the integration status of the Leap Motion Assets and display a message to the user.
        /// </summary>
        [MenuItem("Mixed Reality/Toolkit/Utilities/Leap Motion/Check Integration Status")]
        public static void CheckIntegrationStatus()
        {
            if (isLeapRecognizedByMRTK)
            {
                EditorUtility.DisplayDialog(
                    "Leap Integration Status",
                    "The Leap Motion Unity Modules are recognized by MRTK",
                    "OK");
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Leap Integration Status",
                    "The Leap Motion Unity Modules are currently not recognized by MRTK.  " +
                        "Make sure the assets have been imported into the project and select the Integrate Leap Motion Unity Modules to MRTK menu item.",
                    "OK");
            }
        }
    }
}
