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

using Microsoft.MixedReality.Toolkit.Core.Utilities.Async.AwaitYieldInstructions;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Async
{
    /// <summary>
    /// We could just add a generic GetAwaiter to YieldInstruction and CustomYieldInstruction
    /// but instead we add specific methods to each derived class to allow for return values
    /// that make the most sense for the specific instruction type.
    /// </summary>
    public static class AwaiterExtensions
    {
        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForSeconds instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForUpdate instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForEndOfFrame instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForFixedUpdate instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForSecondsRealtime instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitUntil instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitWhile instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation instruction)
        {
            return GetAwaiterReturnSelf(instruction);
        }

        public static SimpleCoroutineAwaiter<Object> GetAwaiter(this ResourceRequest instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter<Object>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.ResourceRequest(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<AssetBundle> GetAwaiter(this AssetBundleCreateRequest instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter<AssetBundle>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.AssetBundleCreateRequest(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<Object> GetAwaiter(this AssetBundleRequest instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter<Object>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.AssetBundleRequest(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<T> GetAwaiter<T>(this IEnumerator<T> coroutine)
        {
            var awaiter = new SimpleCoroutineAwaiter<T>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                new CoroutineWrapper<T>(coroutine, awaiter).Run()));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<object> GetAwaiter(this IEnumerator coroutine)
        {
            var awaiter = new SimpleCoroutineAwaiter<object>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                new CoroutineWrapper<object>(coroutine, awaiter).Run()));
            return awaiter;
        }

        private static SimpleCoroutineAwaiter GetAwaiterReturnVoid(object instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.ReturnVoid(awaiter, instruction)));
            return awaiter;
        }

        private static SimpleCoroutineAwaiter<T> GetAwaiterReturnSelf<T>(T instruction)
        {
            var awaiter = new SimpleCoroutineAwaiter<T>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.ReturnSelf(awaiter, instruction)));
            return awaiter;
        }

        private static void RunOnUnityScheduler(Action action)
        {
            if (SynchronizationContext.Current == SyncContextUtility.UnitySynchronizationContext)
            {
                action();
            }
            else
            {
                AsyncCoroutineRunner.Post(action);
            }
        }

        /// <summary>
        /// Processes Coroutine and notifies completion with result.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        public class SimpleCoroutineAwaiter<T> : INotifyCompletion
        {
            private Exception exception;
            private Action continuation;
            private T result;

            public bool IsCompleted { get; private set; }

            public T GetResult()
            {
                Debug.Assert(IsCompleted);

                if (exception != null)
                {
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }

                return result;
            }

            public void Complete(T taskResult, Exception e)
            {
                Debug.Assert(!IsCompleted);

                IsCompleted = true;
                exception = e;
                result = taskResult;

                // Always trigger the continuation on the unity thread
                // when awaiting on unity yield instructions.
                if (continuation != null)
                {
                    RunOnUnityScheduler(continuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action notifyContinuation)
            {
                Debug.Assert(continuation == null);
                Debug.Assert(!IsCompleted);

                continuation = notifyContinuation;
            }
        }

        /// <summary>
        /// Processes Coroutine and notifies completion.
        /// </summary>
        public class SimpleCoroutineAwaiter : INotifyCompletion
        {
            private Exception exception;
            private Action continuation;

            public bool IsCompleted { get; private set; }

            public void GetResult()
            {
                Debug.Assert(IsCompleted);

                if (exception != null)
                {
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
            }

            public void Complete(Exception e)
            {
                Debug.Assert(!IsCompleted);

                IsCompleted = true;
                exception = e;

                // Always trigger the continuation on the unity thread
                // when awaiting on unity yield instructions.
                if (continuation != null)
                {
                    RunOnUnityScheduler(continuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action notifyContinuation)
            {
                Debug.Assert(continuation == null);
                Debug.Assert(!IsCompleted);

                continuation = notifyContinuation;
            }
        }

        private class CoroutineWrapper<T>
        {
            private readonly SimpleCoroutineAwaiter<T> awaiter;
            private readonly Stack<IEnumerator> processStack;

            public CoroutineWrapper(IEnumerator coroutine, SimpleCoroutineAwaiter<T> awaiter)
            {
                processStack = new Stack<IEnumerator>();
                processStack.Push(coroutine);
                this.awaiter = awaiter;
            }

            public IEnumerator Run()
            {
                while (true)
                {
                    var topWorker = processStack.Peek();

                    bool isDone;

                    try
                    {
                        isDone = !topWorker.MoveNext();
                    }
                    catch (Exception e)
                    {
                        // The IEnumerators we have in the process stack do not tell us the
                        // actual names of the coroutine methods but it does tell us the objects
                        // that the IEnumerators are associated with, so we can at least try
                        // adding that to the exception output
                        var objectTrace = GenerateObjectTrace(processStack);
                        awaiter.Complete(default(T), objectTrace.Any() ? new Exception(GenerateObjectTraceMessage(objectTrace), e) : e);

                        yield break;
                    }

                    if (isDone)
                    {
                        processStack.Pop();

                        if (processStack.Count == 0)
                        {
                            awaiter.Complete((T)topWorker.Current, null);
                            yield break;
                        }
                    }

                    // We could just yield return nested IEnumerator's here but we choose to do
                    // our own handling here so that we can catch exceptions in nested coroutines
                    // instead of just top level coroutine
                    var item = topWorker.Current as IEnumerator;
                    if (item != null)
                    {
                        processStack.Push(item);
                    }
                    else
                    {
                        // Return the current value to the unity engine so it can handle things like
                        // WaitForSeconds, WaitToEndOfFrame, etc.
                        yield return topWorker.Current;
                    }
                }
            }

            private static string GenerateObjectTraceMessage(List<Type> objTrace)
            {
                var result = new StringBuilder();

                foreach (var objType in objTrace)
                {
                    if (result.Length != 0)
                    {
                        result.Append(" -> ");
                    }

                    result.Append(objType);
                }

                result.AppendLine();
                return $"Unity Coroutine Object Trace: {result}";
            }

            private static List<Type> GenerateObjectTrace(IEnumerable<IEnumerator> enumerators)
            {
                var objTrace = new List<Type>();

                foreach (var enumerator in enumerators)
                {
                    // NOTE: This only works with scripting engine 4.6
                    // And could easily stop working with unity updates
                    var field = enumerator.GetType().GetField("$this", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    if (field == null)
                    {
                        continue;
                    }

                    var obj = field.GetValue(enumerator);

                    if (obj == null)
                    {
                        continue;
                    }

                    var objType = obj.GetType();

                    if (!objTrace.Any() || objType != objTrace.Last())
                    {
                        objTrace.Add(objType);
                    }
                }

                objTrace.Reverse();
                return objTrace;
            }
        }

        private static class InstructionWrappers
        {
            public static IEnumerator ReturnVoid(SimpleCoroutineAwaiter awaiter, object instruction)
            {
                // For simple instructions we assume that they don't throw exceptions
                yield return instruction;
                awaiter.Complete(null);
            }

            public static IEnumerator AssetBundleCreateRequest(SimpleCoroutineAwaiter<AssetBundle> awaiter, AssetBundleCreateRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.assetBundle, null);
            }

            public static IEnumerator ReturnSelf<T>(SimpleCoroutineAwaiter<T> awaiter, T instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction, null);
            }

            public static IEnumerator AssetBundleRequest(SimpleCoroutineAwaiter<Object> awaiter, AssetBundleRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.asset, null);
            }

            public static IEnumerator ResourceRequest(SimpleCoroutineAwaiter<Object> awaiter, ResourceRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.asset, null);
            }
        }
    }
}
