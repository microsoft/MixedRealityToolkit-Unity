using UnityEngine;
using UnityEngine.XR.iOS;

namespace ARKit.Utils
{
    [RequireComponent(typeof(Camera))]
    public class UnityARCameraNearFar : MonoBehaviour
    {
        private Camera attachedCamera;
        private float currentNearZ;
        private float currentFarZ;

        private void Start()
        {
            attachedCamera = GetComponent<Camera>();
            UpdateCameraClipPlanes();
        }

        private void UpdateCameraClipPlanes()
        {
            currentNearZ = attachedCamera.nearClipPlane;
            currentFarZ = attachedCamera.farClipPlane;
            UnityARSessionNativeInterface.GetARSessionNativeInterface().SetCameraClipPlanes(currentNearZ, currentFarZ);
        }

        private void Update()
        {
            if (!currentNearZ.Equals(attachedCamera.nearClipPlane) || !currentFarZ.Equals(attachedCamera.farClipPlane))
            {
                UpdateCameraClipPlanes();
            }
        }
    }
}