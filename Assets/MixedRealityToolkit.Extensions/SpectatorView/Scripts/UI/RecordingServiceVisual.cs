// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Interfaces;
using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Recording;
using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    public class RecordingServiceVisual : MonoBehaviour,
        IRecordingServiceVisual
    {
        [SerializeField] Button _recordButton;
        [SerializeField] Image _startRecordingImage;
        [SerializeField] Image _stopRecordingImage;
        [SerializeField] Button _previewButton;

        IRecordingService _recordingService;

        enum RecordingState
        {
            Ready,
            Initializing,
            Recording
        }

        RecordingState state = RecordingState.Ready;

        bool _updateUI = false;

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

            if (state == RecordingState.Initializing &&
                _recordingService.IsInitialized())
            {
                Debug.Log("Starting recording");
                _recordingService.StartRecording();
                state = RecordingState.Recording;
            }

            if (_updateUI)
            {
                UpdateUI();
            }
        }

        private void OnRecordClick()
        {
            if (_recordingService == null)
            {
                Debug.LogError("Error: Recording service not set for RecordingServiceVisual");
                return;
            }

            if (state == RecordingState.Ready)
            {
                Debug.Log("Initializing recording");
                _recordingService.Initialize();
                state = RecordingState.Initializing;
                _updateUI = true;
            }
            else if (state == RecordingState.Recording)
            {
                Debug.Log("Stopping recording");
                _recordingService.StopRecording();
                _recordingService.Dispose();
                state = RecordingState.Ready;
                _updateUI = true;
            }
        }

        private void OnPreviewClick()
        {
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
        }
    }
}
