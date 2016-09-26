// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Base class Interactable GameObjects can inherit from.
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        protected bool TriggerKeywordOnGazeOnly;

        protected bool IsFocused;

        protected virtual void OnEnable()
        {
            GestureManager.Instance.OnTap += OnTap;
            GazeManager.Instance.OnGazeExit += OnGazeExit;
            GazeManager.Instance.OnGazeEnter += OnGazeEnter;
            KeywordManager.Instance.OnKeywordRecognized += KeywordRecognized;
        }

        protected virtual void OnDisable()
        {
            GestureManager.Instance.OnTap -= OnTap;
            GazeManager.Instance.OnGazeExit -= OnGazeExit;
            GazeManager.Instance.OnGazeEnter -= OnGazeEnter;
            KeywordManager.Instance.OnKeywordRecognized -= KeywordRecognized;
        }

        /// <summary>
        /// Called when a user has tapped any gameObject.
        /// </summary>
        /// <param name="tappedGameObject">GameObject user has tapped.</param>
        protected virtual void OnTap(GameObject tappedGameObject)
        {
            if (tappedGameObject == gameObject)
            {
                // Do something if our gameObject has been tapped
            }
            else
            {
                // Do something if we've tapped any object except for our own
            }
        }

        /// <summary>
        /// Called when a user's gaze enters any gameObject.
        /// </summary>
        /// <param name="focusedObject">The GameObject that the users gaze has entered.</param>
        protected virtual void OnGazeEnter(GameObject focusedObject)
        {
            if (focusedObject == gameObject)
            {
                IsFocused = true;
                // Do something if our gaze has entered this gameObject
            }
            else
            {
                // Do something if our gaze has entered another gameObject
            }
        }

        /// <summary>
        /// Called when a user's gaze exits any gameObject.
        /// </summary>
        /// <param name="focusedObject">The GameObject that the users gaze has left.</param>
        protected virtual void OnGazeExit(GameObject focusedObject)
        {
            if(focusedObject == gameObject)
            {
                IsFocused = false;
                // Do something if our gaze has left this gameObject
            }
            else
            {
                // Do something if our gaze has left another gameObject
            }
        }

        /// <summary>
        /// Called when the Keyword Manager recognizes a spoken keyword.
        /// </summary>
        /// <param name="keyword">Spoke Keyword.</param>
        protected virtual void KeywordRecognized(string keyword)
        {
            if (TriggerKeywordOnGazeOnly && !IsFocused)
            {
                return;
            }

            if (keyword.Equals("replace with your keyword"))
            {
                // Do Something if our spoken keyword matches our string.
            }
        }
    }
}