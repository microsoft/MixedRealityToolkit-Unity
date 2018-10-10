using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions
{
    public interface IExperienceMode
    {
        string Name { get; }
        short ID { get; }
        string LayoutSceneName { get; }
        IEnumerable<StandInSetting> StandInSettings { get; }
        IEnumerable<string> StageNames { get; }
    }
}