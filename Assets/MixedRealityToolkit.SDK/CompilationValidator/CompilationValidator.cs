using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [InitializeOnLoad]
    public class CompilationValidator
    {
        private const string warningTitle = "Assembly reference missing";
        private const string warningMessage = "Unity 2019.2 and later requires that your UnityAR assembly definition be configured differently. We can attempt to do this automatically. Proceed?";
        private const string unityArAsmdefPath = "Assets/MixedRealityToolkit.Staging/UnityAR/Microsoft.MixedReality.Toolkit.Providers.UnityAR.asmdef";
        private const string spatialTrackingReference = "UnityEngine.SpatialTracking";
        private const string powershellScriptPath = "scripts/support2019/setup_for_2019.ps1";

        static CompilationValidator()
        {
#if UNITY_2019_2_OR_NEWER
            TextAsset unityArAsmdef = AssetDatabase.LoadAssetAtPath<TextAsset>(unityArAsmdefPath);

            if (!unityArAsmdef.text.Contains(spatialTrackingReference))
            {
                if (EditorUtility.DisplayDialog(warningTitle, warningMessage, "OK", "Cancel"))
                {
                    string path = Application.dataPath.Replace("Assets", powershellScriptPath);
                    UnityEngine.Debug.Log("Running script " + path);

                    try
                    {
                        var processInfo = new ProcessStartInfo("Powershell.exe", "-executionpolicy bypass -file " + path);
                        processInfo.UseShellExecute = false;
                        processInfo.CreateNoWindow = false;
                        processInfo.WindowStyle = ProcessWindowStyle.Normal;

                        var process = Process.Start(processInfo);

                        process.WaitForExit();
                        process.Close();
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError("Unable to run script " + powershellScriptPath + ": " + e);
                        EditorGUIUtility.PingObject(unityArAsmdef);
                    }
                }
                else
                {
                    EditorGUIUtility.PingObject(unityArAsmdef);
                }
            }
#endif
        }
    }
}
