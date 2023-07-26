// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// This script controls the UI state of the user recording and playback control system.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputRecorderUIController")]
    public class UserInputRecorderUIController : MonoBehaviour
    {
        [Tooltip("Root GameObject that allows the user to start a new recording")]
        [SerializeField]
        private GameObject buttonStartRecording = null;

        [Tooltip("Root GameObject that allows the user to stop a recording")]
        [SerializeField]
        private GameObject buttonStopRecording = null;

        [Tooltip("Root GameObject that allows the user to start playback of a log file")]
        [SerializeField]
        private GameObject buttonStartPlayback = null;

        [Tooltip("Root GameObject that allows the user to pause a recording")]
        [SerializeField]
        private GameObject buttonPausePlayback = null;

        [Tooltip("Root GameObject that allows the user to resume a recording")]
        [SerializeField]
        private GameObject buttonResumePlayback = null;

        private void Start()
        {
            RecordingUI_Reset(true);
            ShowStartButton();
        }

        #region Data recording
        /// <summary>
        /// Updates the control UI when the user starts recording a new eye gaze log file.
        /// </summary>
        public void StartRecording()
        {
            RecordingUI_Reset(false);
        }

        /// <summary>
        /// Updates the control UI when the user finishes recording a new eye gaze log file.
        /// </summary>
        public void StopRecording()
        {
            RecordingUI_Reset(true);
        }

        private void RecordingUI_Reset(bool reset)
        {
            if (buttonStopRecording != null)
            {
                buttonStopRecording.SetActive(!reset);
            }

            if (buttonStartRecording != null)
            {
                buttonStartRecording.SetActive(reset);
            }
        }
        #endregion

        #region Data replay
        /// <summary>
        /// Updates the control UI with the initial state, and when the user completes playback of a eye gaze log file.
        /// </summary>
        public void ShowStartButton()
        {
            StopRecording();
            SetPlaybackButtons(true, false, false);
        }

        /// <summary>
        /// Updates the control UI when the user starts playback of a eye gaze log file.
        /// </summary>
        public void StartReplay()
        {
            Debug.Log("StartReplay");
            SetPlaybackButtons(false, true, false);
        }

        /// <summary>
        /// Updates the control UI when the user pauses playback of a eye gaze log file.
        /// </summary>
        public void PauseReplay()
        {
            Debug.Log("PauseReplay");
            SetPlaybackButtons(false, false, true);
        }

        /// <summary>
        /// Updates the control UI when the user resumes playback of a eye gaze log file.
        /// </summary>
        public void ResumeReplay()
        {
            Debug.Log("ResumePlayback");
            SetPlaybackButtons(false, true, false);
        }
        
        private void SetPlaybackButtons(bool showStartButton, bool showPauseButton, bool showResumeButton)
        {
            if (buttonStartPlayback != null)
            {
                buttonStartPlayback.SetActive(showStartButton);
            }

            if (buttonPausePlayback != null)
            {
                buttonPausePlayback.SetActive(showPauseButton);
            }

            if (buttonResumePlayback != null)
            {
                buttonResumePlayback.SetActive(showResumeButton);
            }
        }
        #endregion
    }
}
