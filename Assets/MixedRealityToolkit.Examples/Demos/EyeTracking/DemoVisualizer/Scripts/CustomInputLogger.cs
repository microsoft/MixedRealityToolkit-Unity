// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    public abstract class CustomInputLogger : BasicInputLogger
    {
        [Tooltip("The data will be saved as CSV files.")]
        protected string filename = null;
        protected static DateTime TimerStart;

        protected bool printedHeader = false;

        protected virtual void CustomAppend(string msg)
        {
            // Check if we've logged the header yet
            if (!printedHeader)
                printedHeader = Append(GetHeader());

            // Log the actual message
            Append(msg);
        }

        protected void CreateNewLog()
        {
            Debug.Log(">> CreateNewLog ");
            ResetLog();
            TimerStart = DateTime.UtcNow; // Reset timer
            printedHeader = false; // Set false, so that a new header is printed
        }

        public void StartLogging()
        {
            Debug.Log(">> START LOGGING! ");
            CreateNewLog();
            isLogging = true;
        }

        public void StopLoggingAndSave()
        {
            Debug.Log(">> STOP LOGGING and save! ");       
            SaveLogs();
            isLogging = false;
        }

        public void CancelLogging()
        {
            isLogging = false;
        }
    }
}