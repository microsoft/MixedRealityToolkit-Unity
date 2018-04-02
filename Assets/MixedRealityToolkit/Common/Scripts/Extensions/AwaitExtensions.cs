using System.Threading.Tasks;

namespace MixedRealityToolkit.Common.Extensions
{
    /// <summary>
    /// Custom GetAwaiters to await new types.
    /// </summary>
    public static class AwaitExtensions
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
    }
}
