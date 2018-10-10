using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.CameraControl
{
    public class AppCamera : MonoBehaviour, IAppCamera
    {
        [Serializable]
        private struct CameraSettings
        {
            public LayerMask CullingMask;
            public CameraClearFlags ClearFlags;
            public Color ClearColor;
            public Transform Parent;
        }

        public Transform CameraTransform { get { return currentCam.transform; } }
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
        private Camera currentCam;
        private AudioListener currentListener;

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
                    appSettings.Parent = cameraParent;
                    break;

                case CameraControlMode.User:
                    userSettings.Parent = cameraParent;
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
            currentCam = baseCamera;
            currentListener = baseListener;

            if (GetOverrideCamera(ref currentCam, ref currentListener))
            {
                baseCamera.enabled = false;
                baseCamera.tag = "Untagged";
                baseListener.enabled = false;
            }

            currentCam.tag = "MainCamera";
            currentCam.enabled = true;
            currentListener.enabled = true;

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

            currentCam.clearFlags = settings.ClearFlags;
            currentCam.backgroundColor = settings.ClearColor;
            currentCam.cullingMask = settings.CullingMask;

            if (currentCam.transform.parent != settings.Parent)
            {
                currentCam.transform.parent = settings.Parent;
            }

        }

        private bool GetOverrideCamera(ref Camera cam, ref AudioListener listener)
        {
            // Clean up the list in case any cameras have been destroyed
            for (int i = overrides.Count - 1; i >= 0; i--)
            {
                if (overrides[i] == null)
                    overrides.RemoveAt(i);
            }

            if (overrides.Count > 0)
            {
                cam = overrides[0].Camera;
                listener = overrides[0].Listener;
                // Turn off all other override cameras
                for (int i = 1; i < overrides.Count; i++)
                {
                    // Disable them and parent them under their original parents
                    overrides[i].Camera.enabled = false;
                    overrides[i].Camera.tag = "Untagged";
                    overrides[i].Listener.enabled = false;
                    overrides[i].Camera.transform.parent = overrides[i].CameraParent;
                }
                return true;
            }

            return false;
        }
    }
}