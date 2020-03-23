// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
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
        /// <param name="message">The message to log.</param>
        public static void DebugLogError(string message)
        {
            Debug.LogError(message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void DebugLogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void DebugLog(string message)
        {
            Debug.Log(message);
        }

        /// <summary>
        /// Draws a point in the Scene window.
        /// </summary>
        public static void DrawPoint(Vector3 point, Color color, float size = 0.05f)
        {
            DrawPoint(point, Quaternion.identity, color, size);
        }
        
        /// <summary>
        /// Draws a point with a rotation in the Scene window.
        /// </summary>
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
        public static void DrawBounds(Bounds bounds, Color minColor, Color maxColor)
        {
            DrawPoint(bounds.min, minColor);
            DrawPoint(bounds.max, maxColor);
        }
    }
}