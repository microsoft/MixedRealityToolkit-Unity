// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    public abstract class CustomInputLogger : BasicInputLogger
    {
        [Tooltip("The data will be saved as CSV files.")]
        protected string Filename = null;
        protected static DateTime TimerStart;

        protected bool PrintedHeader = false;

        protected virtual void CustomAppend(string msg)
        {
            // Check if we've logged the header yet
            if (!PrintedHeader)
                PrintedHeader = Append(GetHeader());

            // Log the actual message
            Append(msg);
        }

        protected void CreateNewLog()
        {
            Debug.Log(">> CreateNewLog ");
            ResetLog();
            TimerStart = DateTime.UtcNow; // Reset timer
            PrintedHeader = false; // Set false, so that a new header is printed
        }

        public void StartLogging()
        {
            Debug.Log(">> START LOGGING! ");
            CreateNewLog();
            IsLogging = true;
        }

        public void StopLoggingAndSave()
        {
            Debug.Log(">> STOP LOGGING and save! ");
            SaveLogs();
            IsLogging = false;
        }

        public void CancelLogging()
        {
            IsLogging = false;
        }
    }
}
