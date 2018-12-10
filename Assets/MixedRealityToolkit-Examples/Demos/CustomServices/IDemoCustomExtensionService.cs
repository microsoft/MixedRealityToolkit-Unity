using Microsoft.MixedReality.Toolkit.Core.Interfaces;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.CustomExtensionServices
{
    /// <summary>
    /// The custom interface for your extension service.
    /// Only use property accessors in these to better control access to internals.
    /// This interface is the contract for others to use to make their own implementations if shared publicly.
    /// </summary>
    public interface IDemoCustomExtensionService : IMixedRealityExtensionService
    {
        /// <summary>
        /// A custom property accessor.
        /// </summary>
        string MyCustomData { get; }

        /// <summary>
        /// A custom method call.
        /// </summary>
        void MyCustomMethod();
    }
}