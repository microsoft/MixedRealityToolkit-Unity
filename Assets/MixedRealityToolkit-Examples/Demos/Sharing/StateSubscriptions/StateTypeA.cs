using Pixie.Core;
using System;

namespace Pixie.Demos
{
    [Serializable]
    [AppStateType]
    public struct StateTypeA : IItemState, IItemStateComparer<StateTypeA>
    {
        public StateTypeA(short stateID)
        {
            StateID = stateID;
            Value = 0;
        }

        short IItemState.Key { get { return StateID; } }

        public short StateID;
        public byte Value;

        public bool IsDifferent(StateTypeA from)
        {
            return !from.Equals(this);
        }

        public StateTypeA Merge(StateTypeA localValue, StateTypeA remoteValue)
        {
            return remoteValue;
        }

        public override string ToString()
        {           
            return StateUtils.StateToString(this);
        }
    }
}