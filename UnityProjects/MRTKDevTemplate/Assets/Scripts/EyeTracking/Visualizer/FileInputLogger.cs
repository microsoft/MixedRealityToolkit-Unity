// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for sample. While nice to have, this documentation is not required for samples.
#pragma warning disable CS1591

using System;
using System.IO;
using UnityEngine;

#if WINDOWS_UWP
using System.Text;
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
        private StreamWriter logFileStream = null;

        public FileInputLogger(string fileName)
        {
            Filename = fileName;
            
#if WINDOWS_UWP
            CreateNewLogFile();
            buffer = new StringBuilder();
#else
            Debug.Log(Application.persistentDataPath + "/" + fileName);
            logFileStream = File.CreateText(Application.persistentDataPath + "/" + fileName);
            logFileStream.AutoFlush = true;
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
            
#if !WINDOWS_UWP
            logFileStream.Close();
            logFileStream.Dispose();
#endif
        }

        private string Filename { get; }

#if WINDOWS_UWP
        private StorageFile logFile;

        private StringBuilder buffer;

        protected virtual async void CreateNewLogFile()
        {
            try
            {
                StorageFolder logRootFolder = KnownFolders.MusicLibrary;
                Debug.Log(">> FileInputLogger.CreateNewLogFile:  " + logRootFolder.ToString());
                if (logRootFolder != null)
                {
                        logFile = await logRootFolder.CreateFileAsync(Filename, CreationCollisionOption.ReplaceExisting);
                        
                        Debug.Log(string.Format("*** The log file path is: {0} -- \n -- {1}", logFile.Name, logFile.Path));
                }
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("Exception in FileInputLogger: {0}", e.Message));
            }
        }
#endif
        
#if WINDOWS_UWP
        public void AppendLog(string message)
        {
            // Log buffer to the file
            buffer.Append(message);
        }

        public async void SaveLogs()
        {
            if (buffer.Length > 0)
            {
                // Log buffer to the file
                await FileIO.AppendTextAsync(logFile, buffer.ToString());
            }
        }
#else
        /// <summary>
        /// Appends a message to the log file.
        /// </summary>
        public void AppendLog(string message)
        {
            logFileStream.Write(message);
        }
#endif
    }
}

#pragma warning restore CS1591
