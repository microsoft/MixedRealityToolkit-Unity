// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for sample. While nice to have, this documentation is not required for samples.
#pragma warning disable CS1591

using System;
using System.IO;
using UnityEngine;

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
            Debug.Log(Application.persistentDataPath + "/" + fileName);
            logFileStream = File.CreateText(Application.persistentDataPath + "/" + fileName);
            logFileStream.AutoFlush = true;
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
            
            logFileStream.Close();
            logFileStream.Dispose();
        }

        /// <summary>
        /// Appends a message to the log file.
        /// </summary>
        public void AppendLog(string message)
        {
            logFileStream.Write(message);
        }
    }
}

#pragma warning restore CS1591
