// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for Unity's Object class
    /// </summary>
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Enable Unity objects to skip "DontDestroyOnLoad" when editor isn't playing so test runner passes.
        /// </summary>
        /// <param name="target"></param>
        public static void DontDestroyOnLoad(this Object target)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
#endif
                Object.DontDestroyOnLoad(target);
        }
    }
}
