// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    public class Speech_VisualFeedback : MonoBehaviour, IMixedRealitySpeechHandler
    {
        #region Variable declarations

        [SerializeField]
        [Tooltip("Acts as the template which will show the speech command that the system understood.")]
        private GameObject visualFeedbackTemplate = null;

        [SerializeField]
        [Tooltip("The duration in seconds for which the visual feedback is shown.")]
        private float maxShowtimeInSeconds = 2.0f;

        private TextMesh myTextMesh;
        private DateTime startedTime;

        #endregion Variable declarations

        private TextMesh MyTextMesh
        {
            get
            {
                if (myTextMesh == null)
                {
                    myTextMesh = visualFeedbackTemplate.GetComponentInChildren<TextMesh>();
                }

                return myTextMesh;
            }
        }

        /// <summary>
        /// Update text to be displayed
        /// </summary>
        /// <param name="msg"></param>
        private void UpdateTextMesh(string msg)
        {
            if (MyTextMesh != null)
            {
                myTextMesh.text = "" + msg;
            }
        }

        /// <summary>
        /// This method handles displaying a given text message.
        /// </summary>
        /// <param name="msg"></param>
        public void ShowVisualFeedback(string msg)
        {
            // Start showing the visual feedback
            if (visualFeedbackTemplate != null)
            {
                // Update text to be displayed
                UpdateTextMesh(msg);

                if (MixedRealityToolkit.InputSystem.GazeProvider != null)
                {
                    // Show the visual feedback at 2m in the direction the user is looking
                    visualFeedbackTemplate.transform.position = CameraCache.Main.transform.position + MixedRealityToolkit.InputSystem.GazeProvider.GazeDirection.normalized * 2f;
                    visualFeedbackTemplate.transform.LookAt(CameraCache.Main.transform.position);
                }

                // Show it
                visualFeedbackTemplate.SetActive(true);

                // Start tracking the show time 
                startedTime = DateTime.Now;
            }
        }

        private void Update()
        {
            if ((visualFeedbackTemplate != null) && (visualFeedbackTemplate.activeSelf))
            {
                // Hide visual feedback once we showed it long enough
                if ((DateTime.Now - startedTime).TotalSeconds > maxShowtimeInSeconds)
                {
                    visualFeedbackTemplate.SetActive(false);
                }
            }
        }

        void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            ShowVisualFeedback(eventData.Command.Keyword);
        }
    }
}
