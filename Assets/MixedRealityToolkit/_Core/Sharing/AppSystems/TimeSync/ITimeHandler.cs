using Pixie.Core;

namespace Pixie.AppSystems.TimeSync
{
    public interface ITimeHandler : ISharingAppObject
    {
        void RespondToLatencyCheck(short userID, float timeRequestSent);
        void UpdateServerTime(float targetTime);
        void AddDevice(IDeviceTime deviceTime);
        void RemoveDevice(IDeviceTime deviceTime);
    }
}