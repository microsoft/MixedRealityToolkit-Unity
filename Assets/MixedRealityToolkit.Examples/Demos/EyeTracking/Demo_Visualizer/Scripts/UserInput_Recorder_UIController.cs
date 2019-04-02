// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    public class UserInput_Recorder_UIController : MonoBehaviour
    {
        public GameObject btn_StartRecording;
        public GameObject btn_StopRecording;
        public GameObject btn_StartPlayback_Inactive;
        public GameObject btn_StartPlayback;
        public GameObject btn_PausePlayback;
        public GameObject btn_LoadRecordedData;

        public void Start()
        {
            RecordingUI_Reset(true);
            ReplayUI_SetActive(false);
        }

        #region Data recording
        public void StartRecording()
        {
            RecordingUI_Reset(false);
        }

        public void StopRecording()
        {
            RecordingUI_Reset(true);
        }
        
        private void RecordingUI_Reset(bool reset)
        {
            if (btn_StopRecording != null)
            {
                btn_StopRecording.SetActive(!reset);
            }

            if (btn_StartRecording != null)
            {
                btn_StartRecording.SetActive(reset);
            }
        }
        #endregion

        #region Data replay
        public void LoadData()
        {
            ReplayUI_SetActive(true);
        }

        private void ReplayUI_SetActive(bool active)
        {
            ResetPlayback(active, false);

            if (btn_StartPlayback_Inactive != null)
            {
                btn_StartPlayback_Inactive.SetActive(!active);
            }
        }

        public void StartReplay()
        {
            Debug.Log("StartReplay");
            ResetPlayback(false, true);
        }

        public void PauseReplay()
        {
            Debug.Log("PauseReplay");
            ResetPlayback(true, false);
        }

        private void ResetPlayback(bool showPlayBtn, bool showPauseBtn)
        {
            if (btn_StartPlayback != null)
            {
                btn_StartPlayback.SetActive(showPlayBtn);
            }

            if (btn_PausePlayback != null)
            {
                btn_PausePlayback.SetActive(showPauseBtn);
            }
        }
        #endregion
    }
}