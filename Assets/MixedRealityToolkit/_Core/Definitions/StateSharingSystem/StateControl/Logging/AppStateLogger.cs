using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl.Logging
{
    public class AppStateLogger : MonoBehaviour, IAppStateLogger
    {
        public LogStateEnum State { get { return state; } }

        private LogStateEnum state = LogStateEnum.Uninitialized;

        public void Initialize()
        {
            switch (state)
            {
                case LogStateEnum.Stopped:
                case LogStateEnum.Uninitialized:
                    break;

                default:
                    throw new Exception("Can't initialize in state " + state);
            }

            // TODO open file stream

            state = LogStateEnum.Stopped;
        }

        public void ClearRecentChanges()
        {
            throw new System.NotImplementedException();
        }

        public void LogChanges(float time, IAppStateReadOnly appState, IEnumerable<Type> typesToLog = null)
        {
            switch (state)
            {
                case LogStateEnum.Idle:
                    break;

                default:
                    throw new Exception("Can't log changes in state " + state);
            }
        }

        public void StartLog()
        {
            switch (state)
            {
                case LogStateEnum.Stopped:
                    break;

                default:
                    throw new Exception("Can't start log in state " + state);
            }
        }

        public void StopLog()
        {
            switch (state)
            {
                case LogStateEnum.Idle:
                case LogStateEnum.Writing:
                    break;

                default:
                    throw new Exception("Can't stop log in state " + state);
            }
        }

        public void TakeSnapshot(float time, IAppStateReadOnly appState, IEnumerable<Type> typesToLog = null)
        {
            switch (state)
            {
                case LogStateEnum.Idle:
                    break;

                default:
                    throw new Exception("Can't take snapshot in state " + state);
            }
        }
    }
}