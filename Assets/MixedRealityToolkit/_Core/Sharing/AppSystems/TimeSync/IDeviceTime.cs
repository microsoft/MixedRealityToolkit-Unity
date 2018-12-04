using Pixie.Core;
using Pixie.DeviceControl;

namespace Pixie.AppSystems.TimeSync
{
    public interface IDeviceTime : ISharingAppObject, IDeviceObject
    {
        short DeviceID { get; }
        float Latency { get; }
        bool Synchronized { get; }
        ITimeHandler TimeHandler { set; }

        void UpdateServerTime(float targetTime);
        void RequestLatencyCheck(float timeRequestSent);
        void UpdateLatency(float latency, bool synchronized);
    }
}