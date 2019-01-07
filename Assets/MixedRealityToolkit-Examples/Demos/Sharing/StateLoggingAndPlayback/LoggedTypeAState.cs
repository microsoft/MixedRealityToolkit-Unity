using Pixie.Core;
using System;
using UnityEngine;

namespace Pixie.Demos
{
    [Serializable]
    [AppStateType]
    public struct LoggedTypeAState : IItemState, IItemStateComparer<LoggedTypeAState>
    {
        const float MaxPositionRange = 20;

        public LoggedTypeAState(short hologramID)
        {
            HologramID = hologramID;
            TargetPosX = 0;
            TargetPosY = 0;
            TargetPosZ = 0;
        }

        public short HologramID;

        short IItemState.Key { get { return HologramID; } }

        public Vector3 TargetPosition
        {
            get { return StateUtils.ShortPosOut(TargetPosX, TargetPosY, TargetPosZ, MaxPositionRange); }
            set { StateUtils.ShortPosIn(value, out TargetPosX, out TargetPosY, out TargetPosZ, MaxPositionRange); }
        }

        public short TargetPosX;
        public short TargetPosY;
        public short TargetPosZ;

        public bool IsDifferent(LoggedTypeAState from)
        {
            return TargetPosX != from.TargetPosX
                || TargetPosY != from.TargetPosY
                || TargetPosZ != from.TargetPosZ;
        }

        public LoggedTypeAState Merge(LoggedTypeAState localValue, LoggedTypeAState remoteValue)
        {
            return remoteValue;
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }
    }
}