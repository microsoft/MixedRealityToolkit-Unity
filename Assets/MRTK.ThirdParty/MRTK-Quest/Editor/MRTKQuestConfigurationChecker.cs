using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace prvncher.MixedReality.Toolkit.OculusQuestInput
{
    /// <summary>
    /// Class that checks if the Oculus Integration Assets are present and configures the project if they are.
    /// </summary>
    [InitializeOnLoad]
    static class OculusConfigurationChecker
    {
        // The presence of the OculusProjectConfig.asset is used to determine if the Oculus Integration Assets are in the project.
        private const string OculusIntegrationProjectConfig = "OculusProjectConfig.asset";
        private static readonly string[] Definitions = { "OCULUSINTEGRATION_PRESENT" };

        static OculusConfigurationChecker()
        {
            // Check if Oculus Integration Package is in the project
            ReconcileOculusIntegrationDefine();
        }

        //seems to work fine as is
        private static bool ReconcileOculusIntegrationDefine()
        {
            FileInfo[] files = FindFilesInAssets(OculusIntegrationProjectConfig);

            if (files.Length > 0)
            {
                AppendScriptingDefinitions(BuildTargetGroup.Android, Definitions);
                AppendScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
                return true;
            }
            else
            {
                RemoveScriptingDefinitions(BuildTargetGroup.Android, Definitions);
                RemoveScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
                return false;
            }
        }

        /// <summary>
        /// Configures the project profiles to support handtracking
        /// </summary>
        [MenuItem("Mixed Reality Toolkit/Utilities/Oculus/Configure Oculus Profiles for Handtracking")]
        static void ConfigureProfilesForHandtracking()
        {
            FileInfo[] files = FindFilesInAssets(OculusIntegrationProjectConfig);

            // Make this set the application to controllers and hands on first setup
#if OCULUSINTEGRATION_PRESENT
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

            //Debug.Log($"Saving {cscFilePath}");
        }


        /// <summary>
        /// Locates the files that match the specified name within the Assets folder structure.
        /// </summary>
        /// <param name="fileName">The name of the file to locate (ex: "TestFile.asmdef")</param>
        /// <returns>Array of FileInfo objects representing the located file(s).</returns>
        public static FileInfo[] FindFilesInAssets(string fileName)
        {
            // FindAssets doesn't take a file extension
            string[] assetGuids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(fileName));

            List<FileInfo> fileInfos = new List<FileInfo>();
            for (int i = 0; i < assetGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                // Since this is an asset search without extension, some filenames may contain parts of other filenames.
                // Therefore, double check that the path actually contains the filename with extension.
                if (assetPath.Contains(fileName))
                {
                    fileInfos.Add(new FileInfo(assetPath));
                }
            }

            return fileInfos.ToArray();
        }

        /// <summary>
        /// Appends a set of symbolic constant definitions to Unity's Scripting Define Symbols for the
        /// specified build target group.
        /// </summary>
        /// <param name="targetGroup">The build target group for which the symbols are to be defined.</param>
        /// <param name="symbols">Array of symbols to define.</param>
        public static void AppendScriptingDefinitions(
            BuildTargetGroup targetGroup,
            string[] symbols)
        {
            if (symbols == null || symbols.Length == 0) { return; }

            List<string> toAdd = new List<string>(symbols);
            List<string> defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';'));

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defines.Union(toAdd).ToArray()));
        }

        /// <summary>
        /// Removes a set of symbolic constant definitions to Unity's Scripting Define Symbols from the
        /// specified build target group.
        /// </summary>
        /// <param name="targetGroup">The build target group for which the symbols are to be removed.</param>
        /// <param name="symbols">Array of symbols to remove.</param>
        public static void RemoveScriptingDefinitions(
            BuildTargetGroup targetGroup,
            string[] symbols)
        {
            if (symbols == null || symbols.Length == 0) { return; }

            List<string> toRemove = new List<string>(symbols);
            List<string> defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';'));

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defines.Except(toRemove).ToArray()));
        }
    }
}