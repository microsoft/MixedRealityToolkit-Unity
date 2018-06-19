// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using SystemDebug = System.Diagnostics.Debug;
using UnityApplication = UnityEngine.Application;
using UnityDebug = UnityEngine.Debug;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities
{
    public static class DebugUtilities
    {
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

        public static void DebugAssert(bool condition)
        {
            DebugAssert(condition, string.Empty);
        }

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