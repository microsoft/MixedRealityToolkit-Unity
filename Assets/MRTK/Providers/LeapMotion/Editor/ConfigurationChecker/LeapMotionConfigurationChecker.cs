// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
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
    [InitializeOnLoad]
    static class LeapMotionConfigurationChecker
    {
        private const string FileName = "README_BEFORE_UPDATING.txt";
        private static readonly string[] definitions = { "LEAPMOTIONCORE_PRESENT" };
        private static bool isLeapInProject = false;

        static LeapMotionConfigurationChecker()
        {
            // Check if leap core is in the project
            isLeapInProject = ReconcileLeapMotionDefine();

            if (isLeapInProject)
            {
                RemoveTestingFolders();
                AddAndUpdateAsmDefs();
                AddLeapEditorAsmDefs();
            }
        }

        /// <summary>
        /// Ensures that the appropriate symbolic constant is defined based on the presence of the Leap Motion Core Assets.
        /// </summary>
        /// <returns>True if the define was added, false otherwise.</returns>
        private static bool ReconcileLeapMotionDefine()
        {
            FileInfo[] files = FileUtilities.FindFilesInAssets(FileName);

            if (files.Length > 0)
            {
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Standalone, definitions);
                return true;
            }
            else
            {
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Standalone, definitions);
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
            string[] pathsToDelete = new string[]
            {
                "LeapMotion/Core/Editor/Tests/",
                "LeapMotion/Core/Scripts/Algorithms/Editor/Tests/",
                "LeapMotion/Core/Scripts/DataStructures/Editor/Tests/",
                "LeapMotion/Core/Scripts/Encoding/Editor/",
                "LeapMotion/Core/Query/Editor/",
                "LeapMotion/Core/Scripts/Utils/Editor/BitConverterNonAllocTests.cs",
                "LeapMotion/Core/Scripts/Utils/Editor/ListAndArrayExtensionTests.cs",
                "LeapMotion/Core/Scripts/Utils/Editor/TransformUtilTests.cs",
                "LeapMotion/Core/Scripts/Utils/Editor/UtilsTests.cs",
                "LeapMotion/Core/Plugins/LeapCSharp/Editor/Tests/",
                "LeapMotion/Core/Scripts/Query/Editor/"
            };

            // If one of the leap test directories exists then the rest have not been deleted
            if (Directory.Exists(Path.Combine(Application.dataPath, pathsToDelete[0])))
            {
                // Find where the leap motion core assets are relative to the assets directory
                string pathDifference = GetPathDifference();

                foreach (string path in pathsToDelete)
                {
                    // What if leap is not imported to the root of assets?
                    string fullPath = Path.Combine(Application.dataPath, pathDifference, path);

                    // If we are deleting a specific file, then we also need to remove the meta associated with the file
                    if (File.Exists(fullPath) && fullPath.Contains(".cs"))
                    {
                        // Delete the test files
                        FileUtil.DeleteFileOrDirectory(fullPath);

                        // Also delete the meta files
                        FileUtil.DeleteFileOrDirectory(fullPath + ".meta");
                    }

                    if (Directory.Exists(fullPath) && !fullPath.Contains(".cs"))
                    {
                        // Delete the test directories
                        FileUtil.DeleteFileOrDirectory(fullPath);

                        // Delete the test directories meta files
                        FileUtil.DeleteFileOrDirectory(fullPath.TrimEnd('/') + ".meta");
                    }
                }

                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Adds an asmdef at the root of the LeapMotion Core Assets once they are imported into the project and adds the newly created LeapMotion.asmdef
        /// as a reference for the existing leap data provider asmdef.
        /// </summary>
        private static void AddAndUpdateAsmDefs()
        {
            // Find where the leap motion core assets are relative to the assets directory
            string pathDifference = GetPathDifference();

            string leapCoreAsmDefPath = Path.Combine(Application.dataPath, pathDifference, "LeapMotion", "LeapMotion.asmdef");

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
                    IncludePlatforms = new string[] { "Editor", "WindowsStandalone32", "WindowsStandalone64"}
                };

                leapAsmDef.Save(leapCoreAsmDefPath);

                // Get the MRTK/Providers/LeapMotion/Microsoft.MixedReality.Toolkit.Providers.LeapMotion.asmdef
                FileInfo[] leapDataProviderAsmDefFile = FileUtilities.FindFilesInAssets("Microsoft.MixedReality.Toolkit.Providers.LeapMotion.asmdef");

                // Add the newly created LeapMotion.asmdef to the references of the leap data provider asmdef
                AssemblyDefinition leapDataProviderAsmDef = AssemblyDefinition.Load(leapDataProviderAsmDefFile[0].FullName);

                leapDataProviderAsmDef.References = new string[]
                { "Microsoft.MixedReality.Toolkit",
                  "LeapMotion"
                };

                leapDataProviderAsmDef.Save(leapDataProviderAsmDefFile[0].FullName);

                // A new asset (LeapMotion.asmdef) was created, refresh the asset database
                AssetDatabase.Refresh();
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
            FileInfo[] leapPathLocationAnchor = FileUtilities.FindFilesInAssets(FileName);
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
        /// Add asmdefs to the editor directories in the leap core assets.
        /// </summary>
        private static void AddLeapEditorAsmDefs()
        {
            if (FileUtilities.FindFilesInAssets("LeapMotion.Core.Editor.asmdef").Length == 0)
            {
                // Names of all Leap Motion editor directories that need assembly definitions
                // Key: Asmdef name
                // Value: References for the asmdef
                Dictionary<string, string[]> leapEditorDirectories = new Dictionary<string, string[]>
                {
                    { "LeapMotion.Core.Editor", new string[] { "LeapMotion" } },
                    { "LeapMotion.Core.Scripts.Animation.Editor", new string[] { "LeapMotion", "LeapMotion.Core.Editor", "LeapMotion.Core.Scripts.Utils.Editor" } },
                    { "LeapMotion.Core.Scripts.Attachments.Editor", new string[] { "LeapMotion", "LeapMotion.Core.Editor" } },
                    { "LeapMotion.Core.Scripts.Attributes.Editor", new string[] { "LeapMotion" } },
                    { "LeapMotion.Core.Scripts.DataStructures.Editor", new string[] { "LeapMotion" } },
                    { "LeapMotion.Core.Scripts.EditorTools.Editor", new string[] { "LeapMotion", "LeapMotion.Core.Scripts.Utils.Editor" } },
                    { "LeapMotion.Core.Scripts.Utils.Editor", new string[] { "LeapMotion", "LeapMotion.Core.Editor" } },
                    { "LeapMotion.Core.Scripts.XR.Editor", new string[] { "LeapMotion", "LeapMotion.Core.Editor" } }
                };

                string pathDifference = GetPathDifference();

                foreach (KeyValuePair<string, string[]> leapAsmDef in leapEditorDirectories)
                {
                    // Convert asmdef name to a path
                    string leapAsmDefPath = leapAsmDef.Key.Replace('.', '/');

                    string leapAsmDefFilename = string.Concat(leapAsmDef.Key, ".asmdef");

                    string fullLeapAsmDefPath = Path.Combine(Application.dataPath, pathDifference, leapAsmDefPath, leapAsmDefFilename);

                    if (!File.Exists(fullLeapAsmDefPath))
                    {
                        // Create and save the new asmdef
                        AssemblyDefinition leapEditorAsmDef = new AssemblyDefinition
                        {
                            Name = leapAsmDef.Key,
                            References = leapAsmDef.Value,
                            IncludePlatforms = new string[] { "Editor" }
                        };

                        leapEditorAsmDef.Save(fullLeapAsmDefPath);
                    }
                }

                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Update/Configure CSC File for Leap Motion", false, 0)]
        static void UpdateCSC()
        {
            string fileName = Path.Combine(Application.dataPath, "csc.rsp");

            FileInfo file = new FileInfo(fileName);

            bool readOnly = (file.Exists) ? file.IsReadOnly : false;
            if (readOnly)
            {
                file.IsReadOnly = false;
            }

            Debug.Log($"Saving {fileName}");
            using (StreamWriter writer = new StreamWriter(fileName, false))
            {
                writer.WriteLine("-warnaserror+\n-nowarn:1701,1702,618,649");
            }

            if (readOnly)
            {
                file.IsReadOnly = true;
            }
        }
    }
}

