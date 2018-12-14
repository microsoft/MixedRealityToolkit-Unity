using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.Sharing
{
    /// <summary>
    /// Simplest possible non-automatic state sharing.
    /// Will typically be networked but could also plug into a file system or local database.
    /// </summary>
    public interface ISimpleState : IMixedRealityService
    {
        Task<bool> HasKey(string key);
        Task SetState<T>(string key, T state);
        Task<T> GetState<T>(string key);
    }
}