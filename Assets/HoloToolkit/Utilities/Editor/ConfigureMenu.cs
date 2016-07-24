// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Configuration options derived from here: 
    /// https://developer.microsoft.com/en-us/windows/holographic/unity_development_overview#Configuring_a_Unity_project_for_HoloLens
    /// </summary>
    public static class AutoConfigureMenu
    {
        [MenuItem("HoloToolkit/Configure/Show Help", priority = 1)]
        public static void ShowHelp()
        {
            Application.OpenURL("https://developer.microsoft.com/en-us/windows/holographic/unity_development_overview#Configuring_a_Unity_project_for_HoloLens");
        }

        [MenuItem("HoloToolkit/Configure/Apply All HoloLens Settings", priority = 0)]
        public static void ApplyAllSettings()
        {
            var output = new StringBuilder();
            ApplySceneSettingsInternal(output);


            var reload = ApplyProjectSettingsInternal(output);

            EditorUtility.DisplayDialog("Results", output.ToString(), "Ok");

            if (reload)
            {
                PromptForReload();
            }
        }


        private static void ApplySceneSettingsInternal(StringBuilder output)
        {
            //todo if we find more than one camera, hololens will always use MainCamera (should)
            //additional cameras could be used from a RenderTexture, etc. Should we warn?

            output.AppendLine("** Checking Camera Settings");
            Camera[] cameras = new Camera[Camera.allCamerasCount];
            Camera.GetAllCameras(cameras);

            if (Camera.main == null)
            {
                output.AppendLine(@"Could not apply camera settings - no camera tagged with ""MainCamera""");
                return;
            }

            SetCameraDefaults(Camera.main, true, output);

            //All done with MainCamera
            cameras = cameras.Where(o => o != Camera.main).ToArray();

            //Check remaining cameras
            if (cameras.Length > 0)
            {
                foreach (var camera in cameras)
                {
                    Debug.Log(camera);
                    var cameraDetails = string.Format("Update this camera for HoloLens defaults? \r\nThis will set position to 0,0,0 (on MainCamera only) and background color to Clear Flags:Solid color(Black). \r\n \r\n---------------\r\nName: {0} \r\nPosition: {1}\r\nTag: {2}\r\n---------------",
                        camera.name, camera.transform.position, camera.tag);
                    if (EditorUtility.DisplayDialog("Multiple Cameras Detected",
    cameraDetails, "Yes", "No"))
                    {
                        //It's not main so we won't touch posiution
                        SetCameraDefaults(camera, false, output);
                    }
                }
            }
            output.AppendLine();
        }

        /// <summary>
        /// Modified camera settings to the HoloLens recommended defaults
        /// </summary>
        /// <param name="camera">Camera to update</param>
        /// <param name="output">Output log</param>
        private static void SetCameraDefaults(Camera camera, bool isMain, StringBuilder output)
        {
            output.AppendLine(string.Format("     Checking camera {0} at {1}", camera.name, camera.transform.position.ToString()));

            if (isMain)
            {
                output.Append("     Checking position:");
                if (camera.transform.position != Vector3.zero)
                {
                    camera.transform.position = Vector3.zero;
                    output.AppendLine("  Updated position");
                }
                else
                {
                    output.AppendLine("  OK");
                }
            }
            else
            {
                output.AppendLine("     Position ignored, not the MainCamera");
            }

            output.Append("     Checking Near Clip Plane is .85:");
            if (camera.nearClipPlane != 0.85f)
            {
                camera.nearClipPlane = 0.85f;
                output.AppendLine("  Updated");
            }
            else
            {
                output.AppendLine("  OK");
            }

            output.Append("     Checking Clear Flags are Black:");
            if (camera.clearFlags != CameraClearFlags.Color)
            {
                camera.clearFlags = CameraClearFlags.Color;
                camera.backgroundColor = Color.black;
                output.AppendLine("  Updated");
            }
            else
            {
                output.AppendLine("  OK");
            }
            output.AppendLine();
        }

        /// <summary>
        /// Applies recommended scene settings to the current scenes
        /// </summary>
        [MenuItem("HoloToolkit/Configure/Apply HoloLens Scene Settings", priority = 0)]
        public static void ApplySceneSettings()
        {
            var output = new StringBuilder();
            ApplySceneSettingsInternal(output);
            EditorUtility.DisplayDialog("Results", output.ToString(), "Ok");
        }

        private static bool ApplyProjectSettingsInternal(StringBuilder output)
        {
            // Build settings
            output.AppendLine("** Checking Build Settings for WSA");
            bool isWsa = EditorUserBuildSettings.activeBuildTarget == BuildTarget.WSAPlayer;
            bool isWsaSdk = EditorUserBuildSettings.wsaSDK == WSASDK.UWP;
            bool isDirect3D = EditorUserBuildSettings.wsaUWPBuildType == WSAUWPBuildType.D3D;
            bool isUnityCSharpProjs = EditorUserBuildSettings.wsaGenerateReferenceProjects;

            output.Append("     Build Target WSA:");
            if (!isWsa)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WSAPlayer);
                output.AppendLine("  Updated");
            }
            else
            {
                output.AppendLine("  OK");
            }

            output.Append("     Targeting Windows 10:");
            if (!isWsaSdk)
            {
                EditorUserBuildSettings.wsaSDK = WSASDK.UWP;
                output.AppendLine("  Updated");
            }
            else
            {
                output.AppendLine("  OK");
            }

            //Says Windows Phone but this is 'device' default, not phone.
            //The default of 'Local Machine' doesn't for for us, since we're 
            //not developing ON a HoloLens but for a HoloLens
            Debug.Log("wsaBuildAndRunDeployTarget:" + EditorUserBuildSettings.wsaBuildAndRunDeployTarget);
            EditorUserBuildSettings.wsaBuildAndRunDeployTarget = WSABuildAndRunDeployTarget.WindowsPhone;

            output.Append("     Targeting Direct3D (Slight perf over XAML):");
            if (!isDirect3D)
            {
                EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;
                output.AppendLine("  Updated");
            }
            else
            {
                output.AppendLine("  OK");
            }
            output.Append("     Generate Unity C# Projects:");

            if (!isUnityCSharpProjs)
            {
                EditorUserBuildSettings.wsaGenerateReferenceProjects = true;
                output.AppendLine("  Updated");
            }
            else
            {
                output.AppendLine("  OK");
            }

            output.AppendLine();

            // See the blow notes for why text asset serialization is required
            if (EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                // NOTE: PlayerSettings.virtualRealitySupported would be ideal, except that it only reports/affects whatever platform tab
                // is currently selected in the Player settings window. As we don't have code control over what view is selected there
                // this property is fairly useless from script.

                // NOTE: There is no current way to change the default quality setting from script

                string title = "Updates require text serialization of assets";
                string message = "Unity doesn't provide apis for updating the default quality.\n\n" +
                    "Is it ok if we force text serialization of assets so that we can modify the properties of the 'Project Settings/Quality' directly? Text serialization is typically better for source control as well, enabling easier diffs between versions.";

                bool forceText = EditorUtility.DisplayDialog(title, message, "Yes", "No");
                if (!forceText)
                {
                    output.AppendLine("Project settings cancelled.");
                    return false;
                }

                output.Append("** Checking Editor Serialization Mode:");
                if (EditorSettings.serializationMode != SerializationMode.ForceText)
                {
                    EditorSettings.serializationMode = SerializationMode.ForceText;
                    output.AppendLine("  Updated");
                }
                else
                {
                    output.AppendLine("  OK");
                }
                output.AppendLine();
            }

            var reload = SetFastestDefaultQuality(output);

            //pick up the reload flag here if we need it too
            reload |= EnableVirtualReality(output);

            return reload;

        }
        /// <summary>
        /// Applies recommended project settings to the current project
        /// </summary>
        [MenuItem("HoloToolkit/Configure/Apply HoloLens Project Settings", priority = 0)]
        public static void ApplyProjectSettings()
        {
            var output = new StringBuilder();

            var reload = ApplyProjectSettingsInternal(output);

            EditorUtility.DisplayDialog("Results", output.ToString(), "Ok");

            if (reload)
            {
                PromptForReload();
            }
        }

        private static void PromptForReload()
        {
            // Since we went behind Unity's back to tweak some settings we 
            // need to reload the project to have them take effect
            bool canReload = EditorUtility.DisplayDialog(
                "Project reload required!",
                "The default quality level change and/or the Windows Store VR change requires a project reload to take effect.\n\nReload now?",
                "Yes", "No");

            if (canReload)
            {
                EditorApplication.OpenProject(Application.dataPath + "/..");
            }
        }

        /// <summary>
        /// Modifies the WSA default quality setting to the fastest.
        /// </summary>
        /// <param name="output"></param>
        /// <returns>True is the project requires a reload because of quality settings change. False if the project quality level is already set to 'Fastest' </returns>
        private static bool SetFastestDefaultQuality(StringBuilder output)
        {
            try
            {
                output.Append("** Checking Quality Settings @ 'Fastest':");
                // Find the WSA element under the platform quality list and replace it's value with 0
                string settingsPath = "ProjectSettings/QualitySettings.asset";
                string matchPatternFastest = "m_PerPlatformDefaultQuality.*Windows Store Apps: 0";
                string matchPattern = @"(m_PerPlatformDefaultQuality.*Windows Store Apps:) (\d+)";
                string replacePattern = @"$1 0";

                string settings = File.ReadAllText(settingsPath);

                //If quality level not already set to "Windows Store Apps: 0"
                //We use SingleLine mode to make . match any char including newlines
                var match = Regex.Match(settings, matchPatternFastest, RegexOptions.Singleline);

                if (!match.Success)
                {
                    //Write updated settings
                    settings = Regex.Replace(settings, matchPattern, replacePattern, RegexOptions.Singleline);
                    File.WriteAllText(settingsPath, settings);
                    output.AppendLine("  Updated");

                    //Project reload required
                    return true;
                }
                else
                {
                    output.AppendLine("  OK");
                }
                output.AppendLine();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            //No reload required
            return false;
        }
    
        /// <summary>
        /// Enables virtual reality for WSA and ensures HoloLens is in the supported SDKs.
        /// </summary>
        private static bool EnableVirtualReality(StringBuilder output)
        {
            output.AppendLine("** Checking VR Settings for WSA");
            //bool vrSupported = PlayerSettings.virtualRealitySupported = true;
            //Warning, not public yet. May change. Above API setting will work if we assume
            //WSA platform is active build target, but if they don't have WSA players installed
            //then this be an installed option. Not a problem in HoloLens Technical Preview
            //but could be if run on any other version or in the future.
            bool vrSupported = UnityEditorInternal.VR.VREditor.GetVREnabled(BuildTargetGroup.WSA);

            output.Append("     VR Enabled: ");
            //enable VR
            if (!vrSupported)
            {
                //PlayerSettings.virtualRealitySupported = true;
                UnityEditorInternal.VR.VREditor.SetVREnabled(BuildTargetGroup.WSA, true);

                output.AppendLine("  Updated");
            }
            else
            {
                output.AppendLine("  OK");
            }

            //Check if Windows Holographic is enabled for WSA
            bool windowsHolographic = false;

            var vrDevices = UnityEditorInternal.VR.VREditor.GetVREnabledDevices(BuildTargetGroup.WSA);
            foreach (var device in vrDevices)
            {
                //Debug.Log(device);
                if (device == "HoloLens")
                {
                    windowsHolographic = true;
                }
            }

            if (!windowsHolographic)
            {
                //Cover the case of someone accidentally deleting Windows Holographic
                Debug.Log("Enumerating installed VR Device Info (HoloLens ie Windows Holographic wasn't found under Virtual Reality SDKs)");
                bool vrHoloLensFound = false;

                //Check to see if we have Windows HoloGraphic installed (keyname is HoloLens)
                foreach (var item in UnityEditorInternal.VR.VREditor.GetAllVRDeviceInfo(BuildTargetGroup.WSA))
                {
                    Debug.Log(string.Format("deviceNameKey:{0} deviceNameUI:{1}", item.deviceNameKey, item.deviceNameUI));
                    Debug.Log(item);
                    if (item.deviceNameKey == "HoloLens")
                    {
                        vrHoloLensFound = true;
                    }
                }

                if (vrHoloLensFound)
                {
                    Debug.Log("Setting enabled VR device to HoloLens");
                    //Yep it's installed, for some reason was removed or not added to list.
                    UnityEditorInternal.VR.VREditor.SetVREnabledDevices(BuildTargetGroup.WSA, new string[] { "HoloLens" });

                    //This change isn't picked up immediately in the UI, reload project to be sure.
                    return true;
                }
                else
                {
                    var noWindowsHolographic = "'Windows Holographic' (HoloLens) was not found as a VR type in the Windows 10 Player settings. This should be listed there, ensure you are using a supported version of Unity for the HoloLens. This process will continue, but when it's done verify in your Player Settings (File-Build Settings, click on Windows Store, ensure Windows 10 is selected in the dropdown, and click Player Settings. Then on the right in 'Other Settings' ensure VR Supported is checked off and Windows Holographic shows up in the options. If not either the SDK name has changed or you possibly have a version of Unity that doesn't support the HoloLens.";
                    output.AppendLine(noWindowsHolographic);
                }
            }
            else
            {
                output.AppendLine("     Windows Holographic SDK Present: OK");
            }
            output.AppendLine();
            return false;
        }

    }
}