// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputRecorderFeedback")]
    public class UserInputRecorderFeedback : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro statusText = null;

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
        #endregion
    }
}
