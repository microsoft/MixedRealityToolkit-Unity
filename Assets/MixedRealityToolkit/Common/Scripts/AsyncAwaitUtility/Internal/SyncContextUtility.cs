// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading;
using UnityEngine;

namespace MixedRealityToolkit.Common.AsyncAwaitUtilities.Internal
{
    public static class SyncContextUtility
    {
#if UNITY_EDITOR
        private static System.Reflection.MethodInfo executionMethod;

        /// <summary>
        /// Hack to get Unity Editor to execute continuations in edit mode.
        /// </summary>
        private static void ExecuteContinuations()
        {
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            var context = SynchronizationContext.Current;

            if (executionMethod == null)
            {
                executionMethod = context.GetType().GetMethod("Exec", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            }

            executionMethod?.Invoke(context, null);
        }

        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += ExecuteContinuations;
#endif
            UnitySynchronizationContext = SynchronizationContext.Current;
            UnityThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static int UnityThreadId { get; private set; }

        public static SynchronizationContext UnitySynchronizationContext { get; private set; }
    }
}
