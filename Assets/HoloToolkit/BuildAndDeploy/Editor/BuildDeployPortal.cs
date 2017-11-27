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

        private static readonly string API_ProcessQuery = @"https://{0}/api/resourcemanager/processes";
        private static readonly string API_PackagesQuery = @"https://{0}/api/appx/packagemanager/packages";
        private static readonly string API_InstallQuery = @"https://{0}/api/app/packagemanager/package";
        private static readonly string API_InstallStatusQuery = @"https://{0}/api/app/packagemanager/state";
        private static readonly string API_AppQuery = @"https://{0}/api/taskmanager/app";
        private static readonly string API_FileQuery = @"https://{0}/api/filesystem/apps/file?knownfolderid=LocalAppData&filename=UnityPlayer.log&packagefullname={1}&path=%5C%5CTempState";

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
        /// <returns>Response string.</returns>
        private static string WebRequestGet(string query, string auth)
        {
            try
            {
                using (var webRequest = UnityWebRequest.Get(query))
                {
                    webRequest.SetRequestHeader("Authorization", auth);
#if UNITY_2017_2_OR_NEWER
                    webRequest.SendWebRequest();
#else
                    webRequest.Send();
#endif

                    while (!webRequest.isDone)
                    {
                        if (webRequest.downloadProgress > -1)
                        {
                            EditorUtility.DisplayProgressBar("Connecting to Device Portal",
                                                             "Progress...", webRequest.downloadProgress);
                        }
                    }

                    EditorUtility.ClearProgressBar();

                    if (webRequest.isNetworkError || webRequest.isHttpError)
                    {
                        Debug.LogError("Network Error: " + webRequest.error);
                    }

                    switch (webRequest.responseCode)
                    {
                        case 200:
                        case 204:
                            return webRequest.downloadHandler.text;
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
        private static string WebRequestPost(string query, List<IMultipartFormSection> postData, string auth)
        {
            try
            {
                using (var webRequest = UnityWebRequest.Post(query, postData))
                {
                    webRequest.SetRequestHeader("Authorization", auth);
#if UNITY_2017_2_OR_NEWER
                    webRequest.SendWebRequest();
#else
                    webRequest.Send();
#endif

                    while (!webRequest.isDone)
                    {
                        if (webRequest.downloadProgress > -1)
                        {
                            EditorUtility.DisplayProgressBar("Connecting to Device Portal",
                                                             "Progress...", webRequest.downloadProgress);
                        }
                    }

                    EditorUtility.ClearProgressBar();

                    if (webRequest.isNetworkError || webRequest.isHttpError)
                    {
                        Debug.LogError("Network Error: " + webRequest.error);
                    }

                    switch (webRequest.responseCode)
                    {
                        case 200:
                            return webRequest.downloadHandler.text;
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
        /// <returns>Response string.</returns>
        private static string WebRequestDelete(string query, string auth)
        {
            try
            {
                using (var webRequest = UnityWebRequest.Delete(query))
                {
                    webRequest.SetRequestHeader("Authorization", auth);
#if UNITY_2017_2_OR_NEWER
                    webRequest.SendWebRequest();
#else
                    webRequest.Send();
#endif

                    while (!webRequest.isDone)
                    {
                        if (webRequest.downloadProgress > -1)
                        {
                            EditorUtility.DisplayProgressBar("Connecting to Device Portal",
                                                             "Progress...", webRequest.downloadProgress);
                        }
                    }

                    EditorUtility.ClearProgressBar();

                    if (webRequest.isNetworkError || webRequest.isHttpError)
                    {
                        Debug.LogError("Network Error: " + webRequest.error);
                    }

                    switch (webRequest.responseCode)
                    {
                        case 200:
                            return webRequest.downloadHandler.text;
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

        [Obsolete("IsAppRunning(string appName, ConnectInfo targetDevice)")]
        public static bool IsAppRunning(string appName, string targetDevice)
        {
            return IsAppRunning(appName, new ConnectInfo(targetDevice, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
        }

        public static bool IsAppRunning(string appName, ConnectInfo targetDevice)
        {
            string response = WebRequestGet(string.Format(API_ProcessQuery, targetDevice.IP), GetBasicAuthHeader(targetDevice));
            bool success = !string.IsNullOrEmpty(response);

            if (success)
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
            string response = WebRequestGet(string.Format(API_InstallStatusQuery, targetDevice.IP), GetBasicAuthHeader(targetDevice));
            bool success = !string.IsNullOrEmpty(response);

            if (success)
            {
                var status = JsonUtility.FromJson<InstallStatus>(response);

                if (status == null)
                {
                    return AppInstallStatus.Installing;
                }

                Debug.LogFormat("Install Status: {0}|{1}|{2}|{3}", status.Code, status.CodeText, status.Reason, status.Success);

                if (status.Success)
                {
                    return AppInstallStatus.InstallSuccess;
                }

                Debug.LogError(status.Reason + "(" + status.CodeText + ")");
            }

            return AppInstallStatus.InstallFail;
        }

        private static AppDetails QueryAppDetails(string packageFamilyName, ConnectInfo targetDevice)
        {
            string response = WebRequestGet(string.Format(API_PackagesQuery, targetDevice.IP), GetBasicAuthHeader(targetDevice));
            bool success = !string.IsNullOrEmpty(response);

            if (success)
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

        public static bool InstallApp(string appFullPath, ConnectInfo connectInfo, bool waitForDone = true)
        {
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
                var stream = new FileStream(appFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var reader = new BinaryReader(stream);
                form.AddBinaryData(fileName, reader.ReadBytes((int)reader.BaseStream.Length), fileName);
                stream.Close();

                // CERT file
                Debug.Assert(certFullPath != null);
                stream = new FileStream(certFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                reader = new BinaryReader(stream);
                form.AddBinaryData(certName, reader.ReadBytes((int)reader.BaseStream.Length), certName);
                stream.Close();

                // Dependencies
                FileInfo[] depFiles = new DirectoryInfo(depPath).GetFiles();
                foreach (FileInfo dep in depFiles)
                {
                    stream = new FileStream(dep.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    reader = new BinaryReader(stream);
                    string depFilename = Path.GetFileName(dep.FullName);
                    form.AddBinaryData(depFilename, reader.ReadBytes((int)reader.BaseStream.Length), depFilename);
                    stream.Close();
                }

                // Credentials
                Dictionary<string, string> headers = form.headers;
                headers["Authorization"] = "Basic " + EncodeTo64(connectInfo.User + ":" + connectInfo.Password);

                // Unity places an extra quote in the content-type boundary parameter that the device portal doesn't care for, remove it
                if (headers.ContainsKey("Content-Type"))
                {
                    headers["Content-Type"] = headers["Content-Type"].Replace("\"", "");
                }

                // Query
                string query = string.Format(API_InstallQuery, connectInfo.IP);
                query += "?package=" + WWW.EscapeURL(fileName);

                var www = new WWW(query, form.data, headers);
                DateTime queryStartTime = DateTime.Now;

                while (!www.isDone && (DateTime.Now - queryStartTime).TotalSeconds < TimeOut)
                {
                    Thread.Sleep(10);
                }

                // Give it a short time before checking
                Thread.Sleep(250);

                // Report
                if (www.isDone)
                {
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError(www.error);
                    }
                    else if (!string.IsNullOrEmpty(www.text))
                    {
                        Debug.Log(JsonUtility.FromJson<Response>(www.text).Reason);
                    }
                    else
                    {
                        Debug.LogWarning("Completed with null response string");
                    }
                }

                // Wait for done (if requested)
                DateTime waitStartTime = DateTime.Now;
                while (waitForDone && (DateTime.Now - waitStartTime).TotalSeconds < MaxWaitTime)
                {
                    AppInstallStatus status = GetInstallStatus(connectInfo);
                    if (status == AppInstallStatus.InstallSuccess)
                    {
                        Debug.Log("Install Successful!");
                        break;
                    }
                    if (status == AppInstallStatus.InstallFail)
                    {
                        Debug.LogError("Install Failed!");
                        break;
                    }

                    // Wait a bit and we'll ask again
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }

            return true;
        }

        [Obsolete("Use UninstallApp(string packageFamilyName, ConnectInfo targetDevice)")]
        public static bool UninstallApp(string packageFamilyName, string targetIp)
        {
            return UninstallApp(packageFamilyName, new ConnectInfo(targetIp, BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword));
        }

        public static bool UninstallApp(string packageFamilyName, ConnectInfo targetDevice)
        {
            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetDevice);
            if (appDetails == null)
            {
                Debug.LogError(string.Format("Application '{0}' not found", packageFamilyName));
                return false;
            }

            string query = string.Format("{0}?package={1}",
                string.Format(API_InstallQuery, targetDevice),
                WWW.EscapeURL(appDetails.PackageFullName));

            string response = WebRequestDelete(query, GetBasicAuthHeader(targetDevice));

            bool success = !string.IsNullOrEmpty(response);

            if (success)
            {
                Debug.Log(response);
            }

            return success;
        }

        public static bool LaunchApp(string packageFamilyName, ConnectInfo targetDevice)
        {
            // Find the app description
            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetDevice);
            if (appDetails == null)
            {
                Debug.LogError("Application not found");
                return false;
            }

            // Query
            string query = string.Format(API_AppQuery, targetDevice.IP);
            var formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection(
                    string.Format("appid={0}&package={1}",
                        WWW.EscapeURL(EncodeTo64(appDetails.PackageRelativeId)),
                        WWW.EscapeURL(appDetails.PackageFullName)))
            };

            string response = WebRequestPost(query, formData, GetBasicAuthHeader(targetDevice));
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

        public static bool KillApp(string packageFamilyName, ConnectInfo targetDevice)
        {
            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetDevice);
            if (appDetails == null)
            {
                Debug.LogError("Application not found");
                return false;
            }

            string query = string.Format("{0}?package={1}",
                string.Format(API_AppQuery, targetDevice),
                WWW.EscapeURL(EncodeTo64(appDetails.PackageFullName)));

            string response = WebRequestDelete(query, GetBasicAuthHeader(targetDevice));
            bool success = !string.IsNullOrEmpty(response);

            if (success)
            {
                Debug.Log(response);
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
            string logFile = Application.temporaryCachePath + @"/deviceLog.txt";

            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetDevice);
            if (appDetails == null)
            {
                Debug.LogError("Application not found on target device (" + packageFamilyName + ")");
                return false;
            }

            string response = WebRequestGet(string.Format(API_FileQuery, targetDevice.IP, appDetails.PackageFullName), GetBasicAuthHeader(targetDevice));
            bool success = !string.IsNullOrEmpty(response);

            if (success)
            {
                File.WriteAllText(logFile, response);
                Process.Start(logFile);
            }

            return success;
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
