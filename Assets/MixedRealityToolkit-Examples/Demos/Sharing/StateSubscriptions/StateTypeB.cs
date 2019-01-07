using Pixie.Core;
using System;

namespace Pixie.Demos
{
    [Serializable]
    [AppStateType]
    public struct StateTypeB : IItemState, IItemStateComparer<StateTypeB>
    {
        public StateTypeB(short stateID)
        {
            StateID = stateID;
            Value = 0;
        }
        short IItemState.Key { get { return StateID; } }

        public short StateID;
        public byte Value;

        public bool IsDifferent(StateTypeB from)
        {
            return !from.Equals(this);
        }

        public StateTypeB Merge(StateTypeB localValue, StateTypeB remoteValue)
        {
            return remoteValue;
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }
    }
}