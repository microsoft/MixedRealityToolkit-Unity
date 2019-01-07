using Pixie.Core;
using System;

namespace Pixie.Demos
{
    [Serializable]
    [AppStateType]
    public struct BasicState : IItemState, IItemStateComparer<BasicState>
    {
        /// <summary>
        /// States must have a constructor with arguments for Filter and Key, in that order.
        /// This unfortunately can't be defined in the item state.
        /// </summary>
        public BasicState(short stateID)
        {
            StateID = stateID;
            Value = 0;
        }

        /// <summary>
        /// We recommend implementing Filter and Key explicitely.
        /// This allows for using different names for keys, eg 'UserID'
        /// </summary>
        short IItemState.Key { get { return StateID; } }

        public short StateID;
        public byte Value;

        /// <summary>
        /// IsDifferent can return a straight value-type comparison, but we don't recommend this.
        /// States will often want to disregard local variables or slight floating-point differences.
        /// Value type comparison can also generate garbage.
        /// </summary>
        public bool IsDifferent(BasicState from)
        {
            return !from.Equals(this);
        }

        /// <summary>
        /// Most of the time you will want to use the remote value.
        /// Exceptions might include a state which includes a local change timestamp.
        /// </summary>
        public BasicState Merge(BasicState localValue, BasicState remoteValue)
        {
            return remoteValue;
        }

        /// <summary>
        /// We recommend using our StateToString utility.
        /// It automatically displays all fields.
        /// Useful for in-editor testing.
        public override string ToString()
        {           
            return StateUtils.StateToString(this);
        }
    }
}