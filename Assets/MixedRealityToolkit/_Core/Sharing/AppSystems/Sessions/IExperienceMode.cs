using Pixie.DeviceControl.Users;
using System.Collections.Generic;

namespace Pixie.AppSystems.Sessions
{
    public interface IExperienceMode
    {
        string Name { get; }
        short ID { get; }
        string LayoutSceneName { get; }
        IEnumerable<UserDefinition> UserDefinitions { get; }
        IEnumerable<string> StageNames { get; }
    }
}