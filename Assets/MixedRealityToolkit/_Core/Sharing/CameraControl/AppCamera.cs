using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.CameraControl
{
    public class AppCamera : MonoBehaviour, IAppCamera
    {
        [Serializable]
        private struct CameraSettings
        {
            public LayerMask CullingMask;
            public CameraClearFlags ClearFlags;
            public Color ClearColor;
            public Transform CameraParent;
        }

        public Transform CameraTransform { get { return camTransform; } }

        public CameraControlMode Mode { get { return mode; } set { mode = value; } }

        [SerializeField]
        private Camera baseCamera;
        [SerializeField]
        private AudioListener baseListener;
        [SerializeField]
        private CameraControlMode mode = CameraControlMode.User;
        [SerializeField]
        private CameraSettings appSettings;
        [SerializeField]
        private CameraSettings userSettings;

        private List<OverrideCamera> overrides = new List<OverrideCamera>();
        private OverrideCamera overrideCam;
        private Camera currentCam;
        private Transform baseTransform;
        private Transform camTransform;
        private Transform camParent;

        public void SetCullingMask(LayerMask cullingMask, CameraControlMode mode = CameraControlMode.User)
        {
            switch (mode)
            {
                case CameraControlMode.App:
                    appSettings.CullingMask = cullingMask;
                    break;

                case CameraControlMode.User:
                    userSettings.CullingMask = cullingMask;
                    break;
            }
        }

        public void SetClearFlags(CameraClearFlags clearFlags, CameraControlMode mode = CameraControlMode.User)
        {
            switch (mode)
            {
                case CameraControlMode.App:
                    appSettings.ClearFlags = clearFlags;
                    break;

                case CameraControlMode.User:
                    userSettings.ClearFlags = clearFlags;
                    break;
            }
        }

        public void SetClearColor(Color clearColor, CameraControlMode mode = CameraControlMode.User)
        {
            switch (mode)
            {
                case CameraControlMode.App:
                    appSettings.ClearColor = clearColor;
                    break;

                case CameraControlMode.User:
                    userSettings.ClearColor = clearColor;
                    break;
            }
        }

        public void SetParent(Transform cameraParent)
        {
            switch (mode)
            {
                case CameraControlMode.App:
                    appSettings.CameraParent = cameraParent;
                    break;

                case CameraControlMode.User:
                    userSettings.CameraParent = cameraParent;
                    break;
            }
        }

        public void AddOverride(OverrideCamera overrideCamera)
        {
            if (overrides.Contains(overrideCamera))
            {
                overrides.Remove(overrideCamera);
            }
            // Push to front of list
            overrides.Insert(0, overrideCamera);
        }

        public void RemoveOverride(OverrideCamera overrideCamera)
        {
            overrides.Remove(overrideCamera);
        }

        private void OnEnable()
        {
            if (!baseCamera.CompareTag("MainCamera"))
                throw new Exception("AppCamera must be main camera");
        }

        private void Update()
        {
            CameraSettings settings = appSettings;
            switch (mode)
            {
                case CameraControlMode.App:
                default:
                    break;

                case CameraControlMode.User:
                    settings = userSettings;
                    break;
            }

            currentCam = baseCamera;
            baseTransform = baseCamera.transform;
            camTransform = baseCamera.transform;
            camParent = settings.CameraParent;

            if (GetOverrideCamera(ref overrideCam))
            {
                baseCamera.enabled = false;
                baseListener.enabled = false;
                if (!baseCamera.CompareTag("Untagged"))
                    baseCamera.tag = "Untagged";

                currentCam = overrideCam.Camera;
                baseTransform = overrideCam.BaseTransform;
                camTransform = overrideCam.CameraTransform;
            }

            currentCam.enabled = true;
            currentCam.enabled = true;
            if (!currentCam.CompareTag("MainCamera"))
                currentCam.tag = "MainCamera";

            currentCam.clearFlags = settings.ClearFlags;
            currentCam.backgroundColor = settings.ClearColor;
            currentCam.cullingMask = settings.CullingMask;

            if (baseTransform.parent != settings.CameraParent)
            {
                baseTransform.parent = settings.CameraParent;
            }

        }

        private bool GetOverrideCamera(ref OverrideCamera cam)
        {
            // Clean up the list in case any cameras have been destroyed
            for (int i = overrides.Count - 1; i >= 0; i--)
            {
                if (overrides[i] == null)
                    overrides.RemoveAt(i);
            }

            if (overrides.Count > 0)
            {
                cam = overrides[0];
                // Turn off all other override cameras
                for (int i = 1; i < overrides.Count; i++)
                {
                    // Disable them and parent them under their original parents
                    overrides[i].Camera.enabled = false;
                    overrides[i].Listener.enabled = false;
                    overrides[i].BaseTransform.parent = overrides[i].OriginalParent;
                    if (!overrides[i].Camera.CompareTag("Untagged"))
                        overrides[i].Camera.tag = "Untagged";
                }
                return true;
            }

            return false;
        }
    }
}