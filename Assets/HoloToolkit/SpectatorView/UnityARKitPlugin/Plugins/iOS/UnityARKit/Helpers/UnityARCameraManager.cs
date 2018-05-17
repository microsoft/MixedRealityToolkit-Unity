using UnityEngine;
using UnityEngine.XR.iOS;

namespace ARKit.Utils
{
    public class UnityARCameraManager : MonoBehaviour
    {
        public Camera m_camera;
        private UnityARSessionNativeInterface m_session;
        private Material savedClearMaterial;

        [Header("AR Config Options")] public UnityARAlignment startAlignment = UnityARAlignment.UnityARAlignmentGravity;
        public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.Horizontal;
        public bool getPointCloud = true;
        public bool enableLightEstimation = true;

        private void Start()
        {

            m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

            if (Application.isEditor)
            {
                //put some defaults so that it doesn't complain
                var arCamera = new UnityARCamera
                {
                    worldTransform = new UnityARMatrix4x4(
                        new Vector4(1, 0, 0, 0),
                        new Vector4(0, 1, 0, 0),
                        new Vector4(0, 0, 1, 0),
                        new Vector4(0, 0, 0, 1))
                };

                Matrix4x4 projMat = Matrix4x4.Perspective(60.0f, 1.33f, 0.1f, 30.0f);
                arCamera.projectionMatrix = new UnityARMatrix4x4(projMat.GetColumn(0), projMat.GetColumn(1),
                    projMat.GetColumn(2), projMat.GetColumn(3));

                UnityARSessionNativeInterface.SetStaticCamera(arCamera);
            }
            else
            {
                Application.targetFrameRate = 60;
                var config = new ARKitWorldTrackingSessionConfiguration
                {
                    planeDetection = planeDetection,
                    alignment = startAlignment,
                    getPointCloudData = getPointCloud,
                    enableLightEstimation = enableLightEstimation
                };

                m_session.RunWithConfig(config);

                if (m_camera == null)
                {
                    m_camera = Camera.main;
                }
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