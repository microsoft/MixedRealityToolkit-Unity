using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public interface ITimeHandler : ISharingAppObject
    {
        float RespondToLatencyCheck(sbyte userNum, float timeRequestSent);
        void UpdateServerTime(float latestSyncedTime);
    }
}