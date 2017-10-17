//
// Copyright (c) @jevertt
// Copyright (c) Rafael Rivera
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

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
        private const float GUISectionOffset = 10.0f;
        private const string GUIHorizontalSpacer = "     ";
        private const float UpdateBuildsPeriod = 1.0f;

        private enum BuildPlatformEnum
        {
            AnyCPU = 0,
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

        private bool ShouldOpenSLNBeEnabled { get { return !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); } }

        private bool ShouldBuildSLNBeEnabled { get { return !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory) && !string.IsNullOrEmpty(PlayerSettings.WSA.certificatePath); } }

        private bool ShouldBuildAppxBeEnabled
        {
            get
            {
                return ShouldBuildSLNBeEnabled &&
                  !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory) &&
                  !string.IsNullOrEmpty(BuildDeployPrefs.MsBuildVersion) &&
                  !string.IsNullOrEmpty(BuildDeployPrefs.BuildConfig);
            }
        }

        private bool ShouldLaunchAppBeEnabled
        {
            get { return !string.IsNullOrEmpty(BuildDeployPrefs.TargetIPs) && !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); }
        }

        private bool ShouldWebPortalBeEnabled
        {
            get { return !string.IsNullOrEmpty(BuildDeployPrefs.TargetIPs) && !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); }
        }

        private bool ShouldLogViewBeEnabled
        {
            get { return !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); }
        }

        private bool LocalIPsOnly { get { return true; } }

        private bool HoloLensUsbConnected
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
                            isConnected = true;
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

        private List<string> builds = new List<string>();
        private float timeLastUpdatedBuilds;
        private string[] windowsSdkPaths;
        private Vector2 scrollPosition;
        private string wsaCertPath;

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
            minSize = new Vector2(600, 575);

            windowsSdkPaths = Directory.GetDirectories(@"C:\Program Files (x86)\Windows Kits\10\Lib");

            for (int i = 0; i < windowsSdkPaths.Length; i++)
            {
                windowsSdkPaths[i] = windowsSdkPaths[i].Substring(windowsSdkPaths[i].LastIndexOf(@"\", StringComparison.Ordinal) + 1);
            }

            wsaCertPath = PlayerSettings.WSA.certificatePath;
            wsaCertPath = wsaCertPath.Replace("\\", "/");
            wsaCertPath = wsaCertPath.Substring(wsaCertPath.LastIndexOf("/", StringComparison.Ordinal) + 1);

            UpdateXdeStatus();
            UpdateBuilds();
        }

        private void UpdateXdeStatus()
        {
            XdeGuestLocator.FindGuestAddressAsync();
        }

        private void OnGUI()
        {
            GUILayout.Space(GUISectionOffset);

            // Setup
            int buttonWidth_Quarter = Screen.width / 4;
            int buttonWidth_Half = Screen.width / 2;
            int buttonWidth_Full = Screen.width - 25;
            var locatorHasData = XdeGuestLocator.HasData;
            var locatorIsSearching = XdeGuestLocator.IsSearching;
            var xdeGuestIpAddress = XdeGuestLocator.GuestIpAddress;

            // Quick Options
            GUILayout.BeginVertical();
            GUILayout.Label("Quick Options");

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Build & Run button...
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUI.enabled = ShouldBuildSLNBeEnabled;

                if (GUILayout.Button((!locatorIsSearching && locatorHasData || HoloLensUsbConnected)
                    ? "Build SLN, Build APPX, then Install"
                    : "Build SLN, Build APPX",
                    GUILayout.Width(buttonWidth_Half - 20)))
                {
                    // Build SLN
                    EditorApplication.delayCall += () => { BuildAll(!locatorIsSearching && locatorHasData || HoloLensUsbConnected); };
                }

                GUI.enabled = true;

                if (GUILayout.Button("Open Player Settings", GUILayout.Width(buttonWidth_Quarter)))
                {
                    EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
                }

                if (GUILayout.Button(string.IsNullOrEmpty(wsaCertPath) ? "Select Certificate" : wsaCertPath, GUILayout.Width(buttonWidth_Quarter)))
                {
                    string path = EditorUtility.OpenFilePanel("Select Certificate", Application.dataPath, "pfx");
                    wsaCertPath = path.Substring(path.LastIndexOf("/", StringComparison.Ordinal) + 1);

                    if (!string.IsNullOrEmpty(path))
                    {
                        CertificatePasswordWindow.Show(path);
                    }
                    else
                    {
                        PlayerSettings.WSA.SetCertificate(string.Empty, string.Empty);
                    }
                }
            }

            GUILayout.EndVertical();

            // Build section
            GUILayout.BeginVertical();
            GUILayout.Label("SLN");

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // Build directory (and save setting, if it's changed)
            string curBuildDirectory = BuildDeployPrefs.BuildDirectory;
            string newBuildDirectory = EditorGUILayout.TextField(new GUIContent(GUIHorizontalSpacer + "Build Directory", "It's recommended to use UWP"), curBuildDirectory);

            if (newBuildDirectory != curBuildDirectory)
            {
                BuildDeployPrefs.BuildDirectory = newBuildDirectory;
                curBuildDirectory = newBuildDirectory;
            }

            GUI.enabled = Directory.Exists(BuildDeployPrefs.AbsoluteBuildDirectory);

            if (GUILayout.Button("Open Build Directory"))
            {
                Process.Start(BuildDeployPrefs.AbsoluteBuildDirectory);
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            // Build SLN button
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                float previousLabelWidth = EditorGUIUtility.labelWidth;

                // Generate C# Project References
                EditorGUIUtility.labelWidth = 105;
                bool generateReferenceProjects = EditorUserBuildSettings.wsaGenerateReferenceProjects;
                bool shouldGenerateProjects = EditorGUILayout.Toggle("Unity C# Projects", generateReferenceProjects);

                if (shouldGenerateProjects != generateReferenceProjects)
                {
                    EditorUserBuildSettings.wsaGenerateReferenceProjects = shouldGenerateProjects;
                }

                // Restore previous label width
                EditorGUIUtility.labelWidth = previousLabelWidth;

                GUI.enabled = ShouldOpenSLNBeEnabled;

                if (GUILayout.Button("Open Project Solution", GUILayout.Width(buttonWidth_Quarter)))
                {
                    // Open SLN
                    string slnFilename = Path.Combine(curBuildDirectory, PlayerSettings.productName + ".sln");

                    if (File.Exists(slnFilename))
                    {
                        var slnFile = new FileInfo(slnFilename);
                        Process.Start(slnFile.FullName);
                    }
                    else if (EditorUtility.DisplayDialog("Solution Not Found", "We couldn't find the Project's Solution. Would you like to Build the project now?", "Yes, Build", "No"))
                    {
                        // Build SLN
                        EditorApplication.delayCall += () => { BuildDeployTools.BuildSLN(curBuildDirectory); };
                    }
                }

                GUI.enabled = ShouldBuildSLNBeEnabled;

                if (GUILayout.Button("Build Unity Project", GUILayout.Width(buttonWidth_Half)))
                {
                    // Build SLN
                    EditorApplication.delayCall += () => { BuildDeployTools.BuildSLN(curBuildDirectory); };
                }

                GUI.enabled = true;
            }

            // Appx sub-section
            GUILayout.BeginVertical();
            GUILayout.Label("APPX");

            GUILayout.BeginHorizontal();

            // SDK and MS Build Version(and save setting, if it's changed)
            string curMSBuildVer = BuildDeployPrefs.MsBuildVersion;
            string currentSDKVersion = EditorUserBuildSettings.wsaUWPSDK;

            int currentSDKVersionIndex = 0;
            int defaultMSBuildVersionIndex = -1;

            for (var i = 0; i < windowsSdkPaths.Length; i++)
            {
                if (string.IsNullOrEmpty(currentSDKVersion))
                {
                    currentSDKVersionIndex = windowsSdkPaths.Length - 1;
                }
                else
                {
                    if (windowsSdkPaths[i].Equals(currentSDKVersion))
                    {
                        currentSDKVersionIndex = i;
                    }

                    if (windowsSdkPaths[i].Equals("10.0.14393.0"))
                    {
                        defaultMSBuildVersionIndex = i;
                    }
                }
            }

            currentSDKVersionIndex = EditorGUILayout.Popup(GUIHorizontalSpacer + "SDK Version", currentSDKVersionIndex, windowsSdkPaths);

            var curScriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA);
            var newScriptingBackend = (ScriptingImplementation)EditorGUILayout.IntPopup(
                GUIHorizontalSpacer + "Scripting Backend",
                (int)curScriptingBackend,
                new[] { "IL2CPP", ".NET" },
                new[] { (int)ScriptingImplementation.IL2CPP, (int)ScriptingImplementation.WinRTDotNET });

            if (newScriptingBackend != curScriptingBackend)
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.WSA, newScriptingBackend);
            }

            string newSDKVersion = windowsSdkPaths[currentSDKVersionIndex];

            if (!newSDKVersion.Equals(currentSDKVersion))
            {
                EditorUserBuildSettings.wsaUWPSDK = newSDKVersion;
            }

            string newMSBuildVer = currentSDKVersionIndex <= defaultMSBuildVersionIndex ? BuildDeployTools.DefaultMSBuildVersion : "15.0";

            if (!newMSBuildVer.Equals(curMSBuildVer))
            {
                BuildDeployPrefs.MsBuildVersion = newMSBuildVer;
                curMSBuildVer = newMSBuildVer;
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

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

            buildConfigOption = (BuildConfigEnum)EditorGUILayout.EnumPopup(GUIHorizontalSpacer + "Build Configuration", buildConfigOption);

            string buildConfigString = buildConfigOption.ToString();

            if (buildConfigString != curBuildConfigString)
            {
                BuildDeployPrefs.BuildConfig = buildConfigString;
                curBuildConfigString = buildConfigString;
            }

            // Build Platform (and save setting, if it's changed)
            string curBuildPlatformString = BuildDeployPrefs.BuildPlatform;
            BuildPlatformEnum buildPlatformOption;

            if (curBuildPlatformString.ToLower().Equals("x86"))
            {
                buildPlatformOption = BuildPlatformEnum.x86;
            }
            else if (curBuildPlatformString.ToLower().Equals("x64"))
            {
                buildPlatformOption = BuildPlatformEnum.x64;
            }
            else
            {
                buildPlatformOption = BuildPlatformEnum.AnyCPU;
            }

            buildPlatformOption = (BuildPlatformEnum)EditorGUILayout.EnumPopup(GUIHorizontalSpacer + "Build Platform", buildPlatformOption);

            string newBuildPlatformString;

            switch (buildPlatformOption)
            {
                case BuildPlatformEnum.AnyCPU:
                    newBuildPlatformString = "Any CPU";
                    break;
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
                curBuildPlatformString = newBuildPlatformString;
            }

            GUILayout.EndHorizontal();

            // Build APPX button
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                float previousLabelWidth = EditorGUIUtility.labelWidth;

                // Force rebuild
                EditorGUIUtility.labelWidth = 50;
                bool curForceRebuildAppx = BuildDeployPrefs.ForceRebuild;
                bool newForceRebuildAppx = EditorGUILayout.Toggle("Rebuild", curForceRebuildAppx);

                if (newForceRebuildAppx != curForceRebuildAppx)
                {
                    BuildDeployPrefs.ForceRebuild = newForceRebuildAppx;
                    curForceRebuildAppx = newForceRebuildAppx;
                }

                // Increment version
                EditorGUIUtility.labelWidth = 110;
                bool curIncrementVersion = BuildDeployPrefs.IncrementBuildVersion;
                bool newIncrementVersion = EditorGUILayout.Toggle("Increment Version", curIncrementVersion);

                if (newIncrementVersion != curIncrementVersion)
                {
                    BuildDeployPrefs.IncrementBuildVersion = newIncrementVersion;
                    curIncrementVersion = newIncrementVersion;
                }

                // Restore previous label width
                EditorGUIUtility.labelWidth = previousLabelWidth;

                // Build APPX
                GUI.enabled = ShouldBuildAppxBeEnabled;

                if (GUILayout.Button("Build APPX", GUILayout.Width(buttonWidth_Half)))
                {
                    // Open SLN
                    string slnFilename = Path.Combine(curBuildDirectory, PlayerSettings.productName + ".sln");

                    if (File.Exists(slnFilename))
                    {
                        // Build APPX
                        EditorApplication.delayCall += () =>
                            BuildDeployTools.BuildAppxFromSLN(
                                PlayerSettings.productName,
                                curMSBuildVer,
                                curForceRebuildAppx,
                                curBuildConfigString,
                                curBuildPlatformString,
                                curBuildDirectory,
                                curIncrementVersion);
                    }
                    else if (EditorUtility.DisplayDialog("Solution Not Found", "We couldn't find the solution. Would you like to Build it?", "Yes, Build", "No"))
                    {
                        // Build SLN then APPX
                        EditorApplication.delayCall += () => BuildAll(install: false);
                    }
                }

                GUI.enabled = true;
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();

            GUILayout.Space(GUISectionOffset);

            // Deploy section
            GUILayout.BeginVertical();
            GUILayout.Label("Deploy");

            // Target IPs (and save setting, if it's changed)
            string curTargetIps = BuildDeployPrefs.TargetIPs;
            if (!LocalIPsOnly)
            {
                string newTargetIPs = EditorGUILayout.TextField(
                    new GUIContent(GUIHorizontalSpacer + "IP Address(es)", "IP(s) of target devices (e.g. 127.0.0.1 | 10.11.12.13)"),
                    curTargetIps);

                if (newTargetIPs != curTargetIps)
                {
                    BuildDeployPrefs.TargetIPs = newTargetIPs;
                    curTargetIps = newTargetIPs;
                }
            }
            else
            {
                // Queue up a repaint if we're still busy, or we'll get stuck
                // in a disabled state.

                if (locatorIsSearching)
                {
                    Repaint();
                }

                var addressesToPresent = new List<string> { "127.0.0.1" };

                if (!locatorIsSearching && locatorHasData)
                {
                    addressesToPresent.Add(xdeGuestIpAddress.ToString());
                }

                var previouslySavedAddress = addressesToPresent.IndexOf(curTargetIps);

                if (previouslySavedAddress == -1)
                {
                    previouslySavedAddress = 0;
                }

                EditorGUILayout.BeginHorizontal();

                if (locatorIsSearching && !locatorHasData)
                {
                    GUI.enabled = false;
                }

                var selectedAddressIndex = EditorGUILayout.Popup(GUIHorizontalSpacer + "IP Address", previouslySavedAddress, addressesToPresent.ToArray());

                if (GUILayout.Button(locatorIsSearching ? "Searching" : "Refresh", GUILayout.Width(buttonWidth_Quarter)))
                {
                    UpdateXdeStatus();
                }

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                var selectedAddress = addressesToPresent[selectedAddressIndex];

                if (curTargetIps != selectedAddress && !locatorIsSearching)
                {
                    BuildDeployPrefs.TargetIPs = selectedAddress;
                }
            }

            // Username/Password (and save seeings, if changed)
            string curUsername = BuildDeployPrefs.DeviceUser;
            string newUsername = EditorGUILayout.TextField(GUIHorizontalSpacer + "Username", curUsername);
            string curPassword = BuildDeployPrefs.DevicePassword;
            string newPassword = EditorGUILayout.PasswordField(GUIHorizontalSpacer + "Password", curPassword);
            bool curFullReinstall = BuildDeployPrefs.FullReinstall;
            bool newFullReinstall = EditorGUILayout.Toggle(
                new GUIContent(GUIHorizontalSpacer + "Uninstall First", "Uninstall application before installing"), curFullReinstall);

            if (newUsername != curUsername ||
                newPassword != curPassword ||
                newFullReinstall != curFullReinstall)
            {
                BuildDeployPrefs.DeviceUser = newUsername;
                BuildDeployPrefs.DevicePassword = newPassword;
                BuildDeployPrefs.FullReinstall = newFullReinstall;
            }

            // Build list (with install buttons)
            if (builds.Count == 0)
            {
                GUILayout.Label(GUIHorizontalSpacer + "*** No builds found in build directory", EditorStyles.boldLabel);
            }
            else
            {
                GUILayout.BeginVertical();
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(buttonWidth_Full), GUILayout.Height(128));

                foreach (var fullBuildLocation in builds)
                {
                    int lastBackslashIndex = fullBuildLocation.LastIndexOf("\\", StringComparison.Ordinal);

                    var directoryDate = Directory.GetLastWriteTime(fullBuildLocation).ToString("yyyy/MM/dd HH:mm:ss");
                    string packageName = fullBuildLocation.Substring(lastBackslashIndex + 1);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(GUISectionOffset + 15);

                    GUI.enabled = (!locatorIsSearching && locatorHasData || HoloLensUsbConnected);
                    if (GUILayout.Button("Install", GUILayout.Width(120.0f)))
                    {
                        string thisBuildLocation = fullBuildLocation;
                        string[] ipList = ParseIPList(curTargetIps);
                        EditorApplication.delayCall += () =>
                        {
                            InstallAppOnDevicesList(thisBuildLocation, ipList);
                        };
                    }

                    GUI.enabled = true;

                    GUILayout.Space(5);
                    GUILayout.Label(packageName + " (" + directoryDate + ")");
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();

                EditorGUILayout.Separator();
            }

            GUILayout.EndVertical();
            GUILayout.Space(GUISectionOffset);

            // Utilities section
            GUILayout.BeginVertical();
            GUILayout.Label("Utilities");

            // Open AppX packages location
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = builds.Count > 0;

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Open APPX Packages Location", GUILayout.Width(buttonWidth_Full)))
                {
                    Process.Start("explorer.exe", "/f /open," + Path.GetFullPath(curBuildDirectory + "/" + PlayerSettings.productName + "/AppPackages"));
                }

                GUI.enabled = true;
            }

            // Open web portal
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = ShouldWebPortalBeEnabled && (!locatorIsSearching && locatorHasData || HoloLensUsbConnected);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Open Device Portal", GUILayout.Width(buttonWidth_Full)))
                {
                    OpenWebPortalForIPs(curTargetIps);
                }

                GUI.enabled = true;
            }

            // Launch app..
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = ShouldLaunchAppBeEnabled && (!locatorIsSearching && locatorHasData || HoloLensUsbConnected);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Launch Application", GUILayout.Width(buttonWidth_Full)))
                {
                    // If already running, kill it (button is a toggle)
                    if (IsAppRunning_FirstIPCheck(PlayerSettings.productName, curTargetIps))
                    {
                        KillAppOnIPs(curTargetIps);
                    }
                    else
                    {
                        LaunchAppOnIPs(curTargetIps);
                    }
                }

                GUI.enabled = true;
            }

            // Log file
            using (new EditorGUILayout.HorizontalScope())
            {
                string localLogPath = string.Empty;
                bool localLogExists = false;

                bool remoteDeviceDetected = !locatorIsSearching && locatorHasData || HoloLensUsbConnected && !string.IsNullOrEmpty(BuildDeployPrefs.TargetIPs);
                bool isLocalBuild = BuildDeployPrefs.BuildPlatform == BuildPlatformEnum.x64.ToString();
                if (!remoteDeviceDetected)
                {
                    localLogPath = string.Format("%USERPROFILE%\\AppData\\Local\\Packages\\{0}\\TempState\\UnityPlayer.log", PlayerSettings.productName);
                    localLogExists = File.Exists(localLogPath);
                }

                GUI.enabled = ShouldLogViewBeEnabled && (remoteDeviceDetected || isLocalBuild && localLogExists);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("View Log File", GUILayout.Width(buttonWidth_Full)))
                {
                    if (remoteDeviceDetected)
                    {
                        OpenLogFileForIPs(curTargetIps);
                    }
                    else
                    {
                        Process.Start(localLogPath);
                    }
                }

                GUI.enabled = true;
            }

            // Uninstall...
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = ShouldLogViewBeEnabled && (!locatorIsSearching && locatorHasData || HoloLensUsbConnected);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Uninstall Application", GUILayout.Width(buttonWidth_Full)))
                {
                    EditorApplication.delayCall += () =>
                    {
                        UninstallAppOnDevicesList(ParseIPList(curTargetIps));
                    };
                }

                GUI.enabled = true;
            }

            //GUILayout.EndScrollView();
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
                string[] ipList = ParseIPList(BuildDeployPrefs.TargetIPs);
                InstallAppOnDevicesList(fullBuildLocation, ipList);
            }
        }

        private string CalcMostRecentBuild()
        {
            UpdateBuilds();
            DateTime mostRecent = DateTime.MinValue;
            string mostRecentBuild = "";

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

        private string CalcPackageFamilyName()
        {
            // Find the manifest
            string[] manifests = Directory.GetFiles(BuildDeployPrefs.AbsoluteBuildDirectory, "Package.appxmanifest", SearchOption.AllDirectories);

            if (manifests.Length == 0)
            {
                Debug.LogError("Unable to find manifest file for build (in path - " + BuildDeployPrefs.AbsoluteBuildDirectory + ")");
                return "";
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

        private void InstallAppOnDevicesList(string buildPath, string[] targetList)
        {
            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            for (int i = 0; i < targetList.Length; i++)
            {
                try
                {
                    bool completedUninstall = false;
                    string ip = FinalizeIP(targetList[i]);
                    if (BuildDeployPrefs.FullReinstall &&
                        BuildDeployPortal.IsAppInstalled(packageFamilyName, new BuildDeployPortal.ConnectInfo(ip, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword)))
                    {
                        EditorUtility.DisplayProgressBar("Installing on devices", "Uninstall (" + ip + ")", i / (float)targetList.Length);
                        if (!UninstallApp(packageFamilyName, ip))
                        {
                            Debug.LogError("Uninstall failed - skipping install (" + ip + ")");
                            continue;
                        }

                        completedUninstall = true;
                    }

                    EditorUtility.DisplayProgressBar("Installing on devices", "Install (" + ip + ")", (i + (completedUninstall ? 0.5f : 0.0f)) / targetList.Length);
                    InstallApp(buildPath, ip);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private static bool InstallApp(string buildPath, string targetDevice)
        {
            // Get the appx path
            FileInfo[] files = new DirectoryInfo(buildPath).GetFiles("*.appx");
            files = files.Length == 0 ? new DirectoryInfo(buildPath).GetFiles("*.appxbundle") : files;

            if (files.Length == 0)
            {
                Debug.LogError("No APPX found in folder build folder (" + buildPath + ")");
                return false;
            }

            // Connection info
            var connectInfo = new BuildDeployPortal.ConnectInfo(targetDevice, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword);

            // Kick off the install
            Debug.Log("Installing build on: " + targetDevice);
            return BuildDeployPortal.InstallApp(files[0].FullName, connectInfo);
        }

        private void UninstallAppOnDevicesList(string[] targetList)
        {
            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            try
            {
                for (int i = 0; i < targetList.Length; i++)
                {
                    string ip = FinalizeIP(targetList[i]);
                    EditorUtility.DisplayProgressBar("Uninstalling application", "Uninstall (" + ip + ")", i / (float)targetList.Length);
                    UninstallApp(packageFamilyName, ip);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }

            EditorUtility.ClearProgressBar();
        }

        private bool UninstallApp(string packageFamilyName, string targetDevice)
        {
            // Connection info
            var connectInfo = new BuildDeployPortal.ConnectInfo(targetDevice, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword);

            // Kick off the install
            Debug.Log("Uninstall build: " + targetDevice);
            return BuildDeployPortal.UninstallApp(packageFamilyName, connectInfo);
        }

        private void UpdateBuilds()
        {
            builds.Clear();

            try
            {
                var appPackageDirectories = new List<string>();
                string[] buildList = Directory.GetDirectories(BuildDeployPrefs.AbsoluteBuildDirectory);

                foreach (string appBuild in buildList)
                {
                    string appPackageDirectory = appBuild + @"\AppPackages";
                    if (Directory.Exists(appPackageDirectory))
                    {
                        appPackageDirectories.AddRange(Directory.GetDirectories(appPackageDirectory));
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

        public static string[] ParseIPList(string targetIPs)
        {
            string[] ipList = { };

            if (string.IsNullOrEmpty(targetIPs))
            {
                return ipList;
            }

            string[] separators = { ";", " " };
            ipList = targetIPs.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return ipList;
        }

        private static string FinalizeIP(string ip)
        {
            // If it's local, add the port
            if (ip == "127.0.0.1")
            {
                ip += ":10080";
            }

            return ip;
        }

        public static void OpenWebPortalForIPs(string targetIPs)
        {
            string[] ipList = ParseIPList(targetIPs);

            for (int i = 0; i < ipList.Length; i++)
            {
                string url = string.Format("http://{0}", FinalizeIP(ipList[i]));

                // Run the process
                Process.Start(url);
            }
        }

        private bool IsAppRunning_FirstIPCheck(string appName, string targetIPs)
        {
            // Just pick the first one and use it...
            string[] ipList = ParseIPList(targetIPs);

            if (ipList.Length > 0)
            {
                string targetIP = FinalizeIP(ipList[0]);
                return BuildDeployPortal.IsAppRunning(
                    appName,
                    new BuildDeployPortal.ConnectInfo(targetIP, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
            }

            return false;
        }

        private void LaunchAppOnIPs(string targetIPs)
        {
            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            string[] ipList = ParseIPList(targetIPs);

            for (int i = 0; i < ipList.Length; i++)
            {
                string targetIP = FinalizeIP(ipList[i]);
                Debug.Log("Launch app on: " + targetIP);
                BuildDeployPortal.LaunchApp(
                    packageFamilyName,
                    new BuildDeployPortal.ConnectInfo(targetIP, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
            }
        }

        private void KillAppOnIPs(string targetIPs)
        {
            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            string[] ipList = ParseIPList(targetIPs);

            for (int i = 0; i < ipList.Length; i++)
            {
                string targetIP = FinalizeIP(ipList[i]);
                Debug.Log("Kill app on: " + targetIP);
                BuildDeployPortal.KillApp(
                    packageFamilyName,
                    new BuildDeployPortal.ConnectInfo(targetIP, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
            }
        }

        public void OpenLogFileForIPs(string targetIPs)
        {
            string packageFamilyName = CalcPackageFamilyName();

            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            string[] ipList = ParseIPList(targetIPs);

            for (int i = 0; i < ipList.Length; i++)
            {
                // Use the Device Portal REST API
                BuildDeployPortal.DeviceLogFile_View(
                    packageFamilyName,
                    new BuildDeployPortal.ConnectInfo(FinalizeIP(ipList[i]), BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
            }
        }

        #endregion // Methods
    }
}
