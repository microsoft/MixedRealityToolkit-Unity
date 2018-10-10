namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public interface IUserTime
    {
        float Latency { get; }
        ITimeHandler TimeHandler { set; }

        void UpdateServerTime(float targetTime);
        void RequestLatencyCheck(float timeRequestSent);
    }
}