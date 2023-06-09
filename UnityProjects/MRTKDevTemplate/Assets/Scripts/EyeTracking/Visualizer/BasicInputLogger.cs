// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    public abstract class BasicInputLogger : MonoBehaviour
    {
        public bool AddTimestampToLogfileName = false;

        public void SetUserName(string name)
        {
            UserName = name;
            Debug.Log("New user name: " + name);
        }

        public void SetSessionDescr(string descr)
        {
            sessionDescr = descr;
            Debug.Log("New session name: " + descr);
        }

        public string UserName = "tester";

        [SerializeField]
        protected string sessionDescr = "Session00";

        public string LogDirectory => "MRTK_ET_Demo" + "/" + UserName;

        public abstract string GetHeader();

#if WINDOWS_UWP
        private StorageFile logFile;
        private StorageFolder logRootFolder = KnownFolders.MusicLibrary;
        private StorageFolder sessionFolder;
#endif

        internal bool IsLogging = false;
        protected abstract string GetFileName();
        private StringBuilder _buffer = null;

#if WINDOWS_UWP
        protected virtual async void CreateNewLogFile()
        {
            try
            {
                Debug.Log(">> BasicInputLogger.CreateNewLogFile:  " + logRootFolder.ToString());
                if (logRootFolder != null)
                {
                    string fullPath = Path.Combine(logRootFolder.Path, LogDirectory);
                    Debug.LogFormat("Does directory already exist {0} --\nLogRootFolder: {2} \n {1}", Directory.Exists(fullPath), fullPath, logRootFolder.Path);

                    try
                    {
                        if (!Directory.Exists(fullPath))
                        {
                            Debug.LogFormat("Trying to create new directory..");
                            Debug.LogFormat("Full path: " + fullPath);
                            sessionFolder = await logRootFolder.CreateFolderAsync(LogDirectory, CreationCollisionOption.GenerateUniqueName);
                        }

                        sessionFolder = await logRootFolder.GetFolderAsync(LogDirectory);
                        logFile = await sessionFolder.CreateFileAsync(Filename, CreationCollisionOption.ReplaceExisting);

                        Debug.Log(string.Format("*** Create log file to: {0} -- \n -- {1}", sessionFolder.Name, sessionFolder.Path));
                        Debug.Log(string.Format("*** The log file path is: {0} -- \n -- {1}", logFile.Name, logFile.Path));
                    }
                    catch (FileNotFoundException)
                    {
                        sessionFolder = await logRootFolder.CreateFolderAsync(LogDirectory, CreationCollisionOption.GenerateUniqueName);
                    }
                    catch (DirectoryNotFoundException) { }
                    catch { }
                }
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("Exception in BasicLogger: {0}", e.Message));
            }
        }
#endif

        void CheckIfInitialized()
        {
            if (_buffer == null)
                ResetLog();
        }

        public void ResetLog()
        {
            _buffer = new StringBuilder();
#if WINDOWS_UWP
            CreateNewLogFile();
#endif
        }

        private static string FormattedTimeStamp
        {
            get
            {
                return DateTime.Now.ToString("yMMddHHmmss");
            }
        }

        #region Append log
        public bool Append(string msg)
        {
            CheckIfInitialized();

            if (IsLogging)
            {
                // post IO to a separate thread.
                _buffer.AppendLine(msg);
                return true;
            }
            return false;
        }
        #endregion

#if WINDOWS_UWP
        public async void LoadLogs()
        {
            try
            {
                if (logRootFolder != null)
                {
                    string fullPath = Path.Combine(logRootFolder.Path, LogDirectory);

                    try
                    {
                        if (!Directory.Exists(fullPath))
                        {
                            return;
                        }

                        sessionFolder = await logRootFolder.GetFolderAsync(LogDirectory);
                        logFile = await sessionFolder.GetFileAsync(Filename);


                    }
                    catch (FileNotFoundException)
                    {
                        sessionFolder = await logRootFolder.CreateFolderAsync(LogDirectory, CreationCollisionOption.GenerateUniqueName);
                    }
                    catch (DirectoryNotFoundException) { }
                    catch (Exception) { }
                }
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("Exception in BasicLogger to load log file: {0}", e.Message));
            }
        }
#endif

#if WINDOWS_UWP
        public async void SaveLogs()
        {
            if (isLogging)
            {
                if (buffer.Length > 0)
                {
                    // Log buffer to the file
                    await FileIO.AppendTextAsync(logFile, buffer.ToString());
                }
            }
        }
#else
        public void SaveLogs()
        {
            if (IsLogging)
            {
                string path = Application.persistentDataPath + "/" + Filename;

                Debug.Log("SAVE LOGS to " + path);
                File.WriteAllText(path, _buffer.ToString());
            }
        }
#endif

        private string Filename
        {
            get { return AddTimestampToLogfileName ? FilenameWithTimestamp : FilenameNoTimestamp; }
        }

        protected string FilenameWithTimestamp
        {
            get { return $"{FormattedTimeStamp}_{FilenameNoTimestamp}"; }
        }

        protected string FilenameNoTimestamp
        {
            get { return GetFileName() + ".csv"; }
        }

        public virtual void OnDestroy()
        {
            if (IsLogging)
                SaveLogs();
        }
    }
}
