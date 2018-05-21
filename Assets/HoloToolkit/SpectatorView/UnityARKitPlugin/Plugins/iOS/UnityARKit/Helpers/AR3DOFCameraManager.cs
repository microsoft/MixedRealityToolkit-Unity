
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class AR3DOFCameraManager : MonoBehaviour {

    public Camera m_camera;
    private UnityARSessionNativeInterface m_session;
    private Material savedClearMaterial;

    // Use this for initialization
    void Start () {
#if !UNITY_EDITOR
        Application.targetFrameRate = 60;
        m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
        ARKitSessionConfiguration config = new ARKitSessionConfiguration();
        config.alignment = UnityARAlignment.UnityARAlignmentGravity;
        config.getPointCloudData = true;
        config.enableLightEstimation = true;
        m_session.RunWithConfig(config);

        if (m_camera == null) {
            m_camera = Camera.main;
        }
#endif
    }

    public void SetCamera(Camera newCamera)
    {
        if (m_camera != null) {
            UnityARVideo oldARVideo = m_camera.gameObject.GetComponent<UnityARVideo> ();
            if (oldARVideo != null) {
                savedClearMaterial = oldARVideo.m_ClearMaterial;
                Destroy (oldARVideo);
            }
        }
        SetupNewCamera (newCamera);
    }

    private void SetupNewCamera(Camera newCamera)
    {
        m_camera = newCamera;

        if (m_camera != null) {
            UnityARVideo unityARVideo = m_camera.gameObject.GetComponent<UnityARVideo> ();
            if (unityARVideo != null) {
                savedClearMaterial = unityARVideo.m_ClearMaterial;
                Destroy (unityARVideo);
            }
            unityARVideo = m_camera.gameObject.AddComponent<UnityARVideo> ();
            unityARVideo.m_ClearMaterial = savedClearMaterial;
        }
    }

    // Update is called once per frame

#if !UNITY_EDITOR
    void Update () {

        if (m_camera != null)
        {
            // JUST WORKS!
            Matrix4x4 matrix = m_session.GetCameraPose();
            m_camera.transform.localPosition = UnityARMatrixOps.GetPosition(matrix);
            m_camera.transform.localRotation = UnityARMatrixOps.GetRotation (matrix);
            m_camera.projectionMatrix = m_session.GetCameraProjection ();
        }

    }
#endif

}
