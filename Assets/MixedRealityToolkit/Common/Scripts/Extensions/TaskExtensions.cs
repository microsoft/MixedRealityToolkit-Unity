// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MixedRealityToolkit.Common.Extensions
{
    /// <summary>
    /// <see cref="Task"/> Extension Methods.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Any time you call an async method from sync code, you can either use this wrapper
        /// method or you can define your own `async void` method that performs the await
        /// on the given Task
        /// </summary>
        /// <param name="task"></param>
        public static async void WrapErrors(this Task task)
        {
            await task;
        }

        /// <summary>
        /// Run Task as IEnumerator.
        /// </summary>
        public static IEnumerator AsIEnumerator(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted && task.Exception != null)
            {
                throw task.Exception;
            }
        }

        /// <summary>
        /// Run Task as IEnumerator.
        /// </summary>
        public static IEnumerator<T> AsIEnumerator<T>(this Task<T> task) where T : class
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted && task.Exception != null)
            {
                throw task.Exception;
            }

            yield return task.Result;
        }
    }
}
