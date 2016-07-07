//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Net;
using System;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Build window - supports SLN creation, APPX from SLN, Deploy on device, and misc helper utilities associated with the build/deploy/test iteration loop
    /// Requires the device to be set in developer mode & to have secure connections disabled (in the security tab in the device portal)
    /// </summary>
    public class BuildDeployWindow : EditorWindow
    {
        // Constants
        public const string EditorPrefs_BuildDir = "BuildDeployWindow_BuildDir";
        public const string EditorPrefs_BuildConfig = "BuildDeployWindow_BuildConfig";
        public const string EditorPrefs_ForceRebuild = "BuildDeployWindow_ForceBuild";
        public const string EditorPrefs_MSBuildVer = "BuildDeployWindow_MSBuildVer";
        public const string EditorPrefs_CustomIP = "BuildDeployWindow_DestIPs";
        public const string EditorPrefs_DeviceUser = "BuildDeployWindow_DeviceUser";
        public const string EditorPrefs_DevicePwd = "BuildDeployWindow_DevicePwd";
        public const string EditorPrefs_FullReinstall = "BuildDeployWindow_FullReinstall";
        private const float GUISectionOffset = 10.0f;
        private const string GUIHorizSpacer = "     ";
        private const float UpdateBuildsPeriod = 1.0f;

        // Properties
        private string BuildDirectory_FullPath { get { return Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, ".."), buildDirectory)); } }
        private FileInfo SLNFile { get { return new FileInfo(Path.Combine(buildDirectory, PlayerSettings.productName + ".sln")); } }
        private bool ShouldOpenSLNBeEnabled { get { return !string.IsNullOrEmpty(buildDirectory); } }
        private bool ShouldBuildSLNBeEnabled { get { return !string.IsNullOrEmpty(buildDirectory); } }
        private bool ShouldBuildAppxBeEnabled { get { return !string.IsNullOrEmpty(buildDirectory) && !string.IsNullOrEmpty(msBuildVer) && !string.IsNullOrEmpty(buildConfig); } }
        private bool ShouldLaunchAppBeEnabled { get { return !string.IsNullOrEmpty(targetIPs) && !string.IsNullOrEmpty(buildDirectory); } }
        private bool ShouldWebPortalBeEnabled { get { return !string.IsNullOrEmpty(targetIPs) && !string.IsNullOrEmpty(buildDirectory); } }
        private bool ShouldLogViewBeEnabled { get { return !string.IsNullOrEmpty(targetIPs) && !string.IsNullOrEmpty(buildDirectory); } }
        private bool LocalIPsOnly {  get { return true; } }

        // Privates
        private List<string> builds = new List<string>();
        private float timeLastUpdatedBuilds = 0.0f;

        private string buildDirectory = "WindowsStoreApp";
        private string msBuildVer = BuildDeployTools.DefaultMSBuildVersion;
        private string buildConfig = "Debug";
        private bool forceRebuildAppx = false;
        private string targetIPs = "127.0.0.1";
        private string deviceUser = "";
        private string devicePassword = "";
        private bool fullReinstall = true;

        // Functions
        [MenuItem("HoloToolkit/Build Window", false, 101)]
        public static void OpenWindow()
        {
            BuildDeployWindow window = GetWindow<BuildDeployWindow>("Build Window") as BuildDeployWindow;
            if (window != null)
            {
                window.Show();
            }
        }

        void OnEnable()
        {
            Setup();
        }

        private void Setup()
        {
            this.titleContent = new GUIContent("Build Window");

            // Query menu defaults
            buildDirectory = GetEditorPref(EditorPrefs_BuildDir, buildDirectory);
            msBuildVer = GetEditorPref(EditorPrefs_MSBuildVer, msBuildVer);
            buildConfig = GetEditorPref(EditorPrefs_BuildConfig, buildConfig);
            forceRebuildAppx = (GetEditorPref(EditorPrefs_ForceRebuild, forceRebuildAppx ? "true" : "false") == "true");
            deviceUser = GetEditorPref(EditorPrefs_DeviceUser, deviceUser);
            devicePassword = GetEditorPref(EditorPrefs_DevicePwd, devicePassword);
            fullReinstall = GetEditorPref(EditorPrefs_FullReinstall, fullReinstall);
            if (!LocalIPsOnly)
            {
                targetIPs = GetEditorPref(EditorPrefs_CustomIP, targetIPs);
            }

            this.minSize = new Vector2(600, 200);

            UpdateBuilds();
        }

        private void OnGUI()
        {
            GUILayout.Space(GUISectionOffset);

            // Setup
            int buttonWidth_Quarter = Screen.width / 4;
            int buttonWidth_Half = Screen.width / 2;
            int buttonWidth_Full = Screen.width - 25;
            string appName = PlayerSettings.productName;

            // Build section
            GUILayout.BeginVertical();
                GUILayout.Label("SLN");

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                // Build directory (and save setting, if it's changed)
                string newBuildDirectory = EditorGUILayout.TextField(GUIHorizSpacer + "Build directory", buildDirectory);
                if (newBuildDirectory != buildDirectory)
                {
                    EditorPrefs.SetString(EditorPrefs_CustomIP, newBuildDirectory);
                    buildDirectory = newBuildDirectory;
                }

                // Build SLN button
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUI.enabled = ShouldOpenSLNBeEnabled;
                    if (GUILayout.Button("Open SLN", GUILayout.Width(buttonWidth_Quarter)))
                    {
                        // Open SLN
                        string slnFilename = Path.Combine(buildDirectory, PlayerSettings.productName + ".sln");
                        if (File.Exists(slnFilename))
                        {
                            FileInfo slnFile = new FileInfo(slnFilename);
                            System.Diagnostics.Process.Start(slnFile.FullName);
                        }
                        else if (EditorUtility.DisplayDialog("Solution Not Found", "We couldn't find the solution. Would you like to Build it?", "Yes, Build", "No"))
                        {
                            // Build SLN
                            EditorApplication.delayCall += () => { BuildDeployTools.BuildSLN(buildDirectory); };
                        }
                    }
                    GUI.enabled = ShouldBuildSLNBeEnabled;
                    if (GUILayout.Button("Build Visual Studio SLN", GUILayout.Width(buttonWidth_Half)))
                    {
                        // Build SLN
                        EditorApplication.delayCall += () => { BuildDeployTools.BuildSLN(buildDirectory); };
                    }
                    GUI.enabled = true;
                }

                // Build & Run button...
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUI.enabled = ShouldBuildSLNBeEnabled;
                    if (GUILayout.Button("Build SLN, Build APPX, then Install", GUILayout.Width(buttonWidth_Half)))
                    {
                        // Build SLN
                        EditorApplication.delayCall += () => { BuildAndRun(appName); };
                    }
                    GUI.enabled = true;
                }

                // Appx sub-section
                GUILayout.BeginVertical();
                    GUILayout.Label("APPX");

                    // MSBuild Ver (and save setting, if it's changed)
                    string newMSBuildVer = EditorGUILayout.TextField(GUIHorizSpacer + "MSBuild Version", msBuildVer);
                    if (newMSBuildVer != msBuildVer)
                    {
                        EditorPrefs.SetString(EditorPrefs_MSBuildVer, newMSBuildVer);
                        msBuildVer = newMSBuildVer;
                    }

                    // Build config (and save setting, if it's changed)
                    string newBuildConfig = EditorGUILayout.TextField(GUIHorizSpacer + "Build Configuration", buildConfig);
                    if (newBuildConfig != buildConfig)
                    {
                        EditorPrefs.SetString(EditorPrefs_BuildConfig, newBuildConfig);
                        buildConfig = newBuildConfig;
                    }

                    // Build APPX button
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();

                        // Force rebuild
                        float labelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 50;
                        bool newForceRebuildAppx = EditorGUILayout.Toggle("Rebuild", forceRebuildAppx);
                        if (newForceRebuildAppx != forceRebuildAppx)
                        {
                            EditorPrefs.SetString(EditorPrefs_ForceRebuild, newForceRebuildAppx ? "true" : "false");
                            forceRebuildAppx = newForceRebuildAppx;
                        }
                        EditorGUIUtility.labelWidth = labelWidth;

                        // Build APPX
                        GUI.enabled = ShouldBuildAppxBeEnabled;
                        if (GUILayout.Button("Build APPX from SLN", GUILayout.Width(buttonWidth_Half)))
                        {
                            BuildDeployTools.BuildAppxFromSolution(appName, msBuildVer, forceRebuildAppx, buildConfig, buildDirectory);
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
                if (!LocalIPsOnly)
                {
                    string newTargetIPs = EditorGUILayout.TextField(new GUIContent(GUIHorizSpacer + "IP Address(es)", "IP(s) of target devices (e.g. 127.0.0.1;10.11.12.13)"), targetIPs);
                    if (newTargetIPs != targetIPs)
                    {
                        EditorPrefs.SetString(EditorPrefs_CustomIP, newTargetIPs);
                        targetIPs = newTargetIPs;
                    }
                }

                // Username/Password (and save seeings, if changed)
                string newUsername = EditorGUILayout.TextField(GUIHorizSpacer + "Username", deviceUser);
                string newPassword = EditorGUILayout.PasswordField(GUIHorizSpacer + "Password", devicePassword);
                bool newFullReinstall = EditorGUILayout.Toggle(new GUIContent(GUIHorizSpacer + "Uninstall first", "Uninstall application before installing"), fullReinstall);
                if ((newUsername != deviceUser) ||
                    (newPassword != devicePassword) ||
                    (newFullReinstall != fullReinstall))
                {
                    EditorPrefs.SetString(EditorPrefs_DeviceUser, newUsername);
                    deviceUser = newUsername;
                    EditorPrefs.SetString(EditorPrefs_DevicePwd, newPassword);
                    devicePassword = newPassword;
                    EditorPrefs.SetBool(EditorPrefs_FullReinstall, newFullReinstall);
                    fullReinstall = newFullReinstall;
                }

                // Build list (with install buttons)
                if (this.builds.Count == 0)
                {
                    GUILayout.Label(GUIHorizSpacer + "*** No builds found in build directory", EditorStyles.boldLabel);
                }
                else
                {
                    foreach (var fullBuildLocation in this.builds)
                    {
                        int lastBackslashIndex = fullBuildLocation.LastIndexOf("\\");

                        var directoryDate = Directory.GetLastWriteTime(fullBuildLocation).ToString("yyyy/MM/dd HH:mm:ss");
                        string packageName = fullBuildLocation.Substring(lastBackslashIndex + 1);

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(GUISectionOffset + 15);
                        if (GUILayout.Button("Install", GUILayout.Width(120.0f)))
                        {
                            string[] IPlist = ParseIPList(targetIPs);
                            EditorApplication.delayCall += () =>
                            {
                                InstallAppOnDevicesList(fullBuildLocation, appName, fullReinstall, IPlist);
                            };
                        }
                        GUILayout.Space(5);
                        GUILayout.Label(packageName + " (" + directoryDate + ")");
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.Separator();
                }
            GUILayout.EndVertical();

            GUILayout.Space(GUISectionOffset);

            // Utilities section
            GUILayout.BeginVertical();
                GUILayout.Label("Utilities");

                // Open web portal
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.enabled = ShouldWebPortalBeEnabled;
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Open Device Portal", GUILayout.Width(buttonWidth_Full)))
                    {
                        OpenWebPortalForIPs(this.targetIPs);
                    }
                    GUI.enabled = true;
                }

                // Launch app..
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.enabled = ShouldLaunchAppBeEnabled;
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Launch Application", GUILayout.Width(buttonWidth_Full)))
                    {
                        // If already running, kill it (button is a toggle)
                        if (IsAppRunning_FirstIPCheck(appName, this.targetIPs))
                        { 
                            KillAppOnIPs(appName, targetIPs);
                        }
                        else
                        {
                            LaunchAppOnIPs(appName, this.targetIPs);
                        }
                    }
                    GUI.enabled = true;
                }

                // Log file
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.enabled = ShouldLogViewBeEnabled;
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("View Log File", GUILayout.Width(buttonWidth_Full)))
                    {
                        OpenLogFileForIPs(this.targetIPs, appName);
                    }
                    GUI.enabled = true;
                }

                // Uninstall...
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.enabled = ShouldLogViewBeEnabled;
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Uninstall Application", GUILayout.Width(buttonWidth_Full)))
                    {
                        string[] IPlist = ParseIPList(targetIPs);
                        EditorApplication.delayCall += () =>
                        {
                            UninstallAppOnDevicesList(appName, IPlist);
                        };
                    }
                    GUI.enabled = true;
                }
            GUILayout.EndVertical();
        }

        void BuildAndRun(string appName)
        {
            // First build SLN
            if (!BuildDeployTools.BuildSLN(buildDirectory, false))
            {
                return;
            }

            // Next, APPX
            if (!BuildDeployTools.BuildAppxFromSolution(appName, msBuildVer, forceRebuildAppx, buildConfig, buildDirectory))
            {
                return;
            }

            // Next, Install
            string fullBuildLocation = CalcMostRecentBuild();
            string[] IPlist = ParseIPList(targetIPs);
            InstallAppOnDevicesList(fullBuildLocation, appName, fullReinstall, IPlist);
        }

        private string CalcMostRecentBuild()
        {
            DateTime mostRecent = DateTime.MinValue;
            string mostRecentBuild = "";
            foreach (var fullBuildLocation in this.builds)
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

        private void InstallAppOnDevicesList(string buildPath, string appName, bool uninstallBeforeInstall, string[] targetList)
        {
            string[] IPlist = ParseIPList(targetIPs);
            for (int i = 0; i < IPlist.Length; i++)
            {
                try
                {
                    bool completedUninstall = false;
                    string IP = FinalizeIP(IPlist[i]);
                    if (fullReinstall &&
                        BuildDeployPortal.IsAppInstalled(appName, new BuildDeployPortal.ConnectInfo(IP, deviceUser, devicePassword)))
                    {
                        EditorUtility.DisplayProgressBar("Installing on devices", "Uninstall (" + IP + ")", (float)i / (float)IPlist.Length);
                        if (!UninstallApp(appName, IP))
                        {
                            Debug.LogError("Uninstall failed - skipping install (" + IP + ")");
                            continue;
                        }
                        completedUninstall = true;
                    }
                    EditorUtility.DisplayProgressBar("Installing on devices", "Install (" + IP + ")", (float)(i + (completedUninstall ? 0.5f : 0.0f)) / (float)IPlist.Length);
                    InstallApp(buildPath, appName, IP);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private bool InstallApp(string buildPath, string appName, string targetDevice)
        {
            // Get the appx path
            FileInfo[] files = (new DirectoryInfo(buildPath)).GetFiles("*.appx");
            if (files.Length == 0)
            {
                Debug.LogError("No APPX found in folder build folder (" + buildPath + ")");
                return false;
            }

            // Connection info
            var connectInfo = new BuildDeployPortal.ConnectInfo(targetDevice, deviceUser, devicePassword);

            // Kick off the install
            Debug.Log("Installing build on: " + targetDevice);
            return BuildDeployPortal.InstallApp(files[0].FullName, connectInfo);
        }

        private void UninstallAppOnDevicesList(string appName, string[] targetList)
        {
            try
            {
                string[] IPlist = ParseIPList(targetIPs);
                for (int i = 0; i < IPlist.Length; i++)
                {
                    string IP = FinalizeIP(IPlist[i]);
                    EditorUtility.DisplayProgressBar("Uninstalling application", "Uninstall (" + IP + ")", (float)i / (float)IPlist.Length);
                    UninstallApp(appName, IP);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            EditorUtility.ClearProgressBar();
        }

        private bool UninstallApp(string appName, string targetDevice)
        {
            // Connection info
            var connectInfo = new BuildDeployPortal.ConnectInfo(targetDevice, deviceUser, devicePassword);

            // Kick off the install
            Debug.Log("Uninstall build: " + targetDevice);
            return BuildDeployPortal.UninstallApp(appName, connectInfo);
        }

        private void UpdateBuilds()
        {
            this.builds.Clear();

            try
            {
                List<string> appPackageDirectories = new List<string>();
                string[] buildList = Directory.GetDirectories(BuildDirectory_FullPath);
                foreach (string appBuild in buildList)
                {
                    string appPackageDirectory = appBuild + @"\AppPackages";
                    if (Directory.Exists(appPackageDirectory))
                    {
                        appPackageDirectories.AddRange(Directory.GetDirectories(appPackageDirectory));
                    }
                }
                IEnumerable<string> selectedDirectories = from string directory in appPackageDirectories orderby Directory.GetLastWriteTime(directory) descending select Path.GetFullPath(directory);
                this.builds.AddRange(selectedDirectories);
            }
            catch (DirectoryNotFoundException)
            {
            }

            timeLastUpdatedBuilds = Time.realtimeSinceStartup;
        }

        void Update()
        {
            if ((Time.realtimeSinceStartup - timeLastUpdatedBuilds) > UpdateBuildsPeriod)
            {
                UpdateBuilds();
            }
        }

        private static string GetEditorPref(string key, string defaultValue)
        {
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetString(key);
            }
            else
            {
                EditorPrefs.SetString(key, defaultValue);
                return defaultValue;
            }
        }

        private static bool GetEditorPref(string key, bool defaultValue)
        {
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetBool(key);
            }
            else
            {
                EditorPrefs.SetBool(key, defaultValue);
                return defaultValue;
            }
        }

        public static string[] ParseIPList(string IPs)
        {
            string[] IPlist = { };

            if (IPs == null || IPs == "")
                return IPlist;

            string[] separators = { ";", " " };
            IPlist = IPs.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
            return IPlist;
        }

        static string FinalizeIP(string ip)
        {
            // If it's local, add the port
            if (ip == "127.0.0.1")
            {
                ip += ":10080";
            }
            return ip;
        }

        public static void OpenWebPortalForIPs(string IPs)
        {
            string[] ipList = ParseIPList(IPs);
            for (int i = 0; i < ipList.Length; i++)
            {
                string url = string.Format("http://{0}", FinalizeIP(ipList[i]));

                // Run the process
                System.Diagnostics.Process.Start(url);
            }
        }

        bool IsAppRunning_FirstIPCheck(string appName, string targetIPs)
        {
            // Just pick the first one and use it...
            string[] IPlist = ParseIPList(targetIPs);
            if (IPlist.Length > 0)
            {
                string targetIP = FinalizeIP(IPlist[0]);
                return BuildDeployPortal.IsAppRunning(appName, new BuildDeployPortal.ConnectInfo(targetIP, deviceUser, devicePassword));
            }
            return false;
        }

        void LaunchAppOnIPs(string appName, string targetIPs)
        {
            string[] IPlist = ParseIPList(targetIPs);
            for (int i = 0; i < IPlist.Length; i++)
            {
                string targetIP = FinalizeIP(IPlist[i]);
                Debug.Log("Launch app on: " + targetIP);
                BuildDeployPortal.LaunchApp(appName, new BuildDeployPortal.ConnectInfo(targetIP, deviceUser, devicePassword));
            }
        }

        void KillAppOnIPs(string appName, string targetIPs)
        {
            string[] IPlist = ParseIPList(targetIPs);
            for (int i = 0; i < IPlist.Length; i++)
            {
                string targetIP = FinalizeIP(IPlist[i]);
                Debug.Log("Kill app on: " + targetIP);
                BuildDeployPortal.KillApp(appName, new BuildDeployPortal.ConnectInfo(targetIP, deviceUser, devicePassword));
            }
        }

        public void OpenLogFileForIPs(string IPs, string appName)
        {
            string[] ipList = ParseIPList(IPs);
            for (int i = 0; i < ipList.Length; i++)
            {
                // Use the Device Portal REST API
                BuildDeployPortal.DeviceLogFile_View(appName, new BuildDeployPortal.ConnectInfo(FinalizeIP(ipList[i]), deviceUser, devicePassword));
            }
        }
    }
}