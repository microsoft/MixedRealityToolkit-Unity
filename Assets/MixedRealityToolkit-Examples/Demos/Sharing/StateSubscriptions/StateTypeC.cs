using Pixie.Core;
using System;

namespace Pixie.Demos
{
    [Serializable]
    [AppStateType]
    public struct StateTypeC : IItemState, IItemStateComparer<StateTypeC>
    {
        public StateTypeC(short stateID)
        {
            StateID = stateID;
            Value = 0;
        }
        short IItemState.Key { get { return StateID; } }

        public short StateID;
        public byte Value;

        public bool IsDifferent(StateTypeC from)
        {
            return !from.Equals(this);
        }

        public StateTypeC Merge(StateTypeC localValue, StateTypeC remoteValue)
        {
            return remoteValue;
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }
    }
}