// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputRecorderFeedback")]
    public class UserInputRecorderFeedback : MonoBehaviour
    {
        [SerializeField]
        private TextMesh statusText = null;

        [SerializeField]
        private float maxShowDurationInSeconds = 2f;

        [SerializeField]
        private AudioClip audio_StartRecording = null;

        [SerializeField]
        private AudioClip audio_StopRecording = null;

        [SerializeField]
        private AudioClip audio_StartPlayback = null;

        [SerializeField]
        private AudioClip audio_PausePlayback = null;

        [SerializeField]
        private AudioClip audio_LoadRecordedData = null;
        
        private void PlayAudio(AudioClip audio)
        {
            if (AudioFeedbackPlayer.Instance != null)
            {
                AudioFeedbackPlayer.Instance.PlaySound(audio);
            }
        }

        bool isShowingSomething = false;
        DateTime startShowTime;
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
            if ((isShowingSomething)&&((DateTime.Now - startShowTime).TotalSeconds > maxShowDurationInSeconds))
            {
                ResetStatusText();
            }
        }

        #region Data recording
        public void StartRecording()
        {
            PlayAudio(audio_StartRecording);
            UpdateStatusText("Recording started...");
        }

        public void StopRecording()
        {
            PlayAudio(audio_StopRecording);
            UpdateStatusText("Recording stopped!");
        }
        #endregion

        #region Data replay
        public void LoadData()
        {
            PlayAudio(audio_LoadRecordedData);
            UpdateStatusText("Loading data...");
        }

        public void StartReplay()
        {
            PlayAudio(audio_StartPlayback);
            UpdateStatusText("Start replay...");
        }

        public void PauseReplay()
        {
            PlayAudio(audio_PausePlayback);
            UpdateStatusText("Replay stopped!");
        }
        #endregion
    }
}