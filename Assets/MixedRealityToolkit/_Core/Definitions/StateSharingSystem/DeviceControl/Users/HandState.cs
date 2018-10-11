using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    [Serializable]
    public struct HandState : IItemState<HandState>
    {
        public HandState(sbyte userNum, sbyte sessionNum)
        {
            ItemNum = userNum;
            SessionNum = 0;

            LHandPosX = 0;
            LHandPosY = 0;
            LHandPosZ = 0;

            LHandDirX = 0;
            LHandDirY = 0;
            LHandDirZ = 1;

            RHandDirX = 0;
            RHandDirY = 0;
            RHandDirZ = 1;

            RHandPosX = 0;
            RHandPosY = 0;
            RHandPosZ = 0;

            RHandVelShort = 0;
            LHandVelShort = 0;
        }

        sbyte IItemState<HandState>.Key { get { return ItemNum; } }
        sbyte IItemState<HandState>.Filter { get { return SessionNum; } }

        public sbyte ItemNum;
        public sbyte SessionNum;

        public Vector3 LHandPos
        {
            get { return StateUtils.ShortPosOut(LHandPosX, LHandPosY, LHandPosZ, Globals.UNet.MaxPositionRange); }
            set { StateUtils.ShortPosIn(value, out LHandPosX, out LHandPosY, out LHandPosZ, Globals.UNet.MaxPositionRange); }
        }

        public Vector3 LHandDir
        {
            get { return StateUtils.ByteDirOut(LHandDirX, LHandDirY, LHandDirZ); }
            set { StateUtils.ByteDirIn(value, out LHandDirX, out LHandDirY, out LHandDirZ); }
        }
        public float LHandVel
        {
            get { return StateUtils.UShortValOut(LHandVelShort, Globals.UNet.MaxObjectVelocity); }
            set { LHandVelShort = StateUtils.UShortValIn(value, Globals.UNet.MaxObjectVelocity); }
        }

        public Vector3 RHandPos
        {
            get { return StateUtils.ShortPosOut(RHandPosX, RHandPosY, RHandPosZ, Globals.UNet.MaxPositionRange); }
            set { StateUtils.ShortPosIn(value, out RHandPosX, out RHandPosY, out RHandPosZ, Globals.UNet.MaxPositionRange); }
        }

        public Vector3 RHandDir
        {
            get { return StateUtils.ByteDirOut(RHandDirX, RHandDirY, RHandDirZ); }
            set { StateUtils.ByteDirIn(value, out RHandDirX, out RHandDirY, out RHandDirZ); }
        }
        public float RHandVel
        {
            get { return StateUtils.UShortValOut(RHandVelShort, Globals.UNet.MaxObjectVelocity); }
            set { RHandVelShort = StateUtils.UShortValIn(value, Globals.UNet.MaxObjectVelocity); }
        }

        public bool IsDifferent(HandState from)
        {
            return ItemNum != from.ItemNum
                || SessionNum != from.SessionNum

                || LHandPosX != from.LHandPosX
                || LHandPosY != from.LHandPosY
                || LHandPosZ != from.LHandPosZ

                || LHandDirX != from.LHandDirX
                || LHandDirY != from.LHandDirY
                || LHandDirZ != from.LHandDirZ

                || RHandPosX != from.RHandPosX
                || RHandPosY != from.RHandPosY
                || RHandPosZ != from.RHandPosZ

                || RHandDirX != from.RHandDirX
                || RHandDirY != from.RHandDirY
                || RHandDirZ != from.RHandDirZ

                || RHandVelShort != from.RHandVelShort
                || LHandVelShort != from.LHandVelShort;
        }

        public HandState Merge(HandState clientValue, HandState serverValue)
        {
            return serverValue;
        }

#region raw values
        public short LHandPosX;
        public short LHandPosY;
        public short LHandPosZ;

        public sbyte LHandDirX;
        public sbyte LHandDirY;
        public sbyte LHandDirZ;

        public sbyte RHandDirX;
        public sbyte RHandDirY;
        public sbyte RHandDirZ;

        public short RHandPosX;
        public short RHandPosY;
        public short RHandPosZ;

        public ushort LHandVelShort;
        public ushort RHandVelShort;
#endregion
    }
}