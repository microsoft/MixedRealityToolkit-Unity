// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.WindowsDevicePortal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        private const string LOCAL_IP_ADDRESS = "127.0.0.1";

        private const string EMPTY_IP_ADDRESS = "0.0.0.0";

        private readonly string[] tabNames = { "Unity Build Options", "Appx Build Options", "Deploy Options" };

        private readonly string[] scriptingBackendNames = { "IL2CPP", ".NET" };

        private readonly int[] scriptingBackendEnum = { (int)ScriptingImplementation.IL2CPP, (int)ScriptingImplementation.WinRTDotNET };

        private readonly string[] deviceNames = { "Any Device", "PC", "Mobile", "HoloLens" };

        private static readonly List<string> Builds = new List<string>(0);

        private static readonly List<string> AppPackageDirectories = new List<string>(0);

        private const string BuildWindowTabKey = "_BuildWindow_Tab";

        #endregion Constants and Readonly Values

        #region Labels

        private readonly GUIContent buildAllThenInstallLabel = new GUIContent("Build all, then Install", "Builds the Unity Project, the APPX, then installs to the target device.");

        private readonly GUIContent buildAllLabel = new GUIContent("Build all", "Builds the Unity Project and APPX");

        private readonly GUIContent BuildDirectoryLabel = new GUIContent("Build Directory", "It's recommended to use 'UWP'");

        private readonly GUIContent useCSharpProjectsLabel = new GUIContent("Generate C# Debug", "Generate C# Project References for debugging.\nOnly available in .NET Scripting runtime.");
        
        private readonly GUIContent gazeInputCapabilityLabel =
            new GUIContent("Gaze Input Capability", 
                           "If checked, the 'Gaze Input' capability will be added to the AppX manifest after the Unity build.");

        private readonly GUIContent autoIncrementLabel = new GUIContent("Auto Increment", "Increases Version Build Number");

        private readonly GUIContent versionNumberLabel = new GUIContent("Version Number", "Major.Minor.Build.Revision\nNote: Revision should always be zero because it's reserved by Windows Store.");

        private readonly GUIContent pairHoloLensUsbLabel = new GUIContent("Pair HoloLens", "Pairs the USB connected HoloLens with the Build Window so you can deploy via USB");

        private readonly GUIContent useSSLLabel = new GUIContent("Use SSL?", "Use SLL to communicate with Device Portal");

        private readonly GUIContent addConnectionLabel = new GUIContent("+", "Add a remote connection");

        private readonly GUIContent removeConnectionLabel = new GUIContent("-", "Remove a remote connection");

        private readonly GUIContent ipAddressLabel = new GUIContent("IpAddress", "Note: Local Machine will install on any HoloLens connected to USB as well.");

        private readonly GUIContent doAllLabel = new GUIContent(" Do actions on all devices", "Should the build options perform actions on all the connected devices?");

        private readonly GUIContent uninstallLabel = new GUIContent("Uninstall First", "Uninstall application before installing");

        private readonly GUIContent researchModeCapabilityLabel = new GUIContent("Enable Research Mode", "Enables research mode of HoloLens. This allows access to raw sensor data.");

        private readonly GUIContent allowUnsafeCode = new GUIContent("Allow Unsafe Code", "Modify 'Assembly-CSharp.csproj' to allow use of unsafe code. Be careful using this in production.");

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
                    canInstall |= DevicePortalConnectionEnabled && IsHoloLensConnectedUsb;
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

        private const float HALF_WIDTH = 256f;

        private static float timeLastUpdatedBuilds;

        private string[] targetIps;
        private List<Version> windowsSdkVersions = new List<Version>();

        private Vector2 scrollPosition;

        private BuildDeployTab currentTab = BuildDeployTab.UnityBuildOptions;
        private BuildDeployTab lastTab = BuildDeployTab.UnityBuildOptions;

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

            LoadWindowsSdkPaths();
            UpdateBuilds();

            currentConnectionInfoIndex = lastSessionConnectionInfoIndex;
            portalConnections = JsonUtility.FromJson<DevicePortalConnections>(UwpBuildDeployPreferences.DevicePortalConnections);
            UpdatePortalConnections();
        }

        private void OnDestroy()
        {
            lastSessionConnectionInfoIndex = currentConnectionInfoIndex;
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
                    // TODO: Troy -> looki at this carefully
                    if (GUILayout.Button(CanInstall ? buildAllThenInstallLabel : buildAllLabel, GUILayout.ExpandWidth(true)))
                    {
                        EditorApplication.delayCall += () => BuildAll(CanInstall);
                    }
                }

                RenderPlayerSettingsButton();
            }

            EditorGUILayout.Space();

            lastTab = currentTab;
            currentTab = (BuildDeployTab)GUILayout.Toolbar(SessionState.GetInt(BuildWindowTabKey, (int)currentTab), tabNames);
            if (currentTab != lastTab && currentTab == BuildDeployTab.DeployOptions)
            {
                // TODO: Troy - investigate
                UpdateBuilds();
            }

            SessionState.SetInt(BuildWindowTabKey, (int)currentTab);

            // TODO: Convert to action array?
            switch (currentTab)
            {
                case BuildDeployTab.UnityBuildOptions:
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        RenderUnityBuildView();
                    }
                    break;
                case BuildDeployTab.AppxBuildOptions:
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        RenderAppxBuildView();
                    }
                    break;
                case BuildDeployTab.DeployOptions:
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        RenderDeployBuildView();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

        private static void RenderOpenBuildDirectory()
        {
            using (new EditorGUI.DisabledGroupScope(!Directory.Exists(BuildDeployPreferences.AbsoluteBuildDirectory)))
            {
                if (GUILayout.Button("Open", EditorStyles.miniButtonRight, GUILayout.Width(100f)))
                {
                    EditorApplication.delayCall += () => Process.Start(BuildDeployPreferences.AbsoluteBuildDirectory);
                }
            }
        }

        private void RenderUnityBuildView()
        {
            RenderBuildDirectory();

            EditorUserBuildSettings.wsaSubtarget = (WSASubtarget)EditorGUILayout.Popup("Target Device", (int)EditorUserBuildSettings.wsaSubtarget, deviceNames, GUILayout.Width(HALF_WIDTH));

#if !UNITY_2019_1_OR_NEWER
            var curScriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA);
            if (curScriptingBackend == ScriptingImplementation.WinRTDotNET)
            {
                EditorGUILayout.HelpBox(".NET Scripting backend is deprecated in Unity 2018 and is removed in Unity 2019.", MessageType.Warning);
            }

            var newScriptingBackend = (ScriptingImplementation)EditorGUILayout.IntPopup("Scripting Backend", (int)curScriptingBackend, scriptingBackendNames, scriptingBackendEnum, GUILayout.Width(HALF_WIDTH));
            if (newScriptingBackend != curScriptingBackend)
            {
                //bool canUpdate = !Directory.Exists(BuildDeployPreferences.AbsoluteBuildDirectory);
                // TODO: Troy - still needed? isn't Unity smart enough?
                /*
                if (EditorUtility.DisplayDialog("Attention!",
                        $"Build path contains project built with {newScriptingBackend.ToString()} scripting backend, while current project is using {curScriptingBackend.ToString()} scripting backend.\n\nSwitching to a new scripting backend requires us to delete all the data currently in your build folder and rebuild the Unity Player!",
                        "Okay", "Cancel"))
                {
                    Directory.Delete(BuildDeployPreferences.AbsoluteBuildDirectory, true);
                    //canUpdate = true;
                }*/

                PlayerSettings.SetScriptingBackend(BuildTargetGroup.WSA, newScriptingBackend);
            }
#endif
            // If the WSA target device is HoloLens, show the checkboxes for research mode
            if (EditorUserBuildSettings.wsaSubtarget == WSASubtarget.HoloLens)
            {
                // Enable Research Mode Capability
                bool researchModeEnabled = UwpBuildDeployPreferences.ResearchModeCapabilityEnabled;
                bool newResearchModeEnabled = EditorGUILayout.ToggleLeft(researchModeCapabilityLabel, UwpBuildDeployPreferences.ResearchModeCapabilityEnabled);
                if (newResearchModeEnabled != researchModeEnabled)
                {
                    UwpBuildDeployPreferences.ResearchModeCapabilityEnabled = newResearchModeEnabled;
                }

#if !UNITY_2019_1_OR_NEWER
                // To prevent potential confusion, only show this when C# projects will be generated
                if (EditorUserBuildSettings.wsaGenerateReferenceProjects &&
                    PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.WinRTDotNET)
                {
                    // Allow unsafe code
                    bool curAllowUnsafeCode = UwpBuildDeployPreferences.AllowUnsafeCode;
                    bool newAllowUnsafeCode = EditorGUILayout.ToggleLeft(allowUnsafeCode, curAllowUnsafeCode);
                    if (newAllowUnsafeCode != curAllowUnsafeCode)
                    {
                        UwpBuildDeployPreferences.AllowUnsafeCode = newAllowUnsafeCode;
                    }
                }
#endif // !UNITY_2019_1_OR_NEWER
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledGroupScope(!ShouldOpenSLNBeEnabled))
                {
                    RenderOpenVisualStudioButton();

                    // TODO: Troy - move to new line*
                    if (GUILayout.Button("Build Unity Project"))
                    {
                        EditorApplication.delayCall += BuildUnityProject;
                    }
                }
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
                        $"Consider updating the 'Minimum Platform Version' in the Build Settings window to match {UwpBuildDeployPreferences.MIN_PLATFORM_VERSION}" ,
                    MessageType.Warning);
            }




            // Build config (and save setting, if it's changed)
            // TODO: Troy - look at changing type?
            var newBuildConfigOption = (WSABuildType)EditorGUILayout.EnumPopup("Build Configuration", UwpBuildDeployPreferences.BuildConfigType, GUILayout.Width(HALF_WIDTH));
            UwpBuildDeployPreferences.BuildConfig = newBuildConfigOption.ToString().ToLower();

            // Build Platform (and save setting, if it's changed)

            // TODO: Troy - save arrays as constants 
            string[] archs = { "x86", "x64", "arm" };
            int currentIndex = Array.IndexOf(archs, EditorUserBuildSettings.wsaArchitecture);
            int newIndex = EditorGUILayout.Popup("Build Platform", currentIndex, archs, GUILayout.Width(HALF_WIDTH));
            if (currentIndex != newIndex)
            {
                EditorUserBuildSettings.wsaArchitecture = archs[newIndex];
            }

            // Platform Toolset
            string[] platformToolsets = { string.Empty, "v141", "v142" };
            currentIndex = Array.IndexOf(platformToolsets, UwpBuildDeployPreferences.PlatformToolset);
            newIndex = EditorGUILayout.Popup("Plaform Toolset", currentIndex, platformToolsets, GUILayout.Width(HALF_WIDTH));
            if (currentIndex != newIndex)
            {
                UwpBuildDeployPreferences.PlatformToolset = platformToolsets[newIndex];
            }

            // The 'Gaze Input' capability support was added for HL2 in the Windows SDK 18362, but 
            // existing versions of Unity don't have support for automatically adding the capability to the generated
            // AppX manifest during the build. This option provides a mechanism for people using the
            // MRTK build tools to auto-append this capability if desired, instead of having to manually
            // do this each time on their own.
            bool curGazeInputCapabilityEnabled = UwpBuildDeployPreferences.GazeInputCapabilityEnabled;
            bool newGazeInputCapabilityEnabled = EditorGUILayout.Toggle(gazeInputCapabilityLabel, curGazeInputCapabilityEnabled);
            if (newGazeInputCapabilityEnabled != curGazeInputCapabilityEnabled)
            {
                UwpBuildDeployPreferences.GazeInputCapabilityEnabled = newGazeInputCapabilityEnabled;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                // Auto Increment version
                bool curIncrementVersion = BuildDeployPreferences.IncrementBuildVersion;
                bool newIncrementVersion = EditorGUILayout.Toggle(autoIncrementLabel, curIncrementVersion);
                if (newIncrementVersion != curIncrementVersion)
                {
                    BuildDeployPreferences.IncrementBuildVersion = newIncrementVersion;
                }

                EditorGUILayout.LabelField(versionNumberLabel, GUILayout.Width(96));
                Vector3 newVersion = Vector3.zero;
                using (var c = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.BeginChangeCheck();

                    newVersion.x = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Major);
                    newVersion.y = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Minor);
                    newVersion.z = EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Build);

                    if (c.changed)
                    {
                        PlayerSettings.WSA.packageVersion = new Version((int)newVersion.x, (int)newVersion.y, (int)newVersion.z, 0);
                    }
                }

                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.IntField(PlayerSettings.WSA.packageVersion.Revision);
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
                    if (GUILayout.Button("Open APPX Packages Location", GUILayout.Width(HALF_WIDTH)))
                    {
                        EditorApplication.delayCall += () => Process.Start("explorer.exe", $"/f /open,{appxBuildPath}");
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                // Force rebuild
                bool curForceRebuildAppx = UwpBuildDeployPreferences.ForceRebuild;
                bool newForceRebuildAppx = EditorGUILayout.Toggle("Rebuild", curForceRebuildAppx);
                if (newForceRebuildAppx != curForceRebuildAppx)
                {
                    UwpBuildDeployPreferences.ForceRebuild = newForceRebuildAppx;
                }

                // Multicore Appx Build
                bool curMulticoreAppxBuildEnabled = UwpBuildDeployPreferences.MulticoreAppxBuildEnabled;
                bool newMulticoreAppxBuildEnabled = EditorGUILayout.Toggle("Multicore Build", curMulticoreAppxBuildEnabled);
                if (newMulticoreAppxBuildEnabled != curMulticoreAppxBuildEnabled)
                {
                    UwpBuildDeployPreferences.MulticoreAppxBuildEnabled = newMulticoreAppxBuildEnabled;
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
                            else if (EditorUtility.DisplayDialog("Solution Not Found", "We couldn't find the solution. Would you like to Build it?", "Yes, Build", "No"))
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
            // TODO: Change asserts*
            Debug.Assert(portalConnections.Connections.Count != 0);
            Debug.Assert(currentConnectionInfoIndex >= 0);

            if (currentConnectionInfoIndex > portalConnections.Connections.Count - 1)
            {
                currentConnectionInfoIndex = 0;
            }

            EditorGUI.BeginChangeCheck();

            var currentConnection = portalConnections.Connections[currentConnectionInfoIndex];
            bool currentConnectionIsLocal = IsLocalConnection(currentConnection);
            bool useSSL = UwpBuildDeployPreferences.UseSSL;
            bool processAll = UwpBuildDeployPreferences.TargetAllConnections;

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledGroupScope(!IsHoloLensConnectedUsb))
                {
                    if (GUILayout.Button(pairHoloLensUsbLabel, GUILayout.Width(128f)))
                    {
                        EditorApplication.delayCall += PairDevice;
                    }
                }

                GUILayout.FlexibleSpace();

                useSSL = EditorGUILayout.Toggle(useSSLLabel, useSSL);

                currentConnectionInfoIndex = EditorGUILayout.Popup(currentConnectionInfoIndex, targetIps);

                if (currentConnectionIsLocal)
                {
                    currentConnection.MachineName = LOCAL_MACHINE;
                }

                using (new EditorGUI.DisabledGroupScope(!IsValidIpAddress(currentConnection.IP)))
                {

                    if (GUILayout.Button(addConnectionLabel, GUILayout.Width(20)))
                    {
                        portalConnections.Connections.Add(new DeviceInfo(EMPTY_IP_ADDRESS, currentConnection.User, currentConnection.Password));
                        currentConnectionInfoIndex++;
                        currentConnection = portalConnections.Connections[currentConnectionInfoIndex];
                        UpdatePortalConnections();
                    }
                }

                bool enabled = portalConnections.Connections.Count > 1 && currentConnectionInfoIndex != 0;
                using (new EditorGUI.DisabledGroupScope(!enabled))
                {
                    if (GUILayout.Button(removeConnectionLabel, GUILayout.Width(20)))
                    {
                        portalConnections.Connections.RemoveAt(currentConnectionInfoIndex);
                        currentConnectionInfoIndex--;
                        currentConnection = portalConnections.Connections[currentConnectionInfoIndex];
                        UpdatePortalConnections();
                    }
                }
            }

            GUILayout.Space(5);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(currentConnection.MachineName, GUILayout.Width(HALF_WIDTH));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUI.enabled = !currentConnectionIsLocal;
                currentConnection.IP = EditorGUILayout.TextField(ipAddressLabel, currentConnection.IP, GUILayout.Width(HALF_WIDTH));
                GUI.enabled = true;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                currentConnection.User = EditorGUILayout.TextField("Username", currentConnection.User, GUILayout.Width(HALF_WIDTH));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                currentConnection.Password = EditorGUILayout.PasswordField("Password", currentConnection.Password, GUILayout.Width(HALF_WIDTH));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                processAll = EditorGUILayout.Toggle(doAllLabel, processAll, GUILayout.Width(176));

                bool fullReinstall = EditorGUILayout.Toggle(uninstallLabel, UwpBuildDeployPreferences.FullReinstall, GUILayout.ExpandWidth(false));

                if (EditorGUI.EndChangeCheck())
                {
                    UwpBuildDeployPreferences.TargetAllConnections = processAll;
                    UwpBuildDeployPreferences.FullReinstall = fullReinstall;
                    UwpBuildDeployPreferences.UseSSL = useSSL;
                    Rest.UseSSL = useSSL;

                    // Format our local connection
                    if (currentConnection.IP.Contains(LOCAL_IP_ADDRESS))
                    {
                        currentConnection.IP = LOCAL_MACHINE;
                    }

                    portalConnections.Connections[currentConnectionInfoIndex] = currentConnection;
                    UpdatePortalConnections();
                    Repaint();
                }

                GUILayout.FlexibleSpace();

                // Connect
                if (!IsLocalConnection(currentConnection))
                {
                    bool enableConnection = IsValidIpAddress(currentConnection.IP) && IsCredentialsValid(currentConnection);
                    using (new EditorGUI.DisabledGroupScope(!enableConnection))
                    {
                        if (GUILayout.Button("Connect"))
                        {
                            EditorApplication.delayCall += () =>
                            {
                                ConnectToDevice(currentConnection);
                            };
                        }
                    }
                }

                bool enabled = DevicePortalConnectionEnabled && CanInstall;
                using (new EditorGUI.DisabledGroupScope(!enabled))
                {
                    // Open web portal
                    if (GUILayout.Button("Open Device Portal", GUILayout.Width(128f)))
                    {
                        EditorApplication.delayCall += () => OpenDevicePortal(portalConnections, currentConnection);
                    }
                }
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Refresh builds", GUILayout.Width(128f)))
            {
                UpdateBuilds();
            }

            // Build list
            if (Builds.Count == 0)
            {
                GUILayout.Label("*** No builds found in build directory", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.Separator();
                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
                {
                    // TODO: Troy
                    //EditorGUILayout.ScrollViewScope()
                    scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

                    foreach (var fullBuildLocation in Builds)
                    {
                        int lastBackslashIndex = fullBuildLocation.LastIndexOf("\\", StringComparison.Ordinal);

                        var directoryDate = Directory.GetLastWriteTime(fullBuildLocation).ToString("yyyy/MM/dd HH:mm:ss");
                        string packageName = fullBuildLocation.Substring(lastBackslashIndex + 1);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new EditorGUI.DisabledGroupScope(!CanInstall))
                            {
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
                            }

                            bool canLaunchLocal = currentConnectionInfoIndex == 0 && IsHoloLensConnectedUsb;
                            bool canLaunchRemote = DevicePortalConnectionEnabled && CanInstall && currentConnectionInfoIndex != 0;

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
                            }

                            EditorGUILayout.LabelField(new GUIContent($"{packageName} ({directoryDate})"));
                        }
                    }

                    GUILayout.EndScrollView();
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
                    // TODO: Troy - change buildirectory to relative?
                    BuildDeployPreferences.BuildDirectory = EditorUtility.OpenFolderPanel("Select Build Directory", currentBuildDirectory, string.Empty);
                }

                RenderOpenBuildDirectory();
            }

            EditorGUILayout.Space();
        }

        private static void RenderPlayerSettingsButton()
        {
            if (GUILayout.Button("Open Player Settings"))
            {
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
            }
        }

#endregion

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
                                        where !address.IpAddress.Contains(EMPTY_IP_ADDRESS)
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
                        await InstallAppOnDevicesListAsync(fullBuildLocation, portalConnections);
                    }
                    else
                    {
                        await InstallOnTargetDeviceAsync(fullBuildLocation, portalConnections.Connections[currentConnectionInfoIndex]);
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
            if (currentConnectionInfoIndex > portalConnections.Connections.Count - 1)
            {
                currentConnectionInfoIndex = portalConnections.Connections.Count - 1;
            }

            targetIps[0] = LOCAL_MACHINE;
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

            UwpBuildDeployPreferences.DevicePortalConnections = JsonUtility.ToJson(devicePortalConnections);
            lastSessionConnectionInfoIndex = currentConnectionInfoIndex;
            Repaint();
        }

        private static bool IsLocalConnection(DeviceInfo connection)
        {
            return connection.IP.Contains(LOCAL_MACHINE) ||
                   connection.IP.Contains(LOCAL_IP_ADDRESS);
        }

        private static bool IsCredentialsValid(DeviceInfo connection)
        {
            return !string.IsNullOrEmpty(connection.User) &&
                   !string.IsNullOrEmpty(connection.IP);
        }

        private static bool IsValidIpAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip) || ip.Contains(EMPTY_IP_ADDRESS))
            {
                return false;
            }

            if (ip.Contains(LOCAL_MACHINE))
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
                        if (UwpBuildDeployPreferences.TargetAllConnections && !isTargetedConnection)
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
                        if (UwpBuildDeployPreferences.TargetAllConnections && !isTargetedConnection)
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

            if (UwpBuildDeployPreferences.FullReinstall)
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
            files = files.Length == 0 ? new DirectoryInfo(buildPath).GetFiles("*.msix") : files;
            files = files.Length == 0 ? new DirectoryInfo(buildPath).GetFiles("*.msixbundle") : files;

            if (files.Length == 0)
            {
                Debug.LogErrorFormat("No APPX or MSIX found in folder build folder ({0})", buildPath);
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
