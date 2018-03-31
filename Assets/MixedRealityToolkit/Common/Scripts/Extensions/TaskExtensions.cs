using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MixedRealityToolkit.Common.Extensions
{
    /// <summary>
    /// Run Tasks as IEnumerators.
    /// </summary>
    public static class TaskExtensions
    {
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
