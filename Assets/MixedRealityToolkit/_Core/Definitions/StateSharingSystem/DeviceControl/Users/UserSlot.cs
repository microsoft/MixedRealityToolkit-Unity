using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    [Serializable]
    public struct UserSlot : IItemState<UserSlot>
    {
        public enum FillStateEnum : byte
        {
            Empty,
            Filled,
            Ignore,
        }

        public UserSlot(sbyte userNum, sbyte sessionNum)
        {
            ItemNum = userNum;
            SessionNum = sessionNum;
            UserRole = UserRoleEnum.None;
            UserTeam = UserTeamEnum.None;
            FillState = FillStateEnum.Empty;
        }

        public UserSlot(sbyte userNum, sbyte sessionNum, UserRoleEnum role, UserTeamEnum team)
        {
            ItemNum = userNum;
            SessionNum = sessionNum;
            UserRole = role;
            UserTeam = team;
            FillState = FillStateEnum.Empty;
        }

        sbyte IItemState<UserSlot>.Key { get { return ItemNum; } }
        sbyte IItemState<UserSlot>.Filter { get { return SessionNum; } }

        public sbyte ItemNum;
        public sbyte SessionNum;
        public UserRoleEnum UserRole;
        public UserTeamEnum UserTeam;
        public FillStateEnum FillState;
                
        public bool IsDifferent(UserSlot from)
        {
            return UserRole != from.UserRole
                || ItemNum != from.ItemNum
                || FillState != from.FillState;
        }

        public UserSlot Merge(UserSlot clientValue, UserSlot serverValue)
        {
            return serverValue;
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }
    }
}