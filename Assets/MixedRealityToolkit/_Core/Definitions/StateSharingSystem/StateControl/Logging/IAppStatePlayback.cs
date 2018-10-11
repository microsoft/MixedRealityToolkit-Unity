namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl.Logging
{
    public interface IAppStatePlayback : IAppStateReadOnly
    {
        void SetTime(float time);
    }
}