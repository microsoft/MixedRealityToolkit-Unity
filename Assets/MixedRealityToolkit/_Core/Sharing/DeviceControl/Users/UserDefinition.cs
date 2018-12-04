using System;
using Pixie.Core;
using UnityEngine;

namespace Pixie.DeviceControl.Users
{
    [Serializable]
    public struct UserDefinition
    {
        public UserTeamEnum Team;
        public UserRoleEnum Role;
        public DeviceTypeEnum DeviceType;
        public TransformTypeEnum[] Transforms;
        public DeviceRoleEnum[] DeviceRoles;
        [Tooltip("Type must include full namespace.")]
        public string StandInClassType;
    }
}