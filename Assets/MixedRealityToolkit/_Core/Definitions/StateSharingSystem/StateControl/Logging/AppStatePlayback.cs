using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl.Logging
{
    public class AppStatePlayback : MonoBehaviour, IAppStatePlayback
    {
        public bool Synchronized { get { return true; } }

        public IEnumerable<Type> RequiredStateTypes { get { yield break; } }

        public IEnumerable<Type> ItemStateTypes { get { yield break; } }

        public int GetNumStates<T>() where T : struct, IItemState<T>
        {
            throw new NotImplementedException();
        }

        public T GetState<T>(sbyte itemNum) where T : struct, IItemState<T>
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetStates<T>() where T : struct, IItemState<T>
        {
            throw new NotImplementedException();
        }

        public void Initialize(bool isServer)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty<T>() where T : struct, IItemState<T>
        {
            throw new NotImplementedException();
        }

        public void SetTime(float time)
        {
            throw new NotImplementedException();
        }

        public bool StateExists<T>(sbyte stateKey)
        {
            throw new NotImplementedException();
        }
    }
}