using Pixie.Initialization;
using UnityEngine;

namespace Pixie.CameraControl
{
    public class OverrideCamera : MonoBehaviour
    {
        public Camera Camera { get { return cam; } }
        public AudioListener Listener { get { return listener; } }
        public Transform OriginalParent { get { return originalParent; } }
        public Transform CameraTransform { get { return cam.transform; } }
        public Transform BaseTransform { get { return baseTransformOverride != null ? baseTransformOverride : cam.transform; } }

        private IAppCamera appCamera;
        private Camera cam;
        private AudioListener listener;
        private bool active;

        [SerializeField]
        private Transform baseTransformOverride;
        private Transform originalParent;
        
        private void OnEnable()
        {
            cam = GetComponent<Camera>();
            listener = GetComponent<AudioListener>();
            originalParent = (baseTransformOverride != null) ? baseTransformOverride.parent : cam.transform.parent;

            // If an app camera doesn't exist, just disable ourselves
            IAppCamera appCamera;
            if (ComponentFinder.FindInScenes<IAppCamera>(out appCamera))
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