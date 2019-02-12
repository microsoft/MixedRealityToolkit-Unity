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
        [SerializeField] Button _previewButton;

        IRecordingService _recordingService;
        bool _recording = false;

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
            _previewButton.enabled = _recordingService.IsRecordingAvailable();
        }

        private void OnRecordClick()
        {
            if (_recordingService == null)
            {
                Debug.LogError("Error: Recording service not set for RecordingServiceVisual");
                return;
            }

            if (!_recording)
            {
                Debug.Log("Starting recording");
                _recording = _recordingService.StartRecording();
            }
            else
            {
                Debug.Log("Stopping recording");
                _recordingService.StopRecording();
                _recording = false;
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
    }
}
