// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine;

namespace MixedRealityToolkit.Examples.InputModule
{
    public class DictationRecordButton : MonoBehaviour, IInputClickHandler, IDictationHandler
    {
        [SerializeField]
        [Range(0.1f, 5f)]
        [Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.")]
        private float initialSilenceTimeout = 5f;

        [SerializeField]
        [Range(5f, 60f)]
        [Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input.")]
        private float autoSilenceTimeout = 20f;

        [SerializeField]
        [Range(1, 60)]
        [Tooltip("Length in seconds for the manager to listen.")]
        private int recordingTime = 10;

        [SerializeField]
        private TextMesh speechToTextOutput = null;

        [SerializeField]
        private GameObject recordLight = null;

        private Renderer buttonRenderer;

        private bool isRecording;

        private void Awake()
        {
            buttonRenderer = GetComponent<Renderer>();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            ToggleRecording();
        }

        private void ToggleRecording()
        {
            if (isRecording)
            {
                isRecording = false;
                StartCoroutine(DictationInputManager.StopRecording());
                speechToTextOutput.color = Color.white;
                buttonRenderer.enabled = true;
                recordLight.SetActive(false);
            }
            else
            {
                isRecording = true;
                StartCoroutine(DictationInputManager.StartRecording(
                    gameObject,
                    initialSilenceTimeout,
                    autoSilenceTimeout,
                    recordingTime));
                speechToTextOutput.color = Color.green;
                recordLight.SetActive(true);
                buttonRenderer.enabled = false;
            }
        }

        public void OnDictationHypothesis(DictationEventData eventData)
        {
            speechToTextOutput.text = eventData.DictationResult;
        }

        public void OnDictationResult(DictationEventData eventData)
        {
            speechToTextOutput.text = eventData.DictationResult;
        }

        public void OnDictationComplete(DictationEventData eventData)
        {
            speechToTextOutput.text = eventData.DictationResult;
        }

        public void OnDictationError(DictationEventData eventData)
        {
            isRecording = false;
            speechToTextOutput.color = Color.red;
            buttonRenderer.enabled = true;
            recordLight.SetActive(false);
            speechToTextOutput.text = eventData.DictationResult;
            Debug.LogError(eventData.DictationResult);
            StartCoroutine(DictationInputManager.StopRecording());
        }
    }
}
