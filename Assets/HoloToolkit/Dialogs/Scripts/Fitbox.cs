using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HoloToolkit.Unity.Dialogs
{
    // Used to place the scene origin on startup
    // Adapted from Holoacadamy's fitbox
    public class Fitbox : MonoBehaviour, IInputClickHandler
    {
        [Tooltip("Reposition the scene object relative to where the Fitbox was dismissed.")]
        public bool MoveCollectionOnDismiss = false;
        [Tooltip("The scene object to activate and reposition.")]
        public GameObject StartupObject;

        private float Distance = 1.0f;
        private Interpolator interpolator;
        // The offset from the Camera to the StartupObject when
        // the app starts up. This is used to place the StartupObject
        // in the correct relative position after the Fitbox is
        // dismissed.
        private Vector3 collectionStartingOffsetFromCamera;

        private void Start()
        {
            // This is the object to show when the Fitbox is dismissed
            if (StartupObject != null)
            {
                collectionStartingOffsetFromCamera = StartupObject.transform.localPosition;
                StartupObject.SetActive(false);
            }
            interpolator = GetComponent<Interpolator>();
            interpolator.PositionPerSecond = 2f;
        }

        void LateUpdate()
        {
            Transform cameraTransform = Camera.main.transform;

            interpolator.SetTargetPosition(cameraTransform.position + (cameraTransform.forward * Distance));
            interpolator.SetTargetRotation(Quaternion.LookRotation(-cameraTransform.forward, -cameraTransform.up));
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            // Show the startup object
            if (StartupObject != null)
            {
                StartupObject.SetActive(true);

                if (MoveCollectionOnDismiss)
                {
                    // Update the Hologram Collection's position so it shows up
                    // where the Fitbox left off. Start with the camera's localRotation...
                    Quaternion camQuat = Camera.main.transform.localRotation;

                    // ... ignore pitch by disabling rotation around the x axis
                    camQuat.x = 0;

                    // Rotate the vector and factor y back into the position
                    Vector3 newPosition = camQuat * collectionStartingOffsetFromCamera;
                    newPosition.y = collectionStartingOffsetFromCamera.y;

                    // Position was "Local Position" so add that to where the camera is now
                    StartupObject.transform.position = Camera.main.transform.position + newPosition;

                    // Rotate the Hologram Collection to face the user.
                    Quaternion toQuat = Camera.main.transform.localRotation * StartupObject.transform.rotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    StartupObject.transform.rotation = toQuat;
                }
            }
            // Destroy the Fitbox
            Destroy(gameObject);
        }
    }
}

