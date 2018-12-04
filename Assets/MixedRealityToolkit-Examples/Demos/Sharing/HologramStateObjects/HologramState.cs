using Pixie.Core;
using System;
using UnityEngine;

namespace Pixie.Demos
{
    [Serializable]
    public struct HologramState : IItemState, IItemStateComparer<HologramState>
    {
        const float MaxPositionRange = 20;

        public HologramState(short hologramID)
        {
            HologramID = hologramID;
            PosX = 0;
            PosY = 0;
            PosZ = 0;
        }

        public short HologramID;

        short IItemState.Key { get { return HologramID; } }

        public Vector3 Position
        {
            get { return StateUtils.ShortPosOut(PosX, PosY, PosZ, MaxPositionRange); }
            set { StateUtils.ShortPosIn(value, out PosX, out PosY, out PosZ, MaxPositionRange); }
        }

        public short PosX;
        public short PosY;
        public short PosZ;

        public bool IsDifferent(HologramState from)
        {
            return PosX != from.PosX
                || PosY != from.PosY
                || PosZ != from.PosZ;
        }

        public HologramState Merge(HologramState localValue, HologramState remoteValue)
        {
            return remoteValue;
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }
    }
}