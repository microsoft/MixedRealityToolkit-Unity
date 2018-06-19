// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using SystemDebug = System.Diagnostics.Debug;
using UnityApplication = UnityEngine.Application;
using UnityDebug = UnityEngine.Debug;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities
{
    /// <summary>
    /// The DebugUtilities class will use the UnityEngine Debug class while in the Editor
    /// and the Debug class from the .NET Framework when running a compiled version of the
    /// application.
    /// </summary>
    public static class DebugUtilities
    {
        /// <summary>
        /// Asserts a condition.
        /// </summary>
        /// <param name="condition">The condition that is expected to be true.</param>
        /// <param name="message">The message to display if the condition evaluates to false.</param>
        public static void DebugAssert(bool condition, string message)
        {
            if (UnityApplication.isEditor)
            {
                UnityDebug.Assert(condition, message);
            }
            else
            {
                SystemDebug.Assert(condition, message);
            }
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
        /// <param name="condition">The message to log.</param>
        public static void DebugLogError(string message)
        {
            if (UnityApplication.isEditor)
            {
                UnityDebug.LogError(message);
            }
            else
            {
                SystemDebug.WriteLine("Error: {0}", message);
            }
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="condition">The message to log.</param>
        public static void DebugLogWarning(string message)
        {
            if (UnityApplication.isEditor)
            {
                UnityDebug.LogWarning(message);
            }
            else
            {
                SystemDebug.WriteLine("Warning: {0}", message);
            }
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="condition">The message to log.</param>
        public static void DebugLog(string message)
        {
            if (UnityApplication.isEditor)
            {
                UnityDebug.Log(message);
            }
            else
            {
                SystemDebug.WriteLine(message);
            }
        }
    }
}