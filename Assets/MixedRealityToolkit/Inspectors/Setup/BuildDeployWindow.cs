// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.WindowsDevicePortal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using FileInfo = System.IO.FileInfo;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    /// <summary>
    /// Build window - supports SLN creation, APPX from SLN, Deploy on device, and misc helper utilities associated with the build/deploy/test iteration loop
    /// Requires the device to be set in developer mode and to have secure connections disabled (in the security tab in the device portal)
    /// </summary>
    public class BuildDeployWindow : EditorWindow
    {
        #region Internal Types

        private enum BuildDeployTab
        {
            UnityBuildOptions,
            AppxBuildOptions,
            DeployOptions
        }

        #endregion Internal Types

        #region Constants and Readonly Values

        private const string LOCAL_MACHINE = "Local Machine";

        private const string HOLOLENS_USB = "HoloLens over USB";

        private const string LOCAL_IP_ADDRESS = "127.0.0.1";

        private const string EMPTY_IP_ADDRESS = "0.0.0.0";

        private static readonly string[] TAB_NAMES = { "Unity Build Options", "Appx Build Options", "Deploy Options" };

        private static readonly string[] SCRIPTING_BACKEND_NAMES = { "IL2CPP", ".NET" };

        private static readonly int[] SCRIPTING_BACKEND_ENUMS = { (int)ScriptingImplementation.IL2CPP, (int)ScriptingImplementation.WinRTDotNET };

        private static readonly string[] TARGET_DEVICE_OPTIONS = { "Any Device", "PC", "Mobile", "HoloLens" };

        private static readonly string[] ARCHITECTURE_OPTIONS = { "x86", "x64", "arm" };

        private static readonly string[] PLATFORM_TOOLSET_VALUES = { string.Empty, "v141", "v142" };

        private static readonly string[] PLATFORM_TOOLSET_NAMES = { "Solution", "v141", "v142" };

        private static readonly List<string> Builds = new List<string>(0);

        private static readonly List<string> AppPackageDirectories = new List<string>(0);

        private const string BuildWindowTabKey = "_BuildWindow_Tab";

        #endregion Constants and Readonly Values

        #region Labels

        private readonly GUIContent BuildAllThenInstallLabel = new GUIContent("Build all, then Install", "Builds the Unity Project, the APPX, then installs to the target device.");

        private readonly GUIContent BuildAllLabel = new GUIContent("Build all", "Builds the Unity Project and APPX");

        private readonly GUIContent BuildDirectoryLabel = new GUIContent("Build Directory", "It's recommended to use 'UWP'");

        private readonly GUIContent UseCSharpProjectsLabel = new GUIContent("Generate C# Debug", "Generate C# Project References for debugging.\nOnly available in .NET Scripting runtime.");

        private readonly GUIContent gazeInputCapabilityLabel =
            new GUIContent("Gaze Input Capability",
                           "If checked, the 'Gaze Input' capability will be added to the AppX manifest after the Unity build.");

        private readonly GUIContent AutoIncrementLabel = new GUIContent("Auto Increment", "Increases Version Build Number");

        private readonly GUIContent VersionNumberLabel = new GUIContent("Version Number", "Major.Minor.Build.Revision\nNote: Revision should always be zero because it's reserved by Windows Store.");

        private readonly GUIContent UseSSLLabel = new GUIContent("Use SSL?", "Use SLL to communicate with Device Portal");

        private readonly GUIContent AddConnectionLabel = new GUIContent("+", "Add a remote connection");

        private readonly GUIContent RemoveConnectionLabel = new GUIContent("-", "Remove a remote connection");

        private readonly GUIContent IPAddressLabel = new GUIContent("IpAddress", "IP Address for this connection to target");

        private readonly GUIContent UsernameLabel = new GUIContent("Username", "Device Portal Username to supply for connections and actions");

        private readonly GUIContent PasswordLabel = new GUIContent("Password", "Device Portal Password to supply for connections and actions");

        private readonly GUIContent ExecuteOnAllDevicesLabel = new GUIContent("Execute action on all devices", "Should the build options perform actions on all the connected devices?");

        private readonly GUIContent AlwaysUninstallLabel = new GUIContent("Always uninstall before install", "Uninstall application before installing");

        private readonly GUIContent ResearchModeCapabilityLabel = new GUIContent("Enable Research Mode", "Enables research mode of HoloLens. This allows access to raw sensor data.");

        private readonly GUIContent AllowUnsafeCodeLabel = new GUIContent("Allow Unsafe Code", "Modify 'Assembly-CSharp.csproj' to allow use of unsafe code. Be careful using this in production.");

        private readonly GUIContent RefreshBuildsLabel = new GUIContent("Refresh Builds", "Re-scan build directory for Appx Packages that can be deployed.");

        private readonly GUIContent InstallAppXLabel = new GUIContent("Install AppX", "Install listed AppX item to either currently selected device or all devices.");

        private readonly GUIContent UninstallAppXLabel = new GUIContent("Uninstall AppX", "Uninstall listed AppX item to either currently selected device or all devices.");

        private readonly GUIContent LaunchAppLabel = new GUIContent("Launch App", "Launch listed app on either currently selected device or all devices.");

        private readonly GUIContent ViewPlayerLogLabel = new GUIContent("View Player Log", "Launch notepad with more recent player log for listed AppX on either currently selected device or from all devices.");

        #endregion Labels

        #region Properties

        private static bool IsValidSdkInstalled { get; set; } = true;

        private static bool ShouldOpenSLNBeEnabled => !string.IsNullOrEmpty(BuildDeployPreferences.BuildDirectory);

        private static bool ShouldBuildSLNBeEnabled => !isBuilding &&
                                                       !UwpAppxBuildTools.IsBuilding &&
                                                       !BuildPipeline.isBuildingPlayer &&
                                                       !string.IsNullOrEmpty(BuildDeployPreferences.BuildDirectory);

        private static bool ShouldBuildAppxBeEnabled => ShouldBuildSLNBeEnabled && !string.IsNullOrEmpty(BuildDeployPreferences.BuildDirectory);

        private static bool DevicePortalConnectionEnabled => (portalConnections.Connections.Count > 1 || IsHoloLensConnectedUsb) &&
                                                             !string.IsNullOrEmpty(BuildDeployPreferences.BuildDirectory);

        private static bool CanInstall
        {
            get
            {
                bool canInstall = true;
                if (EditorUserBuildSettings.wsaSubtarget == WSASubtarget.HoloLens)
                {
                    canInstall = DevicePortalConnectionEnabled && IsHoloLensConnectedUsb;
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

        private static DeviceInfo CurrentConnection
        {
            get
            {
                var connections = portalConnections?.Connections;
                if (connections != null && CurrentConnectionIndex >= 0 && CurrentConnectionIndex < connections.Count)
                {
                    return connections[CurrentConnectionIndex];
                }

                return null;
            }
        }

        private static int CurrentConnectionIndex
        {
            get => currentConnectionInfoIndex;
            set
            {
                var connections = portalConnections?.Connections;
                if (connections != null && value >= 0 && value < connections.Count)
                {
                    currentConnectionInfoIndex = value;
                }
            }
        }

        #endregion Properties

        #region Fields

        private const float HALF_WIDTH = 256f;

        private static float timeLastUpdatedBuilds;

        private string[] targetIps;
        private List<Version> windowsSdkVersions = new List<Version>();

        private Vector2 deployBuildListScrollPosition;

        private BuildDeployTab currentTab = BuildDeployTab.UnityBuildOptions;
        private Action[] tabRenders;

        private static bool isBuilding;
        private static bool isAppRunning;

        [SerializeField]
        private int lastSessionConnectionInfoIndex;
        private static int currentConnectionInfoIndex = 0;
        private static DevicePortalConnections portalConnections = null;
        private static CancellationTokenSource appxCancellationTokenSource = null;

        #endregion Fields

        #region Methods

        [MenuItem("Mixed Reality Toolkit/Utilities/Build Window", false, 0)]
        public static void OpenWindow()
        {
            // Dock it next to the Scene View.
            var window = GetWindow<BuildDeployWindow>(typeof(SceneView));
            window.titleContent = new GUIContent("Build Window", EditorGUIUtility.IconContent("Collab.Build").image);
            window.Show();
        }

        private void OnEnable()
        {
            minSize = new Vector2(512, 256);

            tabRenders = new Action[]
            {
                () => { RenderUnityBuildView(); },
                () => { RenderAppxBuildView(); },
                () => { RenderDeployBuildView(); },
            };

            LoadWindowsSdkPaths();
            UpdateBuilds();

            CurrentConnectionIndex = lastSessionConnectionInfoIndex;
            portalConnections = JsonUtility.FromJson<DevicePortalConnections>(UwpBuildDeployPreferences.DevicePortalConnections);
            UpdatePortalConnections();
        }

        private void OnDestroy()
        {
            lastSessionConnectionInfoIndex = CurrentConnectionIndex;
        }

        private void OnGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WSAPlayer)
            {
                RenderStandaloneBuildView();
            }
            else
            {
                RenderWSABuildView();
            }
        }

        private void RenderWSABuildView()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledGroupScope(!ShouldBuildSLNBeEnabled))
                {
                    if (GUILayout.Button(CanInstall ? BuildAllThenInstallLabel : BuildAllLabel, GUILayout.ExpandWidth(true)))
                    {
                        EditorApplication.delayCall += () => BuildAll(CanInstall);
                    }
                }

                RenderPlayerSettingsButton();
            }

            EditorGUILayout.Space();

            BuildDeployTab lastTab = currentTab;
            currentTab = (BuildDeployTab)GUILayout.Toolbar(SessionState.GetInt(BuildWindowTabKey, (int)currentTab), TAB_NAMES);
            if (currentTab != lastTab)
            {
                SessionState.SetInt(BuildWindowTabKey, (int)currentTab);

                if (currentTab == BuildDeployTab.DeployOptions)
                {
                    UpdateBuilds();
                }
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                tabRenders[(int)currentTab].Invoke();
            }
        }

        private void RenderStandaloneBuildView()
        {
            RenderBuildDirectory();

            using (new EditorGUILayout.HorizontalScope())
            {
                RenderPlayerSettingsButton();

                if (GUILayout.Button("Build Unity Project"))
                {
                    EditorApplication.delayCall += () => UnityPlayerBuildTools.BuildUnityPlayer(new BuildInfo());
                }

                if (GUILayout.Button("Open Unity Build Window"))
                {
                    GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                }
            }
        }

        private void RenderUnityBuildView()
        {
            RenderBuildDirectory();

            EditorUserBuildSettings.wsaSubtarget = (WSASubtarget)EditorGUILayout.Popup("Target Device", (int)EditorUserBuildSettings.wsaSubtarget, TARGET_DEVICE_OPTIONS, GUILayout.Width(HALF_WIDTH));

#if !UNITY_2019_1_OR_NEWER
            var curScriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA);
            if (curScriptingBackend == ScriptingImplementation.WinRTDotNET)
            {
                EditorGUILayout.HelpBox(".NET Scripting backend is deprecated in Unity 2018 and is removed in Unity 2019.", MessageType.Warning);
            }

            var newScriptingBackend = (ScriptingImplementation)EditorGUILayout.IntPopup("Scripting Backend", (int)curScriptingBackend, SCRIPTING_BACKEND_NAMES, SCRIPTING_BACKEND_ENUMS, GUILayout.Width(HALF_WIDTH));
            if (newScriptingBackend != curScriptingBackend)
            {
                bool canUpdate = !Directory.Exists(BuildDeployPreferences.AbsoluteBuildDirectory);
                if (EditorUtility.DisplayDialog("Attention!",
                        $"Build path contains project built with {curScriptingBackend.ToString()} scripting backend, while project wants to use {newScriptingBackend.ToString()} scripting backend.\n\nSwitching to a new scripting backend requires us to delete all the data currently in your build folder and rebuild the Unity Player!",
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

            // To prevent potential confusion, only show this when C# projects will be generated
            if (EditorUserBuildSettings.wsaGenerateReferenceProjects &&
                PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.WinRTDotNET)
            {
                // Allow unsafe code
                bool curAllowUnsafeCode = UwpBuildDeployPreferences.AllowUnsafeCode;
                bool newAllowUnsafeCode = EditorGUILayout.ToggleLeft(AllowUnsafeCodeLabel, curAllowUnsafeCode);
                if (newAllowUnsafeCode != curAllowUnsafeCode)
                {
                    UwpBuildDeployPreferences.AllowUnsafeCode = newAllowUnsafeCode;
                }
            }

            // Generate C# Project References for debugging	
            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.WinRTDotNET)
            {
                bool generateReferenceProjects = EditorUserBuildSettings.wsaGenerateReferenceProjects;
                bool shouldGenerateProjects = EditorGUILayout.ToggleLeft(UseCSharpProjectsLabel, generateReferenceProjects);
                if (shouldGenerateProjects != generateReferenceProjects)
                {
                    EditorUserBuildSettings.wsaGenerateReferenceProjects = shouldGenerateProjects;
                }
            }
#endif // !UNITY_2019_1_OR_NEWER

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledGroupScope(!ShouldOpenSLNBeEnabled))
                {
                    RenderOpenVisualStudioButton();

                    if (GUILayout.Button("Build Unity Project"))
                    {
                        EditorApplication.delayCall += BuildUnityProject;
                    }
                }
            }

            EditorGUILayout.Space();
        }

        private void RenderAppxBuildView()
        {
            // SDK and MS Build Version (and save setting, if it's changed)
            // Note that this is the 'Target SDK Version' which is required to physically build the
            // code on a build machine, not the minimum platform version.
            string currentSDKVersion = EditorUserBuildSettings.wsaUWPSDK;

            Version chosenSDKVersion = null;
            for (var i = 0; i < windowsSdkVersions.Count; i++)
            {
                // windowsSdkVersions is sorted in ascending order, so we always take
                // the highest SDK version that is above our minimum.
                if (windowsSdkVersions[i] >= UwpBuildDeployPreferences.MIN_SDK_VERSION)
                {
                    chosenSDKVersion = windowsSdkVersions[i];
                }
            }

            EditorGUILayout.HelpBox($"Windows SDK Version: {currentSDKVersion}", MessageType.Info);

            // Throw exception if user has no Windows 10 SDK installed
            if (chosenSDKVersion == null)
            {
                if (IsValidSdkInstalled)
                {
                    Debug.LogError($"Unable to find the required Windows 10 SDK Target!\nPlease be sure to install the {UwpBuildDeployPreferences.MIN_SDK_VERSION} SDK from Visual Studio Installer.");
                }

                EditorGUILayout.HelpBox($"Unable to find the required Windows 10 SDK Target!\nPlease be sure to install the {UwpBuildDeployPreferences.MIN_SDK_VERSION} SDK from Visual Studio Installer.", MessageType.Error);
                IsValidSdkInstalled = false;
                return;
            }

            IsValidSdkInstalled = true;

            string newSDKVersion = chosenSDKVersion.ToString();
            if (!newSDKVersion.Equals(currentSDKVersion))
            {
                EditorUserBuildSettings.wsaUWPSDK = newSDKVersion;
            }

            string currentMinPlatformVersion = EditorUserBuildSettings.wsaMinUWPSDK;
            if (string.IsNullOrWhiteSpace(currentMinPlatformVersion))
            {
                // If the min platform version hasn't been specified, set it to the recommended value.
                EditorUserBuildSettings.wsaMinUWPSDK = UwpBuildDeployPreferences.MIN_PLATFORM_VERSION.ToString();
            }
            else if (UwpBuildDeployPreferences.MIN_PLATFORM_VERSION != new Version(currentMinPlatformVersion))
            {
                // If the user has manually changed the minimum platform version in the 'Build Settings' window
                // provide a warning that the generated application may not be deployable to older generation
                // devices. We generally recommend setting to the lowest value and letting the app model's
                // capability and versioning checks kick in for applications at runtime.
                EditorGUILayout.HelpBox(
                    "Minimum platform version is set to a different value from the recommended value: " +
                        $"{UwpBuildDeployPreferences.MIN_PLATFORM_VERSION}, the generated app may not be deployable to older generation devices. " +
                        $"Consider updating the 'Minimum Platform Version' in the Build Settings window to match {UwpBuildDeployPreferences.MIN_PLATFORM_VERSION}",
                    MessageType.Warning);
            }

            using (var c = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.LabelField("Build Options", EditorStyles.boldLabel);
                var newBuildConfigOption = (WSABuildType)EditorGUILayout.EnumPopup("Build Configuration", UwpBuildDeployPreferences.BuildConfigType, GUILayout.Width(HALF_WIDTH));
                UwpBuildDeployPreferences.BuildConfig = newBuildConfigOption.ToString().ToLower();

                // Build Platform
                int currentPlatformIndex = Array.IndexOf(ARCHITECTURE_OPTIONS, EditorUserBuildSettings.wsaArchitecture);
                int buildPlatformIndex = EditorGUILayout.Popup("Build Platform", currentPlatformIndex, ARCHITECTURE_OPTIONS, GUILayout.Width(HALF_WIDTH));

                // Platform Toolset
                int currentPlatformToolsetIndex = Array.IndexOf(PLATFORM_TOOLSET_VALUES, UwpBuildDeployPreferences.PlatformToolset);
                int newPlatformToolsetIndex = EditorGUILayout.Popup("Plaform Toolset", currentPlatformToolsetIndex, PLATFORM_TOOLSET_NAMES, GUILayout.Width(HALF_WIDTH));

                EditorGUILayout.LabelField("Manifest Options", EditorStyles.boldLabel);

                // The 'Gaze Input' capability support was added for HL2 in the Windows SDK 18362, but 
                // existing versions of Unity don't have support for automatically adding the capability to the generated
                // AppX manifest during the build. This option provides a mechanism for people using the
                // MRTK build tools to auto-append this capability if desired, instead of having to manually
                // do this each time on their own.
                bool gazeInputCapabilityEnabled = EditorGUILayout.ToggleLeft(gazeInputCapabilityLabel, UwpBuildDeployPreferences.GazeInputCapabilityEnabled);

                // Enable Research Mode Capability
                bool researchModeEnabled = EditorGUILayout.ToggleLeft(ResearchModeCapabilityLabel, UwpBuildDeployPreferences.ResearchModeCapabilityEnabled);

                if (c.changed)
                {
                    UwpBuildDeployPreferences.PlatformToolset = PLATFORM_TOOLSET_VALUES[newPlatformToolsetIndex];
                    EditorUserBuildSettings.wsaArchitecture = ARCHITECTURE_OPTIONS[buildPlatformIndex];
                    UwpBuildDeployPreferences.GazeInputCapabilityEnabled = gazeInputCapabilityEnabled;
                    UwpBuildDeployPreferences.ResearchModeCapabilityEnabled = researchModeEnabled;
                }
            }

            EditorGUILayout.LabelField("Versioning Options", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (var c = new EditorGUI.ChangeCheckScope())
                {
                    // Auto Increment version
                    bool incrementVersion = EditorGUILayout.ToggleLeft(AutoIncrementLabel, BuildDeployPreferences.IncrementBuildVersion);

                    EditorGUILayout.LabelField(VersionNumberLabel, GUILayout.Width(96));
                    Vector3 newVersion = Vector3.zero;

                    newVersion.x = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Major);
                    newVersion.y = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Minor);
                    newVersion.z = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Build);

                    if (c.changed)
                    {
                        BuildDeployPreferences.IncrementBuildVersion = incrementVersion;
                        PlayerSettings.WSA.packageVersion = new Version((int)newVersion.x, (int)newVersion.y, (int)newVersion.z, 0);
                    }
                }

                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Revision);
                }
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                // Open AppX packages location
                string appxDirectory = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.IL2CPP ? $"/AppPackages/{PlayerSettings.productName}" : $"/{PlayerSettings.productName}/AppPackages";
                string appxBuildPath = Path.GetFullPath($"{BuildDeployPreferences.BuildDirectory}{appxDirectory}");

                using (new EditorGUI.DisabledGroupScope(Builds.Count <= 0 || string.IsNullOrEmpty(appxBuildPath)))
                {
                    if (GUILayout.Button("Open APPX Packages Location", GUILayout.Width(HALF_WIDTH)))
                    {
                        EditorApplication.delayCall += () => Process.Start("explorer.exe", $"/f /open,{appxBuildPath}");
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                using (var c = new EditorGUI.ChangeCheckScope())
                {
                    // Force rebuild
                    bool forceRebuildAppx = EditorGUILayout.ToggleLeft("Force Rebuild", UwpBuildDeployPreferences.ForceRebuild);

                    // Multicore Appx Build
                    bool multicoreAppxBuildEnabled = EditorGUILayout.ToggleLeft("Multicore Build", UwpBuildDeployPreferences.MulticoreAppxBuildEnabled);

                    if (c.changed)
                    {
                        UwpBuildDeployPreferences.ForceRebuild = forceRebuildAppx;
                        UwpBuildDeployPreferences.MulticoreAppxBuildEnabled = multicoreAppxBuildEnabled;
                    }
                }

                if (appxCancellationTokenSource == null)
                {
                    using (new EditorGUI.DisabledGroupScope(!ShouldBuildAppxBeEnabled))
                    {
                        if (GUILayout.Button("Build APPX", GUILayout.Width(HALF_WIDTH)))
                        {
                            // Check if solution exists
                            string slnFilename = Path.Combine(BuildDeployPreferences.BuildDirectory, $"{PlayerSettings.productName}.sln");
                            if (File.Exists(slnFilename))
                            {
                                EditorApplication.delayCall += BuildAppx;
                            }
                            else if (EditorUtility.DisplayDialog("Solution Not Found", "We couldn't find the Visual Studio solution. Would you like to build it?", "Yes, Build Unity", "No"))
                            {
                                EditorApplication.delayCall += () => BuildAll(install: false);
                            }
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("Cancel Build", GUILayout.Width(HALF_WIDTH)))
                    {
                        appxCancellationTokenSource.Cancel();
                    }
                }
            }
        }

        private void RenderDeployBuildView()
        {
            bool useSSL = UwpBuildDeployPreferences.UseSSL;
            bool newUseSSL = EditorGUILayout.ToggleLeft(UseSSLLabel, useSSL);
            if (newUseSSL != useSSL)
            {
                UwpBuildDeployPreferences.UseSSL = useSSL;
                Rest.UseSSL = useSSL;
            }

            RenderConnectionInfoView();

            using (new EditorGUILayout.HorizontalScope())
            {
                DeviceInfo currentConnection = CurrentConnection;

                bool canTestConnection = IsValidIpAddress(currentConnection.IP) && AreCredentialsValid(currentConnection);
                using (new EditorGUI.DisabledGroupScope(!canTestConnection))
                {
                    if (GUILayout.Button("Test Connection"))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            ConnectToDevice();
                        };
                    }

                    using (new EditorGUI.DisabledGroupScope(false))
                    {
                        if (GUILayout.Button("Open Device Portal", GUILayout.Width(128f)))
                        {
                            EditorApplication.delayCall += () => OpenDevicePortal();
                        }
                    }
                }

                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space();

            RenderBuildsList();
        }

        private void RenderConnectionInfoView()
        {
            using (var c = new EditorGUI.ChangeCheckScope())
            {
                if (portalConnections.Connections.Count == 0)
                {
                    AddConnection();
                }

                DeviceInfo currentConnection = CurrentConnection;

                using (new EditorGUILayout.HorizontalScope())
                {
                    CurrentConnectionIndex = EditorGUILayout.Popup(CurrentConnectionIndex, targetIps);

                    if (GUILayout.Button(AddConnectionLabel, EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                    {
                        AddConnection();
                    }

                    using (new EditorGUI.DisabledGroupScope(portalConnections.Connections.Count <= 1))
                    {
                        if (GUILayout.Button(RemoveConnectionLabel, EditorStyles.miniButtonRight, GUILayout.Width(20)))
                        {
                            RemoveConnection();
                        }
                    }

                    GUILayout.FlexibleSpace();
                }

                if (IsHoloLensConnectedUsb && IsLocalConnection(currentConnection))
                {
                    EditorGUILayout.LabelField(HOLOLENS_USB);
                }

                currentConnection.IP = EditorGUILayout.TextField(IPAddressLabel, currentConnection.IP, GUILayout.Width(HALF_WIDTH));

                currentConnection.User = EditorGUILayout.TextField(UsernameLabel, currentConnection.User, GUILayout.Width(HALF_WIDTH));

                currentConnection.Password = EditorGUILayout.PasswordField(PasswordLabel, currentConnection.Password, GUILayout.Width(HALF_WIDTH));

                if (c.changed)
                {
                    UpdatePortalConnections();
                }
            }

            EditorGUILayout.Space();
        }

        private void RenderBuildsList()
        {
            DeviceInfo currentConnection = CurrentConnection;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(RefreshBuildsLabel))
                {
                    UpdateBuilds();
                }

                GUILayout.FlexibleSpace();
            }

            bool processAll = UwpBuildDeployPreferences.TargetAllConnections;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();

                processAll = EditorGUILayout.ToggleLeft(ExecuteOnAllDevicesLabel, processAll);

                bool fullReinstall = EditorGUILayout.ToggleLeft(AlwaysUninstallLabel, UwpBuildDeployPreferences.FullReinstall);

                GUILayout.FlexibleSpace();

                if (EditorGUI.EndChangeCheck())
                {
                    UwpBuildDeployPreferences.TargetAllConnections = processAll;
                    UwpBuildDeployPreferences.FullReinstall = fullReinstall;
                }
            }

            InspectorUIUtility.DrawDivider();

            if (Builds.Count == 0)
            {
                GUILayout.Label("*** No builds found in build directory", EditorStyles.boldLabel);
            }
            else
            {
                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
                {
                    using (var scrollView = new EditorGUILayout.ScrollViewScope(deployBuildListScrollPosition, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)))
                    {
                        deployBuildListScrollPosition = scrollView.scrollPosition;

                        foreach (var fullBuildLocation in Builds)
                        {
                            int lastBackslashIndex = fullBuildLocation.LastIndexOf("\\", StringComparison.Ordinal);

                            var directoryDate = Directory.GetLastWriteTime(fullBuildLocation).ToString("yyyy/MM/dd HH:mm:ss");
                            string packageName = fullBuildLocation.Substring(lastBackslashIndex + 1);

                            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                            {
                                using (new EditorGUI.DisabledGroupScope(!CanInstall))
                                {
                                    if (GUILayout.Button(InstallAppXLabel, GUILayout.Width(96)))
                                    {
                                        EditorApplication.delayCall += () =>
                                        {
                                            InstallApp(fullBuildLocation);
                                        };
                                    }

                                    if (GUILayout.Button(UninstallAppXLabel, GUILayout.Width(96)))
                                    {
                                        EditorApplication.delayCall += () =>
                                        {
                                            UninstallApp();
                                        };
                                    }
                                }

                                bool canLaunchLocal = IsLocalConnection(currentConnection) && IsHoloLensConnectedUsb;
                                bool canLaunchRemote = DevicePortalConnectionEnabled && CanInstall;

                                // Launch app...
                                bool launchAppEnabled = canLaunchLocal || canLaunchRemote;
                                using (new EditorGUI.DisabledGroupScope(!launchAppEnabled))
                                {
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
                                }

                                // Log file
                                string localLogPath = $"%USERPROFILE%\\AppData\\Local\\Packages\\{PlayerSettings.productName}\\TempState\\UnityPlayer.log";
                                bool localLogExists = File.Exists(localLogPath);

                                bool viewLogEnabled = localLogExists || canLaunchRemote || canLaunchLocal;
                                using (new EditorGUI.DisabledGroupScope(!viewLogEnabled))
                                {
                                    if (GUILayout.Button(ViewPlayerLogLabel, GUILayout.Width(126)))
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
                                }

                                EditorGUILayout.LabelField(new GUIContent($"{packageName} ({directoryDate})"));
                            }
                        }
                    }
                }
            }
        }

        #endregion Methods

        #region Render Helpers

        private void RenderBuildDirectory()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                string currentBuildDirectory = BuildDeployPreferences.BuildDirectory;

                EditorGUILayout.LabelField(BuildDirectoryLabel, GUILayout.Width(96));

                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.TextField(currentBuildDirectory, GUILayout.ExpandWidth(true));
                }

                if (GUILayout.Button(new GUIContent("Select Folder"), EditorStyles.miniButtonLeft, GUILayout.Width(100f)))
                {
                    var fullBuildPath = Path.GetFullPath(EditorUtility.OpenFolderPanel("Select Build Directory", currentBuildDirectory, string.Empty));

                    // Temporary code as original design only allowed relative paths
                    string projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../"));

                    if (fullBuildPath.StartsWith(projectPath))
                    {
                        BuildDeployPreferences.BuildDirectory = fullBuildPath.Replace(projectPath, string.Empty);
                    }
                    else
                    {
                        Debug.LogError("Build path must be relative to current Unity project");
                    }
                }

                RenderOpenBuildDirectoryButton();
            }

            EditorGUILayout.Space();
        }

        private static void RenderOpenVisualStudioButton()
        {
            if (GUILayout.Button("Open in Visual Studio", GUILayout.Width(HALF_WIDTH)))
            {
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
        }

        private static void RenderPlayerSettingsButton()
        {
            if (GUILayout.Button("Open Player Settings"))
            {
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
            }
        }

        private static void RenderOpenBuildDirectoryButton()
        {
            using (new EditorGUI.DisabledGroupScope(!Directory.Exists(BuildDeployPreferences.AbsoluteBuildDirectory)))
            {
                if (GUILayout.Button("Open", EditorStyles.miniButtonRight, GUILayout.Width(100f)))
                {
                    EditorApplication.delayCall += () => Process.Start(BuildDeployPreferences.AbsoluteBuildDirectory);
                }
            }
        }

        #endregion

        #region Utilities

        private void RemoveConnection()
        {
            var connections = portalConnections?.Connections;
            if (connections != null && connections.Count > 0)
            {
                portalConnections.Connections.RemoveAt(CurrentConnectionIndex);
                CurrentConnectionIndex--;
                UpdatePortalConnections();
            }
        }

        private void AddConnection()
        {
            DeviceInfo currentConnection = CurrentConnection;
            portalConnections.Connections.Add(new DeviceInfo(EMPTY_IP_ADDRESS, currentConnection.User, currentConnection.Password));
            CurrentConnectionIndex = portalConnections.Connections.Count - 1;
            UpdatePortalConnections();
        }

        private async void ConnectToDevice()
        {
            DeviceInfo currentConnection = CurrentConnection;
            var machineName = await DevicePortal.GetMachineNameAsync(currentConnection);
            currentConnection.MachineName = machineName?.ComputerName;
            UpdatePortalConnections();
            Debug.Log($"Successfully connected to device {machineName?.ComputerName} with IP {currentConnection.IP}");
        }

        public static async void BuildUnityProject()
        {
            Debug.Assert(!isBuilding);
            isBuilding = true;

            appxCancellationTokenSource = new CancellationTokenSource();
            await UwpPlayerBuildTools.BuildPlayer(BuildDeployPreferences.BuildDirectory, cancellationToken: appxCancellationTokenSource.Token);
            appxCancellationTokenSource.Dispose();
            appxCancellationTokenSource = null;

            isBuilding = false;
        }

        public static async void BuildAppx()
        {
            Debug.Assert(!isBuilding);
            isBuilding = true;

            appxCancellationTokenSource = new CancellationTokenSource();

            var buildInfo = new UwpBuildInfo
            {
                RebuildAppx = UwpBuildDeployPreferences.ForceRebuild,
                Configuration = UwpBuildDeployPreferences.BuildConfig,
                BuildPlatform = EditorUserBuildSettings.wsaArchitecture,
                PlatformToolset = UwpBuildDeployPreferences.PlatformToolset,
                OutputDirectory = BuildDeployPreferences.BuildDirectory,
                AutoIncrement = BuildDeployPreferences.IncrementBuildVersion,
                Multicore = UwpBuildDeployPreferences.MulticoreAppxBuildEnabled,
            };

            EditorAssemblyReloadManager.LockReloadAssemblies = true;
            await UwpAppxBuildTools.BuildAppxAsync(buildInfo, appxCancellationTokenSource.Token);
            EditorAssemblyReloadManager.LockReloadAssemblies = false;
            appxCancellationTokenSource.Dispose();
            appxCancellationTokenSource = null;

            isBuilding = false;
        }

        public static async void BuildAll(bool install = true)
        {
            Debug.Assert(!isBuilding);
            isBuilding = true;
            EditorAssemblyReloadManager.LockReloadAssemblies = true;

            appxCancellationTokenSource = new CancellationTokenSource();

            // First build SLN
            if (await UwpPlayerBuildTools.BuildPlayer(BuildDeployPreferences.BuildDirectory, false, appxCancellationTokenSource.Token))
            {
                if (install)
                {
                    string fullBuildLocation = CalcMostRecentBuild();

                    if (UwpBuildDeployPreferences.TargetAllConnections)
                    {
                        await InstallAppOnDeviceListAsync(fullBuildLocation, portalConnections.Connections);
                    }
                    else
                    {
                        await InstallAppOnDeviceAsync(fullBuildLocation, CurrentConnection);
                    }
                }
            }

            appxCancellationTokenSource.Dispose();
            appxCancellationTokenSource = null;
            EditorAssemblyReloadManager.LockReloadAssemblies = false;
            isBuilding = false;
        }

        private static void UpdateBuilds()
        {
            Builds.Clear();

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
                Builds.AddRange(selectedDirectories);
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

            foreach (var fullBuildLocation in Builds)
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

            for (int i = 0; i < targetIps.Length; i++)
            {
                var connection = portalConnections.Connections[i];
                string machineName = connection.MachineName;
                if (string.IsNullOrEmpty(machineName) && IsLocalConnection(connection))
                {
                    connection.MachineName = IsHoloLensConnectedUsb ? HOLOLENS_USB : LOCAL_MACHINE;
                }

                targetIps[i] = machineName + " - " + connection.IP;
            }

            UwpBuildDeployPreferences.DevicePortalConnections = JsonUtility.ToJson(portalConnections);
            lastSessionConnectionInfoIndex = CurrentConnectionIndex;
            Repaint();
        }

        private static bool IsLocalConnection(DeviceInfo connection)
        {
            return connection.IP.Equals(LOCAL_MACHINE, StringComparison.OrdinalIgnoreCase) ||
                   connection.IP.Equals(LOCAL_IP_ADDRESS);
        }

        private static bool AreCredentialsValid(DeviceInfo connection)
        {
            return !string.IsNullOrEmpty(connection.User) &&
                   !string.IsNullOrEmpty(connection.IP) &&
                   !string.IsNullOrEmpty(connection.Password);
        }

        private static bool IsIPAddress(string ip)
        {
            return IPAddress.TryParse(ip, out IPAddress address);
        }

        private static bool IsValidIpAddress(string ip)
        {
            return IsIPAddress(ip) || ip.Contains(LOCAL_MACHINE);
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

        private void LoadWindowsSdkPaths()
        {
            var windowsSdkPaths = Directory.GetDirectories(@"C:\Program Files (x86)\Windows Kits\10\Lib");
            for (int i = 0; i < windowsSdkPaths.Length; i++)
            {
                windowsSdkVersions.Add(new Version(windowsSdkPaths[i].Substring(windowsSdkPaths[i].LastIndexOf(@"\", StringComparison.Ordinal) + 1)));
            }

            // There is no well-defined enumeration of Directory.GetDirectories, so the list
            // is sorted prior to use later in this class.
            windowsSdkVersions.Sort();
        }

#endregion Utilities

#region Device Portal Commands

        private static void OpenDevicePortal()
        {
            DevicePortal.OpenWebPortal(CurrentConnection);
        }

        private static void InstallApp(string buildPath)
        {
            if (UwpBuildDeployPreferences.TargetAllConnections)
            {
                InstallAppOnDeviceList(buildPath, portalConnections.Connections);
            }
            else
            {
                InstallAppOnDevice(buildPath, CurrentConnection);
            }
        }

        private static async void InstallAppOnDevice(string buildPath, DeviceInfo targetDevice)
        {
            await InstallAppOnDeviceAsync(buildPath, targetDevice);
        }

        private static async Task InstallAppOnDeviceAsync(string buildPath, DeviceInfo targetDevice)
        {
            isAppRunning = false;

            if (string.IsNullOrEmpty(PackageName))
            {
                Debug.LogWarning("No Package Name Found");
                return;
            }

            if (UwpBuildDeployPreferences.FullReinstall)
            {
                await UninstallAppOnDeviceAsync(targetDevice);
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
            files = files.Length == 0 ? new DirectoryInfo(buildPath).GetFiles("*.msix") : files;
            files = files.Length == 0 ? new DirectoryInfo(buildPath).GetFiles("*.msixbundle") : files;

            if (files.Length == 0)
            {
                Debug.LogErrorFormat("No APPX or MSIX found in folder build folder ({0})", buildPath);
                return;
            }

            await DevicePortal.InstallAppAsync(files[0].FullName, targetDevice);
        }

        private static async void InstallAppOnDeviceList(string buildPath, List<DeviceInfo> devices)
        {
            await InstallAppOnDeviceListAsync(buildPath, devices);
        }

        private static async Task InstallAppOnDeviceListAsync(string buildPath, List<DeviceInfo> devices)
        {
            if (string.IsNullOrEmpty(PackageName))
            {
                Debug.LogWarning("No Package Name Found");
                return;
            }

            var installTasks = new List<Task>();
            for (int i = 0; i < devices.Count; i++)
            {
                installTasks.Add(InstallAppOnDeviceAsync(buildPath, devices[i]));
            }

            await Task.WhenAll(installTasks);
        }

        private static void UninstallApp()
        {
            if (UwpBuildDeployPreferences.TargetAllConnections)
            {
                UninstallAppOnDevicesList(portalConnections.Connections);
            }
            else
            {
                UninstallAppOnDevice(CurrentConnection);
            }
        }

        private static async void UninstallAppOnDevice(DeviceInfo currentConnection)
        {
            isAppRunning = false;
            await UninstallAppOnDeviceAsync(currentConnection);
        }

        private static async Task UninstallAppOnDeviceAsync(DeviceInfo currentConnection)
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

        private static async void UninstallAppOnDevicesList(List<DeviceInfo> devices)
        {
            if (string.IsNullOrEmpty(PackageName))
            {
                return;
            }

            var uninstallTasks = new List<Task>();
            for (int i = 0; i < devices.Count; i++)
            {
                uninstallTasks.Add(UninstallAppOnDeviceAsync(devices[i]));
            }

            await Task.WhenAll(uninstallTasks);
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
                    try
                    {
                        Process.Start(logFilePath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to open {logFilePath}!\n{e.Message}");
                    }
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
