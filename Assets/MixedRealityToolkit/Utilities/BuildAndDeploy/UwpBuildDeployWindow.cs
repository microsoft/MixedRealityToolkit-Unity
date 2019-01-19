// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Editor.Usb;
using Microsoft.MixedReality.Toolkit.Core.Utilities.WebRequestRest;
using Microsoft.MixedReality.Toolkit.Core.Utilities.WindowsDevicePortal;
using Microsoft.MixedReality.Toolkit.Core.Utilities.WindowsDevicePortal.DataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using FileInfo = System.IO.FileInfo;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Build
{
    /// <summary>
    /// Build window - supports SLN creation, APPX from SLN, Deploy on device, and misc helper utilities associated with the build/deploy/test iteration loop
    /// Requires the device to be set in developer mode and to have secure connections disabled (in the security tab in the device portal)
    /// </summary>
    public class UwpBuildDeployWindow : EditorWindow
    {
        #region Internal Types

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

        #endregion Internal Types

        #region Constants and Readonly Values

        private const string LocalMachine = "Local Machine";

        private const string LocalIpAddress = "127.0.0.1";

        private const string EmptyIpAddress = "0.0.0.0";

        private const float UpdateBuildsPeriod = 1.0f;

        private const string SdkVersion = "10.0.17134.0";

        private readonly string[] tabNames = { "Unity Build Options", "Appx Build Options", "Deploy Options" };

        private readonly string[] scriptingBackendNames = { "IL2CPP", ".NET" };

        private readonly int[] scriptingBackendEnum = { (int)ScriptingImplementation.IL2CPP, (int)ScriptingImplementation.WinRTDotNET };

        private readonly string[] deviceNames = { "Any Device", "PC", "Mobile", "HoloLens" };

        private static readonly List<string> builds = new List<string>(0);

        private static readonly List<string> AppPackageDirectories = new List<string>(0);

        #endregion Constants and Readonly Values

        #region Labels

        private readonly GUIContent buildAllThenInstallLabel = new GUIContent("Build all, then Install", "Builds the Unity Project, the APPX, then installs to the target device.");

        private readonly GUIContent buildAllLabel = new GUIContent("Build all", "Builds the Unity Project and APPX");

        private readonly GUIContent buildDirectoryLabel = new GUIContent("Build Directory", "It's recommended to use 'UWP'");

        private readonly GUIContent useCSharpProjectsLabel = new GUIContent("Generate C# Debug", "Generate C# Project References for debugging.\nOnly available in .NET Scripting runtime.");

        private readonly GUIContent autoIncrementLabel = new GUIContent("Auto Increment", "Increases Version Build Number");

        private readonly GUIContent versionNumberLabel = new GUIContent("Version Number", "Major.Minor.Build.Revision\nNote: Revision should always be zero because it's reserved by Windows Store.");

        private readonly GUIContent pairHoloLensUsbLabel = new GUIContent("Pair HoloLens", "Pairs the USB connected HoloLens with the Build Window so you can deploy via USB");

        private readonly GUIContent useSSLLabel = new GUIContent("Use SSL?", "Use SLL to communicate with Device Portal");

        private readonly GUIContent addConnectionLabel = new GUIContent("+", "Add a remote connection");

        private readonly GUIContent removeConnectionLabel = new GUIContent("-", "Remove a remote connection");

        private readonly GUIContent ipAddressLabel = new GUIContent("IpAddress", "Note: Local Machine will install on any HoloLens connected to USB as well.");

        private readonly GUIContent doAllLabel = new GUIContent(" Do actions on all devices", "Should the build options perform actions on all the connected devices?");

        private readonly GUIContent uninstallLabel = new GUIContent("Uninstall First", "Uninstall application before installing");

        #endregion Labels

        #region Properties

        private static bool IsValidSdkInstalled { get; set; } = true;

        private static bool ShouldOpenSLNBeEnabled => !string.IsNullOrEmpty(BuildDeployPreferences.BuildDirectory);

        private static bool ShouldBuildSLNBeEnabled => !isBuilding &&
                                                       !UwpAppxBuildTools.IsBuilding &&
                                                       !BuildPipeline.isBuildingPlayer &&
                                                       !string.IsNullOrEmpty(BuildDeployPreferences.BuildDirectory);

        private static bool ShouldBuildAppxBeEnabled => ShouldBuildSLNBeEnabled &&
                                                        !string.IsNullOrEmpty(BuildDeployPreferences.BuildDirectory) &&
                                                        !string.IsNullOrEmpty(BuildDeployPreferences.BuildConfig);

        private static bool DevicePortalConnectionEnabled => (portalConnections.Connections.Count > 1 || IsHoloLensConnectedUsb) &&
                                                             !string.IsNullOrEmpty(BuildDeployPreferences.BuildDirectory);

        private static bool CanInstall
        {
            get
            {
                bool canInstall = true;
                if (EditorUserBuildSettings.wsaSubtarget == WSASubtarget.HoloLens)
                {
                    canInstall = DevicePortalConnectionEnabled;
                }

                return canInstall && Directory.Exists(BuildDeployPreferences.AbsoluteBuildDirectory) && !string.IsNullOrEmpty(PackageName);
            }
        }

        private static string PackageName { get; set; }

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

        #endregion Properties

        #region Fields

        private const float HalfWidth = 256f;

        private static float timeLastUpdatedBuilds;

        private string[] targetIps;
        private string[] windowsSdkPaths;

        private Vector2 scrollPosition;

        private BuildDeployTab currentTab = BuildDeployTab.UnityBuildOptions;

        private static bool isBuilding;
        private static bool isAppRunning;

        [SerializeField]
        private int lastSessionConnectionInfoIndex;
        private static int currentConnectionInfoIndex = 0;
        private static DevicePortalConnections portalConnections = null;

        #endregion Fields

        #region Methods

        [MenuItem("Mixed Reality Toolkit/Build Window", false, 0)]
        public static void OpenWindow()
        {
            // Dock it next to the Scene View.
            var window = GetWindow<UwpBuildDeployWindow>(typeof(SceneView));
            window.titleContent = new GUIContent("Build Window");
            window.Show();
        }

        private void OnEnable()
        {
            Setup();
        }

        private void OnDestroy()
        {
            lastSessionConnectionInfoIndex = currentConnectionInfoIndex;
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

            currentConnectionInfoIndex = lastSessionConnectionInfoIndex;
            portalConnections = JsonUtility.FromJson<DevicePortalConnections>(BuildDeployPreferences.DevicePortalConnections);
            UpdatePortalConnections();
        }

        private void OnGUI()
        {
            #region Quick Options

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WSAPlayer)
            {
                EditorGUILayout.HelpBox("Build window only available for UWP build target.", MessageType.Warning);
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();

                // Build directory (and save setting, if it's changed)
                string curBuildDirectory = BuildDeployPreferences.BuildDirectory;
                EditorGUILayout.LabelField(buildDirectoryLabel, GUILayout.Width(96));
                string newBuildDirectory = EditorGUILayout.TextField(curBuildDirectory, GUILayout.Width(64), GUILayout.ExpandWidth(true));

                if (newBuildDirectory != curBuildDirectory)
                {
                    BuildDeployPreferences.BuildDirectory = newBuildDirectory;
                }

                GUI.enabled = Directory.Exists(BuildDeployPreferences.AbsoluteBuildDirectory);

                if (GUILayout.Button("Open Build Directory"))
                {
                    EditorApplication.delayCall += () => Process.Start(BuildDeployPreferences.AbsoluteBuildDirectory);
                }

                GUI.enabled = true;

                if (GUILayout.Button("Open Player Settings"))
                {
                    EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.Label("Quick Options");
            EditorGUILayout.BeginHorizontal();

            EditorUserBuildSettings.wsaSubtarget = (WSASubtarget)EditorGUILayout.Popup((int)EditorUserBuildSettings.wsaSubtarget, deviceNames);

            bool canInstall = CanInstall;

            if (EditorUserBuildSettings.wsaSubtarget == WSASubtarget.HoloLens && !IsHoloLensConnectedUsb)
            {
                canInstall = IsHoloLensConnectedUsb;
            }

            GUI.enabled = ShouldBuildSLNBeEnabled;

            // Build & Run button...
            if (GUILayout.Button(CanInstall ? buildAllThenInstallLabel : buildAllLabel, GUILayout.Width(HalfWidth), GUILayout.ExpandWidth(true)))
            {
                EditorApplication.delayCall += () => BuildAll(canInstall);
            }

            GUI.enabled = true;

            if (GUILayout.Button("Open Player Settings"))
            {
                // TODO: Remove use of "Unsupported" class.
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10);

            #endregion Quick Options

            currentTab = (BuildDeployTab)GUILayout.Toolbar(SessionState.GetInt("_BuildWindow_Tab", (int)currentTab), tabNames);
            SessionState.SetInt("_BuildWindow_Tab", (int)currentTab);

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

            Repaint();
        }

        private void UnityBuildGUI()
        {
            GUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

            // Build directory (and save setting, if it's changed)
            string curBuildDirectory = BuildDeployPreferences.BuildDirectory;
            EditorGUILayout.LabelField(buildDirectoryLabel, GUILayout.Width(96));
            string newBuildDirectory = EditorGUILayout.TextField(curBuildDirectory, GUILayout.Width(64), GUILayout.ExpandWidth(true));

            if (newBuildDirectory != curBuildDirectory)
            {
                BuildDeployPreferences.BuildDirectory = newBuildDirectory;
            }

            GUI.enabled = Directory.Exists(BuildDeployPreferences.AbsoluteBuildDirectory);

            if (GUILayout.Button("Open Build Directory", GUILayout.Width(HalfWidth)))
            {
                EditorApplication.delayCall += () => Process.Start(BuildDeployPreferences.AbsoluteBuildDirectory);
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // Generate C# Project References for debugging
            bool generateReferenceProjects = EditorUserBuildSettings.wsaGenerateReferenceProjects;

            var curScriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA);

            GUI.enabled = curScriptingBackend == ScriptingImplementation.WinRTDotNET;
            bool shouldGenerateProjects = EditorGUILayout.Toggle(useCSharpProjectsLabel, generateReferenceProjects);
            GUI.enabled = true;

            if (shouldGenerateProjects != generateReferenceProjects)
            {
                EditorUserBuildSettings.wsaGenerateReferenceProjects = shouldGenerateProjects;
            }

            GUILayout.FlexibleSpace();

            GUI.enabled = ShouldOpenSLNBeEnabled;

            if (GUILayout.Button("Open in Visual Studio", GUILayout.Width(HalfWidth)))
            {
                // Open SLN
                string slnFilename = Path.Combine(BuildDeployPreferences.BuildDirectory, $"{PlayerSettings.productName}.sln");

                if (File.Exists(slnFilename))
                {
                    EditorApplication.delayCall += () => Process.Start(new FileInfo(slnFilename).FullName);
                }
                else if (EditorUtility.DisplayDialog(
                    "Solution Not Found",
                    "We couldn't find the Project's Solution. Would you like to Build the project now?",
                    "Yes, Build", "No"))
                {
                    EditorApplication.delayCall += BuildUnityProject;
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            // Build Unity Player
            GUI.enabled = ShouldBuildSLNBeEnabled;

            if (GUILayout.Button("Build Unity Project"))
            {
                EditorApplication.delayCall += BuildUnityProject;
            }

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        private void AppxBuildGUI()
        {
            GUILayout.BeginVertical();

            // SDK and MS Build Version(and save setting, if it's changed)
            string currentSDKVersion = EditorUserBuildSettings.wsaUWPSDK;

            int currentSDKVersionIndex = -1;

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

            EditorGUILayout.HelpBox("Required SDK Version: " + SdkVersion, MessageType.Info);

            // Throw exception if user has no Windows 10 SDK installed
            if (currentSDKVersionIndex < 0)
            {
                if (IsValidSdkInstalled)
                {
                    Debug.LogError($"Unable to find the required Windows 10 SDK Target!\nPlease be sure to install the {SdkVersion} SDK from Visual Studio Installer.");
                }

                EditorGUILayout.HelpBox($"Unable to find the required Windows 10 SDK Target!\nPlease be sure to install the {SdkVersion} SDK from Visual Studio Installer.", MessageType.Error);
                GUILayout.EndVertical();
                IsValidSdkInstalled = false;
                return;
            }

            IsValidSdkInstalled = true;

            var curScriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA);

            if (curScriptingBackend == ScriptingImplementation.WinRTDotNET)
            {
                EditorGUILayout.HelpBox(".NET Scripting backend is depreciated, please use IL2CPP.", MessageType.Warning);
            }

            var newScriptingBackend = (ScriptingImplementation)EditorGUILayout.IntPopup("Scripting Backend", (int)curScriptingBackend, scriptingBackendNames, scriptingBackendEnum, GUILayout.Width(HalfWidth));

            if (newScriptingBackend != curScriptingBackend)
            {
                bool canUpdate = !Directory.Exists(BuildDeployPreferences.AbsoluteBuildDirectory);

                if (!canUpdate &&
                    EditorUtility.DisplayDialog("Attention!",
                        $"Build path contains project built with {newScriptingBackend.ToString()} scripting backend, while current project is using {curScriptingBackend.ToString()} scripting backend.\n\nSwitching to a new scripting backend requires us to delete all the data currently in your build folder and rebuild the Unity Player!",
                        "Okay", "Cancel"))
                {
                    Directory.Delete(BuildDeployPreferences.AbsoluteBuildDirectory, true);
                    canUpdate = true;
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

            // Build config (and save setting, if it's changed)
            string curBuildConfigString = BuildDeployPreferences.BuildConfig;

            WSABuildType buildConfigOption;
            if (curBuildConfigString.ToLower().Equals("master"))
            {
                buildConfigOption = WSABuildType.Master;
            }
            else if (curBuildConfigString.ToLower().Equals("release"))
            {
                buildConfigOption = WSABuildType.Release;
            }
            else
            {
                buildConfigOption = WSABuildType.Debug;
            }

            EditorUserBuildSettings.GetWSADotNetNative(buildConfigOption);
            buildConfigOption = (WSABuildType)EditorGUILayout.EnumPopup("Build Configuration", buildConfigOption, GUILayout.Width(HalfWidth));

            string buildConfigString = buildConfigOption.ToString();

            if (buildConfigString != curBuildConfigString)
            {
                BuildDeployPreferences.BuildConfig = buildConfigString;
            }

            // Build Platform (and save setting, if it's changed)
            string curBuildPlatformString = BuildDeployPreferences.BuildPlatform;
            var buildPlatformOption = BuildPlatformEnum.x86;

            if (curBuildPlatformString.ToLower().Equals("x86"))
            {
                buildPlatformOption = BuildPlatformEnum.x86;
            }
            else if (curBuildPlatformString.ToLower().Equals("x64"))
            {
                buildPlatformOption = BuildPlatformEnum.x64;
            }

            buildPlatformOption = (BuildPlatformEnum)EditorGUILayout.EnumPopup("Build Platform", buildPlatformOption, GUILayout.Width(HalfWidth));

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
                BuildDeployPreferences.BuildPlatform = newBuildPlatformString;
            }

            GUILayout.BeginHorizontal();

            var previousLabelWidth = EditorGUIUtility.labelWidth;

            // Auto Increment version
            EditorGUIUtility.labelWidth = 96;
            bool curIncrementVersion = BuildDeployPreferences.IncrementBuildVersion;
            bool newIncrementVersion = EditorGUILayout.Toggle(autoIncrementLabel, curIncrementVersion);

            // Restore previous label width
            EditorGUIUtility.labelWidth = previousLabelWidth;

            if (newIncrementVersion != curIncrementVersion)
            {
                BuildDeployPreferences.IncrementBuildVersion = newIncrementVersion;
            }

            EditorGUILayout.LabelField(versionNumberLabel, GUILayout.Width(96));
            Vector3 newVersion = Vector3.zero;

            EditorGUI.BeginChangeCheck();

            newVersion.x = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Major);
            newVersion.y = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Minor);
            newVersion.z = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Build);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.WSA.packageVersion = new Version((int)newVersion.x, (int)newVersion.y, (int)newVersion.z, 0);
            }

            GUI.enabled = false;
            EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Revision);
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Open AppX packages location
            string appxDirectory = curScriptingBackend == ScriptingImplementation.IL2CPP ? $"/AppPackages/{PlayerSettings.productName}" : $"/{PlayerSettings.productName}/AppPackages";
            string appxBuildPath = Path.GetFullPath($"{BuildDeployPreferences.BuildDirectory}{appxDirectory}");
            GUI.enabled = builds.Count > 0 && !string.IsNullOrEmpty(appxBuildPath);

            if (GUILayout.Button("Open APPX Packages Location", GUILayout.Width(HalfWidth)))
            {
                EditorApplication.delayCall += () => Process.Start("explorer.exe", $"/f /open,{appxBuildPath}");
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Force rebuild
            previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;
            bool curForceRebuildAppx = BuildDeployPreferences.ForceRebuild;
            bool newForceRebuildAppx = EditorGUILayout.Toggle("Rebuild", curForceRebuildAppx);

            if (newForceRebuildAppx != curForceRebuildAppx)
            {
                BuildDeployPreferences.ForceRebuild = newForceRebuildAppx;
            }

            // Restore previous label width
            EditorGUIUtility.labelWidth = previousLabelWidth;

            // Build APPX
            GUI.enabled = ShouldBuildAppxBeEnabled;

            if (GUILayout.Button("Build APPX", GUILayout.Width(HalfWidth)))
            {
                // Check if solution exists
                string slnFilename = Path.Combine(BuildDeployPreferences.BuildDirectory, $"{PlayerSettings.productName}.sln");

                if (File.Exists(slnFilename))
                {
                    EditorApplication.delayCall += BuildAppx;
                }
                else if (EditorUtility.DisplayDialog("Solution Not Found", "We couldn't find the solution. Would you like to Build it?", "Yes, Build", "No"))
                {
                    EditorApplication.delayCall += () => BuildAll(install: false);
                }

                GUI.enabled = true;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DeployGUI()
        {
            Debug.Assert(portalConnections.Connections.Count != 0);
            Debug.Assert(currentConnectionInfoIndex >= 0);

            if (currentConnectionInfoIndex > portalConnections.Connections.Count - 1)
            {
                currentConnectionInfoIndex = 0;
            }

            GUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();

            GUI.enabled = IsHoloLensConnectedUsb;

            if (GUILayout.Button(pairHoloLensUsbLabel, GUILayout.Width(128f)))
            {
                EditorApplication.delayCall += PairDevice;
            }

            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 64;
            bool useSSL = EditorGUILayout.Toggle(useSSLLabel, BuildDeployPreferences.UseSSL);
            EditorGUIUtility.labelWidth = previousLabelWidth;

            currentConnectionInfoIndex = EditorGUILayout.Popup(currentConnectionInfoIndex, targetIps);

            var currentConnection = portalConnections.Connections[currentConnectionInfoIndex];
            bool currentConnectionIsLocal = IsLocalConnection(currentConnection);

            if (currentConnectionIsLocal)
            {
                currentConnection.MachineName = LocalMachine;
            }

            GUI.enabled = IsValidIpAddress(currentConnection.IP);

            if (GUILayout.Button(addConnectionLabel, GUILayout.Width(20)))
            {
                portalConnections.Connections.Add(new DeviceInfo(EmptyIpAddress, currentConnection.User, currentConnection.Password));
                currentConnectionInfoIndex++;
                currentConnection = portalConnections.Connections[currentConnectionInfoIndex];
                UpdatePortalConnections();
            }

            GUI.enabled = portalConnections.Connections.Count > 1 && currentConnectionInfoIndex != 0;

            if (GUILayout.Button(removeConnectionLabel, GUILayout.Width(20)))
            {
                portalConnections.Connections.RemoveAt(currentConnectionInfoIndex);
                currentConnectionInfoIndex--;
                currentConnection = portalConnections.Connections[currentConnectionInfoIndex];
                UpdatePortalConnections();
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label(currentConnection.MachineName, GUILayout.Width(HalfWidth));

            GUILayout.EndHorizontal();

            previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 64;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = !currentConnectionIsLocal;
            currentConnection.IP = EditorGUILayout.TextField(ipAddressLabel, currentConnection.IP, GUILayout.Width(HalfWidth));
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            currentConnection.User = EditorGUILayout.TextField("Username", currentConnection.User, GUILayout.Width(HalfWidth));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            currentConnection.Password = EditorGUILayout.PasswordField("Password", currentConnection.Password, GUILayout.Width(HalfWidth));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            EditorGUIUtility.labelWidth = 152;

            bool processAll = EditorGUILayout.Toggle(doAllLabel, BuildDeployPreferences.TargetAllConnections, GUILayout.Width(176));

            EditorGUIUtility.labelWidth = 86;

            bool fullReinstall = EditorGUILayout.Toggle(uninstallLabel, BuildDeployPreferences.FullReinstall, GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = previousLabelWidth;

            if (EditorGUI.EndChangeCheck())
            {
                BuildDeployPreferences.TargetAllConnections = processAll;
                BuildDeployPreferences.FullReinstall = fullReinstall;
                BuildDeployPreferences.UseSSL = useSSL;
                Rest.UseSSL = useSSL;

                // Format our local connection
                if (currentConnection.IP.Contains(LocalIpAddress))
                {
                    currentConnection.IP = LocalMachine;
                }

                portalConnections.Connections[currentConnectionInfoIndex] = currentConnection;
                UpdatePortalConnections();
                Repaint();
            }

            GUILayout.FlexibleSpace();

            // Connect
            if (!IsLocalConnection(currentConnection))
            {
                GUI.enabled = IsValidIpAddress(currentConnection.IP) && IsCredentialsValid(currentConnection);

                if (GUILayout.Button("Connect"))
                {
                    EditorApplication.delayCall += () =>
                    {
                        ConnectToDevice(currentConnection);
                    };
                }

                GUI.enabled = true;
            }

            GUI.enabled = DevicePortalConnectionEnabled && CanInstall;

            // Open web portal
            if (GUILayout.Button("Open Device Portal", GUILayout.Width(128f)))
            {
                EditorApplication.delayCall += () => OpenDevicePortal(portalConnections, currentConnection);
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
                        EditorApplication.delayCall += () =>
                        {
                            if (processAll)
                            {
                                InstallAppOnDevicesList(fullBuildLocation, portalConnections);
                            }
                            else
                            {
                                InstallOnTargetDevice(fullBuildLocation, currentConnection);
                            }
                        };
                    }

                    GUI.enabled = true;

                    // Uninstall...
                    GUI.enabled = CanInstall;

                    if (GUILayout.Button("Uninstall", GUILayout.Width(96)))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            if (processAll)
                            {
                                UninstallAppOnDevicesList(portalConnections);
                            }
                            else
                            {
                                UninstallAppOnTargetDevice(currentConnection);
                            }
                        };
                    }

                    GUI.enabled = true;

                    bool canLaunchLocal = currentConnectionInfoIndex == 0 && IsHoloLensConnectedUsb;
                    bool canLaunchRemote = DevicePortalConnectionEnabled && CanInstall && currentConnectionInfoIndex != 0;

                    // Launch app...
                    GUI.enabled = canLaunchLocal || canLaunchRemote;

                    if (GUILayout.Button(new GUIContent(isAppRunning ? "Kill App" : "Launch App", "These are remote commands only"), GUILayout.Width(96)))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            if (isAppRunning)
                            {
                                if (processAll)
                                {
                                    KillAppOnDeviceList(portalConnections);
                                    isAppRunning = false;
                                }
                                else
                                {
                                    KillAppOnTargetDevice(currentConnection);
                                }
                            }
                            else
                            {
                                if (processAll)
                                {
                                    LaunchAppOnDeviceList(portalConnections);
                                    isAppRunning = true;
                                }
                                else
                                {
                                    LaunchAppOnTargetDevice(currentConnection);
                                }
                            }
                        };
                    }

                    GUI.enabled = true;

                    // Log file
                    string localLogPath = $"%USERPROFILE%\\AppData\\Local\\Packages\\{PlayerSettings.productName}\\TempState\\UnityPlayer.log";
                    bool localLogExists = File.Exists(localLogPath);

                    GUI.enabled = localLogExists || canLaunchRemote || canLaunchLocal;

                    if (GUILayout.Button("View Log", GUILayout.Width(96)))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            if (processAll)
                            {
                                OpenLogFilesOnDeviceList(portalConnections, localLogPath);
                            }
                            else
                            {
                                OpenLogFileForTargetDevice(currentConnection, localLogPath);
                            }
                        };
                    }

                    GUI.enabled = true;

                    GUILayout.Space(8);
                    GUILayout.Label(new GUIContent($"{packageName} ({directoryDate})"));
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }

        #endregion Methods

        #region Utilities

        private async void ConnectToDevice(DeviceInfo currentConnection)
        {
            var machineName = await DevicePortal.GetMachineNameAsync(currentConnection);
            currentConnection.MachineName = machineName?.ComputerName;
            portalConnections.Connections[currentConnectionInfoIndex] = currentConnection;
            UpdatePortalConnections();
            Repaint();
        }

        private async void PairDevice()
        {
            DeviceInfo newConnection = null;

            for (var i = 0; i < portalConnections.Connections.Count; i++)
            {
                var targetDevice = portalConnections.Connections[i];
                if (!IsLocalConnection(targetDevice))
                {
                    continue;
                }

                var machineName = await DevicePortal.GetMachineNameAsync(targetDevice);
                var networkInfo = await DevicePortal.GetIpConfigInfoAsync(targetDevice);

                if (machineName != null && networkInfo != null)
                {
                    var newIps = new List<string>();
                    foreach (var adapter in networkInfo.Adapters)
                    {
                        newIps.AddRange(from address in adapter.IpAddresses
                                        where !address.IpAddress.Contains(EmptyIpAddress)
                                        select address.IpAddress);
                    }

                    if (newIps.Count == 0)
                    {
                        Debug.LogWarning("This HoloLens is not connected to any networks and cannot be paired.");
                    }

                    for (var j = 0; j < newIps.Count; j++)
                    {
                        var newIp = newIps[j];
                        if (portalConnections.Connections.Any(connection => connection.IP == newIp))
                        {
                            Debug.LogFormat("Already paired");
                            continue;
                        }

                        newConnection = new DeviceInfo(newIp, targetDevice.User, targetDevice.Password, machineName.ComputerName);
                    }
                }
            }

            if (newConnection != null && IsValidIpAddress(newConnection.IP))
            {
                portalConnections.Connections.Add(newConnection);
                DeviceInfo removedDevice = null;
                for (var i = 0; i < portalConnections.Connections.Count; i++)
                {
                    if (portalConnections.Connections[i].IP == newConnection.IP)
                    {
                        currentConnectionInfoIndex = i;
                    }

                    // If we were trying to add a new device, but didn't finish before pressing pair, let's remove it.
                    if (!IsValidIpAddress(portalConnections.Connections[i].IP))
                    {
                        removedDevice = portalConnections.Connections[i];
                    }
                }

                if (removedDevice != null)
                {
                    portalConnections.Connections.Remove(removedDevice);
                }

                UpdatePortalConnections();
            }
        }

        public static void BuildUnityProject()
        {
            Debug.Assert(!isBuilding);
            isBuilding = true;
            UwpAppxBuildTools.BuildUnityPlayer(BuildDeployPreferences.BuildDirectory);
            isBuilding = false;
        }

        public static async void BuildAppx()
        {
            Debug.Assert(!isBuilding);
            isBuilding = true;
            EditorAssemblyReloadManager.LockReloadAssemblies = true;
            await UwpAppxBuildTools.BuildAppxAsync(
                PlayerSettings.productName,
                BuildDeployPreferences.ForceRebuild,
                BuildDeployPreferences.BuildConfig,
                BuildDeployPreferences.BuildPlatform,
                BuildDeployPreferences.BuildDirectory,
                BuildDeployPreferences.IncrementBuildVersion);
            EditorAssemblyReloadManager.LockReloadAssemblies = false;
            isBuilding = false;
        }

        public static async void BuildAll(bool install = true)
        {
            Debug.Assert(!isBuilding);
            isBuilding = true;
            EditorAssemblyReloadManager.LockReloadAssemblies = true;

            // First build SLN
            if (UwpAppxBuildTools.BuildUnityPlayer(BuildDeployPreferences.BuildDirectory, false))
            {
                if (await UwpAppxBuildTools.BuildAppxAsync(
                    PlayerSettings.productName,
                    BuildDeployPreferences.ForceRebuild,
                    BuildDeployPreferences.BuildConfig,
                    BuildDeployPreferences.BuildPlatform,
                    BuildDeployPreferences.BuildDirectory,
                    BuildDeployPreferences.IncrementBuildVersion))
                {
                    if (install)
                    {
                        string fullBuildLocation = CalcMostRecentBuild();

                        if (BuildDeployPreferences.TargetAllConnections)
                        {
                            await InstallAppOnDevicesListAsync(fullBuildLocation, portalConnections);
                        }
                        else
                        {
                            await InstallOnTargetDeviceAsync(fullBuildLocation, portalConnections.Connections[currentConnectionInfoIndex]);
                        }
                    }
                }
            }

            EditorAssemblyReloadManager.LockReloadAssemblies = false;
            isBuilding = false;
        }

        private static void UpdateBuilds()
        {
            builds.Clear();

            var curScriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA);
            string appxDirectory = curScriptingBackend == ScriptingImplementation.IL2CPP ? $"AppPackages\\{PlayerSettings.productName}" : $"{PlayerSettings.productName}\\AppPackages";

            try
            {
                AppPackageDirectories.Clear();
                string[] buildList = Directory.GetDirectories(BuildDeployPreferences.AbsoluteBuildDirectory, "*", SearchOption.AllDirectories);
                foreach (string appBuild in buildList)
                {
                    if (appBuild.Contains(appxDirectory) && !appBuild.Contains($"{appxDirectory}\\"))
                    {
                        AppPackageDirectories.AddRange(Directory.GetDirectories(appBuild));
                    }
                }

                IEnumerable<string> selectedDirectories =
                    from string directory in AppPackageDirectories
                    orderby Directory.GetLastWriteTime(directory) descending
                    select Path.GetFullPath(directory);
                builds.AddRange(selectedDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                // unused
            }

            UpdatePackageName();

            timeLastUpdatedBuilds = Time.realtimeSinceStartup;
        }

        private static string CalcMostRecentBuild()
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

            targetIps[0] = LocalMachine;
            for (int i = 1; i < targetIps.Length; i++)
            {
                if (string.IsNullOrEmpty(portalConnections.Connections[i].MachineName))
                {
                    portalConnections.Connections[i].MachineName = portalConnections.Connections[i].IP;
                }

                targetIps[i] = portalConnections.Connections[i].MachineName;
            }

            var devicePortalConnections = new DevicePortalConnections();
            for (var i = 0; i < portalConnections.Connections.Count; i++)
            {
                devicePortalConnections.Connections.Add(portalConnections.Connections[i]);
            }

            for (var i = 0; i < portalConnections.Connections.Count; i++)
            {
                if (!IsValidIpAddress(devicePortalConnections.Connections[i].IP))
                {
                    devicePortalConnections.Connections.RemoveAt(i);
                }
            }

            BuildDeployPreferences.DevicePortalConnections = JsonUtility.ToJson(devicePortalConnections);
            lastSessionConnectionInfoIndex = currentConnectionInfoIndex;
            Repaint();
        }

        private static bool IsLocalConnection(DeviceInfo connection)
        {
            return connection.IP.Contains(LocalMachine) ||
                   connection.IP.Contains(LocalIpAddress);
        }

        private static bool IsCredentialsValid(DeviceInfo connection)
        {
            return !string.IsNullOrEmpty(connection.User) &&
                   !string.IsNullOrEmpty(connection.IP);
        }

        private static bool IsValidIpAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip) || ip.Contains(EmptyIpAddress))
            {
                return false;
            }

            if (ip.Contains(LocalMachine))
            {
                return true;
            }

            var subAddresses = ip.Split('.');
            return subAddresses.Length > 3;
        }

        private static string UpdatePackageName()
        {
            if (AppPackageDirectories.Count == 0)
            {
                return string.Empty;
            }

            // Find the manifest
            string[] manifests = Directory.GetFiles(BuildDeployPreferences.AbsoluteBuildDirectory, "Package.appxmanifest", SearchOption.AllDirectories);

            if (manifests.Length == 0)
            {
                Debug.LogError($"Unable to find manifest file for build (in path - {BuildDeployPreferences.AbsoluteBuildDirectory})");
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
                                        return PackageName = reader.Value;
                                    }
                                }
                            }

                            break;
                    }
                }
            }

            Debug.LogError($"Unable to find PackageFamilyName in manifest file ({manifest})");
            return string.Empty;
        }

        #endregion Utilities

        #region Device Portal Commands

        private static async void OpenDevicePortal(DevicePortalConnections targetDevices, DeviceInfo currentConnection)
        {
            MachineName usbMachine = null;

            if (IsHoloLensConnectedUsb)
            {
                usbMachine = await DevicePortal.GetMachineNameAsync(targetDevices.Connections.FirstOrDefault(targetDevice => targetDevice.IP.Contains("Local Machine")));
            }


            for (int i = 0; i < targetDevices.Connections.Count; i++)
            {
                bool isLocalMachine = IsLocalConnection(targetDevices.Connections[i]);
                bool isTargetedConnection = currentConnection.IP == targetDevices.Connections[i].IP;

                if (isLocalMachine && !IsHoloLensConnectedUsb)
                {
                    continue;
                }

                if (IsHoloLensConnectedUsb)
                {
                    if (isLocalMachine || usbMachine?.ComputerName != targetDevices.Connections[i].MachineName)
                    {
                        if (BuildDeployPreferences.TargetAllConnections && !isTargetedConnection)
                        {
                            continue;
                        }

                        DevicePortal.OpenWebPortal(targetDevices.Connections[i]);
                    }
                }
                else
                {
                    if (!isLocalMachine)
                    {
                        if (BuildDeployPreferences.TargetAllConnections && !isTargetedConnection)
                        {
                            continue;
                        }

                        DevicePortal.OpenWebPortal(targetDevices.Connections[i]);
                    }
                }
            }
        }

        private static async void InstallOnTargetDevice(string buildPath, DeviceInfo targetDevice)
        {
            await InstallOnTargetDeviceAsync(buildPath, targetDevice);
        }

        private static async Task InstallOnTargetDeviceAsync(string buildPath, DeviceInfo targetDevice)
        {
            isAppRunning = false;

            if (string.IsNullOrEmpty(PackageName))
            {
                Debug.LogWarning("No Package Name Found");
                return;
            }

            if (BuildDeployPreferences.FullReinstall)
            {
                await UninstallAppOnTargetDeviceAsync(targetDevice);
            }

            if (IsLocalConnection(targetDevice) && !IsHoloLensConnectedUsb || buildPath.Contains("x64"))
            {
                FileInfo[] installerFiles = new DirectoryInfo(buildPath).GetFiles("*.ps1");
                if (installerFiles.Length == 1)
                {
                    var pInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        CreateNoWindow = false,
                        Arguments = $"-executionpolicy bypass -File \"{installerFiles[0].FullName}\""
                    };

                    var process = new Process { StartInfo = pInfo };

                    process.Start();
                }

                return;
            }

            if (buildPath.Contains("x64"))
            {
                return;
            }

            // Get the appx path
            FileInfo[] files = new DirectoryInfo(buildPath).GetFiles("*.appx");
            files = files.Length == 0 ? new DirectoryInfo(buildPath).GetFiles("*.appxbundle") : files;

            if (files.Length == 0)
            {
                Debug.LogErrorFormat("No APPX found in folder build folder ({0})", buildPath);
                return;
            }

            await DevicePortal.InstallAppAsync(files[0].FullName, targetDevice);
        }

        private static async void InstallAppOnDevicesList(string buildPath, DevicePortalConnections targetList)
        {
            await InstallAppOnDevicesListAsync(buildPath, targetList);
        }

        private static async Task InstallAppOnDevicesListAsync(string buildPath, DevicePortalConnections targetList)
        {
            if (string.IsNullOrEmpty(PackageName))
            {
                Debug.LogWarning("No Package Name Found");
                return;
            }

            for (int i = 0; i < targetList.Connections.Count; i++)
            {
                await InstallOnTargetDeviceAsync(buildPath, targetList.Connections[i]);
            }
        }

        private static async void UninstallAppOnTargetDevice(DeviceInfo currentConnection)
        {
            isAppRunning = false;
            await UninstallAppOnTargetDeviceAsync(currentConnection);
        }

        private static async Task UninstallAppOnTargetDeviceAsync(DeviceInfo currentConnection)
        {
            if (IsLocalConnection(currentConnection) && !IsHoloLensConnectedUsb)
            {
                var pInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    CreateNoWindow = true,
                    Arguments = $"-windowstyle hidden -nologo Get-AppxPackage *{PackageName}* | Remove-AppxPackage"
                };

                var process = new Process { StartInfo = pInfo };
                process.Start();
            }
            else
            {
                if (await DevicePortal.IsAppInstalledAsync(PackageName, currentConnection))
                {
                    await DevicePortal.UninstallAppAsync(PackageName, currentConnection);
                }
            }
        }

        private static async void UninstallAppOnDevicesList(DevicePortalConnections targetList)
        {
            if (string.IsNullOrEmpty(PackageName))
            {
                return;
            }

            for (int i = 0; i < targetList.Connections.Count; i++)
            {
                await UninstallAppOnTargetDeviceAsync(targetList.Connections[i]);
            }
        }

        private static async void LaunchAppOnTargetDevice(DeviceInfo targetDevice)
        {
            if (string.IsNullOrEmpty(PackageName) ||
                IsLocalConnection(targetDevice) && !IsHoloLensConnectedUsb)
            {
                return;
            }

            if (!await DevicePortal.IsAppRunningAsync(PackageName, targetDevice))
            {
                isAppRunning = await DevicePortal.LaunchAppAsync(PackageName, targetDevice);
            }
        }

        private static void LaunchAppOnDeviceList(DevicePortalConnections targetDevices)
        {
            for (int i = 0; i < targetDevices.Connections.Count; i++)
            {
                LaunchAppOnTargetDevice(targetDevices.Connections[i]);
            }
        }

        private static async void KillAppOnTargetDevice(DeviceInfo targetDevice)
        {
            if (string.IsNullOrEmpty(PackageName) ||
                IsLocalConnection(targetDevice) && !IsHoloLensConnectedUsb)
            {
                return;
            }

            if (await DevicePortal.IsAppRunningAsync(PackageName, targetDevice))
            {
                isAppRunning = !await DevicePortal.StopAppAsync(PackageName, targetDevice);
            }
        }

        private static void KillAppOnDeviceList(DevicePortalConnections targetDevices)
        {
            for (int i = 0; i < targetDevices.Connections.Count; i++)
            {
                KillAppOnTargetDevice(targetDevices.Connections[i]);
            }
        }

        private static async void OpenLogFileForTargetDevice(DeviceInfo targetDevice, string localLogPath)
        {
            if (string.IsNullOrEmpty(PackageName))
            {
                return;
            }

            if (IsLocalConnection(targetDevice) && File.Exists(localLogPath))
            {
                Process.Start(localLogPath);
                return;
            }

            if (!IsLocalConnection(targetDevice) || IsHoloLensConnectedUsb)
            {
                string logFilePath = await DevicePortal.DownloadLogFileAsync(PackageName, targetDevice);

                if (!string.IsNullOrEmpty(logFilePath))
                {
                    Process.Start(logFilePath);
                }

                return;
            }

            Debug.Log("No Log Found");
        }

        private static void OpenLogFilesOnDeviceList(DevicePortalConnections targetDevices, string localLogPath)
        {
            for (int i = 0; i < targetDevices.Connections.Count; i++)
            {
                OpenLogFileForTargetDevice(targetDevices.Connections[i], localLogPath);
            }
        }

        #endregion Device Portal Commands
    }
}
