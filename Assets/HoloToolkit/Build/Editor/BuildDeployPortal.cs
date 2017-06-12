//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using UnityEngine;
using System.Net;
using System;
using System.IO;
using System.Collections.Generic;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Function used to communicate with the device through the REST API
    /// </summary>
    public class BuildDeployPortal
    {
        // Consts
        public const float TimeOut = 6.0f;
        public const int TimeoutMS = (int)(TimeOut * 1000.0f);
        public const float MaxWaitTime = 20.0f;

        public static readonly string kAPI_ProcessQuery = @"http://{0}/api/resourcemanager/processes";
        public static readonly string kAPI_PackagesQuery = @"http://{0}/api/appx/packagemanager/packages";
        public static readonly string kAPI_InstallQuery = @"http://{0}/api/app/packagemanager/package";
        public static readonly string kAPI_InstallStatusQuery = @"http://{0}/api/app/packagemanager/state";
        public static readonly string kAPI_AppQuery = @"http://{0}/api/taskmanager/app";
        public static readonly string kAPI_FileQuery = @"http://{0}/api/filesystem/apps/file";

        // Enums
        public enum AppInstallStatus
        {
            Invalid,
            Installing,
            InstallSuccess,
            InstallFail
        }

        // Classes & Structs
        public struct ConnectInfo
        {
            public ConnectInfo(string ip, string user, string password)
            {
                IP = ip;
                User = user;
                Password = password;
            }

            public string IP;
            public string User;
            public string Password;
        }
        [Serializable]
        public class AppDetails
        {
            public string Name;
            public string PackageFamilyName;
            public string PackageFullName;
            public int PackageOrigin;
            public string PackageRelativeId;
            public string Publisher;
        }
        [Serializable]
        public class AppList
        {
            public AppDetails[] InstalledPackages;
        }
        [Serializable]
        public class ProcessDesc
        {
            public float CPUUsage;
            public string ImageName;
            public float PageFileUsage;
            public int PrivateWorkingSet;
            public int ProcessId;
            public int SessionId;
            public string UserName;
            public int VirtualSize;
            public int WorkingSetSize;
        }
        [Serializable]
        public class ProcessList
        {
            public ProcessDesc[] Processes;
        }
        [Serializable]
        public class InstallStatus
        {
            public int Code;
            public string CodeText;
            public string Reason;
            public bool Success;
        }
        [Serializable]
        public class Response
        {
            public string Reason;
        }
        private class TimeoutWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest lWebRequest = base.GetWebRequest(uri);
                lWebRequest.Timeout = BuildDeployPortal.TimeoutMS;
                ((HttpWebRequest)lWebRequest).ReadWriteTimeout = BuildDeployPortal.TimeoutMS;
                return lWebRequest;
            }
        }

        // Functions
        public static bool IsAppInstalled(string packageFamilyName, ConnectInfo connectInfo)
        {
            // Look at the device for a matching app name (if not there, then not installed)
            return (QueryAppDetails(packageFamilyName, connectInfo) != null);
        }

        public static bool IsAppRunning(string appName, ConnectInfo connectInfo)
        {
            using (var client = new TimeoutWebClient())
            {
                client.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                string query = string.Format(kAPI_ProcessQuery, connectInfo.IP);
                string procListJSON = client.DownloadString(query);

                ProcessList procList = JsonUtility.FromJson<ProcessList>(procListJSON);
                for (int i = 0; i < procList.Processes.Length; ++i)
                {
                    string procName = procList.Processes[i].ImageName;
                    if (procName.Contains(appName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static AppInstallStatus GetInstallStatus(ConnectInfo connectInfo)
        {
            using (var client = new TimeoutWebClient())
            {
                client.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                string query = string.Format(kAPI_InstallStatusQuery, connectInfo.IP);
                string statusJSON = client.DownloadString(query);
                InstallStatus status = JsonUtility.FromJson<InstallStatus>(statusJSON);

                if (status == null)
                {
                    return AppInstallStatus.Installing;
                }
                else if (status.Success == false)
                {
                    Debug.LogError(status.Reason + "(" + status.CodeText + ")");
                    return AppInstallStatus.InstallFail;
                }
                return AppInstallStatus.InstallSuccess;
            }
        }

        public static AppDetails QueryAppDetails(string packageFamilyName, ConnectInfo connectInfo)
        {
            using (var client = new TimeoutWebClient())
            {
                client.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                string query = string.Format(kAPI_PackagesQuery, connectInfo.IP);
                string appListJSON = client.DownloadString(query);

                AppList appList = JsonUtility.FromJson<AppList>(appListJSON);
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
                WWWForm form = new WWWForm();

                // APPX file
                FileStream stream = new FileStream(appFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader reader = new BinaryReader(stream);
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
                string query = string.Format(kAPI_InstallQuery, connectInfo.IP);
                query += "?package=" + WWW.EscapeURL(fileName);
                WWW www = new WWW(query, form.data, headers);
                DateTime queryStartTime = DateTime.Now;
                while (!www.isDone &&
                       ((DateTime.Now - queryStartTime).TotalSeconds < TimeOut))
                {
                    System.Threading.Thread.Sleep(10);
                }

                // Give it a short time before checking
                System.Threading.Thread.Sleep(250);

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
                while (waitForDone &&
                    ((DateTime.Now - waitStartTime).TotalSeconds < MaxWaitTime))
                {
                    AppInstallStatus status = GetInstallStatus(connectInfo);
                    if (status == AppInstallStatus.InstallSuccess)
                    {
                        Debug.Log("Install Successful!");
                        break;
                    }
                    else if (status == AppInstallStatus.InstallFail)
                    {
                        Debug.LogError("Install Failed!");
                        break;
                    }

                    // Wait a bit and we'll ask again
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool UninstallApp(string packageFamilyName, ConnectInfo connectInfo)
        {
            try
            {
                // Find the app description
                AppDetails appDetails = QueryAppDetails(packageFamilyName, connectInfo);
                if (appDetails == null)
                {
                    Debug.LogError(string.Format("Application '{0}' not found", packageFamilyName));
                    return false;
                }

                // Setup the command
                string query = string.Format(kAPI_InstallQuery, connectInfo.IP);
                query += "?package=" + WWW.EscapeURL(appDetails.PackageFullName);

                // Use HttpWebRequest for a delete query
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
                request.Timeout = TimeoutMS;
                request.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                request.Method = "DELETE";
                using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
                {
                    Debug.Log("Response = " + httpResponse.StatusDescription);
                    httpResponse.Close();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool LaunchApp(string packageFamilyName, ConnectInfo connectInfo)
        {
            // Find the app description
            AppDetails appDetails = QueryAppDetails(packageFamilyName, connectInfo);
            if (appDetails == null)
            {
                Debug.LogError("Appliation not found");
                return false;
            }

            // Setup the command
            string query = string.Format(kAPI_AppQuery, connectInfo.IP);
            query += "?appid=" + WWW.EscapeURL(EncodeTo64(appDetails.PackageRelativeId));
            query += "&package=" + WWW.EscapeURL(EncodeTo64(appDetails.PackageFamilyName));

            // Use HttpWebRequest
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
            request.Timeout = TimeoutMS;
            request.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
            request.Method = "POST";

            // Query
            using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
            {
                Debug.Log("Response = " + httpResponse.StatusDescription);
                httpResponse.Close();
            }

            return true;
        }

        public static bool KillApp(string packageFamilyName, ConnectInfo connectInfo)
        {
            try
            {
                // Find the app description
                AppDetails appDetails = QueryAppDetails(packageFamilyName, connectInfo);
                if (appDetails == null)
                {
                    Debug.LogError("Appliation not found");
                    return false;
                }

                // Setup the command
                string query = string.Format(kAPI_AppQuery, connectInfo.IP);
                query += "?package=" + WWW.EscapeURL(EncodeTo64(appDetails.PackageFullName));

                // And send it across
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
                request.Timeout = TimeoutMS;
                request.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                request.Method = "DELETE";
                using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
                {
                    Debug.Log("Response = " + httpResponse.StatusDescription);
                    httpResponse.Close();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool DeviceLogFile_View(string packageFamilyName, ConnectInfo connectInfo)
        {
            using (var client = new TimeoutWebClient())
            {
                client.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                try
                {
                    // Setup
                    string logFile = Application.temporaryCachePath + @"/deviceLog.txt";

                    // Get the app details...
                    AppDetails appDetails = QueryAppDetails(packageFamilyName, connectInfo);
                    if (appDetails == null)
                    {
                        Debug.LogError("Application not found on target device (" + packageFamilyName + ")");
                        return false;
                    }

                    // Download the file
                    string query = string.Format(kAPI_FileQuery, connectInfo.IP);
                    query += "?knownfolderid=LocalAppData";
                    query += "&filename=UnityPlayer.log";
                    query += "&packagefullname=" + appDetails.PackageFullName;
                    query += "&path=%5C%5CTempState";
                    client.DownloadFile(query, logFile);

                    // Open it up in default text editor
                    System.Diagnostics.Process.Start(logFile);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        // Helpers
        static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }
}
