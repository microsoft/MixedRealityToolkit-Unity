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

        private readonly string[] tabNames = { "Unity Build Options", "Appx Build Options", "Deploy Options" };

        private readonly string[] scriptingBackendNames = { "IL2CPP", ".NET" };

        private readonly int[] scriptingBackendEnum = { (int)ScriptingImplementation.IL2CPP, (int)ScriptingImplementation.WinRTDotNET };

        private readonly string[] deviceNames = { "Any Device", "PC", "Mobile", "HoloLens" };

        private readonly List<string> builds = new List<string>(0);

        private readonly List<string> appPackageDirectories = new List<string>(0);

        private enum BuildDeployTab
        {
            UnityBuildOptions,
            AppxBuildOptions,
            DeployOptions
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

        private static bool DevicePortalConnectionEnabled
        {
            get { return (portalConnections.Connections.Count > 1 || IsHoloLensConnectedUsb) && !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); }
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
                    if (USBDeviceListener.USBDevices.Any(device => device.Name.Equals("Microsoft HoloLens")))
                    {
                        isConnected = true;
                    }

                    SessionState.SetBool("HoloLensUsbConnected", isConnected);
                }
                else
                {
                    isConnected = SessionState.GetBool("HoloLensUsbConnected", false);
                }

                return isConnected;
            }
        }

        #endregion // Properties

        #region Fields

        private int halfWidth;
        private int quarterWidth;

        private float timeLastUpdatedBuilds;

        private string[] targetIps;
        private string[] windowsSdkPaths;

        private Vector2 scrollPosition;

        private BuildDeployTab currentTab = BuildDeployTab.UnityBuildOptions;

        private static bool isAppRunning;
        private static int currentConnectionInfoIndex;
        private static DevicePortalConnections portalConnections;

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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void Update()
        {
            if (Time.realtimeSinceStartup - timeLastUpdatedBuilds > UpdateBuildsPeriod)
            {
                UpdateBuilds();
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
                EditorApplication.delayCall += () => Process.Start(BuildDeployPrefs.AbsoluteBuildDirectory);
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
                    EditorApplication.delayCall += () => Process.Start(new FileInfo(slnFilename).FullName);
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
                EditorApplication.delayCall += () => Process.Start("explorer.exe", "/f /open," + appxBuildPath);
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DeployGUI()
        {
            Debug.Assert(portalConnections.Connections.Count != 0);

            GUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();

            // Launch app...
            GUI.enabled = IsHoloLensConnectedUsb;

            if (GUILayout.Button(new GUIContent("Add USB HoloLens", "Pairs the USB connected HoloLens with the Build Window so you can deploy via USB"), GUILayout.Width(quarterWidth)))
            {
                EditorApplication.delayCall += () =>
                {
                    var newConnection = default(ConnectInfo);

                    foreach (var targetDevice in portalConnections.Connections)
                    {
                        if (!IsLocalConnection(targetDevice))
                        {
                            continue;
                        }

                        var machineName = BuildDeployPortal.GetMachineName(targetDevice);
                        var networkInfo = BuildDeployPortal.GetNetworkInfo(targetDevice);

                        if (networkInfo != null)
                        {
                            var newIps = new List<string>();
                            foreach (var adapter in networkInfo.Adapters)
                            {
                                newIps.AddRange(from address in adapter.IpAddresses where !address.IpAddress.Contains("0.0.0.0") select address.IpAddress);
                            }

                            foreach (var ip in newIps)
                            {
                                if (portalConnections.Connections.Any(connection => connection.IP == ip))
                                {
                                    Debug.LogFormat("Already paired");
                                    continue;
                                }

                                newConnection.IP = ip;
                                newConnection.User = targetDevice.User;
                                newConnection.Password = targetDevice.Password;

                                if (machineName != null)
                                {
                                    newConnection.MachineName = machineName.ComputerName;
                                }
                            }
                        }
                    }

                    if (IsValidIpAddress(newConnection.IP))
                    {
                        portalConnections.Connections.Add(newConnection);
                        for (var i = 0; i < portalConnections.Connections.Count; i++)
                        {
                            if (portalConnections.Connections[i].IP == newConnection.IP)
                            {
                                currentConnectionInfoIndex = i;
                                break;
                            }
                        }

                        UpdatePortalConnections();
                    }
                };
            }

            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 64;
            BuildDeployPrefs.UseSSL = EditorGUILayout.Toggle(new GUIContent("Use SSL?", "Use SLL to communicate with Device Portal"), BuildDeployPrefs.UseSSL);
            EditorGUIUtility.labelWidth = previousLabelWidth;
            Debug.Assert(currentConnectionInfoIndex >= 0);

            currentConnectionInfoIndex = EditorGUILayout.Popup(currentConnectionInfoIndex, targetIps, GUILayout.Width(halfWidth - 48));
            var currentConnection = portalConnections.Connections[currentConnectionInfoIndex];
            bool isLocalConnection = IsLocalConnection(currentConnection);

            if (isLocalConnection)
            {
                currentConnection.MachineName = "Local Machine";
            }

            GUI.enabled = IsValidIpAddress(currentConnection.IP);

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                portalConnections.Connections.Add(new ConnectInfo("0.0.0.0", currentConnection.User, currentConnection.Password));
                currentConnectionInfoIndex++;
                currentConnection = portalConnections.Connections[currentConnectionInfoIndex];
                UpdatePortalConnections();
                Repaint();
            }

            GUI.enabled = portalConnections.Connections.Count > 1 && currentConnectionInfoIndex != 0;
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                portalConnections.Connections.RemoveAt(currentConnectionInfoIndex);
                currentConnectionInfoIndex--;
                currentConnection = portalConnections.Connections[currentConnectionInfoIndex];
                UpdatePortalConnections();
                Repaint();
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label(currentConnection.MachineName, GUILayout.Width(halfWidth));

            GUILayout.EndHorizontal();

            previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 64;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = !isLocalConnection;
            currentConnection.IP = EditorGUILayout.TextField(
                new GUIContent("IpAddress", "Note: Local Machine will install on any HoloLens connected to USB as well."),
                currentConnection.IP,
                GUILayout.Width(halfWidth));
            GUI.enabled = true;

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

                // Format our local connection
                if (currentConnection.IP.Contains("127.0.0.1"))
                {
                    currentConnection.IP = "Local Machine";
                    UpdatePortalConnections();
                    Repaint();
                }

                if (!isLocalConnection &&
                    IsValidIpAddress(currentConnection.IP) &&
                    IsCredentialsValid(currentConnection) &&
                    currentConnection.MachineName.Contains(currentConnection.IP))
                {
                    var machineName = BuildDeployPortal.GetMachineName(currentConnection);

                    if (machineName != null)
                    {
                        currentConnection.MachineName = machineName.ComputerName;
                    }
                }

                portalConnections.Connections[currentConnectionInfoIndex] = currentConnection;
                BuildDeployPrefs.DevicePortalConnections = JsonUtility.ToJson(portalConnections);

                UpdatePortalConnections();
            }

            GUILayout.FlexibleSpace();

            // Open web portal
            GUI.enabled = DevicePortalConnectionEnabled && CanInstall;

            if (GUILayout.Button("Open Device Portal", GUILayout.Width(quarterWidth)))
            {
                EditorApplication.delayCall += () => OpenDevicePortal(portalConnections);
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();

            // Build list
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
                        EditorApplication.delayCall += () => InstallAppOnDevicesList(fullBuildLocation, portalConnections);
                    }

                    GUI.enabled = true;

                    // Uninstall...
                    GUI.enabled = CanInstall;

                    if (GUILayout.Button("Uninstall", GUILayout.Width(96)))
                    {
                        EditorApplication.delayCall += () => UninstallAppOnDevicesList(portalConnections);
                    }

                    GUI.enabled = true;

                    // Launch app...
                    GUI.enabled = CanInstall;

                    if (GUILayout.Button(new GUIContent(isAppRunning ? "Kill App" : "Launch App", "These are remote commands only"), GUILayout.Width(96)))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            if (isAppRunning)
                            {
                                KillAppOnIPs(portalConnections);
                            }
                            else
                            {
                                LaunchAppOnIPs(portalConnections);
                            }
                        };

                        isAppRunning = !isAppRunning;
                    }

                    GUI.enabled = true;

                    // Log file

                    string localLogPath = string.Format("%USERPROFILE%\\AppData\\Local\\Packages\\{0}\\TempState\\UnityPlayer.log", PlayerSettings.productName);
                    bool localLogExists = File.Exists(localLogPath);

                    GUI.enabled = DevicePortalConnectionEnabled || localLogExists;

                    if (GUILayout.Button("View Log", GUILayout.Width(96)))
                    {
                        foreach (var connection in portalConnections.Connections)
                        {
                            if (IsLocalConnection(connection))
                            {
                                EditorApplication.delayCall += () => Process.Start(localLogPath);
                            }
                            else
                            {

                                EditorApplication.delayCall += () => OpenLogFileForIPs(portalConnections);
                            }
                        }
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

        private void UpdatePortalConnections()
        {
            targetIps = new string[portalConnections.Connections.Count];
            if (currentConnectionInfoIndex > portalConnections.Connections.Count - 1)
            {
                currentConnectionInfoIndex = portalConnections.Connections.Count - 1;
            }

            targetIps[0] = "Local Machine";
            for (int i = 1; i < targetIps.Length; i++)
            {
                targetIps[i] = portalConnections.Connections[i].MachineName;
            }
        }

        private static bool IsLocalConnection(ConnectInfo connection)
        {
            return connection.IP.Contains("Local Machine") || connection.IP.Contains("127.0.0.1");
        }

        private static bool IsCredentialsValid(ConnectInfo connection)
        {
            return !string.IsNullOrEmpty(connection.User) && !string.IsNullOrEmpty(connection.IP);
        }

        private static bool IsValidIpAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            if (ip.Contains("Local Machine"))
            {
                return true;
            }

            if (ip.Contains("0.0.0.0"))
            {
                return false;
            }

            var subAddresses = ip.Split('.');

            if (subAddresses.Length > 3)
            {
                if (subAddresses.Any(subAddress => !subAddress.Equals("0")))
                {
                    return true;
                }
            }

            return false;
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
            Debug.Log(packageFamilyName);

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            // Uninstall
            if (BuildDeployPrefs.FullReinstall)
            {
                UninstallAppOnDevicesList(targetList);
            }

            try
            {
                for (int i = 0; i < targetList.Connections.Count; i++)
                {
                    if (IsLocalConnection(targetList.Connections[i]) && !IsHoloLensConnectedUsb || buildPath.Contains("x64"))
                    {
                        FileInfo[] installerFiles = new DirectoryInfo(buildPath).GetFiles("*.ps1");
                        if (installerFiles.Length == 1)
                        {
                            var pInfo = new ProcessStartInfo
                            {
                                FileName = "powershell.exe",
                                CreateNoWindow = false,
                                Arguments = string.Format("-executionpolicy bypass -File \"{0}\"", installerFiles[0].FullName)
                            };

                            var process = new Process { StartInfo = pInfo };

                            process.Start();
                        }

                        continue;
                    }

                    if (buildPath.Contains("x64"))
                    {
                        return;
                    }

                    EditorUtility.DisplayProgressBar("Installing on devices",
                                        string.Format("Installing ({0})", targetList.Connections[i].MachineName), i / (float)targetList.Connections.Count);

                    // Get the appx path
                    FileInfo[] files = new DirectoryInfo(buildPath).GetFiles("*.appx");
                    files = files.Length == 0 ? new DirectoryInfo(buildPath).GetFiles("*.appxbundle") : files;

                    if (files.Length == 0)
                    {
                        Debug.LogErrorFormat("No APPX found in folder build folder ({0})", buildPath);
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

                    if (IsLocalConnection(targetList.Connections[i]) && !IsHoloLensConnectedUsb)
                    {
                        var pInfo = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            CreateNoWindow = true,
                            Arguments = string.Format("-windowstyle hidden -nologo Get-AppxPackage *{0}* | Remove-AppxPackage", packageFamilyName)
                        };

                        var process = new Process { StartInfo = pInfo };

                        process.Start();

                        continue;
                    }

                    if (BuildDeployPortal.IsAppInstalled(packageFamilyName, targetList.Connections[i]))
                    {
                        EditorUtility.DisplayProgressBar("Installing on devices", string.Format("Uninstalling ({0})", ip), i / (float)targetList.Connections.Count);
                        BuildDeployPortal.UninstallApp(packageFamilyName, targetList.Connections[i], false);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

            EditorUtility.ClearProgressBar();
        }

        private static void OpenDevicePortal(DevicePortalConnections targetDevices)
        {
            MachineName usbMachine = null;

            if (IsHoloLensConnectedUsb)
            {
                usbMachine = BuildDeployPortal.GetMachineName(targetDevices.Connections.FirstOrDefault(targetDevice => targetDevice.IP.Contains("Local Machine")));
            }

            for (int i = 0; i < targetDevices.Connections.Count; i++)
            {
                if (targetDevices.Connections[i].IP.Contains("Local Machine") && !IsHoloLensConnectedUsb)
                {
                    continue;
                }

                MachineName targetMachine = BuildDeployPortal.GetMachineName(targetDevices.Connections[i]);

                if (IsHoloLensConnectedUsb)
                {
                    if (targetDevices.Connections[i].IP.Contains("Local Machine") ||
                        usbMachine != null && usbMachine.ComputerName != targetMachine.ComputerName)
                    {
                        BuildDeployPortal.OpenWebPortal(targetDevices.Connections[i]);
                    }
                }
                else
                {
                    if (!targetDevices.Connections[i].IP.Contains("Local Machine") || usbMachine != null)
                    {
                        BuildDeployPortal.OpenWebPortal(targetDevices.Connections[i]);
                    }
                }
            }
        }

        private static void LaunchAppOnIPs(DevicePortalConnections targetDevices)
        {
            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            for (int i = 0; i < targetDevices.Connections.Count; i++)
            {
                if (IsLocalConnection(targetDevices.Connections[i]) && !IsHoloLensConnectedUsb)
                {
                    continue;
                }

                if (BuildDeployPortal.IsAppRunning(PlayerSettings.productName, targetDevices.Connections[i]))
                {
                    Debug.LogFormat(" {0} is already running on {1}", packageFamilyName, targetDevices.Connections[i].MachineName);
                    return;
                }

                Debug.LogFormat("Launching {0} on {1}", packageFamilyName, targetDevices.Connections[i].MachineName);
                BuildDeployPortal.LaunchApp(packageFamilyName, targetDevices.Connections[i], false);
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
                if ((!IsLocalConnection(targetDevices.Connections[i]) || IsHoloLensConnectedUsb) &&
                     BuildDeployPortal.IsAppRunning(PlayerSettings.productName, targetDevices.Connections[i]))
                {
                    BuildDeployPortal.KillApp(packageFamilyName, targetDevices.Connections[i], false);
                }
            }
        }

        private static void OpenLogFileForIPs(DevicePortalConnections targetDevices)
        {
            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            for (int i = 0; i < targetDevices.Connections.Count; i++)
            {
                if (!IsLocalConnection(targetDevices.Connections[i]) || IsHoloLensConnectedUsb)
                {
                    BuildDeployPortal.DeviceLogFile_View(packageFamilyName, targetDevices.Connections[i]);
                }
            }
        }

        #endregion // Methods
    }
}
