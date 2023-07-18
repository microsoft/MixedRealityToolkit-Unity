// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using TMPro;
using UnityEngine;

// Disable "missing XML comment" warning for sample. While nice to have, this documentation is not required for samples.
#pragma warning disable CS1591

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// This script displays on screen status updates user recording and playback control system.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputRecorderFeedback")]
    public class UserInputRecorderFeedback : MonoBehaviour
    {
        [Tooltip("Textfield that displays status updates")]
        [SerializeField]
        private TextMeshPro statusText = null;

        [Tooltip("The duration an update will be displayed before it is hidden")]
        [SerializeField]
        private float maxShowDurationInSeconds = 2f;

        private bool isShowingSomething = false;
        private DateTime startShowTime;

        private void UpdateStatusText(string msg)
        {
            if (statusText != null)
            {
                statusText.text = msg;
                statusText.gameObject.SetActive(true);
                isShowingSomething = true;
                startShowTime = DateTime.Now;
            }
        }

        private void ResetStatusText()
        {
            if (statusText != null)
            {
                statusText.gameObject.SetActive(false);
                statusText.text = "";
                isShowingSomething = false;
            }
        }

        private void Update()
        {
            if (isShowingSomething && (DateTime.Now - startShowTime).TotalSeconds > maxShowDurationInSeconds)
            {
                ResetStatusText();
            }
        }

        #region Data recording
        public void StartRecording()
        {
            UpdateStatusText("Recording started...");
        }

        public void StopRecording()
        {
            UpdateStatusText("Recording stopped!");
        }
        #endregion

        #region Data replay
        public void LoadData()
        {
            UpdateStatusText("Loading data...");
        }

        public void StartReplay()
        {
            UpdateStatusText("Start replay...");
        }

        public void PauseReplay()
        {
            UpdateStatusText("Replay stopped!");
        }

        public void ResumeReplay()
        {
            UpdateStatusText("Replay resumed!");
        }

        public void PlaybackCompleted()
        {
            UpdateStatusText("Playback Completed!");
        }
        #endregion
    }
}

#pragma warning restore CS1591
