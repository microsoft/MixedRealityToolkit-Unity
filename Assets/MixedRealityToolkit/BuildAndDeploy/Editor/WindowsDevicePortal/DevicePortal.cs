// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Build.WindowsDevicePortal.DataStructures;
using MixedRealityToolkit.Common.AsyncUtilities;
using MixedRealityToolkit.Common.RestUtility;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using FileInfo = System.IO.FileInfo;

namespace MixedRealityToolkit.Build.WindowsDevicePortal
{
    /// <summary>
    /// Function used to communicate with Windows 10 devices through the device portal REST APIs.
    /// </summary>
    public static class DevicePortal
    {
        private enum AppInstallStatus
        {
            Invalid,
            Installing,
            InstallSuccess,
            InstallFail
        }

        // Device Portal API Resources
        // https://docs.microsoft.com/en-us/windows/uwp/debug-test-perf/device-portal-api-hololens#holographic-os
        // https://docs.microsoft.com/en-us/windows/uwp/debug-test-perf/device-portal-api-core
        private const string GetDeviceOsInfoQuery = @"{0}/api/os/info";
        private const string GetMachineNameQuery = @"{0}/api/os/machinename";
        private const string GetBatteryQuery = @"{0}/api/power/battery";
        private const string GetPowerStateQuery = @"{0}/api/power/state";
        private const string RestartDeviceQuery = @"{0}/api/control/restart";
        private const string ShutdownDeviceQuery = @"{0}/api/control/shutdown";
        private const string ProcessQuery = @"{0}/api/resourcemanager/processes";
        private const string AppQuery = @"{0}/api/taskmanager/app";
        private const string PackagesQuery = @"{0}/api/appx/packagemanager/packages";
        private const string InstallQuery = @"{0}/api/app/packagemanager/package";
        private const string InstallStatusQuery = @"{0}/api/app/packagemanager/state";
        private const string FileQuery = @"{0}/api/filesystem/apps/file?knownfolderid=LocalAppData&filename=UnityPlayer.log&packagefullname={1}&path=%5C%5CTempState";
        private const string IpConfigQuery = @"{0}/api/networking/ipconfig";
        private const string WiFiNetworkQuery = @"{0}/api/wifi/network{1}";
        private const string WiFiInterfacesQuery = @"{0}/api/wifi/interfaces";

#if !UNITY_WSA || UNITY_EDITOR
        /// <summary>
        /// Opens the Device Portal for the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        public static void OpenWebPortal(DeviceInfo targetDevice)
        {
            //TODO: Figure out how to pass username and password to browser?
            Process.Start(FinalizeUrl(targetDevice.IP));
        }
#endif

        /// <summary>
        /// Gets the <see cref="DeviceOsInfo"/> of the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns><see cref="DeviceOsInfo"/></returns>
        public static async Task<DeviceOsInfo> GetDeviceOsInfoAsync(DeviceInfo targetDevice)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return null; }

            string query = string.Format(GetDeviceOsInfoQuery, FinalizeUrl(targetDevice.IP));
            var response = await Rest.GetAsync(query, targetDevice.Authorization);
            return response.Successful ? JsonUtility.FromJson<DeviceOsInfo>(response.ResponseBody) : null;
        }

        /// <summary>
        /// Gets the <see cref="MachineName"/> of the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns><see cref="MachineName"/></returns>
        public static async Task<MachineName> GetMachineNameAsync(DeviceInfo targetDevice)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return null; }

            string query = string.Format(GetMachineNameQuery, FinalizeUrl(targetDevice.IP));
            var response = await Rest.GetAsync(query, targetDevice.Authorization);
            return response.Successful ? JsonUtility.FromJson<MachineName>(response.ResponseBody) : null;
        }

        /// <summary>
        /// Gets the <see cref="BatteryInfo"/> of the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns><see cref="BatteryInfo"/></returns>
        public static async Task<BatteryInfo> GetBatteryStateAsync(DeviceInfo targetDevice)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return null; }

            string query = string.Format(GetBatteryQuery, FinalizeUrl(targetDevice.IP));
            var response = await Rest.GetAsync(query, targetDevice.Authorization);
            return response.Successful ? JsonUtility.FromJson<BatteryInfo>(response.ResponseBody) : null;
        }

        /// <summary>
        /// Gets the <see cref="PowerStateInfo"/> of the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns><see cref="PowerStateInfo"/></returns>
        public static async Task<PowerStateInfo> GetPowerStateAsync(DeviceInfo targetDevice)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return null; }

            string query = string.Format(GetPowerStateQuery, FinalizeUrl(targetDevice.IP));
            var response = await Rest.GetAsync(query, targetDevice.Authorization);
            return response.Successful ? JsonUtility.FromJson<PowerStateInfo>(response.ResponseBody) : null;
        }

        /// <summary>
        /// Restart the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns>True, if the device has successfully restarted.</returns>
        public static async Task<bool> RestartAsync(DeviceInfo targetDevice)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return false; }

            var response = await Rest.PostAsync(string.Format(RestartDeviceQuery, FinalizeUrl(targetDevice.IP)), targetDevice.Authorization);
            if (response.Successful)
            {
                bool hasRestarted = false;
                string query = string.Format(GetPowerStateQuery, FinalizeUrl(targetDevice.IP));

                while (!hasRestarted)
                {
                    response = await Rest.GetAsync(query, targetDevice.Authorization);
                    hasRestarted = response.Successful;
                }
            }

            return response.Successful;
        }

        /// <summary>
        /// Shuts down the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns>True, if the device is shitting down.</returns>
        public static async Task<bool> ShutdownAsync(DeviceInfo targetDevice)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return false; }

            var response = await Rest.PostAsync(string.Format(ShutdownDeviceQuery, FinalizeUrl(targetDevice.IP)), targetDevice.Authorization);
            return response.Successful;
        }

        /// <summary>
        /// Determines if the target application is currently running on the target device.
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="targetDevice"></param>
        /// <returns>True, if application is currently installed on device.</returns>
        public static async Task<bool> IsAppInstalledAsync(string packageName, DeviceInfo targetDevice)
        {
            return await GetApplicationInfoAsync(packageName, targetDevice) != null;
        }

        /// <summary>
        /// Determines if the target application is running on the target device.
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="targetDevice"></param>
        /// <returns>True, if the application is running.</returns>
        public static async Task<bool> IsAppRunningAsync(string packageName, DeviceInfo targetDevice)
        {
            var appInfo = await GetApplicationInfoAsync(packageName, targetDevice);

            if (appInfo == null)
            {
                return false;
            }

            var response = await Rest.GetAsync(string.Format(ProcessQuery, FinalizeUrl(targetDevice.IP)), targetDevice.Authorization);

            if (response.Successful)
            {
                var processList = JsonUtility.FromJson<ProcessList>(response.ResponseBody);
                for (int i = 0; i < processList.Processes.Length; ++i)
                {
                    if (processList.Processes[i].ImageName.Contains(appInfo.Name))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the <see cref="ApplicationInfo"/> of the target application on the target device.
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="targetDevice"></param>
        /// <returns>Returns the <see cref="ApplicationInfo"/> of the target application from the target device.</returns>
        private static async Task<ApplicationInfo> GetApplicationInfoAsync(string packageName, DeviceInfo targetDevice)
        {
            var appList = await GetAllInstalledAppsAsync(targetDevice);

            for (int i = 0; i < appList?.InstalledPackages.Length; ++i)
            {
                if (appList.InstalledPackages[i].PackageFullName.Equals(packageName, StringComparison.OrdinalIgnoreCase))
                {
                    return appList.InstalledPackages[i];
                }

                if (appList.InstalledPackages[i].PackageFamilyName.Equals(packageName, StringComparison.OrdinalIgnoreCase))
                {
                    return appList.InstalledPackages[i];
                }
            }

            return null;
        }

        public static async Task<InstalledApps> GetAllInstalledAppsAsync(DeviceInfo targetDevice)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return null; }

            var response = await Rest.GetAsync(string.Format(PackagesQuery, FinalizeUrl(targetDevice.IP)), targetDevice.Authorization);
            return response.Successful ? JsonUtility.FromJson<InstalledApps>(response.ResponseBody) : null;
        }

        /// <summary>
        /// Installs the target application on the target device.
        /// </summary>
        /// <param name="appFullPath"></param>
        /// <param name="targetDevice"></param>
        /// <param name="waitForDone">Should the thread wait until installation is complete?</param>
        /// <returns>True, if Installation was a success.</returns>
        public static async Task<bool> InstallAppAsync(string appFullPath, DeviceInfo targetDevice, bool waitForDone = true)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return false; }

            // Calculate the cert and dependency paths
            string fileName = Path.GetFileName(appFullPath);
            string certFullPath = Path.ChangeExtension(appFullPath, ".cer");
            string certName = Path.GetFileName(certFullPath);
            string depPath = Path.GetDirectoryName(appFullPath) + @"\Dependencies\x86\";

            var form = new WWWForm();

            try
            {
                // APPX file
                Debug.Assert(appFullPath != null);
                using (var stream = new FileStream(appFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        form.AddBinaryData(fileName, reader.ReadBytes((int)reader.BaseStream.Length), fileName);
                    }
                }

                // CERT file
                Debug.Assert(certFullPath != null);
                using (var stream = new FileStream(certFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        form.AddBinaryData(certName, reader.ReadBytes((int)reader.BaseStream.Length), certName);
                    }
                }

                // Dependencies
                FileInfo[] depFiles = new DirectoryInfo(depPath).GetFiles();
                foreach (FileInfo dep in depFiles)
                {
                    using (var stream = new FileStream(dep.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            string depFilename = Path.GetFileName(dep.FullName);
                            form.AddBinaryData(depFilename, reader.ReadBytes((int)reader.BaseStream.Length), depFilename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            // Query
            string query = $"{string.Format(InstallQuery, FinalizeUrl(targetDevice.IP))}?package={WWW.EscapeURL(fileName)}";

            var response = await Rest.PostAsync(query, form, targetDevice.Authorization);

            if (!response.Successful)
            {
                Debug.LogErrorFormat("Failed to install {0} on {1}.", fileName, targetDevice.MachineName);
                return false;
            }

            var status = AppInstallStatus.Installing;

            // Wait for done (if requested)
            while (waitForDone && status == AppInstallStatus.Installing)
            {
                status = await GetInstallStatusAsync(targetDevice);

                switch (status)
                {
                    case AppInstallStatus.InstallSuccess:
                        Debug.LogFormat("Successfully installed {0} on {1}.", fileName, targetDevice.MachineName);
                        return true;
                    case AppInstallStatus.InstallFail:
                        Debug.LogErrorFormat("Failed to install {0} on {1}.", fileName, targetDevice.MachineName);
                        return false;
                }
            }

            return true;
        }

        private static async Task<AppInstallStatus> GetInstallStatusAsync(DeviceInfo targetDevice)
        {
            var response = await Rest.GetAsync(string.Format(InstallStatusQuery, FinalizeUrl(targetDevice.IP)), targetDevice.Authorization);

            if (response.Successful)
            {
                var status = JsonUtility.FromJson<InstallStatus>(response.ResponseBody);

                if (status == null)
                {
                    return AppInstallStatus.Installing;
                }

                if (status.Success)
                {
                    return AppInstallStatus.InstallSuccess;
                }

                Debug.LogErrorFormat("{0} ({1})", status.Reason, status.CodeText);
            }
            else
            {
                return AppInstallStatus.Installing;
            }

            return AppInstallStatus.InstallFail;
        }

        /// <summary>
        /// Uninstalls the target application on the target device
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="targetDevice"></param>
        /// <returns>True, if uninstall was a success.</returns>
        public static async Task<bool> UninstallAppAsync(string packageName, DeviceInfo targetDevice)
        {
            ApplicationInfo applicationInfo = await GetApplicationInfoAsync(packageName, targetDevice);

            if (applicationInfo == null)
            {
                Debug.Log($"Application '{packageName}' not found");
                return false;
            }

            string query = $"{string.Format(InstallQuery, FinalizeUrl(targetDevice.IP))}?package={WWW.EscapeURL(applicationInfo.PackageFullName)}";
            var response = await Rest.DeleteAsync(query, targetDevice.Authorization);

            if (response.Successful)
            {
                Debug.LogFormat("Successfully uninstalled {0} on {1}.", packageName, targetDevice.MachineName);
            }
            else
            {
                Debug.LogErrorFormat("Failed to uninstall {0} on {1}", packageName, targetDevice.MachineName);
            }

            return response.Successful;
        }

        /// <summary>
        /// Launches the target application on the target device.
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="targetDevice"></param>
        /// <returns>True, if application was successfully launched and is currently running on the target device.</returns>
        public static async Task<bool> LaunchAppAsync(string packageName, DeviceInfo targetDevice)
        {
            // Find the app description
            ApplicationInfo applicationInfo = await GetApplicationInfoAsync(packageName, targetDevice);

            if (applicationInfo == null)
            {
                Debug.LogWarning("Application not found");
                return false;
            }

            string query = $"{string.Format(AppQuery, FinalizeUrl(targetDevice.IP))}?appid={WWW.EscapeURL(EncodeTo64(applicationInfo.PackageRelativeId))}&package={WWW.EscapeURL(applicationInfo.PackageFullName)}";
            var response = await Rest.PostAsync(query, targetDevice.Authorization);
            return response.Successful;
        }

        /// <summary>
        /// Stops the target application on the target device.
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="targetDevice"></param>
        /// <returns>true, if application was successfully stopped.</returns>
        public static async Task<bool> StopAppAsync(string packageName, DeviceInfo targetDevice)
        {
            ApplicationInfo applicationInfo = await GetApplicationInfoAsync(packageName, targetDevice);
            if (applicationInfo == null)
            {
                Debug.LogError("Application not found");
                return false;
            }

            string query = $"{string.Format(AppQuery, FinalizeUrl(targetDevice.IP))}?package={WWW.EscapeURL(EncodeTo64(applicationInfo.PackageFullName))}";
            Response response = await Rest.DeleteAsync(query, targetDevice.Authorization);
            return response.Successful;
        }

        /// <summary>
        /// Downloads and launches the Log file for the target application on the target device.
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="targetDevice"></param>
        /// <returns>The path of the downloaded log file.</returns>
        public static async Task<string> DownloadLogFileAsync(string packageName, DeviceInfo targetDevice)
        {
            ApplicationInfo applicationInfo = await GetApplicationInfoAsync(packageName, targetDevice);

            if (applicationInfo == null)
            {
                Debug.LogWarningFormat("{0} not installed on target device", packageName);
                return string.Empty;
            }

            string logFile = $"{Application.temporaryCachePath}/{targetDevice.MachineName}_{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}_deviceLog.txt";
            var response = await Rest.GetAsync(string.Format(FileQuery, FinalizeUrl(targetDevice.IP), applicationInfo.PackageFullName), targetDevice.Authorization);

            if (response.Successful)
            {
                File.WriteAllText(logFile, response.ResponseBody);
                return logFile;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the <see cref="IpConfigInfo"/> of the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns><see cref="IpConfigInfo"/></returns>
        public static async Task<IpConfigInfo> GetIpConfigInfoAsync(DeviceInfo targetDevice)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return null; }

            string query = string.Format(IpConfigQuery, FinalizeUrl(targetDevice.IP));
            var response = await Rest.GetAsync(query, targetDevice.Authorization);
            return response.Successful ? JsonUtility.FromJson<IpConfigInfo>(response.ResponseBody) : null;
        }

        /// <summary>
        /// Gets the <see cref="AvailableWiFiNetworks"/> of the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <param name="interfaceInfo">The GUID for the network interface to use to search for wireless networks, without brackets.</param>
        /// <returns><see cref="AvailableWiFiNetworks"/></returns>
        public static async Task<AvailableWiFiNetworks> GetAvailableWiFiNetworksAsync(DeviceInfo targetDevice, InterfaceInfo interfaceInfo)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return null; }

            string query = string.Format(WiFiNetworkQuery, FinalizeUrl(targetDevice.IP), $"s?interface={interfaceInfo.GUID}");
            var response = await Rest.GetAsync(query, targetDevice.Authorization);
            return response.Successful ? JsonUtility.FromJson<AvailableWiFiNetworks>(response.ResponseBody) : null;
        }

        /// <summary>
        /// Connects to the specified WiFi Network.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <param name="interfaceInfo">The interface to use to connect.</param>
        /// <param name="wifiNetwork">The network to connect to.</param>
        /// <param name="password">Password for network access.</param>
        /// <returns>True, if connection successful.</returns>
        public static async Task<Response> ConnectToWiFiNetworkAsync(DeviceInfo targetDevice, InterfaceInfo interfaceInfo, WirelessNetworkInfo wifiNetwork, string password)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return new Response(false, "Unable to authenticate with device", null, 403); }

            string query = string.Format(WiFiNetworkQuery, FinalizeUrl(targetDevice.IP),
                $"?interface={interfaceInfo.GUID}&ssid={EncodeTo64(wifiNetwork.SSID)}&op=connect&createprofile=yes&key={password}");
            return await Rest.PostAsync(query, targetDevice.Authorization);
        }

        /// <summary>
        /// Gets the <see cref="NetworkInterfaces"/> of the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns><see cref="NetworkInterfaces"/></returns>
        public static async Task<NetworkInterfaces> GetWiFiNetworkInterfacesAsync(DeviceInfo targetDevice)
        {
            var isAuth = await EnsureAuthenticationAsync(targetDevice);
            if (!isAuth) { return null; }

            string query = string.Format(WiFiInterfacesQuery, FinalizeUrl(targetDevice.IP));
            var response = await Rest.GetAsync(query, targetDevice.Authorization);
            return response.Successful ? JsonUtility.FromJson<NetworkInterfaces>(response.ResponseBody) : null;
        }

        /// <summary>
        /// This Utility method finalizes the URL and formats the HTTPS string if needed.
        /// <remarks>Local Machine will be changed to 127.0.1:10080 for HoloLens connections.</remarks>
        /// </summary>
        /// <param name="targetUrl">The target URL i.e. 128.128.128.128</param>
        /// <returns>The finalized URL with http/https prefix.</returns>
        public static string FinalizeUrl(string targetUrl)
        {
            string ssl = Rest.UseSSL ? "s" : string.Empty;

            if (targetUrl.Contains("Local Machine"))
            {
                targetUrl = "127.0.0.1:10080";
                ssl = string.Empty;
            }

            return $@"http{ssl}://{targetUrl}";
        }

        public static async Task<bool> EnsureAuthenticationAsync(DeviceInfo targetDevice)
        {
            string auth = Rest.GetBasicAuthentication(targetDevice.User, targetDevice.Password);

            if (targetDevice.Authorization.ContainsKey("Authorization"))
            {
                targetDevice.Authorization["Authorization"] = auth;
            }
            else
            {
                targetDevice.Authorization.Add("Authorization", auth);
            }

            bool success;

            if (!targetDevice.Authorization.ContainsKey("cookie"))
            {
                var response = await DevicePortalAuthorizationAsync(targetDevice);
                success = response.Successful;
                if (success)
                {
                    targetDevice.CsrfToken = response.ResponseBody;

                    // Strip the beginning of the cookie
                    targetDevice.CsrfToken = targetDevice.CsrfToken?.Replace("CSRF-Token=", string.Empty);
                }

                if (!string.IsNullOrEmpty(targetDevice.CsrfToken))
                {
                    if (!targetDevice.Authorization.ContainsKey("cookie"))
                    {
                        targetDevice.Authorization.Add("cookie", targetDevice.CsrfToken);
                    }
                    else
                    {
                        targetDevice.Authorization["cookie"] = targetDevice.CsrfToken;
                    }

                    if (targetDevice.Authorization.ContainsKey("x-csrf-token"))
                    {
                        targetDevice.Authorization["x-csrf-token"] = targetDevice.CsrfToken;
                    }
                    else
                    {
                        targetDevice.Authorization.Add("x-csrf-token", targetDevice.CsrfToken);
                    }
                }
            }
            else
            {
                success = true;
            }

            return success;
        }

        private static async Task<Response> DevicePortalAuthorizationAsync(DeviceInfo targetDevice)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(FinalizeUrl(targetDevice.IP));

            webRequest.timeout = 5;

            webRequest.SetRequestHeader("Authorization", targetDevice.Authorization["Authorization"]);

            await webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                if (webRequest.responseCode == 401)
                {
                    Debug.LogError($"Invalid Credentials: {webRequest.responseCode}");
                    return new Response(false, "Invalid Credentials", null, webRequest.responseCode);
                }

                if (webRequest.GetResponseHeaders() == null)
                {
                    return new Response(false, "Device Not Found", null, webRequest.responseCode);
                }

                string responseHeaders = webRequest.GetResponseHeaders().Aggregate(string.Empty, (current, header) => $"\n{header.Key}: {header.Value}");
                Debug.LogError($"REST Auth Error: {webRequest.responseCode}\n{webRequest.downloadHandler?.text}{responseHeaders}");
                return new Response(false, webRequest.downloadHandler?.text, null, webRequest.responseCode);
            }

            return new Response(true, webRequest.GetResponseHeader("Set-Cookie"), null, webRequest.responseCode);
        }

        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }
}
