using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.CameraControl
{
    public interface IAppCamera
    {
        CameraControlMode Mode { get; set; }
        Transform CameraTransform { get; }

        void SetCullingMask(LayerMask cullingMask, CameraControlMode mode = CameraControlMode.User);
        void SetClearFlags(CameraClearFlags clearFlags, CameraControlMode mode = CameraControlMode.User);
        void SetClearColor(Color clearColor, CameraControlMode mode = CameraControlMode.User);
        void SetParent(Transform cameraParent);
        void AddOverride(OverrideCamera overrideCamera);
        void RemoveOverride(OverrideCamera overrideCamera);
    }
}