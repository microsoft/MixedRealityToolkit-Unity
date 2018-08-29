// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities
{
    public static class DebugUtilities
    {
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
        /// <param name="condition">The message to log.</param>
        public static void DebugLogError(string message)
        {
            Debug.LogError(message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="condition">The message to log.</param>
        public static void DebugLogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="condition">The message to log.</param>
        public static void DebugLog(string message)
        {
            Debug.Log(message);
        }
    }
}