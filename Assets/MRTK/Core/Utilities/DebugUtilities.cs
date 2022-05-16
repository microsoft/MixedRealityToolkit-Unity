// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public static class DebugUtilities
    {
        /// <summary>
        /// An enum describing the level of logs that DebugUtilities will emit to the
        /// Unity player log.
        /// </summary>
        /// <remarks>
        /// <para>Note that the order of enums below is important, since when the log level is
        /// set to any of the options below, all of the other log levels greater than it in
        /// value will also be enabled. For example, setting LogLevel to Warn will show both
        /// warnings and errors. The only exception for this is "None", which, when set, will
        /// not show any logs.</para>
        /// </remarks>
        public enum LoggingLevel
        {
            Verbose,
            Information,
            Warn,
            Error,
            None,
        }

        /// <summary>
        /// This helper defaults to showing all messages of information and above (i.e. including warnings and errors)
        /// </summary>
        public static LoggingLevel LogLevel = LoggingLevel.Information;

        /// <summary>
        /// Asserts a condition.
        /// </summary>
        /// <param name="condition">The condition that is expected to be true.</param>
        /// <param name="message">The message to display if the condition evaluates to false.</param>      
        public static void DebugAssert(bool condition, string message)
        {
            Debug.Assert(condition, message);
        }

        /// <summary>
        /// Asserts a condition.
        /// </summary>
        /// <param name="condition">The condition that is expected to be true.</param>
        public static void DebugAssert(bool condition)
        {
            DebugAssert(condition, string.Empty);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        [Obsolete("Deprecated. Use LogError instead.")]
        public static void DebugLogError(string message)
        {
            LogError(message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        [Obsolete("Deprecated. Use LogWarning instead.")]
        public static void DebugLogWarning(string message)
        {
            LogWarning(message);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        [Obsolete("Deprecated. Use Log instead.")]
        public static void DebugLog(string message)
        {
            Log(message);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogError(string message)
        {
            if (LogLevel <= LoggingLevel.Error)
            {
                Debug.LogError(message);
            }
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(string message)
        {
            if (LogLevel <= LoggingLevel.Warn)
            {
                Debug.LogWarning(message);
            }
        }

        /// <summary>
        /// Logs a message at the informational level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            // Note that the naming of this function (DebugUtilities.Log instead of DebugUtilities.LogInfo)
            // is to ensure consistency with the naming of Unity's Debug.Log function.
            if (LogLevel <= LoggingLevel.Information)
            {
                Debug.Log(message);
            }
        }

        /// <summary>
        /// Logs the given message to the Unity console and player log if verbose logging is enabled.
        /// </summary>
        /// <remarks>
        /// <para>If you are doing string concatenation or manipulation, use LogVerboseFormat
        /// that formats the string, as that will not allocate memory when verbose logging is enabled.</para>
        ///
        /// <para>For example, don't do:
        /// Debug.LogVerbose("This is my message: " + text);</para>
        ///
        /// <para>Do:
        /// Debug.LogVerbose("This is my message: {0}", text);</para>
        ///
        /// <para>Note that verbose logs do not include the callstack to reduce the noise in the generated
        /// editor log. Even with default stack trace parameter (StackTraceLogType.ScriptOnly),
        /// the editor logs will grow significantly (i.e. a 10x+ line growth factor).</para>
        /// </remarks>
        public static void LogVerbose(string message)
        {
            if (LogLevel <= LoggingLevel.Verbose)
            {
                // Save/restore the previous stack trace configuration, rather than assuming that
                // it's the default (StackTraceLogType.ScriptOnly)
                StackTraceLogType stackTraceLogType = Application.GetStackTraceLogType(LogType.Log);
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
                Debug.Log(message);
                Application.SetStackTraceLogType(LogType.Log, stackTraceLogType);
            }
        }

        /// <summary>
        /// Formats and logs the given message to the Unity console and player log if verbose logging is enabled.
        /// Note that verbose logs do not include the callstack
        /// </summary>
        public static void LogVerboseFormat(string message, params object[] args)
        {
            if (LogLevel <= LoggingLevel.Verbose)
            {
                // Save/restore the previous stack trace configuration, rather than assuming that
                // it's the default (StackTraceLogType.ScriptOnly)
                StackTraceLogType stackTraceLogType = Application.GetStackTraceLogType(LogType.Log);
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
                Debug.LogFormat(message, args);
                Application.SetStackTraceLogType(LogType.Log, stackTraceLogType);
            }
        }

        /// <summary>
        /// Draws a point in the Scene window.
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawPoint(Vector3 point, Color color, float size = 0.05f)
        {
            DrawPoint(point, Quaternion.identity, color, size);
        }

        /// <summary>
        /// Draws a point with a rotation in the Scene window.
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawPoint(Vector3 point, Quaternion rotation, Color color, float size = 0.05f)
        {
            Vector3[] axes = { rotation * Vector3.up, rotation * Vector3.right, rotation * Vector3.forward };

            for (int i = 0; i < axes.Length; ++i)
            {
                Vector3 a = point + size * axes[i];
                Vector3 b = point - size * axes[i];
                Debug.DrawLine(a, b, color);
            }
        }

        /// <summary>
        /// Draws the minimum and maximum points of the given bounds
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawBounds(Bounds bounds, Color minColor, Color maxColor)
        {
            DrawPoint(bounds.min, minColor);
            DrawPoint(bounds.max, maxColor);
        }
    }
}
