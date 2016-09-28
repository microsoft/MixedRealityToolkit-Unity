// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Use this class for easy access to OnTap, OnGazeEnder, and OnGazeExit events.
    /// Be sure to override each of these methods with your own implimentation.
    /// Examples of code usage in each method below in comments.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            GestureManager.Instance.OnTap += OnTap;
            GazeManager.Instance.OnGazeExit += OnGazeExit;
            GazeManager.Instance.OnGazeEnter += OnGazeEnter;
        }

        protected virtual void OnDisable()
        {
            GestureManager.Instance.OnTap -= OnTap;
            GazeManager.Instance.OnGazeExit -= OnGazeExit;
            GazeManager.Instance.OnGazeEnter -= OnGazeEnter;
        }

        /// <summary>
        /// Called when a user has tapped any GameObject.
        /// </summary>
        /// <param name="tappedGameObject">GameObject user has tapped.</param>
        protected virtual void OnTap(GameObject tappedGameObject)
        {
            // Do something if ANY GameObject has been tapped.

            //if (tappedGameObject == gameObject)
            //{
            //    // Do something if this GameObject has been tapped.
            //}
            //else
            //{
            //    // Do something if we've tapped any object except for this one.
            //}
        }

        /// <summary>
        /// Called when a user's gaze enters any GameObject.
        /// </summary>
        /// <param name="focusedObject">The GameObject that the users gaze has entered.</param>
        protected virtual void OnGazeEnter(GameObject focusedObject)
        {
            // Do something if ANY GameObject has gained focus.

            //if (focusedObject == gameObject)
            //{
            //    // Do something if our gaze has entered this GameObject.
            //}
            //else
            //{
            //    // Do something if our gaze has entered another GameObject other than this one.
            //}
        }

        /// <summary>
        /// Called when a user's gaze exits any GameObject.
        /// </summary>
        /// <param name="focusedObject">The GameObject that the users gaze has left.</param>
        protected virtual void OnGazeExit(GameObject focusedObject)
        {
            // Do something if ANY GameObject has lost focus.

            //if(focusedObject == gameObject)
            //{
            //    // Do something if our gaze has left this GameObject.
            //}
            //else
            //{
            //    // Do something if our gaze has left another GameObject other than this one.
            //}
        }
    }
}