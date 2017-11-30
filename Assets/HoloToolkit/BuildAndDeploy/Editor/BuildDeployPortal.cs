// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        private const float TimeOut = 6.0f;
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

        private static string GetBasicAuthHeader(ConnectInfo connectionInfo)
        {
            var auth = string.Format("{0}:{1}", connectionInfo.User, connectionInfo.Password);
            auth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
            return string.Format("Basic {0}", auth);
        }

        /// <summary>
        /// Send a Unity Web Request to GET.
        /// </summary>
        /// <param name="query">Full Query to GET</param>
        /// <param name="auth">Authorization header</param>
        /// <param name="showProgressDialog"></param>
        /// <returns>Response string.</returns>
        private static string WebRequestGet(string query, string auth, bool showProgressDialog = true)
        {
            try
            {
                using (var webRequest = UnityWebRequest.Get(query))
                {
                    webRequest.SetRequestHeader("Authorization", auth);
                    webRequest.timeout = (int)TimeOut;
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

                    if (webRequest.isNetworkError || webRequest.isHttpError && webRequest.responseCode != 401 && webRequest.responseCode != 403)
                    {
                        Debug.LogError("Network Error: " + webRequest.error);
                    }

                    switch (webRequest.responseCode)
                    {
                        case 200:
                        case 204:
                            return webRequest.downloadHandler.text;
                        case 401:
                            Debug.LogError("Unauthorized: Access is denied due to invalid credentials.");
                            break;
                        case 403:
                            RequestDeviceCert();
                            break;
                        default:
                            Debug.LogError(webRequest.responseCode);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Send a Unity Web Request to POST.
        /// </summary>
        /// <param name="query">Full Query to GET</param>
        /// <param name="postData"></param>
        /// <param name="auth">Authorization Header</param>
        /// <returns>Response string.</returns>
        private static string WebRequestPost(string query, List<IMultipartFormSection> postData, string auth, bool showDialog = true)
        {
            try
            {
                using (var webRequest = UnityWebRequest.Post(query, postData))
                {
                    webRequest.SetRequestHeader("Authorization", auth);
                    webRequest.timeout = (int)TimeOut;
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
                                "Uploading...", webRequest.downloadProgress);
                        }
                        else if (webRequest.downloadProgress > -1 && showDialog)
                        {
                            EditorUtility.DisplayProgressBar("Connecting to Device Portal",
                                                             "Progress...", webRequest.downloadProgress);
                        }
                    }

                    EditorUtility.ClearProgressBar();

                    if (webRequest.isNetworkError || webRequest.isHttpError && webRequest.responseCode != 401 && webRequest.responseCode != 403)
                    {
                        Debug.LogError("Network Error: " + webRequest.error);
                    }

                    switch (webRequest.responseCode)
                    {
                        case 200:
                            return webRequest.downloadHandler.text;
                        case 401:
                            Debug.LogError("Unauthorized: Access is denied due to invalid credentials.");
                            break;
                        case 403:
                            RequestDeviceCert();
                            break;
                        default:
                            Debug.LogError(webRequest.responseCode);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Send a Unity Web Request to DELETE
        /// </summary>
        /// <param name="query">Full Query.</param>
        /// <param name="auth">Authorization Header</param>
        /// <param name="showDialog"></param>
        /// <returns>Successful or not.</returns>
        private static bool WebRequestDelete(string query, string auth, bool showDialog = true)
        {
            try
            {
                using (var webRequest = UnityWebRequest.Delete(query))
                {
                    webRequest.SetRequestHeader("Authorization", auth);
                    webRequest.timeout = (int)TimeOut;
#if UNITY_2017_2_OR_NEWER
                    webRequest.SendWebRequest();
#else
                    webRequest.Send();
#endif

                    while (!webRequest.isDone)
                    {
                        if (showDialog && webRequest.downloadProgress > -1)
                        {
                            EditorUtility.DisplayProgressBar("Connecting to Device Portal", "Progress...", webRequest.downloadProgress);
                        }
                    }

                    EditorUtility.ClearProgressBar();

                    if (webRequest.isNetworkError || webRequest.isHttpError && webRequest.responseCode != 401 && webRequest.responseCode != 403)
                    {
                        Debug.LogError("Network Error: " + webRequest.error);
                    }

                    switch (webRequest.responseCode)
                    {
                        case 200:
                            return true;
                        case 401:
                            Debug.LogError("Unauthorized: Access is denied due to invalid credentials.");
                            break;
                        case 403:
                            RequestDeviceCert();
                            break;
                        default:
                            Debug.LogError(webRequest.responseCode);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return false;
        }

        /// <summary>
        /// Sends a request to download the devices cert.
        /// Only works for locally connected devices.
        /// </summary>
        /// <returns></returns>
        public static void RequestDeviceCert()
        {
            Debug.LogError("Invalid Cert Detected! You need to download and install the root certificate for your device via USB.");
        }

        public static void OpenWebPortal(ConnectInfo targetDevice)
        {
            //TODO: Figure out how to pass username and password to browser?
            Process.Start(FinalizeUrl(targetDevice.IP));
        }

        public static MachineName GetMachineName(ConnectInfo targetDevice)
        {
            MachineName machineName = null;
            string query = string.Format(API_GetMachineNameQuery, FinalizeUrl(targetDevice.IP));
            string response = WebRequestGet(query, GetBasicAuthHeader(targetDevice), false);

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
        /// Look at the device for a matching app name (if not there, then not installed)
        /// </summary>
        /// <param name="packageFamilyName"></param>
        /// <param name="targetDevice"></param>
        /// <returns></returns>
        public static bool IsAppInstalled(string packageFamilyName, ConnectInfo targetDevice)
        {
            return QueryAppDetails(packageFamilyName, targetDevice) != null;
        }

        [Obsolete("IsAppRunning(string appName, ConnectInfo targetDevice)")]
        public static bool IsAppRunning(string appName, string targetDevice)
        {
            return IsAppRunning(appName, new ConnectInfo(targetDevice, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
        }

        public static bool IsAppRunning(string appName, ConnectInfo targetDevice)
        {
            string response = WebRequestGet(string.Format(API_ProcessQuery, FinalizeUrl(targetDevice.IP)), GetBasicAuthHeader(targetDevice), false);

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

        private static AppInstallStatus GetInstallStatus(ConnectInfo targetDevice)
        {
            string response = WebRequestGet(string.Format(API_InstallStatusQuery, FinalizeUrl(targetDevice.IP)), GetBasicAuthHeader(targetDevice), false);

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

        private static AppDetails QueryAppDetails(string packageFamilyName, ConnectInfo targetDevice)
        {
            string response = WebRequestGet(string.Format(API_PackagesQuery, FinalizeUrl(targetDevice.IP)), GetBasicAuthHeader(targetDevice), false);

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

        public static bool InstallApp(string appFullPath, ConnectInfo targetDevice, bool waitForDone = true)
        {
            bool success = false;
            MachineName targetMachineName = GetMachineName(targetDevice);

            try
            {
                // Calc the cert and dep paths
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

                Dictionary<string, string> headers = form.headers;

                // Unity places an extra quote in the content-type boundary parameter that the device portal doesn't care for, remove it
                if (headers.ContainsKey("Content-Type"))
                {
                    headers["Content-Type"] = headers["Content-Type"].Replace("\"", "");
                }

                // Credentials
                headers["Authorization"] = GetBasicAuthHeader(targetDevice);

                var www = new WWW(query, form.data, headers);
                DateTime queryStartTime = DateTime.Now;

                while (!www.isDone && (DateTime.Now - queryStartTime).TotalSeconds < TimeOut)
                {
                    if (www.uploadProgress < 1)
                    {
                        EditorUtility.DisplayProgressBar("Connecting to Device Portal",
                            "Uploading...", www.uploadProgress);
                    }

                    Thread.Sleep(10);
                }

                if (www.isDone)
                {
                    EditorUtility.DisplayProgressBar("Connecting to Device Portal", "Installing...", 0);

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError(www.error);
                    }
                }

                // Wait for done (if requested)
                DateTime waitStartTime = DateTime.Now;
                while (waitForDone && (DateTime.Now - waitStartTime).TotalSeconds < MaxWaitTime)
                {
                    EditorUtility.DisplayProgressBar("Connecting to Device Portal", "Installing...", (float)((DateTime.Now - waitStartTime).TotalSeconds / MaxWaitTime));
                    AppInstallStatus status = GetInstallStatus(targetDevice);

                    if (status == AppInstallStatus.InstallSuccess)
                    {
                        Debug.LogFormat("Successfully installed {0} on {1}.", fileName, targetMachineName.ComputerName);
                        success = true;
                        break;
                    }

                    if (status == AppInstallStatus.InstallFail)
                    {
                        Debug.LogErrorFormat("Failed to install {0} on {1}.\n", fileName, targetMachineName.ComputerName);
                        break;
                    }

                    // Wait a bit and we'll ask again
                    Thread.Sleep(1000);
                }

                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                success = false;
            }

            return success;
        }

        [Obsolete("Use UninstallApp(string packageFamilyName, ConnectInfo targetDevice)")]
        public static bool UninstallApp(string packageFamilyName, string targetIp)
        {
            return UninstallApp(packageFamilyName, new ConnectInfo(targetIp, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
        }

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

        public static bool LaunchApp(string packageFamilyName, ConnectInfo targetDevice, bool showDialog = true)
        {
            // Find the app description
            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetDevice);

            if (appDetails == null)
            {
                Debug.LogError("Application not found");
                return false;
            }

            string query = string.Format(API_AppQuery, FinalizeUrl(targetDevice.IP)) +
                string.Format("?appid={0}&package={1}",
                WWW.EscapeURL(EncodeTo64(appDetails.PackageRelativeId)),
                WWW.EscapeURL(appDetails.PackageFullName));
            string response = WebRequestPost(query, null, GetBasicAuthHeader(targetDevice), false);
            bool success = !string.IsNullOrEmpty(response);

            if (success)
            {
                Debug.Log(response);
            }

            return success;
        }

        [Obsolete("KillApp(string packageFamilyName, ConnectInfo targetDevice)")]
        public static bool KillApp(string packageFamilyName, string targetIp)
        {
            return KillApp(packageFamilyName, new ConnectInfo(targetIp, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
        }

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

        public static bool DeviceLogFile_View(string packageFamilyName, ConnectInfo targetDevice)
        {
            string logFile = string.Format("{0}/{1}_{2}{3}{4}{5}{6}{7}_deviceLog.txt",
                Application.temporaryCachePath,
                targetDevice.MachineName,
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second);

            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetDevice);
            if (appDetails == null)
            {
                Debug.LogError("Application not found on target device (" + packageFamilyName + ")");
                return false;
            }

            string response = WebRequestGet(string.Format(API_FileQuery, FinalizeUrl(targetDevice.IP), appDetails.PackageFullName), GetBasicAuthHeader(targetDevice));
            bool success = !string.IsNullOrEmpty(response);

            if (success)
            {
                File.WriteAllText(logFile, response);
                Process.Start(logFile);
            }

            return success;
        }

        public static NetworkInfo GetNetworkInfo(ConnectInfo targetDevice)
        {
            string response = WebRequestGet(string.Format(API_IpConfigQuery, FinalizeUrl(targetDevice.IP)), GetBasicAuthHeader(targetDevice), false);
            if (!string.IsNullOrEmpty(response))
            {
                return JsonUtility.FromJson<NetworkInfo>(response);
            }

            return null;
        }

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
