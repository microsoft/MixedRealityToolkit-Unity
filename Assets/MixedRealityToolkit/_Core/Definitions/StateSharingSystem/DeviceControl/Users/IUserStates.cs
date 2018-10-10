using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public interface IUserStates : INetworkBehaviour
    {
        UserState UserState { get; set; }
        HandState HandState { get; set; }
    }
}