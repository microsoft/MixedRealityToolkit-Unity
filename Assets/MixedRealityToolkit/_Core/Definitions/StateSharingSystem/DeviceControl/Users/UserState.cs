using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    [Serializable]
    public struct UserState : IItemState<UserState>
    {
        const float MaxPositionRange = 10;
        const float MaxObjectVelocity = 2;

        public UserState(sbyte sessionNum, sbyte userNum)
        {
            ItemNum = userNum;
            SessionNum = sessionNum;
            UserRole = UserRoleEnum.None;
            UserTeam = UserTeamEnum.None;
            UserDevice = UserDeviceEnum.Unknown;

            HeadPosX = 0;
            HeadPosY = 0;
            HeadPosZ = 0;

            HeadDirX = 0;
            HeadDirY = 0;
            HeadDirZ = 1;

            HeadVelShort = 0;

            TimeStamp = 0;
        }

        public UserState(sbyte sessionNum, sbyte userNum, UserRoleEnum userRole, UserTeamEnum userTeam)
        {
            ItemNum = userNum;
            SessionNum = sessionNum;
            UserRole = userRole;
            UserTeam = userTeam;
            UserDevice = UserDeviceEnum.Unknown;

            HeadPosX = 0;
            HeadPosY = 0;
            HeadPosZ = 0;

            HeadDirX = 0;
            HeadDirY = 0;
            HeadDirZ = 1;

            HeadVelShort = 0;

            TimeStamp = 0;
        }

        sbyte IItemState<UserState>.Key { get { return ItemNum; } }
        sbyte IItemState<UserState>.Filter { get { return SessionNum; } }

        public sbyte ItemNum;
        public sbyte SessionNum;
        public UserRoleEnum UserRole;
        public UserTeamEnum UserTeam;
        public UserDeviceEnum UserDevice;
        public float TimeStamp;

        public Vector3 HeadPos
        {
            get { return StateUtils.ShortPosOut(HeadPosX, HeadPosY, HeadPosZ, MaxPositionRange); }
            set { StateUtils.ShortPosIn(value, out HeadPosX, out HeadPosY, out HeadPosZ, MaxPositionRange); }
        }

        public Vector3 HeadDir
        {
            get { return StateUtils.ShortDirOut(HeadDirX, HeadDirY, HeadDirZ); }
            set { StateUtils.ShortDirIn(value, out HeadDirX, out HeadDirY, out HeadDirZ); }
        }

        public float HeadVel
        {
            get { return StateUtils.UShortValOut(HeadVelShort, MaxObjectVelocity); }
            set { HeadVelShort = StateUtils.UShortValIn(value, MaxObjectVelocity); }
        }

        public bool IsDifferent(UserState from)
        {
            return ItemNum != from.ItemNum
                || SessionNum != from.SessionNum
                || UserRole != from.UserRole
                || UserTeam != from.UserTeam

                || HeadPosX != from.HeadPosX
                || HeadPosY != from.HeadPosY
                || HeadPosZ != from.HeadPosZ

                || HeadDirX != from.HeadDirX
                || HeadDirY != from.HeadDirY
                || HeadDirZ != from.HeadDirZ

                || HeadVelShort != from.HeadVelShort;
        }

        public UserState Merge(UserState clientValue, UserState serverValue)
        {
            return serverValue;
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }

        #region raw values
        public short HeadPosX;
        public short HeadPosY;
        public short HeadPosZ;

        public short HeadDirX;
        public short HeadDirY;
        public short HeadDirZ;

        public ushort HeadVelShort;
#endregion
    }
}