// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Sharing.Utilities
{
    /// <summary>
    /// Utility class that writes the sharing service log messages to the Unity Engine's console.
    /// </summary>
    public class ConsoleLogWriter : LogWriter
    {
        public override void WriteLogEntry(LogSeverity severity, string message)
        {
            if (severity == LogSeverity.Info)
            {
                Debug.Log(message);
            }
            else if (severity == LogSeverity.Warning)
            {
                Debug.LogWarning(message);
            }
            else
            {
                Debug.LogError(message);
            }
        }
    }
}