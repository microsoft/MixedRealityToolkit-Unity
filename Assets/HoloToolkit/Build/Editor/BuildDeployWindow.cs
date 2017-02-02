//
// Copyright (c) @jevertt
// Copyright (c) Rafael Rivera
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Net;
using System;
using System.Xml;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Build window - supports SLN creation, APPX from SLN, Deploy on device, and misc helper utilities associated with the build/deploy/test iteration loop
    /// Requires the device to be set in developer mode & to have secure connections disabled (in the security tab in the device portal)
    /// </summary>
    public class BuildDeployWindow : EditorWindow
    {
        private const float GUISectionOffset = 10.0f;
        private const string GUIHorizSpacer = "     ";
        private const float UpdateBuildsPeriod = 1.0f;

        // Properties
        private bool ShouldOpenSLNBeEnabled { get { return !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); } }
        private bool ShouldBuildSLNBeEnabled { get { return !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); } }
        private bool ShouldBuildAppxBeEnabled
        {
            get { return 
                    !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory) && 
                    !string.IsNullOrEmpty(BuildDeployPrefs.MsBuildVersion) && 
                    !string.IsNullOrEmpty(BuildDeployPrefs.BuildConfig); }
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
            get { return !string.IsNullOrEmpty(BuildDeployPrefs.TargetIPs) && !string.IsNullOrEmpty(BuildDeployPrefs.BuildDirectory); }
        }
        private bool LocalIPsOnly { get { return true; } }

        // Privates
        private List<string> builds = new List<string>();
        private float timeLastUpdatedBuilds = 0.0f;

        // Functions
        [MenuItem("HoloToolkit/Build Window", false, 0)]
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
            this.minSize = new Vector2(600, 200);

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
            string appName = PlayerSettings.productName;

            // Build section
            GUILayout.BeginVertical();
            GUILayout.Label("SLN");

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Build directory (and save setting, if it's changed)
            string curBuildDirectory = BuildDeployPrefs.BuildDirectory;
            string newBuildDirectory = EditorGUILayout.TextField(GUIHorizSpacer + "Build directory", curBuildDirectory);
            if (newBuildDirectory != curBuildDirectory)
            {
                BuildDeployPrefs.BuildDirectory = newBuildDirectory;
                curBuildDirectory = newBuildDirectory;
            }

            // Build SLN button
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUI.enabled = ShouldOpenSLNBeEnabled;
                if (GUILayout.Button("Open SLN", GUILayout.Width(buttonWidth_Quarter)))
                {
                    // Open SLN
                    string slnFilename = Path.Combine(curBuildDirectory, PlayerSettings.productName + ".sln");
                    if (File.Exists(slnFilename))
                    {
                        FileInfo slnFile = new FileInfo(slnFilename);
                        System.Diagnostics.Process.Start(slnFile.FullName);
                    }
                    else if (EditorUtility.DisplayDialog("Solution Not Found", "We couldn't find the solution. Would you like to Build it?", "Yes, Build", "No"))
                    {
                        // Build SLN
                        EditorApplication.delayCall += () => { BuildDeployTools.BuildSLN(curBuildDirectory); };
                    }
                }
                GUI.enabled = ShouldBuildSLNBeEnabled;
                if (GUILayout.Button("Build Visual Studio SLN", GUILayout.Width(buttonWidth_Half)))
                {
                    // Build SLN
                    EditorApplication.delayCall += () => { BuildDeployTools.BuildSLN(curBuildDirectory); };
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
            string curMSBuildVer = BuildDeployPrefs.MsBuildVersion;
            string newMSBuildVer = EditorGUILayout.TextField(GUIHorizSpacer + "MSBuild Version", curMSBuildVer);
            if (newMSBuildVer != curMSBuildVer)
            {
                BuildDeployPrefs.MsBuildVersion = newMSBuildVer;
                curMSBuildVer = newMSBuildVer;
            }

            // Build config (and save setting, if it's changed)
            string curBuildConfig = BuildDeployPrefs.BuildConfig;
            string newBuildConfig = EditorGUILayout.TextField(GUIHorizSpacer + "Build Configuration", curBuildConfig);
            if (newBuildConfig != curBuildConfig)
            {
                BuildDeployPrefs.BuildConfig = newBuildConfig;
                curBuildConfig = newBuildConfig;
            }

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
                bool newIncrementVersion = EditorGUILayout.Toggle("Increment version", curIncrementVersion);
                if (newIncrementVersion != curIncrementVersion) {
                    BuildDeployPrefs.IncrementBuildVersion = newIncrementVersion;
                    curIncrementVersion = newIncrementVersion;
                }

                // Restore previous label width
                EditorGUIUtility.labelWidth = previousLabelWidth;

                // Build APPX
                GUI.enabled = ShouldBuildAppxBeEnabled;
                if (GUILayout.Button("Build APPX from SLN", GUILayout.Width(buttonWidth_Half)))
                {
                    BuildDeployTools.BuildAppxFromSolution(appName, curMSBuildVer, curForceRebuildAppx, curBuildConfig, curBuildDirectory, curIncrementVersion);
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
                    new GUIContent(GUIHorizSpacer + "IP Address(es)", "IP(s) of target devices (e.g. 127.0.0.1;10.11.12.13)"),
                    curTargetIps);

                if (newTargetIPs != curTargetIps)
                {
                    BuildDeployPrefs.TargetIPs = newTargetIPs;
                    curTargetIps = newTargetIPs;
                }
            }
            else
            {
                var locatorIsSearching = XdeGuestLocator.IsSearching;
                var locatorHasData = XdeGuestLocator.HasData;
                var xdeGuestIpAddress = XdeGuestLocator.GuestIpAddress;

                // Queue up a repaint if we're still busy, or we'll get stuck
                // in a disabled state.

                if (locatorIsSearching)
                {
                    Repaint();
                }

                var addressesToPresent = new List<string>();
                addressesToPresent.Add("127.0.0.1");

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

                var selectedAddressIndex = EditorGUILayout.Popup(GUIHorizSpacer + "IP Address", previouslySavedAddress, addressesToPresent.ToArray());

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
            string newUsername = EditorGUILayout.TextField(GUIHorizSpacer + "Username", curUsername);
            string curPassword = BuildDeployPrefs.DevicePassword;
            string newPassword = EditorGUILayout.PasswordField(GUIHorizSpacer + "Password", curPassword);
            bool curFullReinstall = BuildDeployPrefs.FullReinstall;
            bool newFullReinstall = EditorGUILayout.Toggle(
                new GUIContent(GUIHorizSpacer + "Uninstall first", "Uninstall application before installing"), curFullReinstall);
            if ((newUsername != curUsername) ||
                (newPassword != curPassword) ||
                (newFullReinstall != curFullReinstall))
            {
                BuildDeployPrefs.DeviceUser = newUsername;
                BuildDeployPrefs.DevicePassword = newPassword;
                BuildDeployPrefs.FullReinstall = newFullReinstall;

                curUsername = newUsername;
                curPassword = newPassword;
                curFullReinstall = newFullReinstall;
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
                        string thisBuildLocation = fullBuildLocation;
                        string[] IPlist = ParseIPList(curTargetIps);
                        EditorApplication.delayCall += () =>
                        {
                            InstallAppOnDevicesList(thisBuildLocation, curFullReinstall, IPlist);
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
                    OpenWebPortalForIPs(curTargetIps);
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
                    if (IsAppRunning_FirstIPCheck(appName, curTargetIps))
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
                GUI.enabled = ShouldLogViewBeEnabled;
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("View Log File", GUILayout.Width(buttonWidth_Full)))
                {
                    OpenLogFileForIPs(curTargetIps);
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
                    string[] IPlist = ParseIPList(curTargetIps);
                    EditorApplication.delayCall += () =>
                    {
                        UninstallAppOnDevicesList(IPlist);
                    };
                }
                GUI.enabled = true;
            }
            GUILayout.EndVertical();
        }

        void BuildAndRun(string appName)
        {
            // First build SLN
            if (!BuildDeployTools.BuildSLN(BuildDeployPrefs.BuildDirectory, false))
            {
                return;
            }

            // Next, APPX
            if (!BuildDeployTools.BuildAppxFromSolution(
                appName, 
                BuildDeployPrefs.MsBuildVersion, 
                BuildDeployPrefs.ForceRebuild, 
                BuildDeployPrefs.BuildConfig, 
                BuildDeployPrefs.BuildDirectory,
                BuildDeployPrefs.IncrementBuildVersion))
            {
                return;
            }

            // Next, Install
            string fullBuildLocation = CalcMostRecentBuild();
            string[] IPlist = ParseIPList(BuildDeployPrefs.TargetIPs);
            InstallAppOnDevicesList(fullBuildLocation, BuildDeployPrefs.FullReinstall, IPlist);
        }

        private string CalcMostRecentBuild()
        {
            UpdateBuilds();
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
            using (XmlTextReader reader = new XmlTextReader(manifest))
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
            return "";
        }

        private void InstallAppOnDevicesList(string buildPath, bool uninstallBeforeInstall, string[] targetList)
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
                    string IP = FinalizeIP(targetList[i]);
                    if (BuildDeployPrefs.FullReinstall &&
                        BuildDeployPortal.IsAppInstalled(packageFamilyName, new BuildDeployPortal.ConnectInfo(IP, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword)))
                    {
                        EditorUtility.DisplayProgressBar("Installing on devices", "Uninstall (" + IP + ")", (float)i / (float)targetList.Length);
                        if (!UninstallApp(packageFamilyName, IP))
                        {
                            Debug.LogError("Uninstall failed - skipping install (" + IP + ")");
                            continue;
                        }
                        completedUninstall = true;
                    }
                    EditorUtility.DisplayProgressBar("Installing on devices", "Install (" + IP + ")", (float)(i + (completedUninstall ? 0.5f : 0.0f)) / (float)targetList.Length);
                    InstallApp(buildPath, packageFamilyName, IP);
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
            files = (files.Length == 0) ? (new DirectoryInfo(buildPath)).GetFiles("*.appxbundle") : files;
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
                    string IP = FinalizeIP(targetList[i]);
                    EditorUtility.DisplayProgressBar("Uninstalling application", "Uninstall (" + IP + ")", (float)i / (float)targetList.Length);
                    UninstallApp(packageFamilyName, IP);
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
            this.builds.Clear();

            try
            {
                List<string> appPackageDirectories = new List<string>();
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
                return BuildDeployPortal.IsAppRunning(
                    appName, new BuildDeployPortal.ConnectInfo(targetIP, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
            }
            return false;
        }

        void LaunchAppOnIPs(string targetIPs)
        {
            string packageFamilyName = CalcPackageFamilyName();
            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            string[] IPlist = ParseIPList(targetIPs);
            for (int i = 0; i < IPlist.Length; i++)
            {
                string targetIP = FinalizeIP(IPlist[i]);
                Debug.Log("Launch app on: " + targetIP);
                BuildDeployPortal.LaunchApp(
                    packageFamilyName, new BuildDeployPortal.ConnectInfo(targetIP, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
            }
        }

        void KillAppOnIPs(string targetIPs)
        {
            string packageFamilyName = CalcPackageFamilyName();
            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            string[] IPlist = ParseIPList(targetIPs);
            for (int i = 0; i < IPlist.Length; i++)
            {
                string targetIP = FinalizeIP(IPlist[i]);
                Debug.Log("Kill app on: " + targetIP);
                BuildDeployPortal.KillApp(
                    packageFamilyName, new BuildDeployPortal.ConnectInfo(targetIP, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
            }
        }

        public void OpenLogFileForIPs(string IPs)
        {
            string packageFamilyName = CalcPackageFamilyName();
            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return;
            }

            string[] ipList = ParseIPList(IPs);
            for (int i = 0; i < ipList.Length; i++)
            {
                // Use the Device Portal REST API
                BuildDeployPortal.DeviceLogFile_View(
                    packageFamilyName, new BuildDeployPortal.ConnectInfo(FinalizeIP(ipList[i]), BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
            }
        }
    }
}