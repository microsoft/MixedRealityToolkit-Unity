// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Renders the UI and handles update logic for HoloToolkit/Configure/Apply Mixed Reality Project Settings.
    /// </summary>
    public class ProjectSettingsWindow : AutoConfigureWindow<ProjectSettingsWindow.ProjectSetting>
    {
        private const int SpatialMappingLayerId = 31;
        private const string SpatialMappingLayerName = "Spatial Mapping";

        private const string SharingServiceURL = "https://raw.githubusercontent.com/Microsoft/MixedRealityToolkit-Unity/master/External/HoloToolkit/Sharing/Server/SharingService.exe";

        /// <summary>
        /// This is used to keep a local list of axis names, so we don't have to keep iterating through each SerializedProperty.
        /// </summary>
        private List<string> axisNames = new List<string>();

        /// <summary>
        /// This is used to keep a single reference to InputManager.asset, refreshed when necessary.
        /// </summary>
        private SerializedObject inputManagerAsset;

        /// <summary>
        /// Define new axes here adding a new InputManagerAxis to the array.
        /// </summary>
        private readonly InputManagerAxis[] newInputAxes =
        {
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_STICK_HORIZONTAL,  Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_STICK_VERTICAL,    Dead = 0.19f, Sensitivity = 1, Invert = true,  Type = AxisType.JoystickAxis, Axis = 2 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_SHARED_TRIGGER,               Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 3 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_HORIZONTAL, Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 4 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_VERTICAL,   Dead = 0.19f, Sensitivity = 1, Invert = true,  Type = AxisType.JoystickAxis, Axis = 5 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_DPAD_HORIZONTAL,              Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 6 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_DPAD_VERTICAL,                Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 7 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_TRIGGER,           Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 9 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_TRIGGER,          Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 10 },

            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_A,                          PositiveButton = "joystick button 0", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_B,                          PositiveButton = "joystick button 1", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_X,                          PositiveButton = "joystick button 2", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_Y,                          PositiveButton = "joystick button 3", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_BUMPER_OR_GRIP,  PositiveButton = "joystick button 4", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_BUMPER_OR_GRIP, PositiveButton = "joystick button 5", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_MENU,            PositiveButton = "joystick button 6", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_MENU,           PositiveButton = "joystick button 7", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_STICK_CLICK,     PositiveButton = "joystick button 8", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_CLICK,    PositiveButton = "joystick button 9", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
        };

        /// <summary>
        /// As axes in newInputAxes are removed or renamed, move them here for proper clean-up in user projects.
        /// </summary>
        private readonly InputManagerAxis[] obsoleteInputAxes = { };

        #region Nested Types

        public enum ProjectSetting
        {
            BuildWsaUwp = 0,
            WsaEnableXR,
            WsaUwpBuildToD3D,
            TargetOccludedDevices,
            SharingServices,
            UseInputManagerAxes,
            DotNetScriptingBackend,
            SetDefaultSpatialMappingLayer
        }

        /// <summary>
        /// Used to map AxisType from a useful name to the int value the InputManager wants.
        /// </summary>
        private enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement,
            JoystickAxis
        };

        /// <summary>
        /// Used to define an entire InputManagerAxis, with each variable defined by the same term the Inspector shows.
        /// </summary>
        private class InputManagerAxis
        {
            public string Name = "";
            public string DescriptiveName = "";
            public string DescriptiveNegativeName = "";
            public string NegativeButton = "";
            public string PositiveButton = "";
            public string AltNegativeButton = "";
            public string AltPositiveButton = "";
            public float Gravity = 0.0f;
            public float Dead = 0.0f;
            public float Sensitivity = 0.0f;
            public bool Snap = false;
            public bool Invert = false;
            public AxisType Type = default(AxisType);
            public int Axis = 0;
            public int JoyNum = 0;
        }

        #endregion // Nested Types

        #region Overrides / Event Handlers

        protected override void ApplySettings()
        {
            // Apply individual settings
            if (Values[ProjectSetting.BuildWsaUwp])
            {
                if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WSAPlayer)
                {
#if UNITY_2017_1_OR_NEWER
                    EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
#else
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
#endif
                }
                else
                {
                    UpdateSettings(EditorUserBuildSettings.activeBuildTarget);
                }
            }
            else
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            }
        }

        protected override void LoadSettings()
        {
            for (int i = (int)ProjectSetting.BuildWsaUwp; i <= (int)ProjectSetting.SetDefaultSpatialMappingLayer; i++)
            {
                switch ((ProjectSetting)i)
                {
                    case ProjectSetting.BuildWsaUwp:
                    case ProjectSetting.WsaEnableXR:
                    case ProjectSetting.WsaUwpBuildToD3D:
                    case ProjectSetting.DotNetScriptingBackend:
                    case ProjectSetting.SetDefaultSpatialMappingLayer:
                        Values[(ProjectSetting)i] = true;
                        break;
                    case ProjectSetting.TargetOccludedDevices:
                        Values[(ProjectSetting)i] = EditorPrefsUtility.GetEditorPref(Names[(ProjectSetting)i], false);
                        break;
                    case ProjectSetting.SharingServices:
                        Values[(ProjectSetting)i] = EditorPrefsUtility.GetEditorPref(Names[(ProjectSetting)i], false);
                        break;
                    case ProjectSetting.UseInputManagerAxes:
                        Values[(ProjectSetting)i] = EditorPrefsUtility.GetEditorPref(Names[(ProjectSetting)i], false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

        private void UpdateSettings(BuildTarget currentBuildTarget)
        {
            EditorPrefsUtility.SetEditorPref(Names[ProjectSetting.SharingServices], Values[ProjectSetting.SharingServices]);
            if (Values[ProjectSetting.SharingServices])
            {
                string sharingServiceDirectory = Directory.GetParent(Path.GetFullPath(Application.dataPath)).FullName + "\\External\\HoloToolkit\\Sharing\\Server";
                string sharingServicePath = sharingServiceDirectory + "\\SharingService.exe";
                if (!File.Exists(sharingServicePath) &&
                    EditorUtility.DisplayDialog("Attention!",
                        "You're missing the Sharing Service Executable in your project.\n\n" +
                        "Would you like to download the missing files from GitHub?\n\n" +
                        "Alternatively, you can download it yourself or specify a target IP to connect to at runtime on the Sharing Stage.",
                        "Yes", "Cancel"))
                {
                    using (var webRequest = UnityWebRequest.Get(SharingServiceURL))
                    {
#if UNITY_2017_2_OR_NEWER
                        webRequest.SendWebRequest();
#else
                        webRequest.Send();
#endif
                        while (!webRequest.isDone)
                        {
                            if (webRequest.downloadProgress > -1)
                            {
                                EditorUtility.DisplayProgressBar(
                                    "Downloading the SharingService executable from GitHub",
                                    "Progress...", webRequest.downloadProgress);
                            }
                        }

                        EditorUtility.ClearProgressBar();

#if UNITY_2017_1_OR_NEWER
                        if (webRequest.isNetworkError || webRequest.isHttpError)
#else
                        if (webRequest.isError)
#endif
                        {
                            Debug.LogError("Network Error: " + webRequest.error);
                        }
                        else
                        {
                            byte[] sharingServiceData = webRequest.downloadHandler.data;
                            Directory.CreateDirectory(sharingServiceDirectory);
                            File.WriteAllBytes(sharingServicePath, sharingServiceData);
                        }
                    }
                }
                else
                {
                    Debug.LogFormat("Alternatively, you can download from this link: {0}", SharingServiceURL);
                }

                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.PrivateNetworkClientServer, true);
            }
            else
            {
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, false);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, false);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.PrivateNetworkClientServer, false);
            }

            bool useToolkitAxes = Values[ProjectSetting.UseInputManagerAxes];

            if (useToolkitAxes != EditorPrefsUtility.GetEditorPref(Names[ProjectSetting.UseInputManagerAxes], false))
            {
                EditorPrefsUtility.SetEditorPref(Names[ProjectSetting.UseInputManagerAxes], useToolkitAxes);

                // Grabs the actual asset file into a SerializedObject, so we can iterate through it and edit it.
                inputManagerAsset = new SerializedObject(AssetDatabase.LoadAssetAtPath("ProjectSettings/InputManager.asset", typeof(UnityEngine.Object)));

                if (useToolkitAxes)
                {
                    foreach (InputManagerAxis axis in newInputAxes)
                    {
                        if (!DoesAxisNameExist(axis.Name))
                        {
                            AddAxis(axis);
                        }
                    }

                }
                else
                {
                    foreach (InputManagerAxis axis in newInputAxes)
                    {
                        if (DoesAxisNameExist(axis.Name))
                        {
                            RemoveAxis(axis.Name);
                        }
                    }

                    foreach (InputManagerAxis axis in obsoleteInputAxes)
                    {
                        if (DoesAxisNameExist(axis.Name))
                        {
                            RemoveAxis(axis.Name);
                        }
                    }
                }

                inputManagerAsset.ApplyModifiedProperties();
            }

            if (currentBuildTarget != BuildTarget.WSAPlayer)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                Close();
                return;
            }

            EditorUserBuildSettings.wsaUWPBuildType = Values[ProjectSetting.WsaUwpBuildToD3D]
                ? WSAUWPBuildType.D3D
                : WSAUWPBuildType.XAML;

            UnityEditorInternal.VR.VREditor.SetVREnabledOnTargetGroup(BuildTargetGroup.WSA, Values[ProjectSetting.WsaEnableXR]);

            if (!Values[ProjectSetting.WsaEnableXR])
            {
                EditorUserBuildSettings.wsaSubtarget = WSASubtarget.AnyDevice;
                UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.WSA, new[] { "None" });
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.HumanInterfaceDevice, false);
                BuildDeployPrefs.BuildPlatform = "Any CPU";
            }
            else
            {
#if !UNITY_2017_2_OR_NEWER
                Values[ProjectSetting.TargetOccludedDevices] = false;
#endif
                if (!Values[ProjectSetting.TargetOccludedDevices])
                {
                    EditorUserBuildSettings.wsaSubtarget = WSASubtarget.HoloLens;
#if UNITY_2017_2_OR_NEWER
                    UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.WSA, new[] { "WindowsMR" });
#else
                    UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.WSA, new[] { "HoloLens" });
#endif
                    PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.HumanInterfaceDevice, Values[ProjectSetting.UseInputManagerAxes]);
                    BuildDeployPrefs.BuildPlatform = "x86";

                    for (var i = 0; i < QualitySettings.names.Length; i++)
                    {
                        QualitySettings.DecreaseLevel(true);
                    }
                }
                else
                {
                    EditorUserBuildSettings.wsaSubtarget = WSASubtarget.PC;
                    UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.WSA, new[] { "WindowsMR" });
                    PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.HumanInterfaceDevice, false);
                    BuildDeployPrefs.BuildPlatform = "x64";

                    for (var i = 0; i < QualitySettings.names.Length; i++)
                    {
                        QualitySettings.IncreaseLevel(true);
                    }
                }

                int currentQualityLevel = QualitySettings.GetQualityLevel();

                // HACK: Edits QualitySettings.asset Directly
                // TODO: replace with friendlier version that uses built in APIs when Unity fixes or makes available.
                // See: http://answers.unity3d.com/questions/886160/how-do-i-change-qualitysetting-for-my-platform-fro.html
                try
                {
                    // Find the WSA element under the platform quality list and replace it's value with the current level.
                    string settingsPath = "ProjectSettings/QualitySettings.asset";
                    string matchPattern = @"(m_PerPlatformDefaultQuality.*Windows Store Apps:) (\d+)";
                    string replacePattern = @"$1 " + currentQualityLevel;

                    string settings = File.ReadAllText(settingsPath);
                    settings = Regex.Replace(settings, matchPattern, replacePattern, RegexOptions.Singleline);

                    File.WriteAllText(settingsPath, settings);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            EditorPrefsUtility.SetEditorPref(Names[ProjectSetting.TargetOccludedDevices], Values[ProjectSetting.TargetOccludedDevices]);

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.WSA,
                Values[ProjectSetting.DotNetScriptingBackend]
                    ? ScriptingImplementation.WinRTDotNET
                    : ScriptingImplementation.IL2CPP);

            if (Values[ProjectSetting.SetDefaultSpatialMappingLayer])
            {
                if (SetSpatialMappingLayer())
                {
                    // Setting the Spatial Mapping layer implies the need for the Spatial Perception capability.
                    PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, true);
                }
                else
                {
                    EditorUtility.DisplayDialog("Attention!",
                        "Unable to set the Spatial Mapping layer.\n\n" +
                        "This likely means that layer " + SpatialMappingLayerId.ToString() + " is already in use.\n\n" +
                        "Please check your project's Tags && Layers settings in the Inspector.",
                        "Ok");
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Close();
        }

        protected override void OnGuiChanged()
        {
        }

        protected override void LoadStrings()
        {
            Names[ProjectSetting.BuildWsaUwp] = "Target Windows Universal UWP";
            Descriptions[ProjectSetting.BuildWsaUwp] =
                "<b>Required</b>\n\n" +
                "Switches the currently active target to produce a Store app targeting the Universal Windows Platform.\n\n" +
                "<color=#ffff00ff><b>Note:</b></color> Cross platform development can be done with this toolkit, but many features and " +
                "tools will not work if the build target is not Windows Universal.";

            Names[ProjectSetting.WsaEnableXR] = "Enable XR";
            Descriptions[ProjectSetting.WsaEnableXR] =
                "<b>Required</b>\n\n" +
                "Enables 'Windows Holographic' for Windows Store apps.\n\n" +
                "If disabled, your application will run as a normal UWP app on PC, and will launch as a 2D app on HoloLens.\n\n" +
                "<color=#ff0000ff><b>Warning!</b></color> HoloLens and tools like 'Holographic Remoting' will not function without this enabled.";

            Names[ProjectSetting.WsaUwpBuildToD3D] = "Build for Direct3D";
            Descriptions[ProjectSetting.WsaUwpBuildToD3D] =
                "Recommended\n\n" +
                "Produces an app that targets Direct3D instead of XAML.\n\n" +
                "Pure Direct3D apps run faster than applications that include XAML. This option should remain checked unless you plan to " +
                "overlay Unity content with XAML content or you plan to switch between Unity views and XAML views at runtime.";

            Names[ProjectSetting.TargetOccludedDevices] = "Target Occluded Devices";
            Descriptions[ProjectSetting.TargetOccludedDevices] =
                "Changes the target Device and updates the default quality settings, if needed. Occluded devices are generally VR hardware (like the Acer HMD) " +
                "that do not have a 'see through' display, while transparent devices (like the HoloLens) are generally AR hardware where users can see " +
                "and interact with digital elements in the physical world around them.\n\n" +
#if !UNITY_2017_2_OR_NEWER
                "<color=#ff0000ff><b>Warning!</b></color> Occluded Devices are only supported in Unity 2017.2 and newer and cannot be enabled.\n\n" +
#endif
                "<color=#ffff00ff><b>Note:</b></color> If you're not targeting Occluded devices, It's generally recommended that Transparent devices use " +
                "the lowest default quality setting, and is set automatically for you. This can be manually changed in your the Project's Quality Settings.";

            Names[ProjectSetting.SharingServices] = "Enable Sharing Services";
            Descriptions[ProjectSetting.SharingServices] =
                "Enables the use of the Sharing Services in your project for all apps on any platform.\n\n" +
                "<color=#ffff00ff><b>Note:</b></color> Start the Sharing Server via 'Mixed Reality Toolkit/Sharing Service/Launch Sharing Service'.\n\n" +
                "<color=#ffff00ff><b>Note:</b></color> The InternetClientServer and PrivateNetworkClientServer capabilities will be enabled in the " +
                "appx manifest for you.";

            Names[ProjectSetting.UseInputManagerAxes] = "Use Toolkit-specific InputManager axes";
            Descriptions[ProjectSetting.UseInputManagerAxes] =
                "Enables the use of the Xbox Controller for all apps on any platform.\n\n" +
                "To remove the added axes, simply disable this setting.\n\n" +
                "<color=#ffff00ff><b>Note:</b></color> The HoloLens platform target requires the HID capability to be defined in the appx manifest. " +
                "This capability is automatically enabled for you if you select this setting, \"Enable XR\", and don't select \"Target Occluded Devices\".";

            Names[ProjectSetting.DotNetScriptingBackend] = "Enable .NET scripting backend";
            Descriptions[ProjectSetting.DotNetScriptingBackend] =
                "Recommended\n\n" +
                "If you have the .NET unity module installed this will update the backend scripting profile, otherwise the scripting backend will be IL2CPP.";

            Names[ProjectSetting.SetDefaultSpatialMappingLayer] = "Set Default Spatial Mapping Layer";
            Descriptions[ProjectSetting.SetDefaultSpatialMappingLayer] =
                "Recommended\n\n" +
                "Sets the default Spatial Mapping physics layer.\n\n" +
                "On HoloLens, this enables specifying the Spatial Mapping mesh for collision detection and raycasting.\n\n" +
                "<color=#ffff00ff><b>Note:</b></color> Selecting \"Set Default Spatial Mapping Layer\" implies the project will be using Spatial Mapping. " +
                "The Spatial Perception capability is automatically enabled for you.";
        }

        protected override void OnEnable()
        {
            base.OnEnable();

#if UNITY_2017_1_OR_NEWER
            AutoConfigureMenu.ActiveBuildTargetChanged += UpdateSettings;
#endif

            minSize = new Vector2(350, 350);
            maxSize = minSize;
        }

        private void OnDisable()
        {
            if (inputManagerAsset != null)
            {
                inputManagerAsset.Dispose();
            }
        }

        #endregion // Overrides / Event Handlers

        private void AddAxis(InputManagerAxis axis)
        {
            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            // Creates a new axis by incrementing the size of the m_Axes array.
            axesProperty.arraySize++;

            // Get the new axis be querying for the last array element.
            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            // Iterate through all the properties of the new axis.
            while (axisProperty.Next(true))
            {
                switch (axisProperty.name)
                {
                    case "m_Name":
                        axisProperty.stringValue = axis.Name;
                        break;
                    case "descriptiveName":
                        axisProperty.stringValue = axis.DescriptiveName;
                        break;
                    case "descriptiveNegativeName":
                        axisProperty.stringValue = axis.DescriptiveNegativeName;
                        break;
                    case "negativeButton":
                        axisProperty.stringValue = axis.NegativeButton;
                        break;
                    case "positiveButton":
                        axisProperty.stringValue = axis.PositiveButton;
                        break;
                    case "altNegativeButton":
                        axisProperty.stringValue = axis.AltNegativeButton;
                        break;
                    case "altPositiveButton":
                        axisProperty.stringValue = axis.AltPositiveButton;
                        break;
                    case "gravity":
                        axisProperty.floatValue = axis.Gravity;
                        break;
                    case "dead":
                        axisProperty.floatValue = axis.Dead;
                        break;
                    case "sensitivity":
                        axisProperty.floatValue = axis.Sensitivity;
                        break;
                    case "snap":
                        axisProperty.boolValue = axis.Snap;
                        break;
                    case "invert":
                        axisProperty.boolValue = axis.Invert;
                        break;
                    case "type":
                        axisProperty.intValue = (int)axis.Type;
                        break;
                    case "axis":
                        axisProperty.intValue = axis.Axis - 1;
                        break;
                    case "joyNum":
                        axisProperty.intValue = axis.JoyNum;
                        break;
                }
            }
        }

        private void RemoveAxis(string axis)
        {
            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            // This loop accounts for multiple axes with the same name.
            while (axisNames.Contains(axis))
            {
                int index = axisNames.IndexOf(axis);
                axesProperty.DeleteArrayElementAtIndex(index);
                axisNames.RemoveAt(index);
            }
        }

        /// <summary>
        /// Checks our local cache of axis names to see if an axis exists. This cache is refreshed if it's empty or if InputManager.asset has been changed.
        /// </summary>
        private bool DoesAxisNameExist(string axisName)
        {
            if (axisNames.Count == 0 || inputManagerAsset.UpdateIfRequiredOrScript())
            {
                RefreshLocalAxesList();
            }

            return axisNames.Contains(axisName);
        }

        /// <summary>
        /// Clears our local cache, then refills it by iterating through the m_Axes arrays and storing the display names.
        /// </summary>
        private void RefreshLocalAxesList()
        {
            axisNames.Clear();

            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                axisNames.Add(axesProperty.GetArrayElementAtIndex(i).displayName);
            }
        }

        /// <summary>
        /// Attempts to set or clear the Spatial Mapping physics layer.
        /// </summary>
        /// <returns>
        /// True if the target layer as successfully changed.
        /// </returns>
        private bool SetSpatialMappingLayer()
        {
            UnityEngine.Object[] tagAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if ((tagAssets == null) || 
                (tagAssets.Length == 0))
            {
                return false;
            }

            SerializedObject tagsManager = new SerializedObject(tagAssets);
            if (tagsManager == null)
            {
                return false;
            }

            SerializedProperty layers = tagsManager.FindProperty("layers");
            if (layers == null)
            {
                return false;
            }

            SerializedProperty spatialMappingLayer = layers.GetArrayElementAtIndex(SpatialMappingLayerId);
            if (spatialMappingLayer.stringValue == SpatialMappingLayerName)
            {
                // Spatial Mapping layer already set
                return true;
            }
            else if (spatialMappingLayer.stringValue != string.Empty)
            {
                // Target layer in use and may be being used for something other than Spatial Mapping
                return false;
            }

            // Set the layer name.
            spatialMappingLayer.stringValue = SpatialMappingLayerName;
            return tagsManager.ApplyModifiedProperties();
        }
    }
}
