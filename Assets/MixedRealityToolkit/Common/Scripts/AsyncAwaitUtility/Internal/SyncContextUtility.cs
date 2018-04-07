// MIT License

// Copyright(c) 2016 Modest Tree Media Inc

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
