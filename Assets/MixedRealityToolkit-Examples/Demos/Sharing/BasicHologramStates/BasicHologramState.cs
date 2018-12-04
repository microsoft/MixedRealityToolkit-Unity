using Pixie.Core;
using System;
using UnityEngine;

namespace Pixie.Demos
{
    [Serializable]
    public struct BasicHologramState : IItemState, IItemStateComparer<BasicHologramState>
    {
        public BasicHologramState(short hologramID)
        {
            HologramID = hologramID;
            ColorR = 255;
            ColorG = 255;
            ColorB = 255;
        }

        public short HologramID;

        short IItemState.Key { get { return HologramID; } }

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

        public byte ColorR;
        public byte ColorG;
        public byte ColorB;

        public bool IsDifferent(BasicHologramState from)
        {
            return ColorR != from.ColorR
                || ColorG != from.ColorG
                || ColorB != from.ColorB;
        }

        public BasicHologramState Merge(BasicHologramState localValue, BasicHologramState remoteValue)
        {
            return remoteValue;
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }
    }
}