// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities
{
    /// <summary>
    /// Utility methods that wrap the Unity Debug class. These methods provide
    /// additional functionality (ex: filtering log messages by logging level) 
    /// that can be used in Mixed Reality Toolkit and application code.
    /// </summary>
    public static class DebugUtilities
    {
        /// <summary>
        /// Returns the currently configured Mixed Reality Toolkit logging level.
        /// </summary>
        private static LoggingLevels MixedRealityToolkitLoggingLevel =>
            (MixedRealityToolkit.Instance.ActiveProfile != null) ? MixedRealityToolkit.Instance.ActiveProfile.LoggingLevel : (LoggingLevels)(-1);

        /// <summary>
        /// Assert a condition and log a message if the condition is not satisfied, using the Mixed Reality Toolkit
        /// object's currently configured logging level.
        /// </summary>
        /// <param name="condition">The condition to be evaluated.</param>
        public static void Assert(bool condition)
        {
            Assert(MixedRealityToolkitLoggingLevel, condition);
        }

        /// <summary>
        /// Assert a condition and log a message if the condition is not satisfied.
        /// </summary>
        /// <param name="logLevel">The logging level used to filter debug output.</param>
        /// <param name="condition">The condition to be evaluated.</param>
        public static void Assert(LoggingLevels logLevel, bool condition)
        {
            Assert(logLevel, condition, string.Empty);
        }

        /// <summary>
        /// Assert a condition and log a message if the condition is not satisfied, using the Mixed Reality Toolkit
        /// object's currently configured logging level.
        /// </summary>
        /// <param name="condition">The condition to be evaluated.</param>
        /// <param name="message">Message to display if the condition is not satisfied.</param>
        public static void Assert(bool condition, string message)
        {
            Assert(MixedRealityToolkitLoggingLevel, condition, message);
        }

        /// <summary>
        /// Assert a condition and log a message if the condition is not satisfied.
        /// </summary>
        /// <param name="logLevel">The logging level used to filter debug output.</param>
        /// <param name="condition">The condition to be evaluated.</param>
        /// <param name="message">Message to display if the condition is not satisfied.</param>
        public static void Assert(LoggingLevels logLevel, bool condition, string message)
        {
            if (ShouldWriteLog(logLevel, LoggingLevels.Assert))
            {
                Debug.Assert(condition, message);
            }
        }

        /// <summary>
        /// Logs a critial error message, using the Mixed Reality Toolkit
        /// object's currently configured logging level.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <remarks>
        /// To help differentiate the output of this method from that of LogError, the
        /// string "Critical: " is prepended to the supplied message.
        /// </remarks>
        public static void LogCriticalError(string message)
        {
            LogCriticalError(MixedRealityToolkitLoggingLevel, message);
        }

        /// <summary>
        /// Logs a critial error message.
        /// </summary>
        /// <param name="logLevel">The logging level used to filter debug output.</param>
        /// <param name="message">The message to display.</param>
        /// <remarks>
        /// To help differentiate the output of this method from that of LogError, the
        /// string "Critical: " is prepended to the supplied message.
        /// </remarks>
        public static void LogCriticalError(LoggingLevels logLevel, string message)
        {
            if (ShouldWriteLog(logLevel, LoggingLevels.CriticalError))
            {
                Debug.LogError($"Critical: {message}");
            }
        }

        /// <summary>
        /// Logs an error message, using the Mixed Reality Toolkit
        /// object's currently configured logging level.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static void LogError(string message)
        {
            LogError(MixedRealityToolkitLoggingLevel, message);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="logLevel">The logging level used to filter debug output.</param>
        /// <param name="message">The message to display.</param>
        public static void LogError(LoggingLevels logLevel, string message)
        {
            if (ShouldWriteLog(logLevel, LoggingLevels.Error))
            {
                Debug.LogError(message);
            }
        }

        /// <summary>
        /// Logs an informational message, using the Mixed Reality Toolkit
        /// object's currently configured logging level.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static void LogInformation(string message)
        {
            LogInformation(MixedRealityToolkitLoggingLevel, message);
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="logLevel">The logging level used to filter debug output.</param>
        /// <param name="message">The message to display.</param>
        public static void LogInformation(LoggingLevels logLevel, string message)
        {
            if (ShouldWriteLog(logLevel, LoggingLevels.Informational))
            {
                Debug.Log(message);
            }
        }

        /// <summary>
        /// Logs a warning message, using the Mixed Reality Toolkit
        /// object's currently configured logging level.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static void LogWarning(string message)
        {
            LogWarning(MixedRealityToolkitLoggingLevel, message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="logLevel">The logging level used to filter debug output.</param>
        /// <param name="message">The message to display.</param>
        public static void LogWarning(LoggingLevels logLevel, string message)
        {
            if (ShouldWriteLog(logLevel, LoggingLevels.Warning))
            {
                Debug.LogWarning(message);
            }
        }

        /// <summary>
        /// Checks to see if a log message should be written, using the Mixed Reality Toolkit
        /// object's currently configured logging level.
        /// </summary>
        /// <param name="messageType">The type of log message being checked.</param>
        private static bool ShouldWriteLog(LoggingLevels messageType)
        {
            return ShouldWriteLog(MixedRealityToolkitLoggingLevel, messageType);
        }

        /// <summary>
        /// Checks to see if a log message should be written.
        /// </summary>
        /// <param name="logLevel">The logging level used to filter debug output.</param>
        /// <param name="messageType">The type of log message being checked.</param>
        private static bool ShouldWriteLog(LoggingLevels logLevel, LoggingLevels messageType)
        {
            return (logLevel & messageType) != 0;
        }
    }
}