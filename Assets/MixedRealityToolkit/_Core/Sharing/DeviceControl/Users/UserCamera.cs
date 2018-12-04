using Pixie.CameraControl;
using UnityEngine;

namespace Pixie.DeviceControl.Users
{
    public class UserCamera : MonoBehaviour, IUserCamera
    {
        public bool HasCamera { get { return appCamera != null; } }
        public Transform CameraTransform { get { return appCamera.CameraTransform; } }

        private IAppCamera appCamera;
        private IUserTransforms userTransforms;
        private ICameraProfile profile;
        private Transform cameraParent;

        public void AssignProfile(ICameraProfile profile)
        {
            this.profile = profile;
        }

        public void AssignCamera(IAppCamera appCamera)
        {
            this.appCamera = appCamera;
        }
        
        private void OnEnable()
        {
            userTransforms = (IUserTransforms)gameObject.GetComponent(typeof(IUserTransforms));

            if (userTransforms == null)
                throw new System.Exception("This component requires component of type " + typeof(IUserCamera).Name + " to function!");
        }

        private void Update()
        {
            if (appCamera == null || profile == null)
                return;

            // We need to parent our camera under the camera parent
            // If none exists, there's nothing for us to do
            if (cameraParent == null && !userTransforms.GetTransform(Core.TransformTypeEnum.CameraParent, out cameraParent))
                return;

            appCamera.SetClearFlags(profile.Lighting.CameraClearFlags);
            appCamera.SetClearColor(profile.Lighting.CameraClearColor);
            appCamera.SetCullingMask(profile.CameraCullingMask);
            appCamera.SetParent(cameraParent);
        }
    }
}