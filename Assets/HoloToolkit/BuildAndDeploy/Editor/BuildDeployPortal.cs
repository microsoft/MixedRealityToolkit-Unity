// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Function used to communicate with the device through the REST API
    /// </summary>
    public static class BuildDeployPortal
    {
        private enum AppInstallStatus
        {
            Invalid,
            Installing,
            InstallSuccess,
            InstallFail
        }

        private const float TimeOut = 10.0f;
        private const float MaxWaitTime = 20.0f;

        // Device Portal API Resources
        // https://docs.microsoft.com/en-us/windows/uwp/debug-test-perf/device-portal-api-hololens#holographic-os
        // https://docs.microsoft.com/en-us/windows/uwp/debug-test-perf/device-portal-api-core
        private static readonly string API_GetMachineNameQuery = @"{0}/api/os/machinename";
        private static readonly string API_ProcessQuery = @"{0}/api/resourcemanager/processes";
        private static readonly string API_PackagesQuery = @"{0}/api/appx/packagemanager/packages";
        private static readonly string API_InstallQuery = @"{0}/api/app/packagemanager/package";
        private static readonly string API_InstallStatusQuery = @"{0}/api/app/packagemanager/state";
        private static readonly string API_AppQuery = @"{0}/api/taskmanager/app";
        private static readonly string API_FileQuery = @"{0}/api/filesystem/apps/file?knownfolderid=LocalAppData&filename=UnityPlayer.log&packagefullname={1}&path=%5C%5CTempState";
        private static readonly string API_IpConfigQuery = @"{0}/api/networking/ipconfig";

        /// <summary>
        /// Gets the Basic auth header.
        /// <remarks>If you're using SSL and making HTTPS requests you must also specify if the request is of GET type or not, 
        /// so we know if we should append the "auto-" prefix to bypass CSRF.</remarks>
        /// </summary>
        /// <param name="connectionInfo">target device connection info.</param>
        /// <param name="isGetRequest">If the request you're attempting to make is a GET type</param>
        /// <returns></returns>
        private static string GetBasicAuthHeader(ConnectInfo connectionInfo, bool isGetRequest = false)
        {
            var auth = string.Format("{0}{1}:{2}", BuildDeployPrefs.UseSSL && !isGetRequest ? "auto-" : "", connectionInfo.User, connectionInfo.Password);
            auth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
            return string.Format("Basic {0}", auth);
        }

        /// <summary>
        /// Send a Unity Web Request to GET.
        /// </summary>
        /// <param name="query">Full Query to GET</param>
        /// <param name="auth">Authorization header</param>
        /// <param name="showProgressDialog">Show the progress dialog.</param>
        /// <returns>Response string.</returns>
        private static string WebRequestGet(string query, string auth, bool showProgressDialog = true)
        {
            try
            {
                using (var webRequest = UnityWebRequest.Get(query))
                {
                    webRequest.SetRequestHeader("Authorization", auth);
#if UNITY_2017_1_OR_NEWER
                    webRequest.timeout = (int)TimeOut;
#endif

#if UNITY_2017_2_OR_NEWER
                    webRequest.SendWebRequest();
#else
                    webRequest.Send();
#endif

                    while (!webRequest.isDone)
                    {
                        if (webRequest.downloadProgress > -1 && showProgressDialog)
                        {
                            EditorUtility.DisplayProgressBar("Connecting to Device Portal",
                                                             "Progress...", webRequest.downloadProgress);
                        }
                    }

                    if (showProgressDialog)
                    {
                        EditorUtility.ClearProgressBar();
                    }

                    if (
#if UNITY_2017_2_OR_NEWER
                        webRequest.isNetworkError || webRequest.isHttpError && 
#else
                        webRequest.isError &&
#endif
                        webRequest.responseCode != 401)
                    {
                        string response = string.Empty;
                        var responseHeaders = webRequest.GetResponseHeaders();
                        if (responseHeaders != null)
                        {
                            response = responseHeaders.Aggregate(string.Empty, (current, header) => string.Format("{0}{1}: {2}\n", current, header.Key, header.Value));
                        }

                        Debug.LogErrorFormat("Network Error: {0}\n{1}", webRequest.error, response);
                        return string.Empty;
                    }

                    switch (webRequest.responseCode)
                    {
                        case 200:
                        case 204:
                            return webRequest.downloadHandler.text;
                        case 401:
                            Debug.LogError("Unauthorized: Access is denied due to invalid credentials.");
                            break;
                        default:
                            Debug.LogError(webRequest.responseCode);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return string.Empty;
        }

        /// <summary>
        /// Send a Unity Web Request to POST.
        /// </summary>
        /// <param name="query">Full Query to GET</param>
        /// <param name="postData">Post Data</param>
        /// <param name="auth">Authorization Header</param>
        /// <param name="showDialog">Show the progress dialog.</param>
        /// <returns>Response string.</returns>
        private static string WebRequestPost(string query, WWWForm postData, string auth, bool showDialog = true)
        {
            try
            {
                using (var webRequest = UnityWebRequest.Post(query, postData))
                {
                    webRequest.SetRequestHeader("Authorization", auth);
#if UNITY_2017_1_OR_NEWER
                    webRequest.timeout = (int)TimeOut;
#endif

                    // HACK: Workaround for extra quotes around boundary.
                    string contentType = webRequest.GetRequestHeader("Content-Type");
                    if (contentType != null)
                    {
                        contentType = contentType.Replace("\"", "");
                        webRequest.SetRequestHeader("Content-Type", contentType);
                    }

#if UNITY_2017_2_OR_NEWER
                    webRequest.SendWebRequest();
#else
                    webRequest.Send();
#endif

                    while (!webRequest.isDone)
                    {
                        if (webRequest.uploadProgress > -1 && showDialog)
                        {
                            EditorUtility.DisplayProgressBar("Connecting to Device Portal",
                                                             "Uploading...", webRequest.uploadProgress);
                        }
                        else if (webRequest.downloadProgress > -1 && showDialog)
                        {
                            EditorUtility.DisplayProgressBar("Connecting to Device Portal",
                                                             "Progress...", webRequest.downloadProgress);
                        }
                    }

                    EditorUtility.ClearProgressBar();

                    if (
#if UNITY_2017_2_OR_NEWER
                        webRequest.isNetworkError || webRequest.isHttpError && 
#else
                        webRequest.isError &&
#endif
                        webRequest.responseCode != 401)
                    {
                        string response = string.Empty;
                        var responseHeaders = webRequest.GetResponseHeaders();
                        if (responseHeaders != null)
                        {
                            response = responseHeaders.Aggregate(string.Empty, (current, header) => string.Format("{0}{1}: {2}\n", current, header.Key, header.Value));
                        }

                        Debug.LogErrorFormat("Network Error: {0}\n{1}", webRequest.error, response);
                        return string.Empty;
                    }

                    switch (webRequest.responseCode)
                    {
                        case 200:
                        case 202:
                            return webRequest.downloadHandler.text;
                        case 401:
                            Debug.LogError("Unauthorized: Access is denied due to invalid credentials.");
                            break;
                        default:
                            Debug.LogError(webRequest.responseCode);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return string.Empty;
        }

        /// <summary>
        /// Send a Unity Web Request to DELETE
        /// </summary>
        /// <param name="query">Full Query.</param>
        /// <param name="auth">Authorization Header</param>
        /// <param name="showDialog">Show to progress dialog</param>
        /// <returns>Successful or not.</returns>
        private static bool WebRequestDelete(string query, string auth, bool showDialog = true)
        {
            try
            {
                using (var webRequest = UnityWebRequest.Delete(query))
                {
                    webRequest.SetRequestHeader("Authorization", auth);
#if UNITY_2017_1_OR_NEWER
                    webRequest.timeout = (int)TimeOut;
#endif

#if UNITY_2017_2_OR_NEWER
                    webRequest.SendWebRequest();
#else
                    webRequest.Send();
#endif

                    while (!webRequest.isDone)
                    {
                        if (showDialog && webRequest.downloadProgress > -1)
                        {
                            EditorUtility.DisplayProgressBar("Connecting to Device Portal",
                                                             "Progress...", webRequest.downloadProgress);
                        }
                    }

                    EditorUtility.ClearProgressBar();

                    if (
#if UNITY_2017_2_OR_NEWER
                        webRequest.isNetworkError || webRequest.isHttpError && 
#else
                        webRequest.isError &&
#endif
                        webRequest.responseCode != 401)
                    {
                        string response = string.Empty;
                        var responseHeaders = webRequest.GetResponseHeaders();
                        if (responseHeaders != null)
                        {
                            response = responseHeaders.Aggregate(string.Empty, (current, header) => string.Format("{0}{1}: {2}\n", current, header.Key, header.Value));
                        }

                        Debug.LogErrorFormat("Network Error: {0}\n{1}", webRequest.error, response);
                        return false;
                    }

                    switch (webRequest.responseCode)
                    {
                        case 200:
                            return true;
                        case 401:
                            Debug.LogError("Unauthorized: Access is denied due to invalid credentials.");
                            break;
                        default:
                            Debug.LogError(webRequest.responseCode);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return false;
        }

        /// <summary>
        /// Opens the Device Portal for the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        public static void OpenWebPortal(ConnectInfo targetDevice)
        {
            //TODO: Figure out how to pass username and password to browser?
            Process.Start(FinalizeUrl(targetDevice.IP));
        }

        /// <summary>
        /// Gets the <see cref="MachineName"/> of the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns><see cref="MachineName"/></returns>
        public static MachineName GetMachineName(ConnectInfo targetDevice)
        {
            MachineName machineName = null;
            string query = string.Format(API_GetMachineNameQuery, FinalizeUrl(targetDevice.IP));
            string response = WebRequestGet(query, GetBasicAuthHeader(targetDevice, true), false);

            if (!string.IsNullOrEmpty(response))
            {
                machineName = JsonUtility.FromJson<MachineName>(response);
            }

            return machineName;
        }

        [Obsolete("Use IsAppInstalled(string packageFamilyName, ConnectInfo targetDevice)")]
        public static bool IsAppInstalled(string packageFamilyName, string targetIp)
        {
            return QueryAppDetails(packageFamilyName, new ConnectInfo(targetIp, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword)) != null;
        }

        /// <summary>
        /// Determines if the target application is currently running on the target device.
        /// </summary>
        /// <param name="packageFamilyName"></param>
        /// <param name="targetDevice"></param>
        /// <returns>True, if application is currently installed on device.</returns>
        public static bool IsAppInstalled(string packageFamilyName, ConnectInfo targetDevice)
        {
            return QueryAppDetails(packageFamilyName, targetDevice) != null;
        }

        [Obsolete("IsAppRunning(string appName, ConnectInfo targetDevice)")]
        public static bool IsAppRunning(string appName, string targetDevice)
        {
            return IsAppRunning(appName, new ConnectInfo(targetDevice, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
        }

        /// <summary>
        /// Determines if the target application is running on the target device.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="targetDevice"></param>
        /// <returns>True, if the application is running.</returns>
        public static bool IsAppRunning(string appName, ConnectInfo targetDevice)
        {
            string response = WebRequestGet(string.Format(API_ProcessQuery, FinalizeUrl(targetDevice.IP)), GetBasicAuthHeader(targetDevice, true), false);

            if (!string.IsNullOrEmpty(response))
            {
                var processList = JsonUtility.FromJson<ProcessList>(response);
                for (int i = 0; i < processList.Processes.Length; ++i)
                {
                    string processName = processList.Processes[i].ImageName;

                    if (processName.Contains(appName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the <see cref="AppDetails"/> of the target application from the target device.
        /// </summary>
        /// <param name="packageFamilyName"></param>
        /// <param name="targetDevice"></param>
        /// <returns>null if application is not currently installed on the target device.</returns>
        private static AppDetails QueryAppDetails(string packageFamilyName, ConnectInfo targetDevice)
        {
            string response = WebRequestGet(string.Format(API_PackagesQuery, FinalizeUrl(targetDevice.IP)), GetBasicAuthHeader(targetDevice, true), false);

            if (!string.IsNullOrEmpty(response))
            {
                var appList = JsonUtility.FromJson<AppList>(response);
                for (int i = 0; i < appList.InstalledPackages.Length; ++i)
                {
                    string thisAppName = appList.InstalledPackages[i].PackageFamilyName;
                    if (thisAppName.Equals(packageFamilyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return appList.InstalledPackages[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Installs the target application on the target device.
        /// </summary>
        /// <param name="appFullPath"></param>
        /// <param name="targetDevice"></param>
        /// <param name="waitForDone">Should the thread wait until installation is complete?</param>
        /// <returns>True, if Installation was a success.</returns>
        public static bool InstallApp(string appFullPath, ConnectInfo targetDevice, bool waitForDone = true)
        {
            bool success = false;

            try
            {
                // Calculate the cert and dependency paths
                string fileName = Path.GetFileName(appFullPath);
                string certFullPath = Path.ChangeExtension(appFullPath, ".cer");
                string certName = Path.GetFileName(certFullPath);
                string depPath = Path.GetDirectoryName(appFullPath) + @"\Dependencies\x86\";

                // Post it using the REST API
                var form = new WWWForm();

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

                // Query
                string query = string.Format(API_InstallQuery, FinalizeUrl(targetDevice.IP));
                query += "?package=" + WWW.EscapeURL(fileName);

                var response = WebRequestPost(query, form, GetBasicAuthHeader(targetDevice));

                if (string.IsNullOrEmpty(response))
                {
                    Debug.LogErrorFormat("Failed to install {0} on {1}.\n", fileName, targetDevice.MachineName);
                    return false;
                }

                // Wait for done (if requested)
                DateTime waitStartTime = DateTime.Now;
                while (waitForDone && (DateTime.Now - waitStartTime).TotalSeconds < MaxWaitTime)
                {
                    EditorUtility.DisplayProgressBar("Connecting to Device Portal", "Installing...", (float)((DateTime.Now - waitStartTime).TotalSeconds / MaxWaitTime));
                    AppInstallStatus status = GetInstallStatus(targetDevice);

                    if (status == AppInstallStatus.InstallSuccess)
                    {
                        Debug.LogFormat("Successfully installed {0} on {1}.", fileName, targetDevice.MachineName);
                        success = true;
                        break;
                    }

                    if (status == AppInstallStatus.InstallFail)
                    {
                        Debug.LogErrorFormat("Failed to install {0} on {1}.\n", fileName, targetDevice.MachineName);
                        break;
                    }

                    // Wait a bit and we'll ask again
                    Thread.Sleep(1000);
                }

                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                success = false;
            }

            return success;
        }

        private static AppInstallStatus GetInstallStatus(ConnectInfo targetDevice)
        {
            string response = WebRequestGet(string.Format(API_InstallStatusQuery, FinalizeUrl(targetDevice.IP)), GetBasicAuthHeader(targetDevice, true), false);

            if (!string.IsNullOrEmpty(response))
            {
                var status = JsonUtility.FromJson<InstallStatus>(response);

                if (status == null)
                {
                    return AppInstallStatus.Installing;
                }

                if (status.Success)
                {
                    return AppInstallStatus.InstallSuccess;
                }

                Debug.LogError(status.Reason + "(" + status.CodeText + ")");
            }
            else
            {
                return AppInstallStatus.Installing;
            }

            return AppInstallStatus.InstallFail;
        }

        [Obsolete("Use UninstallApp(string packageFamilyName, ConnectInfo targetDevice)")]
        public static bool UninstallApp(string packageFamilyName, string targetIp)
        {
            return UninstallApp(packageFamilyName, new ConnectInfo(targetIp, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
        }

        /// <summary>
        /// Uninstalls the target application on the target device.
        /// </summary>
        /// <param name="packageFamilyName"></param>
        /// <param name="targetDevice"></param>
        /// <param name="showDialog"></param>
        /// <returns>True, if uninstall was a success.</returns>
        public static bool UninstallApp(string packageFamilyName, ConnectInfo targetDevice, bool showDialog = true)
        {
            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetDevice);
            if (appDetails == null)
            {
                Debug.Log(string.Format("Application '{0}' not found", packageFamilyName));
                return false;
            }

            string query = string.Format("{0}?package={1}",
                string.Format(API_InstallQuery, FinalizeUrl(targetDevice.IP)),
                WWW.EscapeURL(appDetails.PackageFullName));

            bool success = WebRequestDelete(query, GetBasicAuthHeader(targetDevice), showDialog);
            MachineName targetMachine = GetMachineName(targetDevice);

            if (success)
            {
                Debug.LogFormat("Successfully uninstalled {0} on {1}.", packageFamilyName, targetMachine.ComputerName);
            }
            else
            {
                Debug.LogErrorFormat("Failed to uninstall {0} on {1}", packageFamilyName, targetMachine.ComputerName);
            }

            return success;
        }

        /// <summary>
        /// Launches the target application on the target device.
        /// </summary>
        /// <param name="packageFamilyName"></param>
        /// <param name="targetDevice"></param>
        /// <param name="showDialog"></param>
        /// <returns>True, if application was successfully launched and is currently running on the target device.</returns>
        public static bool LaunchApp(string packageFamilyName, ConnectInfo targetDevice, bool showDialog = true)
        {
            // Find the app description
            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetDevice);

            if (appDetails == null)
            {
                Debug.LogWarning("Application not found");
                return false;
            }

            string query = string.Format(API_AppQuery, FinalizeUrl(targetDevice.IP)) +
                string.Format("?appid={0}&package={1}",
                WWW.EscapeURL(EncodeTo64(appDetails.PackageRelativeId)),
                WWW.EscapeURL(appDetails.PackageFullName));
            WebRequestPost(query, null, GetBasicAuthHeader(targetDevice), false);

            return IsAppRunning(PlayerSettings.productName, targetDevice);
        }

        [Obsolete("KillApp(string packageFamilyName, ConnectInfo targetDevice)")]
        public static bool KillApp(string packageFamilyName, string targetIp)
        {
            return KillApp(packageFamilyName, new ConnectInfo(targetIp, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
        }

        /// <summary>
        /// Kills the target application on the target device.
        /// </summary>
        /// <param name="packageFamilyName"></param>
        /// <param name="targetDevice"></param>
        /// <param name="showDialog"></param>
        /// <returns>true, if application was successfully stopped.</returns>
        public static bool KillApp(string packageFamilyName, ConnectInfo targetDevice, bool showDialog = true)
        {
            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetDevice);
            if (appDetails == null)
            {
                Debug.LogError("Application not found");
                return false;
            }

            string query = string.Format("{0}?package={1}",
                string.Format(API_AppQuery, FinalizeUrl(targetDevice.IP)),
                WWW.EscapeURL(EncodeTo64(appDetails.PackageFullName)));

            bool success = WebRequestDelete(query, GetBasicAuthHeader(targetDevice), showDialog);
            MachineName targetMachine = GetMachineName(targetDevice);

            if (success)
            {
                Debug.LogFormat("Successfully stopped {0} on {1}.", packageFamilyName, targetMachine.ComputerName);
            }

            return success;
        }

        [Obsolete("DeviceLogFile_View(string packageFamilyName, ConnectInfo targetDevice)")]
        public static bool DeviceLogFile_View(string packageFamilyName, string targetIp)
        {
            return DeviceLogFile_View(packageFamilyName, new ConnectInfo(targetIp, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
        }

        /// <summary>
        /// Downloads and launches the Log file for the target application on the target device.
        /// </summary>
        /// <param name="packageFamilyName"></param>
        /// <param name="targetDevice"></param>
        /// <returns>True, if download success.</returns>
        public static bool DeviceLogFile_View(string packageFamilyName, ConnectInfo targetDevice)
        {
            EditorUtility.DisplayProgressBar("Download Log", "Downloading Log File for " + packageFamilyName, 0.25f);

            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetDevice);
            if (appDetails == null)
            {
                Debug.LogWarningFormat("{0} not installed on target device", packageFamilyName);
                EditorUtility.ClearProgressBar();
                return false;
            }

            string logFile = string.Format("{0}/{1}_{2}{3}{4}{5}{6}{7}_deviceLog.txt",
                Application.temporaryCachePath,
                targetDevice.MachineName,
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second);

            string response = WebRequestGet(string.Format(API_FileQuery, FinalizeUrl(targetDevice.IP), appDetails.PackageFullName), GetBasicAuthHeader(targetDevice, true));
            bool success = !string.IsNullOrEmpty(response);

            if (success)
            {
                File.WriteAllText(logFile, response);
                Process.Start(logFile);
            }

            EditorUtility.ClearProgressBar();

            return success;
        }

        /// <summary>
        /// Returns the <see cref="NetworkInfo"/> for the target device.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns></returns>
        public static NetworkInfo GetNetworkInfo(ConnectInfo targetDevice)
        {
            string response = WebRequestGet(string.Format(API_IpConfigQuery, FinalizeUrl(targetDevice.IP)), GetBasicAuthHeader(targetDevice, true), false);
            if (!string.IsNullOrEmpty(response))
            {
                return JsonUtility.FromJson<NetworkInfo>(response);
            }

            return null;
        }

        /// <summary>
        /// This Utility method finalizes the URL and formats the HTTPS string if needed.
        /// <remarks>Local Machine will be changed to 127.0.1:10080 for HoloLens connections.</remarks>
        /// </summary>
        /// <param name="targetUrl"></param>
        /// <returns></returns>
        private static string FinalizeUrl(string targetUrl)
        {
            string ssl = BuildDeployPrefs.UseSSL ? "s" : string.Empty;

            if (targetUrl.Contains("Local Machine"))
            {
                targetUrl = "127.0.0.1:10080";
                ssl = string.Empty;
            }
            return string.Format(@"http{0}://{1}", ssl, targetUrl);
        }

        private static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        private static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }
}
