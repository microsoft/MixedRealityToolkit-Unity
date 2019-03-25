namespace Microsoft.MixedReality.Toolkit.Extensions.StateControl.Core
{
    /// <summary>
    /// Manager objects are expected to:
    /// - Call OnAppConnect as soon as network connection is made
    /// - Call OnAppSynchronize when ready to synchronize
    /// </summary>
    public interface ISharingAppObject
    {
        bool ReadyToSynchronize { get; }

        void OnAppConnect();
        void OnAppSynchronize();
    }
}