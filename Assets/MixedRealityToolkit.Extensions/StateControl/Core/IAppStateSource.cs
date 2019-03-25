using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.StateControl
{
    public interface IAppStateSource
    {
        IEnumerable<Type> StateTypes { get; }
        void GenerateRequiredStates(IAppStateReadWrite appState);
    }
}