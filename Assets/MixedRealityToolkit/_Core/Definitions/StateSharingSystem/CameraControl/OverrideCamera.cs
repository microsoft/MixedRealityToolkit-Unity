using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.CameraControl
{
    public class OverrideCamera : MonoBehaviour
    {
        public Camera Camera { get { return cam; } }
        public AudioListener Listener { get { return listener; } }
        public Transform CameraParent { get { return transform.parent; } }

        private IAppCamera appCamera;
        private Camera cam;
        private AudioListener listener;
        private bool active;
        
        private void OnEnable()
        {
            cam = GetComponent<Camera>();
            listener = GetComponent<AudioListener>();

            // If an app camera doesn't exist, just disable ourselves
            IAppCamera appCamera;
            if (SceneScraper.FindInScenes<IAppCamera>(out appCamera))
            {
                appCamera.AddOverride(this);
            }
            else
            {
                enabled = false;
            }
        }

        private void OnDisable()
        {
            if (appCamera != null)
            {
                appCamera.RemoveOverride(this);
            }
        }
    }
}