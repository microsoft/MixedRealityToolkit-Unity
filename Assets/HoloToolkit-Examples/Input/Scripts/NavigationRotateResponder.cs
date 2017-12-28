using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// This is an example of how to use navigation gesture for a continuous rotation response.
    /// This class implements the INavigationHandler interface.
    /// It rotates the object along the Y axis ready the navigation X values.
    /// </summary>
    public class NavigationRotateResponder : MonoBehaviour, INavigationHandler
    {
        [Tooltip("Rotation sensitivity controls the amount of rotation.")]
        public float RotationSensitivity = 10.0f;

        private float rotationFactor = 0.0f;
        private Vector3 navigationDelta = Vector3.zero;
        
        private void Update()
        {
            PerformRotation();
        }

        private void PerformRotation()
        {
            if (navigationDelta == Vector3.zero)
            {
                return;
            }

            // This will help control the amount of rotation.
            // Taking the delta along the horizontal axis movement.
            rotationFactor = navigationDelta.x * RotationSensitivity;

            // Rotate object along the Y axis using.
            transform.Rotate(new Vector3(0, -1 * rotationFactor, 0));
        }

        public void OnNavigationCanceled(NavigationEventData eventData)
        {
            navigationDelta = Vector3.zero;
            InputManager.Instance.OverrideFocusedObject = null;
        }

        public void OnNavigationCompleted(NavigationEventData eventData)
        {
            navigationDelta = Vector3.zero;
            InputManager.Instance.OverrideFocusedObject = null;
        }

        public void OnNavigationStarted(NavigationEventData eventData)
        {
            InputManager.Instance.OverrideFocusedObject = gameObject;
            navigationDelta = eventData.NormalizedOffset;
        }

        public void OnNavigationUpdated(NavigationEventData eventData)
        {
            navigationDelta = eventData.NormalizedOffset;
        }
    }
}