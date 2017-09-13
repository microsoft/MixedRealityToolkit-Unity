// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace HoloToolkit.Unity
{
    public static class ExternalResourcesHelper
    {
        [MenuItem("HoloToolkit/Configure/Update External Resources", false, 99)]
        public static void UpdateExternalResources()
        {
            string dirPath = Path.GetDirectoryName(Application.dataPath);

            if (!string.IsNullOrEmpty(dirPath))
            {
                dirPath = dirPath.Replace("/", "\\");
                string zipPath = SearchDir(dirPath + "\\Assets");

                if (!string.IsNullOrEmpty(zipPath))
                {
                    string args = string.Format("/C PowerShell Expand-Archive -Path \'{0}\' -DestinationPath \'{1}\'", zipPath, dirPath);

                    try
                    {
                        var processInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = args,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                        };

                        var process = new Process { StartInfo = processInfo };

                        process.Start();
                        process.WaitForExit();
                        process.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }

                    if (Directory.Exists(dirPath + "/External"))
                    {
                        Debug.LogWarning("Successfully unpacked External Resources to " + dirPath + "\\External");
                    }
                }
                else
                {
                    Debug.LogError("Unable to find zip");
                }
            }
        }

        private static string SearchDir(string sDir)
        {
            try
            {
                foreach (string directory in Directory.GetDirectories(sDir))
                {
                    foreach (string file in Directory.GetFiles(directory, "*.zip"))
                    {
                        string ext = Path.GetExtension(file);
                        if (ext.Equals(".zip"))
                        {
                            if (file.Contains("External"))
                            {
                                return file;
                            }
                        }
                    }

                    string results = SearchDir(directory);
                    if (!string.IsNullOrEmpty(results))
                    {
                        return results;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
            return null;
        }
    }
}
