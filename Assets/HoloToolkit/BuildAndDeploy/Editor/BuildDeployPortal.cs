// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
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
        private const float TimeOut = 6.0f;
        private const int TimeoutMS = (int)(TimeOut * 1000.0f);
        private const float MaxWaitTime = 20.0f;
        private static readonly string API_ProcessQuery = @"http://{0}/api/resourcemanager/processes";
        private static readonly string API_PackagesQuery = @"http://{0}/api/appx/packagemanager/packages";
        private static readonly string API_InstallQuery = @"http://{0}/api/app/packagemanager/package";
        private static readonly string API_InstallStatusQuery = @"http://{0}/api/app/packagemanager/state";
        private static readonly string API_AppQuery = @"http://{0}/api/taskmanager/app";
        private static readonly string API_FileQuery = @"http://{0}/api/filesystem/apps/file?knownfolderid=LocalAppData&filename=UnityPlayer.log&packagefullname={1}&path=%5C%5CTempState";

        /// <summary>
        /// Look at the device for a matching app name (if not there, then not installed)
        /// </summary>
        /// <param name="packageFamilyName"></param>
        /// <param name="connectInfo"></param>
        /// <returns></returns>
        public static bool IsAppInstalled(string packageFamilyName, string targetIp)
        {
            return QueryAppDetails(packageFamilyName, targetIp) != null;
        }

        private static string GetBasicAuthHeader()
        {
            var auth = string.Format("{0}:{1}", BuildDeployPrefs.DeviceUser, BuildDeployPrefs.DevicePassword);
            auth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
            return string.Format("Basic {0}", auth);
        }

        /// <summary>
        /// Send a Unity Web Request to GET.
        /// </summary>
        /// <param name="query">Full Query to GET</param>
        /// <returns>Response string.</returns>
        private static string WebRequestGet(string query)
        {
            try
            {
                using (var webRequest = UnityWebRequest.Get(query))
                {
                    webRequest.SetRequestHeader("Authorization", GetBasicAuthHeader());
                    webRequest.Send();

                    while (!webRequest.isDone)
                    {
                        if (webRequest.downloadProgress != -1)
                        {
                            EditorUtility.DisplayProgressBar("Getting App Status",
                                "Progress...", webRequest.downloadProgress);
                        }
                    }

                    EditorUtility.ClearProgressBar();

                    if (webRequest.isNetworkError || webRequest.isHttpError)
                    {
                        throw new UnityException("Network Error: " + webRequest.error);
                    }

                    // TODO: Handle response codes: webRequest.responseCode
                    return webRequest.downloadHandler.text;
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
        /// <returns>Response string.</returns>
        private static string WebRequestDelete(string query)
        {
            try
            {
                using (var webRequest = UnityWebRequest.Delete(query))
                {
                    webRequest.SetRequestHeader("Authorization", GetBasicAuthHeader());
                    webRequest.Send();

                    while (!webRequest.isDone)
                    {
                        if (webRequest.downloadProgress != -1)
                        {
                            EditorUtility.DisplayProgressBar("Getting App Status",
                                "Progress...", webRequest.downloadProgress);
                        }
                    }

                    EditorUtility.ClearProgressBar();

                    if (webRequest.isNetworkError || webRequest.isHttpError)
                    {
                        throw new UnityException("Network Error: " + webRequest.error);
                    }

                    // TODO: Handle response codes: webRequest.responseCode
                    return webRequest.downloadHandler.text;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return string.Empty;
        }

        public static bool IsAppRunning(string appName, string targetIp)
        {
            string response = WebRequestGet(string.Format(API_ProcessQuery, targetIp));

            var processList = JsonUtility.FromJson<ProcessList>(response);
            for (int i = 0; i < processList.Processes.Length; ++i)
            {
                string processName = processList.Processes[i].ImageName;

                if (processName.Contains(appName))
                {
                    return true;
                }
            }

            return false;
        }

        private static AppInstallStatus GetInstallStatus(string targetIp)
        {
            string response = WebRequestGet(string.Format(API_InstallStatusQuery, targetIp));
            var status = JsonUtility.FromJson<InstallStatus>(response);

            if (status == null)
            {
                return AppInstallStatus.Installing;
            }

            Debug.LogFormat("Install Status: {0}|{1}|{2}|{3}", status.Code, status.CodeText, status.Reason, status.Success);

            if (status.Success == false)
            {
                Debug.LogError(status.Reason + "(" + status.CodeText + ")");
                return AppInstallStatus.InstallFail;
            }

            return AppInstallStatus.InstallSuccess;
        }

        private static AppDetails QueryAppDetails(string packageFamilyName, string targetIp)
        {
            string response = WebRequestGet(string.Format(API_PackagesQuery, targetIp));

            var appList = JsonUtility.FromJson<AppList>(response);
            for (int i = 0; i < appList.InstalledPackages.Length; ++i)
            {
                string thisAppName = appList.InstalledPackages[i].PackageFamilyName;
                if (thisAppName.Equals(packageFamilyName, StringComparison.OrdinalIgnoreCase))
                {
                    return appList.InstalledPackages[i];
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
                var stream = new FileStream(appFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var reader = new BinaryReader(stream);
                form.AddBinaryData(fileName, reader.ReadBytes((int)reader.BaseStream.Length), fileName);
                stream.Close();

                // CERT file
                stream = new FileStream(certFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                reader = new BinaryReader(stream);
                form.AddBinaryData(certName, reader.ReadBytes((int)reader.BaseStream.Length), certName);
                stream.Close();

                // Dependencies
                FileInfo[] depFiles = (new DirectoryInfo(depPath)).GetFiles();
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
                    AppInstallStatus status = GetInstallStatus(connectInfo.IP);
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
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool UninstallApp(string packageFamilyName, string targetIp)
        {
            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetIp);
            if (appDetails == null)
            {
                Debug.LogError(string.Format("Application '{0}' not found", packageFamilyName));
                return false;
            }

            string query = string.Format(API_InstallQuery, targetIp);
            query += "?package=" + WWW.EscapeURL(appDetails.PackageFullName);
            Debug.Log("Response = " + WebRequestDelete(query));

            return true;
        }

        public static bool LaunchApp(string packageFamilyName, ConnectInfo connectInfo)
        {
            // Find the app description
            AppDetails appDetails = QueryAppDetails(packageFamilyName, connectInfo.IP);
            if (appDetails == null)
            {
                Debug.LogError("Application not found");
                return false;
            }

            // Setup the command
            string query = string.Format(API_AppQuery, connectInfo.IP);
            query += "?appid=" + WWW.EscapeURL(EncodeTo64(appDetails.PackageRelativeId));
            query += "&package=" + WWW.EscapeURL(appDetails.PackageFullName);

            // Use HttpWebRequest
            var request = (HttpWebRequest)WebRequest.Create(query);
            request.Timeout = TimeoutMS;
            request.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
            request.Method = "POST";

            // Query
            using (var httpResponse = (HttpWebResponse)request.GetResponse())
            {
                Debug.Log("Response = " + httpResponse.StatusDescription);
                httpResponse.Close();
            }

            return true;
        }

        public static bool KillApp(string packageFamilyName, string targetIp)
        {
            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetIp);
            if (appDetails == null)
            {
                Debug.LogError("Application not found");
                return false;
            }

            string query = string.Format(API_AppQuery, targetIp);
            query = string.Format("{0}?package={1}", query, WWW.EscapeURL(EncodeTo64(appDetails.PackageFullName)));

            string response = WebRequestDelete(query);
            Debug.Log("Response = " + response);

            return true;
        }

        public static bool DeviceLogFile_View(string packageFamilyName, string targetIp)
        {
            string logFile = Application.temporaryCachePath + @"/deviceLog.txt";

            AppDetails appDetails = QueryAppDetails(packageFamilyName, targetIp);
            if (appDetails == null)
            {
                Debug.LogError("Application not found on target device (" + packageFamilyName + ")");
                return false;
            }

            string query = string.Format(API_FileQuery, targetIp, appDetails.PackageFullName);
            string response = WebRequestGet(query);

            File.WriteAllText(logFile, response);

            Process.Start(logFile);

            return true;
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
