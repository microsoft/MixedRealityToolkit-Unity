using Pixie.Core;
using System;
using UnityEngine;

namespace Pixie.Demos
{
    [Serializable]
    public struct SessionHologramState : IItemState, IItemStateComparer<SessionHologramState>
    {
        const float MaxPositionRange = 20;

        public SessionHologramState(short hologramID)
        {
            HologramID = hologramID;
            PosX = 0;
            PosY = 0;
            PosZ = 0;
            ColorR = 255;
            ColorG = 255;
            ColorB = 255;
        }

        public short HologramID;

        short IItemState.Key { get { return HologramID; } }

        public Vector3 Position
        {
            get { return StateUtils.ShortPosOut(PosX, PosY, PosZ, MaxPositionRange); }
            set { StateUtils.ShortPosIn(value, out PosX, out PosY, out PosZ, MaxPositionRange); }
        }

        public Color32 Color
        {
            get { return new Color32(ColorR, ColorG, ColorB, 255); }
            set
            {
                ColorR = value.r;
                ColorG = value.g;
                ColorB = value.b;
            }
        }

        public short PosX;
        public short PosY;
        public short PosZ;
        public byte ColorR;
        public byte ColorG;
        public byte ColorB;

        public bool IsDifferent(SessionHologramState from)
        {
            return PosX != from.PosX
                || PosY != from.PosY
                || PosZ != from.PosZ
                || ColorR != from.ColorR
                || ColorG != from.ColorG
                || ColorB != from.ColorB;
        }

        public SessionHologramState Merge(SessionHologramState localValue, SessionHologramState remoteValue)
        {
            return remoteValue;
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }
    }
}