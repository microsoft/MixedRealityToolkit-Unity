using UnityEngine;
using UnityEngine.XR.iOS;

namespace ARKit.Utils
{
    public class AR3DOFCameraManager : MonoBehaviour
    {
        public Camera m_camera;
        private UnityARSessionNativeInterface m_session;
        private Material savedClearMaterial;

        private void Start()
        {
            if (Application.isEditor) { return; }

            Application.targetFrameRate = 60;
            m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
            var config = new ARKitSessionConfiguration
            {
                alignment = UnityARAlignment.UnityARAlignmentGravity,
                getPointCloudData = true,
                enableLightEstimation = true
            };

            m_session.RunWithConfig(config);

            if (m_camera == null)
            {
                m_camera = Camera.main;
            }
        }

        public void SetCamera(Camera newCamera)
        {
            if (m_camera != null)
            {
                var oldARVideo = m_camera.gameObject.GetComponent<UnityARVideo>();
                if (oldARVideo != null)
                {
                    savedClearMaterial = oldARVideo.m_ClearMaterial;
                    Destroy(oldARVideo);
                }
            }

            SetupNewCamera(newCamera);
        }

        private void SetupNewCamera(Camera newCamera)
        {
            m_camera = newCamera;

            if (m_camera != null)
            {
                var unityARVideo = m_camera.gameObject.GetComponent<UnityARVideo>();
                if (unityARVideo != null)
                {
                    savedClearMaterial = unityARVideo.m_ClearMaterial;
                    Destroy(unityARVideo);
                }

                unityARVideo = m_camera.gameObject.AddComponent<UnityARVideo>();
                unityARVideo.m_ClearMaterial = savedClearMaterial;
            }
        }

        private void Update()
        {
            if (m_camera != null)
            {
                // JUST WORKS!
                Matrix4x4 matrix = m_session.GetCameraPose();
                m_camera.transform.localPosition = UnityARMatrixOps.GetPosition(matrix);
                m_camera.transform.localRotation = UnityARMatrixOps.GetRotation(matrix);
                m_camera.projectionMatrix = m_session.GetCameraProjection();
            }
        }
    }
}