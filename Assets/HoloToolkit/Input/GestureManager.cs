using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager creates a gesture recognizer and signs up for a tap gesture.
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// GestureManager then sends a message to that game object.
    /// </summary>
    [RequireComponent(typeof(GazeManager))]
    public class GestureManager : Singleton<GestureManager>
    {
        private GestureRecognizer gestureRecognizer;
        private GameObject focusedObject;

        void Start()
        {
            // Create a new GestureRecognizer.  Sign up for tapped events.
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

            // Start looking for gestures.
            gestureRecognizer.StartCapturingGestures();
        }

        private void GestureRecognizer_TappedEvent(SourceKind source, Ray headRay)
        {
            if (focusedObject != null)
            {
                focusedObject.SendMessage("OnSelect");
            }
        }

        void Update()
        {
            GameObject oldFocusedObject = focusedObject;

            if (GazeManager.Instance.Hit)
            {
                // If gaze hits a hologram, set the focused object to that game object.
                focusedObject = GazeManager.Instance.HitInfo.collider.gameObject;
            }
            else
            {
                // If our gaze doesn't hit a hologram, set the focused object to null.
                focusedObject = null;
            }

            if (focusedObject != oldFocusedObject)
            {
                // If the currently focused object doesn't match the old focused object, cancel the current gesture.
                // Start looking for new gestures.  This is to prevent applying gestures from one hologram to another.
                gestureRecognizer.CancelGestures();
                gestureRecognizer.StartCapturingGestures();
            }
        }

        void OnDestroy()
        {
            gestureRecognizer.StopCapturingGestures();
            gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;
        }
    }
}