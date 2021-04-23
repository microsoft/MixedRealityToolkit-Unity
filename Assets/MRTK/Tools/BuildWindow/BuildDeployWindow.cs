// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        private const string UseRemoteTargetSessionKey = "DeployWindow_UseRemoteTarget";

        private const string HOLOLENS_USB = "HoloLens over USB";

        private const string EMPTY_IP_ADDRESS = "0.0.0.0";

        private const string WifiAdapterType = "IEEE 802";

        private static readonly string[] TAB_NAMES = { "Unity Build Options", "Appx Build Options", "Deploy Options" };

        private static readonly string[] SCRIPTING_BACKEND_NAMES = { "IL2CPP", ".NET" };

        private static readonly int[] SCRIPTING_BACKEND_ENUMS = { (int)ScriptingImplementation.IL2CPP, (int)ScriptingImplementation.WinRTDotNET };

        private static readonly string[] TARGET_DEVICE_OPTIONS = { "Any Device", "PC", "Mobile", "HoloLens" };

        private static readonly string[] ARCHITECTURE_OPTIONS = {
            "x86",
            "x64",
            "ARM",
            #if UNITY_2019_1_OR_NEWER
            "ARM64"
            #endif // UNITY_2019_1_OR_NEWER
        };

        private static readonly string[] PLATFORM_TOOLSET_VALUES = { string.Empty, "v141", "v142" };

        private static readonly string[] PLATFORM_TOOLSET_NAMES = { "Solution", "v141", "v142" };

        private static readonly string[] LocalRemoteOptions = { "Local", "Remote" };

        private static readonly List<string> Builds = new List<string>(0);

        private static readonly List<string> AppPackageDirectories = new List<string>(0);

        private const string BuildWindowTabKey = "_BuildWindow_Tab";

        private const string WINDOWS_10_KITS_PATH_REGISTRY_PATH = @"SOFTWARE\Microsoft\Windows Kits\Installed Roots";

        private const string WINDOWS_10_KITS_PATH_ALTERNATE_REGISTRY_PATH = @"SOFTWARE\WOW6432Node\Microsoft\Windows Kits\Installed Roots";

        private const string WINDOWS_10_KITS_PATH_REGISTRY_KEY = "KitsRoot10";

        private const string WINDOWS_10_KITS_PATH_POSTFIX = "Lib";

        private const string WINDOWS_10_KITS_DEFAULT_PATH = @"C:\Program Files (x86)\Windows Kits\10\Lib";

        #endregion Constants and Readonly Values

        #region Labels

        private readonly GUIContent BuildAllThenInstallLabel = new GUIContent("Build all, then Install", "Builds the Unity Project, the APPX, then installs to the target device.");

        private readonly GUIContent BuildAllLabel = new GUIContent("Build all", "Builds the Unity Project and APPX");

        private readonly GUIContent BuildDirectoryLabel = new GUIContent("Build Directory", "It's recommended to use 'UWP'");

        private readonly GUIContent UseCSharpProjectsLabel = new GUIContent("Generate C# Debug", "Generate C# Project References for debugging.\nOnly available in .NET Scripting runtime.");

        private readonly GUIContent GazeInputCapabilityLabel =
            new GUIContent("Gaze Input Capability",
                           "If checked, the 'Gaze Input' capability will be added to the AppX manifest after the Unity build.");

        private readonly GUIContent AutoIncrementLabel = new GUIContent("Auto Increment", "Increases Version Build Number");

        private readonly GUIContent VersionNumberLabel = new GUIContent("Version Number", "Major.Minor.Build.Revision\nNote: Revision should always be zero because it's reserved by Windows Store.");

        private readonly GUIContent UseSSLLabel = new GUIContent("Use SSL?", "Use SSL to communicate with Device Portal");

        private readonly GUIContent VerifySSLLabel = new GUIContent("Verify SSL Certificates?", "When using SSL for Device Portal communication, verify the SSL certificate against Root Certificates. For self-signed Device Portal certificates disabling this omits SSL rejection errors.");

        private readonly GUIContent TargetTypeLabel = new GUIContent("Target Type", "Target either local connection or a remote device");

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

        private readonly GUIContent KillAppLabel = new GUIContent("Kill App", "Kill listed app on either currently selected device or all devices.");

        private readonly GUIContent LaunchAppLabel = new GUIContent("Launch App", "Launch listed app on either currently selected device or all devices.");

        private readonly GUIContent ViewPlayerLogLabel = new GUIContent("View Player Log", "Launch notepad with more recent player log for listed AppX on either currently selected device or from all devices.");

        private readonly GUIContent NugetPathLabel = new GUIContent("Nuget Executable Path", "Only set this when restoring packages with nuget.exe (instead of msbuild) is desired.");

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

        private static DeviceInfo CurrentConnection
        {
            get => UseRemoteTarget ? CurrentRemoteConnection : localConnection;
        }

        private static DeviceInfo CurrentRemoteConnection
        {
            get
            {
                var connections = portalConnections?.Connections;
                if (connections != null && CurrentRemoteConnectionIndex >= 0 && CurrentRemoteConnectionIndex < connections.Count)
                {
                    return connections[CurrentRemoteConnectionIndex];
                }

                return null;
            }
        }

        private static int CurrentRemoteConnectionIndex
        {
            get => portalConnections != null ? portalConnections.CurrentConnectionIndex : -1;
            set
            {
                var connections = portalConnections?.Connections;
                if (connections != null && value >= 0 && value < connections.Count)
                {
                    portalConnections.CurrentConnectionIndex = value;
                }
            }
        }

        /// <summary>
        /// Tracks whether the current UI preference is to target the local machine or remote machine for deployment.
        /// Saves state for duration of current Unity session
        /// </summary>
        private static bool UseRemoteTarget
        {
            get => SessionState.GetBool(UseRemoteTargetSessionKey, false);
            set => SessionState.SetBool(UseRemoteTargetSessionKey, value);
        }

        #endregion Properties

        #region Fields

        private const float HALF_WIDTH = 256f;

        private string[] targetIps;
        private readonly List<Version> windowsSdkVersions = new List<Version>();

        private Vector2 buildSceneListScrollPosition;
        private Vector2 deployBuildListScrollPosition;
        private Vector2 appxBuildOptionsScrollPosition;

        private BuildDeployTab currentTab = BuildDeployTab.UnityBuildOptions;
        private Action[] tabRenders;

        private static bool isBuilding;
        private static bool isAppRunning;

        private static DevicePortalConnections portalConnections = null;
        private static CancellationTokenSource appxCancellationTokenSource = null;
        private static float appxProgressBarTimer = 0.0f;
        private static DeviceInfo localConnection;

        private static bool lastTestConnectionSuccessful = false;
        private static DeviceInfo lastTestConnectionTarget;
        private static DateTime? lastTestConnectionTime = null;

        #endregion Fields

        #region Methods

        [MenuItem("Mixed Reality/Toolkit/Utilities/Build Window", false, 0)]
        public static void OpenWindow()
        {
            // Dock it next to the Scene View.
            var window = GetWindow<BuildDeployWindow>(typeof(SceneView));
            window.titleContent = new GUIContent("Build Window", EditorGUIUtility.IconContent("CustomTool").image);
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

            DevicePortal.UseSSL = UwpBuildDeployPreferences.UseSSL;

            localConnection = JsonUtility.FromJson<DeviceInfo>(UwpBuildDeployPreferences.LocalConnectionInfo);

            portalConnections = JsonUtility.FromJson<DevicePortalConnections>(UwpBuildDeployPreferences.DevicePortalConnections);

            SaveRemotePortalConnections();
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
            RenderBuildDirectory();

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

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Scenes in Build", EditorStyles.boldLabel);

                using (var scrollView = new EditorGUILayout.ScrollViewScope(buildSceneListScrollPosition, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)))
                {
                    buildSceneListScrollPosition = scrollView.scrollPosition;

                    using (new EditorGUI.DisabledGroupScope(true))
                    {
                        var scenes = EditorBuildSettings.scenes;
                        for (int i = 0; i < scenes.Length; i++)
                        {
                            EditorGUILayout.ToggleLeft($"{i} {scenes[i].path}", scenes[i].enabled);
                        }
                    }
                }
            }

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
            using (var scrollView = new EditorGUILayout.ScrollViewScope(appxBuildOptionsScrollPosition, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)))
            {
                appxBuildOptionsScrollPosition = scrollView.scrollPosition;

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
                    int newPlatformToolsetIndex = EditorGUILayout.Popup("Platform Toolset", currentPlatformToolsetIndex, PLATFORM_TOOLSET_NAMES, GUILayout.Width(HALF_WIDTH));

                    // Force rebuild
                    bool forceRebuildAppx = EditorGUILayout.ToggleLeft("Force Rebuild", UwpBuildDeployPreferences.ForceRebuild);

                    // Multicore Appx Build
                    bool multicoreAppxBuildEnabled = EditorGUILayout.ToggleLeft("Multicore Build", UwpBuildDeployPreferences.MulticoreAppxBuildEnabled);

                    EditorGUILayout.LabelField("Manifest Options", EditorStyles.boldLabel);

                    // The 'Gaze Input' capability support was added for HL2 in the Windows SDK 18362, but
                    // existing versions of Unity don't have support for automatically adding the capability to the generated
                    // AppX manifest during the build. This option provides a mechanism for people using the
                    // MRTK build tools to auto-append this capability if desired, instead of having to manually
                    // do this each time on their own.
                    bool gazeInputCapabilityEnabled = EditorGUILayout.ToggleLeft(GazeInputCapabilityLabel, UwpBuildDeployPreferences.GazeInputCapabilityEnabled);

                    // Enable Research Mode Capability
                    bool researchModeEnabled = EditorGUILayout.ToggleLeft(ResearchModeCapabilityLabel, UwpBuildDeployPreferences.ResearchModeCapabilityEnabled);

                    // Don't draw the preview while building (when appxCancellationTokenSource will be non-null),
                    // since there's a null texture issue when Unity reloads the assets during a build
                    MixedRealityBuildPreferences.DrawAppLauncherModelField(appxCancellationTokenSource == null);

                    // Draw the section for nuget executable path
                    EditorGUILayout.LabelField("Nuget Path (Optional)", EditorStyles.boldLabel);

                    string nugetExecutablePath = EditorGUILayout.TextField(NugetPathLabel, UwpBuildDeployPreferences.NugetExecutablePath);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Select Nuget Executable Path"))
                        {
                            nugetExecutablePath = EditorUtility.OpenFilePanel(
                                "Select Nuget Executable Path", "", "exe");
                        }
                        if (GUILayout.Button("Use msbuild for Restore & Clear Path"))
                        {
                            nugetExecutablePath = "";
                        }
                    }
                    if (c.changed)
                    {
                        UwpBuildDeployPreferences.PlatformToolset = PLATFORM_TOOLSET_VALUES[newPlatformToolsetIndex];
                        EditorUserBuildSettings.wsaArchitecture = ARCHITECTURE_OPTIONS[buildPlatformIndex];
                        UwpBuildDeployPreferences.GazeInputCapabilityEnabled = gazeInputCapabilityEnabled;
                        UwpBuildDeployPreferences.ResearchModeCapabilityEnabled = researchModeEnabled;
                        UwpBuildDeployPreferences.ForceRebuild = forceRebuildAppx;
                        UwpBuildDeployPreferences.MulticoreAppxBuildEnabled = multicoreAppxBuildEnabled;
                        UwpBuildDeployPreferences.NugetExecutablePath = nugetExecutablePath;
                    }
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

            if (appxCancellationTokenSource != null)
            {
                using (var progressBarRect = new EditorGUILayout.VerticalScope())
                {
                    appxProgressBarTimer = Mathf.Clamp01(Time.realtimeSinceStartup % 1.0f);

                    EditorGUI.ProgressBar(progressBarRect.rect, appxProgressBarTimer, "Building AppX...");
                    GUILayout.Space(16);
                    Repaint();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                // Open AppX packages location
                string appxDirectory = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.IL2CPP ? $"/AppPackages/{PlayerSettings.productName}" : $"/{PlayerSettings.productName}/AppPackages";
                string appxBuildPath = Path.GetFullPath($"{BuildDeployPreferences.BuildDirectory}{appxDirectory}");

                using (new EditorGUI.DisabledGroupScope(Builds.Count <= 0 || string.IsNullOrEmpty(appxBuildPath)))
                {
                    if (GUILayout.Button("Open AppX Packages Location", GUILayout.Width(HALF_WIDTH)))
                    {
                        EditorApplication.delayCall += () => Process.Start("explorer.exe", $"/f /open,{appxBuildPath}");
                    }
                }

                if (appxCancellationTokenSource == null)
                {
                    using (new EditorGUI.DisabledGroupScope(!ShouldBuildAppxBeEnabled))
                    {
                        if (GUILayout.Button("Build AppX", GUILayout.Width(HALF_WIDTH)))
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
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(TargetTypeLabel, GUILayout.Width(75));
                UseRemoteTarget = EditorGUILayout.Popup(UseRemoteTarget ? 1 : 0, LocalRemoteOptions, GUILayout.Width(100)) != 0;
                GUILayout.FlexibleSpace();
            }

            using (new GUILayout.VerticalScope("box"))
            {
                if (UseRemoteTarget)
                {
                    RenderRemoteConnections();

                    RenderSSLButtons();
                }
                else
                {
                    RenderLocalConnection();

                    if (IsHoloLensConnectedUsb)
                    {
                        using (new EditorGUI.DisabledGroupScope(!AreCredentialsValid(localConnection)))
                        {
                            if (GUILayout.Button("Discover HoloLens WiFi IP", GUILayout.Width(HALF_WIDTH)))
                            {
                                EditorApplication.delayCall += () =>
                                {
                                    DiscoverLocalHololensIP();
                                };
                            }
                        }
                    }

                    RenderSSLButtons();
                }

                EditorGUILayout.Space();

                RenderConnectionButtons();
            }

            EditorGUILayout.Space();

            RenderBuildsList();
        }

        private void RenderSSLButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                bool useSSL = UwpBuildDeployPreferences.UseSSL;
                bool newUseSSL = EditorGUILayout.ToggleLeft(UseSSLLabel, useSSL);
                if (newUseSSL != useSSL)
                {
                    UwpBuildDeployPreferences.UseSSL = newUseSSL;
                    DevicePortal.UseSSL = newUseSSL;
                }
                else if (UwpBuildDeployPreferences.UseSSL != DevicePortal.UseSSL)
                {
                    DevicePortal.UseSSL = UwpBuildDeployPreferences.UseSSL;
                }

                bool verifySSL = UwpBuildDeployPreferences.VerifySSL;
                bool newVerifySSL = EditorGUILayout.ToggleLeft(VerifySSLLabel, verifySSL);
                if (newVerifySSL != verifySSL)
                {
                    UwpBuildDeployPreferences.VerifySSL = newVerifySSL;
                    DevicePortal.VerifySSLCertificates = verifySSL;
                }
                else if (UwpBuildDeployPreferences.VerifySSL != DevicePortal.VerifySSLCertificates)
                {
                    DevicePortal.VerifySSLCertificates = UwpBuildDeployPreferences.VerifySSL;
                }

                GUILayout.FlexibleSpace();
            }
        }

        private void RenderConnectionButtons()
        {
            DeviceInfo currentConnection = CurrentConnection;

            bool canTestConnection = (!UseRemoteTarget || IsValidIpAddress(currentConnection.IP)) && AreCredentialsValid(currentConnection);
            using (new EditorGUI.DisabledGroupScope(!canTestConnection))
            {

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Test Connection", GUILayout.Width(128f)))
                    {
                        lastTestConnectionTime = null;

                        EditorApplication.delayCall += async () =>
                        {
                            lastTestConnectionSuccessful = await ConnectToDevice();
                            lastTestConnectionTime = DateTime.UtcNow;
                            lastTestConnectionTarget = currentConnection;
                        };
                    }

                    if (lastTestConnectionTime != null)
                    {
                        string successStatus = lastTestConnectionSuccessful ? "Successful" : "Failed";
                        EditorGUILayout.LabelField($"{successStatus} connection to {lastTestConnectionTarget.ToString()}, {lastTestConnectionTime.Value.GetRelativeTime()}");
                    }
                }

                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledGroupScope(false))
                    {
                        if (GUILayout.Button("Open Device Portal", GUILayout.Width(128f)))
                        {
                            EditorApplication.delayCall += () => OpenDevicePortal();
                        }
                    }

                    GUILayout.FlexibleSpace();
                }
            }
        }

        private void RenderLocalConnection()
        {
            using (var c = new EditorGUI.ChangeCheckScope())
            {
                string target = IsHoloLensConnectedUsb ? HOLOLENS_USB : DeviceInfo.LocalMachine;
                EditorGUILayout.LabelField(target, GUILayout.Width(HALF_WIDTH));

                EditorGUILayout.LabelField(IPAddressLabel, new GUIContent(localConnection.IP), GUILayout.Width(HALF_WIDTH));

                localConnection.User = EditorGUILayout.TextField(UsernameLabel, localConnection.User, GUILayout.Width(HALF_WIDTH));

                localConnection.Password = EditorGUILayout.PasswordField(PasswordLabel, localConnection.Password, GUILayout.Width(HALF_WIDTH));

                if (c.changed)
                {
                    SaveLocalConnection();
                }
            }
        }

        private void RenderRemoteConnections()
        {
            using (var c = new EditorGUI.ChangeCheckScope())
            {
                if (portalConnections.Connections.Count == 0)
                {
                    AddRemoteConnection();
                }

                DeviceInfo currentConnection = CurrentRemoteConnection;

                using (new EditorGUILayout.HorizontalScope())
                {
                    CurrentRemoteConnectionIndex = EditorGUILayout.Popup(CurrentRemoteConnectionIndex, targetIps, GUILayout.Width(260));

                    if (GUILayout.Button(AddConnectionLabel, EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                    {
                        AddRemoteConnection();
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
                    SaveRemotePortalConnections();
                }
            }
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
                EditorGUILayout.HelpBox("***No builds found in build directory", MessageType.Info);
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
                            if (!Directory.Exists(fullBuildLocation))
                            {
                                continue;
                            }

                            int lastBackslashIndex = fullBuildLocation.LastIndexOf("\\", StringComparison.Ordinal);

                            var directoryDate = Directory.GetLastWriteTime(fullBuildLocation).ToString("yyyy/MM/dd HH:mm:ss");
                            string packageName = fullBuildLocation.Substring(lastBackslashIndex + 1);

                            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                            {
                                using (new EditorGUI.DisabledGroupScope(!CanInstall))
                                {
                                    if (GUILayout.Button(InstallAppXLabel, GUILayout.Width(120)))
                                    {
                                        EditorApplication.delayCall += () =>
                                        {
                                            ExecuteAction((DeviceInfo connection)
                                                => InstallAppOnDeviceAsync(fullBuildLocation, connection));
                                        };
                                    }

                                    if (GUILayout.Button(UninstallAppXLabel, GUILayout.Width(120)))
                                    {
                                        EditorApplication.delayCall += () =>
                                        {
                                            ExecuteAction((DeviceInfo connection)
                                                => UninstallAppOnDeviceAsync(connection));
                                        };
                                    }
                                }

                                bool canLaunchLocal = IsLocalConnection(currentConnection) && IsHoloLensConnectedUsb;
                                bool canLaunchRemote = DevicePortalConnectionEnabled && CanInstall;

                                // Launch app...
                                bool launchAppEnabled = canLaunchLocal || canLaunchRemote;
                                using (new EditorGUI.DisabledGroupScope(!launchAppEnabled))
                                {
                                    if (isAppRunning)
                                    {
                                        if (GUILayout.Button(KillAppLabel, GUILayout.Width(96)))
                                        {
                                            ExecuteAction((DeviceInfo connection)
                                                => KillAppOnDeviceAsync(connection));

                                            isAppRunning = false;
                                        }
                                    }
                                    else
                                    {
                                        if (GUILayout.Button(LaunchAppLabel, GUILayout.Width(96)))
                                        {
                                            ExecuteAction((DeviceInfo connection)
                                                => LaunchAppOnDeviceAsync(connection));

                                            isAppRunning = true;
                                        }
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
                                            ExecuteAction((DeviceInfo connection)
                                                => OpenLogFileForDeviceAsync(connection, localLogPath));
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
                portalConnections.Connections.RemoveAt(CurrentRemoteConnectionIndex);
                CurrentRemoteConnectionIndex--;
                SaveRemotePortalConnections();
            }
        }

        private void AddRemoteConnection()
        {
            DeviceInfo currentConnection = CurrentRemoteConnection;
            AddRemoteConnection(new DeviceInfo(EMPTY_IP_ADDRESS, currentConnection.User, currentConnection.Password));
        }

        private void AddRemoteConnection(DeviceInfo newDevice)
        {
            portalConnections.Connections.Add(newDevice);
            CurrentRemoteConnectionIndex = portalConnections.Connections.Count - 1;
            SaveRemotePortalConnections();
        }

        private async void DiscoverLocalHololensIP()
        {
            var machineName = await DevicePortal.GetMachineNameAsync(localConnection);
            var networkInfo = await DevicePortal.GetIpConfigInfoAsync(localConnection);
            if (machineName != null && networkInfo != null)
            {
                foreach (var adapter in networkInfo.Adapters)
                {
                    if (adapter.Type.Contains(WifiAdapterType))
                    {
                        foreach (var address in adapter.IpAddresses)
                        {
                            string ipAddress = address.IpAddress;
                            if (IsValidIpAddress(ipAddress)
                                && !portalConnections.Connections.Any(connection => connection.IP == ipAddress))
                            {
                                Debug.Log($"Adding new IP {ipAddress} for local HoloLens {machineName.ComputerName} to remote connection list");

                                AddRemoteConnection(new DeviceInfo(ipAddress,
                                    localConnection.User,
                                    localConnection.Password,
                                    machineName.ComputerName));

                                return;
                            }
                        }
                    }
                }

                Debug.Log($"No new or valid WiFi IP Addresses found for local HoloLens {machineName.ComputerName}");
            }
        }

        private async Task<bool> ConnectToDevice()
        {
            DeviceInfo currentConnection = CurrentConnection;
            var machineName = await DevicePortal.GetMachineNameAsync(currentConnection);
            if (machineName != null)
            {
                currentConnection.MachineName = machineName?.ComputerName;
                SaveRemotePortalConnections();
                Debug.Log($"Successfully connected to device {machineName?.ComputerName} with IP {currentConnection.IP}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Builds the open Unity project for
        /// <see href="https://docs.unity3d.com/ScriptReference/BuildTarget.WSAPlayer.html">BuildTarget.WSAPlayer</see>.
        /// </summary>
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

        /// <summary>
        /// Builds an AppX for the open Unity project for
        /// <see href="https://docs.unity3d.com/ScriptReference/BuildTarget.WSAPlayer.html">BuildTarget.WSAPlayer</see>.
        /// </summary>
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

        /// <summary>
        /// Builds the open Unity project and its AppX for
        /// <see href="https://docs.unity3d.com/ScriptReference/BuildTarget.WSAPlayer.html">BuildTarget.WSAPlayer</see>.
        /// </summary>
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

                    await ExecuteActionAsync((DeviceInfo connection)
                        => InstallAppOnDeviceAsync(fullBuildLocation, connection));
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
            string appxDirectory = curScriptingBackend == ScriptingImplementation.IL2CPP ? Path.Combine("AppPackages", PlayerSettings.productName) : Path.Combine(PlayerSettings.productName, "AppPackages");

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

        private void SaveLocalConnection()
        {
            UwpBuildDeployPreferences.LocalConnectionInfo = JsonUtility.ToJson(localConnection);
        }

        private void SaveRemotePortalConnections()
        {
            targetIps = new string[portalConnections.Connections.Count];

            for (int i = 0; i < targetIps.Length; i++)
            {
                var connection = portalConnections.Connections[i];

                if (string.IsNullOrEmpty(connection.MachineName))
                {
                    targetIps[i] = connection.IP;
                }
                else
                {
                    targetIps[i] = $"{connection.MachineName} - {connection.IP}";
                }
            }

            UwpBuildDeployPreferences.DevicePortalConnections = JsonUtility.ToJson(portalConnections);
            Repaint();
        }

        private static bool IsLocalConnection(DeviceInfo connection)
        {
            return connection.IP.Contains(DeviceInfo.LocalMachine) ||
                   connection.IP.Contains(DeviceInfo.LocalIPAddress);
        }

        private static bool AreCredentialsValid(DeviceInfo connection)
        {
            return !string.IsNullOrEmpty(connection.User) &&
                   !string.IsNullOrEmpty(connection.IP) &&
                   !string.IsNullOrEmpty(connection.Password);
        }

        private static bool IsValidIpAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            string ipAddr = ip;

            var portSegments = ip.Split(':');
            if (portSegments.Length > 2)
            {
                return false;
            }
            else if (portSegments.Length == 2)
            {
                if (!UInt16.TryParse(portSegments[1], out UInt16 result))
                {
                    return false;
                }

                ipAddr = portSegments[0];
            }

            return ipAddr.Split('.').Length == 4 && !ipAddr.Contains(EMPTY_IP_ADDRESS) &&
                (IPAddress.TryParse(ipAddr, out IPAddress address) || ipAddr.Contains(DeviceInfo.LocalMachine));
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
            string win10KitsPath = WINDOWS_10_KITS_DEFAULT_PATH;
#if UNITY_EDITOR_WIN
            // Windows 10 sdk might not be installed on C: drive.
            // Try to detect the installation path by checking the registry.
            try
            {
                var registryKey = Win32.Registry.LocalMachine.OpenSubKey(WINDOWS_10_KITS_PATH_REGISTRY_PATH);
                var registryValue = registryKey.GetValue(WINDOWS_10_KITS_PATH_REGISTRY_KEY) as string;
                win10KitsPath = Path.Combine(registryValue, WINDOWS_10_KITS_PATH_POSTFIX);

                if (!Directory.Exists(win10KitsPath))
                {
                    registryKey = Win32.Registry.LocalMachine.OpenSubKey(WINDOWS_10_KITS_PATH_ALTERNATE_REGISTRY_PATH);
                    registryValue = registryKey.GetValue(WINDOWS_10_KITS_PATH_REGISTRY_KEY) as string;
                    win10KitsPath = Path.Combine(registryValue, WINDOWS_10_KITS_PATH_POSTFIX);

                    if (!Directory.Exists(win10KitsPath))
                    {
                        Debug.LogWarning($"Could not find the Windows 10 SDK installation path via registry. Reverting to default path.");
                        win10KitsPath = WINDOWS_10_KITS_DEFAULT_PATH;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Could not find the Windows 10 SDK installation path via registry. Reverting to default path. {e}");
                win10KitsPath = WINDOWS_10_KITS_DEFAULT_PATH;
            }
#endif
            var windowsSdkPaths = Directory.GetDirectories(win10KitsPath);
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

        private static async void ExecuteAction(Func<DeviceInfo, Task> exec)
        {
            await ExecuteActionAsync(exec);
        }

        private static async Task ExecuteActionAsync(Func<DeviceInfo, Task> exec)
        {
            List<DeviceInfo> targetDevices;

            if (UwpBuildDeployPreferences.TargetAllConnections)
            {
                targetDevices = new List<DeviceInfo>() { localConnection };
                targetDevices.AddRange(portalConnections.Connections);
            }
            else
            {
                targetDevices = new List<DeviceInfo>() { CurrentConnection };
            }

            await ExecuteActionOnDevicesAsync(exec, targetDevices);
        }

        private static async Task ExecuteActionOnDevicesAsync(Func<DeviceInfo, Task> exec, List<DeviceInfo> devices)
        {
            var installTasks = new List<Task>();
            for (int i = 0; i < devices.Count; i++)
            {
                installTasks.Add(exec(devices[i]));
            }

            await Task.WhenAll(installTasks);
        }

        private static async Task InstallAppOnDeviceAsync(string buildPath, DeviceInfo targetDevice)
        {
            isAppRunning = false;
            Debug.Log($"Initiating app install on device {targetDevice.ToString()}");

            if (string.IsNullOrEmpty(PackageName))
            {
                Debug.LogWarning("No Package Name Found");
                return;
            }

            if (UwpBuildDeployPreferences.FullReinstall)
            {
                await UninstallAppOnDeviceAsync(targetDevice);
            }

            if (IsLocalConnection(targetDevice) && (!IsHoloLensConnectedUsb || buildPath.Contains("x64")))
            {
                FileInfo[] installerFiles = new DirectoryInfo(buildPath).GetFiles("Install.ps1");
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
                Debug.Log("Cannot install a x64 app on HoloLens");
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

        private static async Task UninstallAppOnDeviceAsync(DeviceInfo currentConnection)
        {
            isAppRunning = false;

            if (string.IsNullOrEmpty(PackageName))
            {
                Debug.LogWarning("No Package Name Found");
                return;
            }

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

        private static async Task LaunchAppOnDeviceAsync(DeviceInfo targetDevice)
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

        private static async Task KillAppOnDeviceAsync(DeviceInfo targetDevice)
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

        private static async Task OpenLogFileForDeviceAsync(DeviceInfo targetDevice, string localLogPath)
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

        #endregion Device Portal Commands
    }
}
