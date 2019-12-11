// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Utility class to make input recording service accessible through game objects.
    /// Hook up buttons to the public functions to start and stop recording input.
    /// </summary>
    public class InputRecordingControls : MonoBehaviour
    {
        private InputRecordingService recordingService = null;
        private InputRecordingService RecordingService
        {
            get
            {
                if (recordingService == null)
                {
                    recordingService = (CoreServices.InputSystem as IMixedRealityDataProviderAccess).GetDataProvider<IMixedRealityInputRecordingService>() as InputRecordingService;
                }
                return recordingService;
            }
        }

        /// <summary>
        /// Event raised when input recording is started.
        /// </summary>
        public UnityEvent OnRecordingStarted = new UnityEvent();
        /// <summary>
        /// Event raised when input recording is stopped.
        /// </summary>
        public UnityEvent OnRecordingStopped = new UnityEvent();

        private async void Start()
        {
            await new WaitUntil(() => RecordingService != null);

            RecordingService.OnRecordingStarted += () => OnRecordingStarted.Invoke();
            RecordingService.OnRecordingStopped += () => OnRecordingStopped.Invoke();
        }

        /// <summary>
        /// Toggle input recording.
        /// </summary>
        public void ToggleRecording()
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
        /// Export recorded input.
        /// </summary>
        /// <remarks>
        /// This will only save recorded input after recording has been stopped.
        /// </remarks>
        public void SaveRecordedInput()
        {
            if (!RecordingService.IsRecording)
            {
                RecordingService.SaveInputAnimation();
                RecordingService.DiscardRecordedInput();
            }
        }
    }
}
