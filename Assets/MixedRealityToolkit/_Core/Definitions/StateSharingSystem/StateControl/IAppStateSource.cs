using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl
{
    public interface IAppStateSource : INetworkBehaviour
    {
        bool Synchronized { get; set; }
        IEnumerable<Type> StateTypes { get; }

        void GenerateRequiredStates(IAppStateReadWrite appState);
    }
}