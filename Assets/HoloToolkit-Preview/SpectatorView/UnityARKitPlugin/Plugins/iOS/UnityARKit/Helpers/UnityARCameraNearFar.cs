using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

[RequireComponent(typeof(Camera))]
public class UnityARCameraNearFar : MonoBehaviour {

    private Camera attachedCamera;
    private float currentNearZ;
    private float currentFarZ;

#if UNITY_IOS || UNITY_EDITOR
    // Use this for initialization
    void Start () 
    {
        attachedCamera = GetComponent<Camera> ();
        UpdateCameraClipPlanes ();
    }

    void UpdateCameraClipPlanes()
    {
        currentNearZ = attachedCamera.nearClipPlane;
        currentFarZ = attachedCamera.farClipPlane;
        UnityARSessionNativeInterface.GetARSessionNativeInterface ().SetCameraClipPlanes (currentNearZ, currentFarZ);
    }
    
    // Update is called once per frame
    void Update () {
        if (currentNearZ != attachedCamera.nearClipPlane || currentFarZ != attachedCamera.farClipPlane) {
            UpdateCameraClipPlanes ();
        }
    }
#endif
}
