// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Focus;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities
{
    public static class DebugUtilities
    {
        public static void DebugAssert(bool condition, string message)
        {
#if UNITY_EDITOR
            Debug.Assert(condition, message);
#endif
        }
        public static void DebugAssert(bool condition)
        {
#if UNITY_EDITOR
            Debug.Assert(condition);
#endif
        }

        public static void DebugLogError(string message)
        {
#if UNITY_EDITOR
            Debug.LogError(message);
#endif
        }

        public static void DebugLogErrorFormat(string format, params object[] args)
        {
#if UNITY_EDITOR
            Debug.LogErrorFormat(format, args);
#endif
        }

        public static void DebugLogWarning(string message)
        {
#if UNITY_EDITOR
            Debug.LogWarning(message);
#endif
        }

        public static void DebugLogWarningFormat(string format, params object[] args)
        {
#if UNITY_EDITOR
            Debug.LogWarningFormat(format, args);
#endif
        }

        public static void DebugLog(string message)
        {
#if UNITY_EDITOR
            Debug.Log(message);
#endif
        }
    }
}