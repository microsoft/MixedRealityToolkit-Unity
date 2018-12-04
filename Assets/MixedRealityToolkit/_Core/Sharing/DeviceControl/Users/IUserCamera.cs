using Pixie.CameraControl;
using UnityEngine;

namespace Pixie.DeviceControl.Users
{
    public interface IUserCamera
    {
        bool HasCamera { get; }
        Transform CameraTransform { get; }
        void AssignProfile(ICameraProfile profile);
        void AssignCamera(IAppCamera appCamera);
    }
}