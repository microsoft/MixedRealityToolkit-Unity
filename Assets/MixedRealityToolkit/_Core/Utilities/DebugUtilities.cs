// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
            if (Application.isEditor)
            {
                Debug.LogError(message);
            }
        }

        public static void DebugLogWarning(string message)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning(message);
            }
        }

        public static void DebugLog(string message)
        {
            if (Application.isEditor)
            {
                Debug.Log(message);
            }
        }
    }
}