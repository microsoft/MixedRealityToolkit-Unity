// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Interfaces;
using Microsoft.MixedReality.Toolkit.Extensions.ScreenRecording;
using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    public class RecordingServiceVisual : MonoBehaviour,
        IRecordingServiceVisual,
        IMobileOverlayVisualChild
    {
        enum RecordingState
        {
            Ready,
            Initializing,
            Recording
        }

        [SerializeField] Button _recordButton;
        [SerializeField] Image _startRecordingImage;
        [SerializeField] Image _stopRecordingImage;
        [SerializeField] Button _previewButton;
        [SerializeField] Image _countdownImage;
        [SerializeField] Text _countdownText;
        [SerializeField] float _countdownLength = 3;

        IRecordingService _recordingService;
        float _recordingStartTime = 0;
        RecordingState state = RecordingState.Ready;
        bool _updateUI = false;
        bool _readyToRecord = false;

        public event OverlayVisibilityRequest OverlayVisibilityRequest;

        public void SetRecordingService(IRecordingService recordingService)
        {
            _recordingService = recordingService;
        }

        void Awake()
        {
            if (_recordingService == null)
            {
                Debug.LogError("Error: Recording service not set for RecordingServiceVisual");
                return;
            }

            _recordButton.onClick.AddListener(OnRecordClick);
            _previewButton.onClick.AddListener(OnPreviewClick);
        }

        void Update()
        {
            if (_recordingService != null &&
                _previewButton != null)
            {
                _previewButton.gameObject?.SetActive(_recordingService.IsRecordingAvailable());
            }

            if (state == RecordingState.Initializing)
            {
                // When initializing, we need to always update the ui based on the countdown timer
                _updateUI = true;

                if (!_readyToRecord)
                {
                    var countdownComplete = (Time.time - _recordingStartTime) > _countdownLength;
                    if (countdownComplete &&
                        _recordingService.IsInitialized())
                    {
                        Debug.Log("Preparing to record");
                        // Because recording is currently based on screen capture logic, we need to delay recording until we've
                        // hidden our overlay visuals
                        _readyToRecord = true;
                        OverlayVisibilityRequest?.Invoke(false);
                    }
                }
            }

            if (_updateUI)
            {
                UpdateUI();
            }
        }

        private void OnRecordClick()
        {
            Debug.Log("Record button clicked");

            if (_recordingService == null)
            {
                Debug.LogError("Error: Recording service not set for RecordingServiceVisual");
                return;
            }

            if (state == RecordingState.Ready)
            {
                StartRecording();
            }
            else if (state == RecordingState.Recording)
            {
                StopRecording();
            }
        }

        private void StartRecording()
        {
            Debug.Log("Initializing recording");
            _recordingService.Initialize();
            state = RecordingState.Initializing;
            _updateUI = true;
            _recordingStartTime = Time.time;
            _readyToRecord = false;
        }

        private void StopRecording()
        {
            // Todo - support stopping recording during initialization

            Debug.Log("Stopping recording");
            _recordingService.StopRecording();
            _recordingService.Dispose();
            state = RecordingState.Ready;
            _updateUI = true;
            _readyToRecord = false;
        }

        private void OnPreviewClick()
        {
            Debug.Log("Preview button clicked");

            if (_recordingService == null)
            {
                Debug.LogError("Error: Recording service not set for RecordingServiceVisual");
                return;
            }

            if (_recordingService.IsRecordingAvailable())
            {
                Debug.Log("Showing recording");
                _recordingService.ShowRecording();
            }
            else
            {
                Debug.LogError("Recording wasn't available to show");
            }
        }

        private void UpdateUI()
        {
            _updateUI = false;

            if (_startRecordingImage != null)
            {
                var startImageActive = (state == RecordingState.Ready);
                _startRecordingImage.gameObject.SetActive(startImageActive);
            }

            if (_stopRecordingImage != null)
            {
                var stopImageActive = (state == RecordingState.Initializing) || (state == RecordingState.Recording);
                _stopRecordingImage.gameObject.SetActive(stopImageActive);
            }

            if (_countdownImage != null)
            {
                var countdownActive = (state == RecordingState.Initializing) && (!_readyToRecord);
                _countdownImage.gameObject.SetActive(countdownActive);

                if (countdownActive)
                {
                    var dt = Time.time - _recordingStartTime;
                    var countdownVal = (int)(_countdownLength - dt);
                    if (countdownVal < 0)
                    {
                        countdownVal = 0;
                    }
                    _countdownText.text = countdownVal.ToString();
                }
            }
        }

        public void Show()
        {
            if (state == RecordingState.Recording)
            {
                StopRecording();
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            if (_readyToRecord)
            {
                Debug.Log("Starting recording");
                _recordingService.StartRecording();
                state = RecordingState.Recording;
            }
        }

        public bool IsShowing()
        {
            return gameObject.activeSelf;
        }
    }
}
