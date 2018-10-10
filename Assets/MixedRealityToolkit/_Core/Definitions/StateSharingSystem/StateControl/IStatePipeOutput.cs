using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl
{
    public interface IStatePipeOutput
    {
        Queue<object> StatesReceived { get; }
    }
}