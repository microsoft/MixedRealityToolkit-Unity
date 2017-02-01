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
            string zipPath = dirPath + "/Assets/HoloToolkit/Utilities/Plugins/External.zip";

            if (!string.IsNullOrEmpty(dirPath))
            {
                dirPath = dirPath.Replace("/", "\\");
                zipPath = zipPath.Replace("/", "\\");

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
                    Debug.LogWarning("Sucessfully unpacked External Resources to " + dirPath + "/External");
                }
            }
        }
    }
}
