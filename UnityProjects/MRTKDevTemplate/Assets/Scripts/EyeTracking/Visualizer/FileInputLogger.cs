// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Stores collected eye gaze data to a file.
    /// </summary>
    public class FileInputLogger : IDisposable
    {
        private bool disposed = false;
        private StreamWriter logFile = null;

        public FileInputLogger(string userNameFolder, string fileName)
        {
            UserNameFolder = userNameFolder;

#if WINDOWS_UWP
            await CreateNewLogFile();
#else
            logFile = File.CreateText(Application.persistentDataPath + "/" + fileName);
            logFile.AutoFlush = true;
#endif
        }

        /// <summary>
        /// Closes the log file and releases any managed resources.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            logFile.Close();
            logFile.Dispose();
        }

        /// <summary>
        /// A user folder to store logs file in UWP
        /// </summary>
        private string UserNameFolder { get; set; }

#if WINDOWS_UWP
        public string LogDirectory => "MRTK_ET_Demo/" + UserNameFolder;

        private StorageFile logFile;
        private StorageFolder logRootFolder = KnownFolders.MusicLibrary;
        private StorageFolder sessionFolder;

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
                        logFile = await sessionFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

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
        
#region Append Log
#if WINDOWS_UWP
        public async void AppendLog(string message)
        {
            // Log buffer to the file
            await FileIO.AppendTextAsync(logFile, message);
        }
#else
        /// <summary>
        /// Appends a message to the log file.
        /// </summary>
        public void AppendLog(string message)
        {
            logFile.Write(message);
        }
#endif
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
                        logFile = await sessionFolder.GetFileAsync(filename);
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
    }
}
