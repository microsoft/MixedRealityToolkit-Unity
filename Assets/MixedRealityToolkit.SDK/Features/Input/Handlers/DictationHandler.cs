// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Script used to start and stop recording sessions in the current dictation system and report the transcribed text via UnityEvents.
    /// For this script to work, a dictation system like 'Windows Dictation Input Provider' must be added to the Data Providers in the Input System profile.
    /// </summary>
    public class DictationHandler : BaseInputHandler, IMixedRealityDictationHandler
    {
        [SerializeField]
        [Tooltip("Time length in seconds before the dictation session ends due to lack of audio input in case there was no audio heard in the current session")]
        private float initialSilenceTimeout = 5f;

        [SerializeField]
        [Tooltip("Time length in seconds before the dictation session ends due to lack of audio input.")]
        private float autoSilenceTimeout = 20f;

        [SerializeField]
        [Tooltip("Length in seconds for the dictation service to listen")]
        private int recordingTime = 10;

        [SerializeField]
        [Tooltip("Whether recording should start automatically on start")]
        private bool startRecordingOnStart = false;

        [System.Serializable]
        public class StringUnityEvent : UnityEvent<string> { }

        /// <summary>
        /// Event raised while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        public StringUnityEvent OnDictationHypothesis;

        /// <summary>
        /// Event raised after the user pauses, typically at the end of a sentence. Contains the full recognized string so far.
        /// </summary>
        public StringUnityEvent OnDictationResult;

        /// <summary>
        /// Event raised when the recognizer stops. Contains the final recognized string.
        /// </summary>
        public StringUnityEvent OnDictationComplete;

        /// <summary>
        /// Event raised when an error occurs. Contains the string representation of the error reason.
        /// </summary>
        public StringUnityEvent OnDictationError;

        private IMixedRealityDictationSystem dictationSystem;

        /// <summary>
        /// Start a recording session in the dictation system.
        /// </summary>
        public void StartRecording()
        {
            if (dictationSystem != null)
            {
                dictationSystem.StartRecording(gameObject, initialSilenceTimeout, autoSilenceTimeout, recordingTime);
            }
        }

        /// <summary>
        /// Stop a recording session in the dictation system.
        /// </summary>
        public void StopRecording()
        {
            if (dictationSystem != null)
            {
                dictationSystem.StopRecording();
            }
        }

        #region InputSystemGlobalHandlerListener Implementation

        /// <inheritdoc />
        protected override void RegisterHandlers()
        {
            InputSystem?.RegisterHandler<IMixedRealityDictationHandler>(this);
        }

        /// <inheritdoc />
        protected override void UnregisterHandlers()
        {
            InputSystem?.UnregisterHandler<IMixedRealityDictationHandler>(this);
        }

        #endregion InputSystemGlobalHandlerListener Implementation

        #region IMixedRealityDictationHandler implementation

        void IMixedRealityDictationHandler.OnDictationHypothesis(DictationEventData eventData)
        {
            OnDictationHypothesis.Invoke(eventData.DictationResult);
        }

        void IMixedRealityDictationHandler.OnDictationResult(DictationEventData eventData)
        {
            OnDictationResult.Invoke(eventData.DictationResult);
        }

        void IMixedRealityDictationHandler.OnDictationComplete(DictationEventData eventData)
        {
            OnDictationComplete.Invoke(eventData.DictationResult);
        }

        void IMixedRealityDictationHandler.OnDictationError(DictationEventData eventData)
        {
            OnDictationError.Invoke(eventData.DictationResult);
        }

        #endregion IMixedRealityDictationHandler implementation

        #region MonoBehaviour implementation

        protected override void Start()
        {
            base.Start();

            dictationSystem = (InputSystem as IMixedRealityDataProviderAccess)?.GetDataProvider<IMixedRealityDictationSystem>();
            Debug.Assert(dictationSystem != null, "No dictation system found. In order to use dictation, add a dictation system like 'Windows Dictation Input Provider' to the Data Providers in the Input System profile");

            if (startRecordingOnStart)
            {
                StartRecording();
            }
        }

        protected override void OnDisable()
        {
            StopRecording();

            base.OnDisable();
        }

        #endregion MonoBehaviour implementation
    }
}