// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputRecorderFeedback")]
    public class UserInputRecorderFeedback : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro _statusText = null;

        [SerializeField]
        private float _maxShowDurationInSeconds = 2f;

        private bool _isShowingSomething = false;
        private DateTime _startShowTime;

        private void UpdateStatusText(string msg)
        {
            if (_statusText != null)
            {
                _statusText.text = msg;
                _statusText.gameObject.SetActive(true);
                _isShowingSomething = true;
                _startShowTime = DateTime.Now;
            }
        }

        private void ResetStatusText()
        {
            if (_statusText != null)
            {
                _statusText.gameObject.SetActive(false);
                _statusText.text = "";
                _isShowingSomething = false;
            }
        }

        private void Update()
        {
            if ((_isShowingSomething) && ((DateTime.Now - _startShowTime).TotalSeconds > _maxShowDurationInSeconds))
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
