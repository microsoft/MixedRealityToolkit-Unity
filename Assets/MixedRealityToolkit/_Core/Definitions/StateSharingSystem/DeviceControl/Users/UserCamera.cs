using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.CameraControl;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public interface IUserCamera
    {
        bool HasCamera { get; }
        Transform CameraTransform { get; }

        void AssignProfile(UserProfile profile);
        void AssignCamera(IAppCamera appCamera);
    }

    public class UserCamera : MonoBehaviour, IUserCamera
    {
        public bool HasCamera { get { return appCamera != null; } }
        public Transform CameraTransform { get { return appCamera.CameraTransform; } }

        private IAppCamera appCamera;
        private ILocalUserObject localUser;
        private UserProfile profile;

        public void AssignProfile(UserProfile profile)
        {
            this.profile = profile;
        }

        public void AssignCamera(IAppCamera appCamera)
        {
            this.appCamera = appCamera;
        }
        
        private void OnEnable()
        {
            localUser = (ILocalUserObject)gameObject.GetComponent(typeof(ILocalUserObject));
        }

        private void Update()
        {
            if (appCamera == null || profile == null)
                return;

            appCamera.SetClearFlags(profile.Lighting.CameraClearFlags);
            appCamera.SetClearColor(profile.Lighting.CameraClearColor);
            appCamera.SetCullingMask(profile.CameraCullingMask);
            appCamera.SetParent(localUser.CameraParent);
        }
    }
}