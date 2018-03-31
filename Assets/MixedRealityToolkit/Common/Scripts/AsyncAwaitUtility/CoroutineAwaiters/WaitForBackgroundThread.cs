using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MixedRealityToolkit.Common.AsyncAwaitUtilities.CoroutineAwaiters
{
    public class WaitForBackgroundThread
    {
        public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
        {
            return Task.Run(() => { }).ConfigureAwait(false).GetAwaiter();
        }
    }
}
