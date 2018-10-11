using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl
{
    public interface IStatePipeOutputSource
    {
        IEnumerable<IStatePipeOutput> StatePipeOutputs { get; }
    }

    public interface IStatePipeInput
    {
        bool Sending { get; }
        void SendStates(IEnumerable<object> states);
    }
}