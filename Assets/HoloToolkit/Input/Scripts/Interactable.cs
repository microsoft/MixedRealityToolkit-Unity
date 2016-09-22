// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Base class Interactable GameObjects can inherit from
    /// </summary>
    public class Interactable : MonoBehaviour
    {
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
        /// Called when a user has tapped any gameObject
        /// </summary>
        /// <param name="go"></param>
        protected virtual void OnTap(GameObject go)
        {
            if (go == gameObject)
            {
                //Do something if our gameObject has been tapped
            }
            else
            {
                //Do something if we've tapped any object except for our own
            }
        }

        /// <summary>
        /// Called when a user's gaze enters any gameObject
        /// </summary>
        /// <param name="go"></param>
        protected virtual void OnGazeEnter(GameObject go)
        {
            if( go == gameObject )
            {
                //Do something if our gaze has entered this gameObject
            }
            else
            {
                //Do something if our gaze has entered another gameObject
            }
        }

        /// <summary>
        /// Called when a user's gaze exits any gameObject
        /// </summary>
        /// <param name="go"></param>
        protected virtual void OnGazeExit(GameObject go)
        {
            if( go == gameObject )
            {
                //Do something if our gaze has left this gameObject
            }
            else
            {
                //Do something if our gaze has left another gameObject
            }
        }

        protected virtual void KeywordRecognized(string keyword)
        {
            if ( keyword.Equals( "String" ) )
            {
                //Do Something if our spoken keyword matches our string.
            }
        }
    }
}