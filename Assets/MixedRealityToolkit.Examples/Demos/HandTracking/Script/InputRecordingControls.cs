// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class InputRecordingControls : MonoBehaviour
    {
        private IMixedRealityInputRecordingService recordingService = null;
        private IMixedRealityInputRecordingService RecordingService
        {
            get
            {
                if (recordingService == null)
                {
                    recordingService = MixedRealityToolkit.Instance.GetService<IMixedRealityInputRecordingService>();
                }
                return recordingService;
            }
        }

        private bool wasRecording;
        public UnityEvent OnRecordingStarted = new UnityEvent();
        public UnityEvent OnRecordingStopped = new UnityEvent();

        private async void Start()
        {
            await new WaitUntil(() => RecordingService != null);

            wasRecording = RecordingService.IsRecording;
        }

        public void Update()
        {
            bool isRecording = RecordingService.IsRecording;
            if (isRecording != wasRecording)
            {
                if (isRecording)
                {
                    OnRecordingStarted.Invoke();
                }
                else
                {
                    OnRecordingStopped.Invoke();
                }

                wasRecording = isRecording;
            }
        }

        /// <summary>
        /// Toggle input recording.
        /// </summary>
        public void OnToggleRecording()
        {
            if (RecordingService.IsRecording)
            {
                RecordingService.StopRecording();
            }
            else
            {
                RecordingService.UseBufferTimeLimit = true;
                RecordingService.StartRecording();
            }
        }

        /// <summary>
        /// Export recorded input
        /// </summary>
        public void OnExportRecordedInput()
        {
            RecordingService.ExportRecordedInput();
        }
    }
}
