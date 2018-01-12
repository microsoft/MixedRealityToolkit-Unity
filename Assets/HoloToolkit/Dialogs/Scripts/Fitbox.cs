using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HoloToolkit.Unity.Dialogs
{

    /// <summary>
    /// 
    /// Used to place the scene origin on startup
    /// 
    /// A Fitbox can be used to allow the user to move to a position and place content in that position.
    /// 
    /// A Fitbox covers a healthy portion of the screen and follows the users gaze(but doesnt screen lock).  It initially disables the content associated with it.
    /// It can be used to give important guidance to the user before allowing him/her to continue with the application.
    /// Once a user clicks on the Fitbox, it will dissapear and allow the user to view the scene.Additionally you can set a flag to reposition the scene object relative to where the Fitbox was dismissed.  Some have used it as a type of splash screen before showing a initial set of menus or content.
    /// 
    /// Fitbox has three properties that can be set in the editor.
    ///     bool MoveCollectionOnDismiss
    ///     GameObject StartupObject
    ///     float Distance 
    ///     
    /// StartupObject refers to The scene object to activate and possibly reposition
    /// MoveCollectionOnDismiss toggles wether you want to reposition the StartupObject to the place where the Fitbox was clicked on (defaults to false) 
    /// Distance refers to the distance at which you want the Fitbox, and subsequent content, to display in front of the user. This is fed to the interpolator. (defaults to 1.0f )
    /// 
    /// Fitbox requires an Interpolator, A Holotoolkit MonoBehaviour that interpolates a transform's position, rotation or scale.
    /// and helps the fitbox smoothly stay in front of the user wherever he/she moves
    /// 
    /// A FitBox also has a child object Quad whose material can be set to a logo or some image representing guidance to the user
    /// You could replace this with your own child objects.
    /// 
    /// If the user moves anywhere the fitbox will stay at the same distance set in front of the user and this is where the followup content may be moved to if MoveCollectionOnDismiss is set to true
    ///
    /// </summary>
    public class Fitbox : MonoBehaviour, IInputClickHandler
    {
        [Tooltip("Reposition the scene object relative to where the Fitbox was dismissed.")]
        public bool MoveCollectionOnDismiss = false;
        [Tooltip("The scene object to activate and reposition.")]
        public GameObject StartupObject;
        [Tooltip("The distance at which you want the Fitbox, and subsequent content display in front of the user.")]
        public float Distance = 1.0f;
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

            eventData.Use();
        }
    }
}

