// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Sharing.Utilities
{
    /// <summary>
    /// Utility class that writes the sharing service log messages to the Unity Engine's console.
    /// </summary>
    public class ConsoleLogWriter : LogWriter
    {
        public bool ShowDetailedLogs = false;

        public override void WriteLogEntry(LogSeverity severity, string message)
        {
            switch (severity)
            {
                case LogSeverity.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogSeverity.Error:
                    Debug.LogError(message);
                    break;
                case LogSeverity.Info:
                default:
                    if (ShowDetailedLogs)
                    {
                        Debug.Log(message);
                    }
                    break;
            }
        }
    }
}