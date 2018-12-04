using Pixie.Core;
using System;

namespace Pixie.DeviceControl
{
    [Serializable]
    public struct UserSlot : IItemState, IItemStateComparer<UserSlot>
    {
        public enum FillStateEnum : byte
        {
            Empty,
            Filled,
            Ignore,
        }

        public UserSlot(short key)
        {
            UserID = key;
            UserRole = UserRoleEnum.None;
            UserTeam = UserTeamEnum.None;
            DeviceType = DeviceTypeEnum.None;
            FillState = FillStateEnum.Empty;
            Transforms = new TransformTypeEnum[2] { TransformTypeEnum.Head, TransformTypeEnum.CameraParent };
            DeviceRoles = new DeviceRoleEnum[1] { DeviceRoleEnum.Primary };
        }

        public UserSlot(short key, UserRoleEnum role, UserTeamEnum team, DeviceTypeEnum deviceType, TransformTypeEnum[] transforms, DeviceRoleEnum[] deviceRoles)
        {
            UserID = key;
            UserRole = role;
            UserTeam = team;
            DeviceType = deviceType;
            FillState = FillStateEnum.Empty;
            Transforms = transforms;
            DeviceRoles = deviceRoles;
        }

        short IItemState.Key { get { return UserID; } }

        public short UserID;
        public UserRoleEnum UserRole;
        public UserTeamEnum UserTeam;
        public DeviceTypeEnum DeviceType;
        public FillStateEnum FillState;
        public TransformTypeEnum[] Transforms;
        public DeviceRoleEnum[] DeviceRoles;

        public bool IsDifferent(UserSlot from)
        {
            if (UserRole != from.UserRole
                || UserTeam != from.UserTeam
                || DeviceType != from.DeviceType
                || UserID != from.UserID
                || FillState != from.FillState)
                return true;

            if (Transforms.Length != from.Transforms.Length
                || DeviceRoles.Length != from.DeviceRoles.Length)
                return true;

            for (int i = 0; i < Transforms.Length; i++)
            {
                if (Transforms[i] != from.Transforms[i])
                    return true;
            }

            for (int i = 0; i < DeviceRoles.Length; i++)
            {
                if (DeviceRoles[i] != from.DeviceRoles[i])
                    return true;
            }

            return false;
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