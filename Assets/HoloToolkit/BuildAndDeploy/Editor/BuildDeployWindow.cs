// Copyright (c) Microsoft Corporation. All rights reserved.
// Copyright (c) @jevertt
// Copyright (c) Rafael Rivera
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Build window - supports SLN creation, APPX from SLN, Deploy on device, and misc helper utilities associated with the build/deploy/test iteration loop
    /// Requires the device to be set in developer mode & to have secure connections disabled (in the security tab in the device portal)
    /// </summary>
    public class BuildDeployWindow : EditorWindow
    {
        private const float UpdateBuildsPeriod = 1.0f;

        private const string SdkVersion =
#if UNITY_2017_2_OR_NEWER
                "10.0.16299.0";
#else
                "10.0.15063.0";
#endif

        private readonly string[] tabNames = { "Unity Build Options", "Appx Build Options", "Deploy Options", "Utilities" };

        private readonly string[] scriptingBackendNames = { "IL2CPP", ".NET" };

        private readonly int[] scriptingBackendEnum = { (int)ScriptingImplementation.IL2CPP, (int)ScriptingImplementation.WinRTDotNET };

        private readonly string[] deviceNames = { "Any Device", "PC", "Mobile", "HoloLens" };

        private readonly List<string> builds = new List<string>(0);

        private enum BuildDeployTab
        {
            UnityBuildOptions,
            AppxBuildOptions,
            DeployOptions,
            Utilities
        }

        private enum BuildPlatformEnum
        {
            x86 = 1,
            x64 = 2
        }

        private enum BuildConfigEnum
        {
            Debug = 0,
            Release = 1,
            Master = 2
        }

        #region Properties

        private static bool ShouldOpenSLNBeEnabled
        {
            get { return !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); }
        }

        private static bool ShouldBuildSLNBeEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory) &&
                       !string.IsNullOrEmpty(PlayerSettings.WSA.certificatePath);
            }
        }

        private static bool ShouldBuildAppxBeEnabled
        {
            get
            {
                return ShouldBuildSLNBeEnabled &&
                  !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory) &&
                  !string.IsNullOrEmpty(BuildDeployPrefs.MsBuildVersion) &&
                  !string.IsNullOrEmpty(BuildDeployPrefs.BuildConfig);
            }
        }

        private static bool ShouldLaunchAppBeEnabled
        {
            get { return portalConnections.Connections.Count > 0 && !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); }
        }

        private static bool ShouldWebPortalBeEnabled
        {
            get { return portalConnections.Connections.Count > 0 && !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); }
        }

        private static bool ShouldLogViewBeEnabled
        {
            get { return !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); }
        }

        private static bool CanInstall
        {
            get
            {
                return Directory.Exists(BuildDeployPrefs.AbsoluteBuildDirectory);
            }
        }

        private static bool IsHoloLensConnectedUsb
        {
            get
            {
                bool isConnected = false;
                if (USBDeviceListener.USBDevices != null)
                {
                    foreach (USBDeviceInfo device in USBDeviceListener.USBDevices)
                    {
                        if (device.Name.Equals("Microsoft HoloLens"))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    isConnected = SessionState.GetBool("HoloLensUsbConnected", false);
                }

                SessionState.SetBool("HoloLensUsbConnected", isConnected);
                return isConnected;
            }
        }

        #endregion // Properties

        #region Fields

        private int halfWidth;
        private int quarterWidth;
        private static int currentConnectionInfoIndex;

        private float timeLastUpdatedBuilds;

        private string[] targetIps;
        private string[] windowsSdkPaths;

        private Vector2 scrollPosition;

        private BuildDeployTab currentTab = BuildDeployTab.UnityBuildOptions;

        private static DevicePortalConnections portalConnections;
        private readonly List<string> appPackageDirectories = new List<string>(0);

        #endregion // Fields

        #region Methods

        [MenuItem("Mixed Reality Toolkit/Build Window", false, 0)]
        public static void OpenWindow()
        {
            // Dock it next to the Scene View.
            var window = GetWindow<BuildDeployWindow>(typeof(SceneView));
            window.titleContent = new GUIContent("Build Window");
            window.Show();
        }

        private void OnEnable()
        {
            Setup();
        }

        private void Setup()
        {
            titleContent = new GUIContent("Build Window");
            minSize = new Vector2(512, 256);

            windowsSdkPaths = Directory.GetDirectories(@"C:\Program Files (x86)\Windows Kits\10\Lib");

            for (int i = 0; i < windowsSdkPaths.Length; i++)
            {
                windowsSdkPaths[i] = windowsSdkPaths[i].Substring(windowsSdkPaths[i].LastIndexOf(@"\", StringComparison.Ordinal) + 1);
            }

            UpdateBuilds();

            portalConnections = JsonUtility.FromJson<DevicePortalConnections>(BuildDeployPrefs.DevicePortalConnections);
            UpdatePortalConnections();
        }

        private void OnGUI()
        {
            quarterWidth = Screen.width / 4;
            halfWidth = Screen.width / 2;

            #region Quick Options

            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.Label("Quick Options");
            EditorGUILayout.BeginHorizontal();

            EditorUserBuildSettings.wsaSubtarget = (WSASubtarget)EditorGUILayout.Popup((int)EditorUserBuildSettings.wsaSubtarget, deviceNames);

            GUI.enabled = ShouldBuildSLNBeEnabled;

            // Build & Run button...
            if (GUILayout.Button(CanInstall
                ? new GUIContent("Build all, then Install", "Builds the Unity Project, the APPX, then installs to the target device.")
                : new GUIContent("Build all", "Builds the Unity Project and APPX"),
                GUILayout.Width(halfWidth - 20)))
            {
                EditorApplication.delayCall += () => BuildAll(CanInstall);
            }

            GUI.enabled = true;

            if (GUILayout.Button("Open Player Settings", GUILayout.Width(quarterWidth)))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
            }

            // If Xbox Controller support is enabled and we're targeting the HoloLens device,
            // Enable the HID capability.
            if (EditorUserBuildSettings.wsaSubtarget == WSASubtarget.HoloLens)
            {
                PlayerSettings.WSA.SetCapability(
                    PlayerSettings.WSACapability.HumanInterfaceDevice,
                    EditorPrefsUtility.GetEditorPref("Enable Xbox Controller Support", false));

                BuildDeployPrefs.BuildPlatform = BuildPlatformEnum.x86.ToString();
            }
            else
            {
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.HumanInterfaceDevice, false);
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10);

            #endregion

            currentTab = (BuildDeployTab)GUILayout.Toolbar((int)currentTab, tabNames);

            GUILayout.Space(10);

            switch (currentTab)
            {
                case BuildDeployTab.UnityBuildOptions:
                    UnityBuildGUI();
                    break;
                case BuildDeployTab.AppxBuildOptions:
                    AppxBuildGUI();
                    break;
                case BuildDeployTab.DeployOptions:
                    DeployGUI();
                    break;
                case BuildDeployTab.Utilities:
                    UtilitiesGUI();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UnityBuildGUI()
        {
            GUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

            // Build directory (and save setting, if it's changed)
            string curBuildDirectory = BuildDeployPrefs.BuildDirectory;
            EditorGUILayout.LabelField(new GUIContent("Build Directory", "It's recommended to use 'UWP'"), GUILayout.Width(96));
            string newBuildDirectory = EditorGUILayout.TextField(
                curBuildDirectory,
                GUILayout.Width(64), GUILayout.ExpandWidth(true));

            if (newBuildDirectory != curBuildDirectory)
            {
                BuildDeployPrefs.BuildDirectory = newBuildDirectory;
            }

            GUI.enabled = Directory.Exists(BuildDeployPrefs.AbsoluteBuildDirectory);

            if (GUILayout.Button("Open Build Directory", GUILayout.Width(halfWidth)))
            {
                Process.Start(BuildDeployPrefs.AbsoluteBuildDirectory);
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = ShouldOpenSLNBeEnabled;

            if (GUILayout.Button("Open in Visual Studio", GUILayout.Width(halfWidth)))
            {
                // Open SLN
                string slnFilename = Path.Combine(BuildDeployPrefs.BuildDirectory, PlayerSettings.productName + ".sln");

                if (File.Exists(slnFilename))
                {
                    var slnFile = new FileInfo(slnFilename);
                    Process.Start(slnFile.FullName);
                }
                else if (EditorUtility.DisplayDialog(
                    "Solution Not Found",
                    "We couldn't find the Project's Solution. Would you like to Build the project now?",
                    "Yes, Build", "No"))
                {
                    EditorApplication.delayCall += () => BuildDeployTools.BuildSLN(BuildDeployPrefs.BuildDirectory);
                }
            }


            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // Generate C# Project References for debugging
            GUILayout.FlexibleSpace();
            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 105;
            bool generateReferenceProjects = EditorUserBuildSettings.wsaGenerateReferenceProjects;
            bool shouldGenerateProjects = EditorGUILayout.Toggle(
                new GUIContent("Unity C# Projects", "Generate C# Project References for debugging"),
                generateReferenceProjects);

            if (shouldGenerateProjects != generateReferenceProjects)
            {
                EditorUserBuildSettings.wsaGenerateReferenceProjects = shouldGenerateProjects;
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;

            // Build Unity Player
            GUI.enabled = ShouldBuildSLNBeEnabled;

            if (GUILayout.Button("Build Unity Project", GUILayout.Width(halfWidth)))
            {
                EditorApplication.delayCall += () => BuildDeployTools.BuildSLN(BuildDeployPrefs.BuildDirectory);
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void AppxBuildGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            // SDK and MS Build Version(and save setting, if it's changed)
            string currentSDKVersion = EditorUserBuildSettings.wsaUWPSDK;

            int currentSDKVersionIndex = 0;

            for (var i = 0; i < windowsSdkPaths.Length; i++)
            {
                if (string.IsNullOrEmpty(currentSDKVersion))
                {
                    currentSDKVersionIndex = windowsSdkPaths.Length - 1;
                }
                else
                {
                    if (windowsSdkPaths[i].Equals(SdkVersion))
                    {
                        currentSDKVersionIndex = i;
                    }
                }
            }

            EditorGUILayout.LabelField("Required SDK Version: " + SdkVersion);

            if (currentSDKVersionIndex == 0)
            {
                Debug.LogErrorFormat("Unable to find the required Windows 10 SDK Target!\n" +
                                     "Please be sure to install the {0} SDK from Visual Studio Installer.", SdkVersion);
                GUILayout.EndHorizontal();

                EditorGUILayout.HelpBox(string.Format("Unable to find the required Windows 10 SDK Target!\n" +
                                        "Please be sure to install the {0} SDK from Visual Studio Installer.", SdkVersion), MessageType.Error);

                GUILayout.BeginHorizontal();
            }

            var curScriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA);
            var newScriptingBackend = (ScriptingImplementation)EditorGUILayout.IntPopup(
                "Scripting Backend",
                (int)curScriptingBackend,
                scriptingBackendNames,
                scriptingBackendEnum,
                GUILayout.Width(halfWidth));

            if (newScriptingBackend != curScriptingBackend)
            {
                bool canUpdate = !Directory.Exists(BuildDeployPrefs.AbsoluteBuildDirectory);

                if (!canUpdate &&
                    EditorUtility.DisplayDialog("Attention!",
                        string.Format("Build path contains project built with {0} scripting backend, while current project is using {1} scripting backend.\n\n" +
                                      "Switching to a new scripting backend requires us to delete all the data currently in your build folder and rebuild the Unity Player!",
                            newScriptingBackend.ToString(),
                            curScriptingBackend.ToString()),
                        "Okay", "Cancel"))
                {
                    Directory.Delete(BuildDeployPrefs.AbsoluteBuildDirectory, true);
                }

                if (canUpdate)
                {
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.WSA, newScriptingBackend);
                }
            }

            string newSDKVersion = windowsSdkPaths[currentSDKVersionIndex];

            if (!newSDKVersion.Equals(currentSDKVersion))
            {
                EditorUserBuildSettings.wsaUWPSDK = newSDKVersion;
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Build config (and save setting, if it's changed)
            string curBuildConfigString = BuildDeployPrefs.BuildConfig;

            BuildConfigEnum buildConfigOption;
            if (curBuildConfigString.ToLower().Equals("master"))
            {
                buildConfigOption = BuildConfigEnum.Master;
            }
            else if (curBuildConfigString.ToLower().Equals("release"))
            {
                buildConfigOption = BuildConfigEnum.Release;
            }
            else
            {
                buildConfigOption = BuildConfigEnum.Debug;
            }

            buildConfigOption = (BuildConfigEnum)EditorGUILayout.EnumPopup("Build Configuration", buildConfigOption, GUILayout.Width(halfWidth));

            string buildConfigString = buildConfigOption.ToString();

            if (buildConfigString != curBuildConfigString)
            {
                BuildDeployPrefs.BuildConfig = buildConfigString;
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Build Platform (and save setting, if it's changed)
            string curBuildPlatformString = BuildDeployPrefs.BuildPlatform;
            var buildPlatformOption = BuildPlatformEnum.x86;

            if (curBuildPlatformString.ToLower().Equals("x86"))
            {
                buildPlatformOption = BuildPlatformEnum.x86;
            }
            else if (curBuildPlatformString.ToLower().Equals("x64"))
            {
                buildPlatformOption = BuildPlatformEnum.x64;
            }

            buildPlatformOption = (BuildPlatformEnum)EditorGUILayout.EnumPopup("Build Platform", buildPlatformOption, GUILayout.Width(halfWidth));

            string newBuildPlatformString;

            switch (buildPlatformOption)
            {
                case BuildPlatformEnum.x86:
                case BuildPlatformEnum.x64:
                    newBuildPlatformString = buildPlatformOption.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (newBuildPlatformString != curBuildPlatformString)
            {
                BuildDeployPrefs.BuildPlatform = newBuildPlatformString;
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var previousLabelWidth = EditorGUIUtility.labelWidth;

            // Auto Increment version
            EditorGUIUtility.labelWidth = 96;
            bool curIncrementVersion = BuildDeployPrefs.IncrementBuildVersion;
            bool newIncrementVersion = EditorGUILayout.Toggle(new GUIContent("Auto Increment", "Increases Version Build Number"), curIncrementVersion);

            // Restore previous label width
            EditorGUIUtility.labelWidth = previousLabelWidth;

            if (newIncrementVersion != curIncrementVersion)
            {
                BuildDeployPrefs.IncrementBuildVersion = newIncrementVersion;
            }

            EditorGUILayout.LabelField(new GUIContent("Version Number", "Major.Minor.Build.Revision\n" +
                                       "Note: Revision should always be zero because it's reserved by Windows Store."), GUILayout.Width(96));
            Vector3Int newVersion = Vector3Int.zero;
            EditorGUI.BeginChangeCheck();
            newVersion.x = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Major, GUILayout.Width(quarterWidth / 2 - 3));
            newVersion.y = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Minor, GUILayout.Width(quarterWidth / 2 - 3));
            newVersion.z = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Build, GUILayout.Width(quarterWidth / 2 - 3));

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.WSA.packageVersion = new Version(newVersion.x, newVersion.y, newVersion.z, 0);
            }

            GUI.enabled = false;
            EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Revision, GUILayout.Width(quarterWidth / 2 - 3));
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Force rebuild
            previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;
            bool curForceRebuildAppx = BuildDeployPrefs.ForceRebuild;
            bool newForceRebuildAppx = EditorGUILayout.Toggle("Rebuild", curForceRebuildAppx);

            if (newForceRebuildAppx != curForceRebuildAppx)
            {
                BuildDeployPrefs.ForceRebuild = newForceRebuildAppx;
            }

            // Restore previous label width
            EditorGUIUtility.labelWidth = previousLabelWidth;

            // Build APPX
            GUI.enabled = ShouldBuildAppxBeEnabled;

            if (GUILayout.Button("Build APPX", GUILayout.Width(halfWidth)))
            {
                // Check if SLN exists
                string slnFilename = Path.Combine(BuildDeployPrefs.BuildDirectory, PlayerSettings.productName + ".sln");

                if (File.Exists(slnFilename))
                {
                    // Build APPX
                    EditorApplication.delayCall += () =>
                    {
                        BuildDeployTools.BuildAppxFromSLN(
                            PlayerSettings.productName,
                            BuildDeployTools.DefaultMSBuildVersion,
                            BuildDeployPrefs.ForceRebuild,
                            BuildDeployPrefs.BuildConfig,
                            BuildDeployPrefs.BuildPlatform,
                            BuildDeployPrefs.BuildDirectory,
                            BuildDeployPrefs.IncrementBuildVersion);
                    };
                }
                else if (EditorUtility.DisplayDialog("Solution Not Found", "We couldn't find the solution. Would you like to Build it?", "Yes, Build", "No"))
                {
                    // Build SLN then APPX
                    EditorApplication.delayCall += () => BuildAll(install: false);
                }

                GUI.enabled = true;
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Open AppX packages location
            string appxBuildPath = Path.GetFullPath(BuildDeployPrefs.BuildDirectory + "/" + PlayerSettings.productName + "/AppPackages");
            GUI.enabled = builds.Count > 0 && !string.IsNullOrEmpty(appxBuildPath);

            if (GUILayout.Button("Open APPX Packages Location", GUILayout.Width(halfWidth)))
            {
                Process.Start("explorer.exe", "/f /open," + appxBuildPath);
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DeployGUI()
        {
            GUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 64;
            BuildDeployPrefs.UseSSL = EditorGUILayout.Toggle(new GUIContent("Use SSL?", "Use SLL to communicate with Device Portal"), BuildDeployPrefs.UseSSL);
            EditorGUIUtility.labelWidth = previousLabelWidth;

            Debug.Assert(portalConnections.Connections.Count != 0);
            Debug.Assert(currentConnectionInfoIndex >= 0);

            currentConnectionInfoIndex = EditorGUILayout.Popup(currentConnectionInfoIndex, targetIps, GUILayout.Width(halfWidth - 48));
            var currentConnection = portalConnections.Connections[currentConnectionInfoIndex];

            GUI.enabled = currentConnection.IP != "0.0.0.0";
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                portalConnections.Connections.Add(new ConnectInfo("0.0.0.0", currentConnection.User, currentConnection.Password));
                currentConnectionInfoIndex++;
                currentConnection = portalConnections.Connections[currentConnectionInfoIndex];
                Repaint();
            }

            GUI.enabled = portalConnections.Connections.Count > 1 && currentConnectionInfoIndex != 0;
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                portalConnections.Connections.RemoveAt(currentConnectionInfoIndex);
                currentConnectionInfoIndex--;
                currentConnection = portalConnections.Connections[currentConnectionInfoIndex];
                Repaint();
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            // Username/Password (and save seeings, if changed)
            previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 64;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = !currentConnection.IP.Contains("127.0.0.1") && !currentConnection.IP.Contains("Local Machine");
            currentConnection.IP = EditorGUILayout.TextField(
                new GUIContent("IpAddress", "Note: Local Machine will install on any HoloLens connected to USB as well."),
                currentConnection.IP,
                GUILayout.Width(halfWidth));
            GUI.enabled = true;
            // If it's local, add the port if we're connected to the HoloLens
            if (currentConnection.IP.Contains("127.0.0.1"))
            {
                currentConnection.IP = "Local Machine";
                UpdatePortalConnections();
                Repaint();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            currentConnection.User = EditorGUILayout.TextField("Username", currentConnection.User, GUILayout.Width(halfWidth));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            currentConnection.Password = EditorGUILayout.PasswordField("Password", currentConnection.Password, GUILayout.Width(halfWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            EditorGUIUtility.labelWidth = 86;
            bool fullReinstall = EditorGUILayout.Toggle(
                new GUIContent(" Uninstall First", "Uninstall application before installing"),
                BuildDeployPrefs.FullReinstall,
                GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = previousLabelWidth;

            if (EditorGUI.EndChangeCheck())
            {
                BuildDeployPrefs.FullReinstall = fullReinstall;

                portalConnections.Connections[currentConnectionInfoIndex] = currentConnection;
                BuildDeployPrefs.DevicePortalConnections = JsonUtility.ToJson(portalConnections);

                UpdatePortalConnections();
            }

            GUILayout.FlexibleSpace();

            // Uninstall...
            GUI.enabled = ShouldLogViewBeEnabled && CanInstall;

            if (GUILayout.Button("Uninstall Application Now", GUILayout.Width(halfWidth)))
            {
                EditorApplication.delayCall += () => UninstallAppOnDevicesList(portalConnections);
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();

            // Build list (with install buttons)
            if (builds.Count == 0)
            {
                GUILayout.Label("*** No builds found in build directory", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.Separator();
                GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

                foreach (var fullBuildLocation in builds)
                {
                    int lastBackslashIndex = fullBuildLocation.LastIndexOf("\\", StringComparison.Ordinal);

                    var directoryDate = Directory.GetLastWriteTime(fullBuildLocation).ToString("yyyy/MM/dd HH:mm:ss");
                    string packageName = fullBuildLocation.Substring(lastBackslashIndex + 1);

                    GUILayout.Space(2);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(12);

                    GUI.enabled = CanInstall;
                    if (GUILayout.Button("Install", GUILayout.Width(96)))
                    {
                        string thisBuildLocation = fullBuildLocation;
                        EditorApplication.delayCall += () => InstallAppOnDevicesList(thisBuildLocation, portalConnections);
                    }

                    GUI.enabled = true;

                    GUILayout.Space(8);
                    GUILayout.Label(new GUIContent(packageName + " (" + directoryDate + ")"));
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }


            GUILayout.EndVertical();
        }

        private void UtilitiesGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Open web portal
            GUI.enabled = ShouldWebPortalBeEnabled && CanInstall;

            if (GUILayout.Button("Open Device Portal", GUILayout.Width(halfWidth)))
            {
                EditorApplication.delayCall += () => OpenWebPortalForIPs(portalConnections);
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Launch app...
            GUI.enabled = ShouldLaunchAppBeEnabled && CanInstall;

            if (GUILayout.Button("Launch Application", GUILayout.Width(halfWidth)))
            {
                // If already running, kill it (button is a toggle)
                if (IsAppRunning_FirstIPCheck(PlayerSettings.productName, portalConnections))
                {
                    KillAppOnIPs(portalConnections);
                }
                else
                {
                    LaunchAppOnIPs(portalConnections);
                }
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Log file
            string localLogPath = string.Empty;
            bool localLogExists = false;

            bool remoteDeviceDetected = portalConnections.Connections.Count > 1 || IsHoloLensConnectedUsb;
            bool isLocalBuild = BuildDeployPrefs.BuildPlatform == BuildPlatformEnum.x64.ToString();

            if (!remoteDeviceDetected)
            {
                localLogPath = string.Format("%USERPROFILE%\\AppData\\Local\\Packages\\{0}\\TempState\\UnityPlayer.log", PlayerSettings.productName);
                localLogExists = File.Exists(localLogPath);
            }

            GUI.enabled = ShouldLogViewBeEnabled && (remoteDeviceDetected || isLocalBuild && localLogExists);

            if (GUILayout.Button("View Log File", GUILayout.Width(halfWidth)))
            {
                if (remoteDeviceDetected)
                {
                    OpenLogFileForIPs(portalConnections);
                }

                if (localLogExists)
                {
                    Process.Start(localLogPath);
                }
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void BuildAll(bool install = true)
        {
            // First build SLN
            if (!BuildDeployTools.BuildSLN(BuildDeployPrefs.BuildDirectory, false))
            {
                return;
            }

            // Next, APPX
            if (!BuildDeployTools.BuildAppxFromSLN(
                PlayerSettings.productName,
                BuildDeployPrefs.MsBuildVersion,
                BuildDeployPrefs.ForceRebuild,
                BuildDeployPrefs.BuildConfig,
                BuildDeployPrefs.BuildPlatform,
                BuildDeployPrefs.BuildDirectory,
                BuildDeployPrefs.IncrementBuildVersion,
                showDialog: !install))
            {
                return;
            }

            // Next, Install
            if (install)
            {
                string fullBuildLocation = CalcMostRecentBuild();
                InstallAppOnDevicesList(fullBuildLocation, portalConnections);
            }
        }

        private void UpdatePortalConnections()
        {
            targetIps = new string[portalConnections.Connections.Count];
            if (currentConnectionInfoIndex > portalConnections.Connections.Count - 1)
            {
                currentConnectionInfoIndex = portalConnections.Connections.Count - 1;
            }

            for (int i = 0; i < targetIps.Length; i++)
            {
                targetIps[i] = portalConnections.Connections[i].IP;
            }
        }

        private string CalcMostRecentBuild()
        {
            UpdateBuilds();
            DateTime mostRecent = DateTime.MinValue;
            string mostRecentBuild = string.Empty;

            foreach (var fullBuildLocation in builds)
            {
                DateTime directoryDate = Directory.GetLastWriteTime(fullBuildLocation);

                if (directoryDate > mostRecent)
                {
                    mostRecentBuild = fullBuildLocation;
                    mostRecent = directoryDate;
                }
            }

            return mostRecentBuild;
        }

        private static string CalcPackageFamilyName()
        {
            // Find the manifest
            string[] manifests = Directory.GetFiles(BuildDeployPrefs.AbsoluteBuildDirectory, "Package.appxmanifest", SearchOption.AllDirectories);

            if (manifests.Length == 0)
            {
                Debug.LogError("Unable to find manifest file for build (in path - " + BuildDeployPrefs.AbsoluteBuildDirectory + ")");
                return string.Empty;
            }

            string manifest = manifests[0];

            // Parse it
            using (var reader = new XmlTextReader(manifest))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.Equals("identity", StringComparison.OrdinalIgnoreCase))
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name.Equals("name", StringComparison.OrdinalIgnoreCase))
                                    {
                                        return reader.Value;
                                    }
                                }
                            }

                            break;
                    }
                }
            }

            Debug.LogError("Unable to find PackageFamilyName in manifest file (" + manifest + ")");
            return string.Empty;
        }

        private static void InstallAppOnDevicesList(string buildPath, DevicePortalConnections targetList)
        {
            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            try
            {
                for (int i = 0; i < targetList.Connections.Count; i++)
                {
                    bool completedUninstall = false;
                    string ip = targetList.Connections[i].IP;

                    if (ip.Contains("Local Machine") && !IsHoloLensConnectedUsb)
                    {
                        continue;
                    }

                    if (BuildDeployPrefs.FullReinstall && BuildDeployPortal.IsAppInstalled(packageFamilyName, targetList.Connections[i]))
                    {
                        EditorUtility.DisplayProgressBar("Installing on devices", "Uninstall (" + ip + ")", i / (float)targetList.Connections.Count);
                        if (!BuildDeployPortal.UninstallApp(packageFamilyName, targetList.Connections[i]))
                        {
                            Debug.LogError("Uninstall failed - skipping install (" + ip + ")");
                            continue;
                        }

                        completedUninstall = true;
                    }

                    EditorUtility.DisplayProgressBar("Installing on devices", "Install (" + ip + ")", (i + (completedUninstall ? 0.5f : 0.0f)) / targetList.Connections.Count);

                    // Get the appx path
                    FileInfo[] files = new DirectoryInfo(buildPath).GetFiles("*.appx");
                    files = files.Length == 0 ? new DirectoryInfo(buildPath).GetFiles("*.appxbundle") : files;

                    if (files.Length == 0)
                    {
                        Debug.LogError("No APPX found in folder build folder (" + buildPath + ")");
                        return;
                    }

                    // Kick off the install
                    BuildDeployPortal.InstallApp(files[0].FullName, targetList.Connections[i]);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            EditorUtility.ClearProgressBar();
        }

        private static void UninstallAppOnDevicesList(DevicePortalConnections targetList)
        {
            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            try
            {
                for (int i = 0; i < targetList.Connections.Count; i++)
                {
                    string ip = targetList.Connections[i].IP;

                    if (ip.Contains("Local Machine") && !IsHoloLensConnectedUsb)
                    {
                        continue;
                    }

                    EditorUtility.DisplayProgressBar("Uninstalling application", "Uninstall (" + ip + ")", i / (float)targetList.Connections.Count);
                    BuildDeployPortal.UninstallApp(packageFamilyName, targetList.Connections[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }

            EditorUtility.ClearProgressBar();
        }

        private void UpdateBuilds()
        {
            builds.Clear();

            try
            {
                appPackageDirectories.Clear();
                string[] buildList = Directory.GetDirectories(BuildDeployPrefs.AbsoluteBuildDirectory, "*", SearchOption.AllDirectories);
                foreach (string appBuild in buildList)
                {
                    if (appBuild.Contains("AppPackages") && !appBuild.Contains("AppPackages\\"))
                    {
                        appPackageDirectories.AddRange(Directory.GetDirectories(appBuild));
                    }
                }

                IEnumerable<string> selectedDirectories =
                    from string directory in appPackageDirectories
                    orderby Directory.GetLastWriteTime(directory) descending
                    select Path.GetFullPath(directory);
                builds.AddRange(selectedDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                // unused
            }

            timeLastUpdatedBuilds = Time.realtimeSinceStartup;
        }

        private void Update()
        {
            if (Time.realtimeSinceStartup - timeLastUpdatedBuilds > UpdateBuildsPeriod)
            {
                UpdateBuilds();
            }
        }

        private static void OpenWebPortalForIPs(DevicePortalConnections targetDevices)
        {
            for (int i = 0; i < targetDevices.Connections.Count; i++)
            {
                if (targetDevices.Connections[i].IP.Contains("Local Machine") && !IsHoloLensConnectedUsb)
                {
                    continue;
                }

                if (IsHoloLensConnectedUsb)
                {
                    if (targetDevices.Connections[i].IP.Contains("Local Machine"))
                    {
                        BuildDeployPortal.LoginToWebPortal(targetDevices.Connections[i]);
                    }
                }
                else
                {
                    if (!targetDevices.Connections[i].IP.Contains("Local Machine"))
                    {
                        BuildDeployPortal.LoginToWebPortal(targetDevices.Connections[i]);
                    }
                }
            }
        }

        private static bool IsAppRunning_FirstIPCheck(string appName, DevicePortalConnections targetDevices)
        {
            return BuildDeployPortal.IsAppRunning(appName, targetDevices.Connections[currentConnectionInfoIndex]);
        }

        private static void LaunchAppOnIPs(DevicePortalConnections targetDevices)
        {
            if (!IsHoloLensConnectedUsb)
            {
                Debug.LogWarning("No suitable device found.");
                return;
            }

            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            for (int i = 0; i < targetDevices.Connections.Count; i++)
            {
                string targetIP = targetDevices.Connections[i].IP;

                if (targetIP.Contains("Local Machine") && !IsHoloLensConnectedUsb)
                {
                    continue;
                }

                Debug.Log("Launching app on: " + targetIP);
                BuildDeployPortal.LaunchApp(
                    packageFamilyName,
                    new ConnectInfo(targetIP, targetDevices.Connections[i].User, targetDevices.Connections[i].Password));
            }
        }

        private static void KillAppOnIPs(DevicePortalConnections targetDevices)
        {
            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            for (int i = 0; i < targetDevices.Connections.Count; i++)
            {
                if (targetDevices.Connections[i].IP.Contains("Local Machine") && !IsHoloLensConnectedUsb)
                {
                    continue;
                }

                Debug.LogFormat("Killing app {0} on: {1}", packageFamilyName, targetDevices.Connections[i].IP);
                BuildDeployPortal.KillApp(packageFamilyName, targetDevices.Connections[i]);
            }
        }

        public static void OpenLogFileForIPs(DevicePortalConnections targetDevices)
        {
            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            for (int i = 0; i < targetDevices.Connections.Count; i++)
            {
                if (targetDevices.Connections[i].IP.Contains("Local Machine") && !IsHoloLensConnectedUsb)
                {
                    continue;
                }

                BuildDeployPortal.DeviceLogFile_View(packageFamilyName, targetDevices.Connections[i]);
            }
        }

        #endregion // Methods
    }
}
