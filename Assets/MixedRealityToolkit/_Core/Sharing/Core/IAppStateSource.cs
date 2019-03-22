using System;
using System.Collections.Generic;

namespace MRTK.StateControl
{
    public interface IAppStateSource
    {
        IEnumerable<Type> StateTypes { get; }
        void GenerateRequiredStates(IAppStateReadWrite appState);
    }
}