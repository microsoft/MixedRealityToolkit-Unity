using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl.Logging
{
    public interface IAppStateLogger
    {
        LogStateEnum State { get; }

        void Initialize();
        void StartLog();
        void TakeSnapshot(float time, IAppStateReadOnly appState, IEnumerable<Type> typesToLog = null);
        void LogChanges(float time, IAppStateReadOnly appState, IEnumerable<Type> typesToLog = null);
        void ClearRecentChanges();
        void StopLog();
    }
}