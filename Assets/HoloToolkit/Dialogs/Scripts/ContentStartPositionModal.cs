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
    /// A ContentStartPositionModal can be used to allow the user to move to a position and place content in that position.
    /// 
    /// A ContentStartPositionModal covers a healthy portion of the screen and follows the users gaze(but doesnt screen lock).  It initially disables the content associated with it.
    /// It can be used to give important guidance to the user before allowing him/her to continue with the application.
    /// Once a user clicks on the ContentStartPositionModal, it will dissapear and allow the user to view the scene.Additionally you can set a flag to reposition the scene object relative to where the ContentStartPositionModal was dismissed.  Some have used it as a type of splash screen before showing a initial set of menus or content.
    /// 
    /// ContentStartPositionModal has three properties that can be set in the editor.
    ///     bool MoveCollectionOnDismiss
    ///     GameObject StartupObject
    ///     float Distance 
    ///     
    /// StartupObject refers to The scene object to activate and possibly reposition
    /// MoveCollectionOnDismiss toggles wether you want to reposition the StartupObject to the place where the ContentStartPositionModal was clicked on (defaults to false) 
    /// ContentStartPositionModalDistance refers to the distance at which you want the ContentStartPositionModal, and subsequent content, to display in front of the user. This is fed to the interpolator. (defaults to 1.0f )
    /// ContentDistance refers to The distance at which you want the Content, relative to the ContentStartPositionModal, if MoveCollectionOnDismiss selected.
    /// 
    /// ContentStartPositionModal requires an Interpolator, A Holotoolkit MonoBehaviour that interpolates a transform's position, rotation or scale.
    /// and helps the ContentStartPositionModal smoothly stay in front of the user wherever he/she moves
    /// 
    /// A ContentStartPositionModal also has a child object Quad whose material can be set to a logo or some image representing guidance to the user
    /// You could replace this with your own child objects.
    /// 
    /// If the user moves anywhere the ContentStartPositionModal will stay at the same distance set in front of the user and this is where the followup content may be moved to if MoveCollectionOnDismiss is set to true
    ///
    /// </summary>
    public class ContentStartPositionModal : MonoBehaviour, IInputClickHandler
    {
        [Tooltip("Reposition the scene object relative to where the ContentStartPositionModal was dismissed.")]
        public bool MoveCollectionOnDismiss = false;
        [Tooltip("The scene object to activate and reposition.")]
        public GameObject StartupObject;
        [Tooltip("The distance at which you want the ContentStartPositionModal, ")]
        public float ContentStartPositionModalDistance = 1.0f;

        [Tooltip("The distance at which you want the Content, relative to the ContentStartPositionModal, if MoveCollectionOnDismiss selected.")]
        public float ContentDistance = 2.0f;


        private Interpolator interpolator;
        // The offset from the Camera to the StartupObject when
        // the app starts up. This is used to place the StartupObject
        // in the correct relative position after the ContentStartPositionModal is
        // dismissed.
        private Vector3 collectionStartingOffsetFromCamera;

        private void Start()
        {
            // This is the object to show when the ContentStartPositionModal is dismissed
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

            interpolator.SetTargetPosition(cameraTransform.position + (cameraTransform.forward * ContentStartPositionModalDistance));
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
                    // where the ContentStartPositionModal left off. Start with the camera's localRotation...
                    Quaternion camQuat = Camera.main.transform.localRotation;

                    // ... ignore pitch by disabling rotation around the x axis
                    camQuat.x = 0;

                    // Rotate the vector and factor y back into the position
                    Vector3 newPosition = camQuat * collectionStartingOffsetFromCamera;
                    newPosition.y = collectionStartingOffsetFromCamera.y;

                    Transform cameraTransform = Camera.main.transform;

                    // Position was "Local Position" so add that to where the camera is now
                    //StartupObject.transform.position = Camera.main.transform.position + newPosition;
                    StartupObject.transform.position = cameraTransform.position + (cameraTransform.forward * ContentDistance);

                    // Rotate the Hologram Collection to face the user.
                    Quaternion toQuat = Camera.main.transform.localRotation * StartupObject.transform.rotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    StartupObject.transform.rotation = toQuat;
                }
            }
            // Destroy the ContentStartPositionModal
            Destroy(gameObject);

            eventData.Use();
        }
    }
}



