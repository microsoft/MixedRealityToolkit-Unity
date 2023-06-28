// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputRecorderUIController")]
    public class UserInputRecorderUIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject buttonStartRecording = null;

        [SerializeField]
        private GameObject buttonStopRecording = null;

        [SerializeField]
        private GameObject buttonStartPlaybackInactive = null;

        [SerializeField]
        private GameObject buttonStartPlayback = null;

        [SerializeField]
        private GameObject buttonPausePlayback = null;

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
        public void LoadData()
        {
            ReplayUI_SetActive(true);
        }

        private void ReplayUI_SetActive(bool active)
        {
            ResetPlayback(active, false);

            if (buttonStartPlaybackInactive != null)
            {
                buttonStartPlaybackInactive.SetActive(!active);
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
            if (buttonStartPlayback != null)
            {
                buttonStartPlayback.SetActive(showPlayBtn);
            }

            if (buttonPausePlayback != null)
            {
                buttonPausePlayback.SetActive(showPauseBtn);
            }
        }
        #endregion
    }
}
