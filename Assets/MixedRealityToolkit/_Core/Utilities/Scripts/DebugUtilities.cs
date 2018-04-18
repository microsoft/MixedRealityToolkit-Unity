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
            Debug.Assert(condition, message);
        }

        public static void DebugAssert(bool condition)
        {
            Debug.Assert(condition);
        }

        public static void DebugLogError(string message)
        {
#if UNITY_EDITOR
            Debug.LogError(message);
#endif
        }

        public static void DebugLogWarning(string message)
        {
#if UNITY_EDITOR
            Debug.LogWarning(message);
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