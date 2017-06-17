// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Helper class for launching external processes inside of the unity editor.
    /// </summary>
    public class ExternalProcess : IDisposable
    {
        [DllImport("ExternalProcessAPI", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ExternalProcessAPI_CreateProcess([MarshalAs(UnmanagedType.LPStr)] string cmdline);
        [DllImport("ExternalProcessAPI", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ExternalProcessAPI_IsRunning(IntPtr handle);
        [DllImport("ExternalProcessAPI", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ExternalProcessAPI_SendLine(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string line);
        [DllImport("ExternalProcessAPI", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ExternalProcessAPI_GetLine(IntPtr handle);
        [DllImport("ExternalProcessAPI", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ExternalProcessAPI_DestroyProcess(IntPtr handle);
        [DllImport("ExternalProcessAPI", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ExternalProcessAPI_ConfirmOrBeginProcess([MarshalAs(UnmanagedType.LPStr)] string processName);

        private IntPtr mHandle;

        /// <summary>
        /// First some static utility functions, used by some other code as well.
        /// They are related to "external processes" so they appear here.
        /// </summary>
        private static string sAppDataPath;

        public static void Launch(string appName)
        {
            // Full or relative paths only. Currently unused.

            if (!appName.StartsWith(@"\"))
            {
                appName += @"\";
            }

            string appPath = AppDataPath + appName;
            string appDir = Path.GetDirectoryName(appPath);

            Process pr = new Process();
            pr.StartInfo.FileName = appPath;
            pr.StartInfo.WorkingDirectory = appDir;
            pr.Start();
        }

        private static string AppDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(sAppDataPath))
                {
                    sAppDataPath = Application.dataPath.Replace("/", @"\");
                }

                return sAppDataPath;
            }
        }

        public static bool FindAndLaunch(string appName)
        {
            return FindAndLaunch(appName, null);
        }

        public static bool FindAndLaunch(string appName, string args)
        {
            // Start at working directory, append appName (should read "appRelativePath"), see if it exists.
            // If not go up to parent and try again till drive level reached.

            string appPath = FindPathToExecutable(appName);
            if (appPath == null)
            {
                return false;
            }

            string appDir = Path.GetDirectoryName(appPath);

            Process pr = new Process();
            pr.StartInfo.FileName = appPath;
            pr.StartInfo.WorkingDirectory = appDir;
            pr.StartInfo.Arguments = args;

            return pr.Start();
        }

        public static string FindPathToExecutable(string appName)
        {
            // Start at working directory, append appName (should read "appRelativePath"), see if it exists.
            // If not go up to parent and try again till drive level reached.

            if (!appName.StartsWith(@"\"))
            {
                appName = @"\" + appName;
            }

            string searchDir = AppDataPath;

            while (searchDir.Length > 3)
            {
                string appPath = searchDir + appName;

                if (File.Exists(appPath))
                {
                    return appPath;
                }

                searchDir = Path.GetDirectoryName(searchDir);
            }

            return null;
        }

        public static string MakeRelativePath(string path1, string path2)
        {
            // TBD- doesn't really belong in ExternalProcess.

            path1 = path1.Replace('\\', '/');
            path2 = path2.Replace('\\', '/');
            path1 = path1.Replace("\"", "");
            path2 = path2.Replace("\"", "");

            Uri uri1 = new Uri(path1);
            Uri uri2 = new Uri(path2);
            Uri relativePath = uri1.MakeRelativeUri(uri2);
            return relativePath.OriginalString;
        }

        /// <summary>
        /// The actual ExternalProcess class.
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static ExternalProcess CreateExternalProcess(string appName)
        {
            return CreateExternalProcess(appName, null);
        }

        public static ExternalProcess CreateExternalProcess(string appName, string args)
        {
            // Seems like it would be safer and more informative to call this static method and test for null after.
            try
            {
                return new ExternalProcess(appName, args);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Unable to start process " + appName + ", " + ex.Message + ".");
            }
            return null;
        }

        private ExternalProcess(string appName, string args)
        {
            appName = appName.Replace("/", @"\");
            string appPath = appName;
            if (!File.Exists(appPath))
            {
                appPath = FindPathToExecutable(appName);
            }

            if (appPath == null)
            {
                throw new ArgumentException("Unable to find app " + appPath);
            }

            // This may throw, calling code should catch the exception.
            string launchString = args == null ? appPath : appPath + " " + args;
            mHandle = ExternalProcessAPI_CreateProcess(launchString);
        }

        ~ExternalProcess()
        {
            Dispose(false);
        }

        public bool IsRunning()
        {
            try
            {
                if (mHandle != IntPtr.Zero)
                {
                    return ExternalProcessAPI_IsRunning(mHandle);
                }
            }
            catch
            {
                Terminate();
            }

            return false;
        }

        public bool WaitForStart(float seconds)
        {
            return WaitFor(seconds, () => { return ExternalProcessAPI_IsRunning(mHandle); });
        }

        public bool WaitForShutdown(float seconds)
        {
            return WaitFor(seconds, () => { return !ExternalProcessAPI_IsRunning(mHandle); });
        }

        public bool WaitFor(float seconds, Func<bool> func)
        {
            if (seconds <= 0.0f)
                seconds = 5.0f;
            float end = Time.realtimeSinceStartup + seconds;

            bool hasHappened = false;
            while (Time.realtimeSinceStartup < end)
            {
                hasHappened = func();
                if (hasHappened)
                {
                    break;
                }
                Thread.Sleep(Math.Min(500, (int)(seconds * 1000)));
            }
            return hasHappened;
        }

        public void SendLine(string line)
        {
            try
            {
                if (mHandle != IntPtr.Zero)
                {
                    ExternalProcessAPI_SendLine(mHandle, line);
                }
            }
            catch
            {
                Terminate();
            }
        }

        public string GetLine()
        {
            try
            {
                if (mHandle != IntPtr.Zero)
                {
                    return Marshal.PtrToStringAnsi(ExternalProcessAPI_GetLine(mHandle));
                }
            }
            catch
            {
                Terminate();
            }

            return null;
        }

        public void Terminate()
        {
            try
            {
                if (mHandle != IntPtr.Zero)
                {
                    ExternalProcessAPI_DestroyProcess(mHandle);
                }
            }
            catch
            {
                // TODO: Should we be catching something here?
            }

            mHandle = IntPtr.Zero;
        }

        // IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Terminate();
        }
    }
}
#endif