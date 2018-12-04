using Pixie.Core;
using UnityEngine;

namespace Pixie.DeviceControl
{
    public interface IUserObject : IGameObject
    {
        short UserID { get; }
        UserRoleEnum UserRole { get; }
        UserTeamEnum UserTeam { get; }

        bool IsAssigned { get; }
        bool Simulated { get; }
        bool IsDestroyed { get; }
        bool IsLocalUser { get; }

        Transform SceneAlignment { get; }
        Transform SceneOffset { get; }

        void AssignUser(UserSlot slot);
        void AssignLocalUser(bool isLocalUser);
    }
}