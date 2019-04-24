// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEditor;
using System;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Utility component to record an InputAnimation.
    /// </summary>
    internal class InputAnimationRecorder : MonoBehaviour
    {
        public InputRecordingSettings settings = null;

        private InputAnimation inputAnimation;
        public InputAnimation InputAnimation => inputAnimation;

        private float currentTime;

        public void Awake()
        {
            inputAnimation = new InputAnimation();
            currentTime = 0.0f;
        }

        public void Update()
        {
            currentTime += Time.deltaTime;
            InputAnimationUtils.RecordKeyframe(inputAnimation, currentTime);
        }
    }

    /// <summary>
    /// Tools for recording input tests.
    /// </summary>
    public class InputTestRecordingWindow : EditorWindow
    {
        private InputAnimationAsset inputAnimation;

        public enum RecordingMode
        {
            InputAnimation,
            TestValues,
        }

        private RecordingMode mode = RecordingMode.InputAnimation;

        private InputAnimationRecorder recorder = null;

        [MenuItem("Mixed Reality Toolkit/Utilities/Input Test Recording")]
        private static void ShowWindow()
        {
            InputTestRecordingWindow window = GetWindow<InputTestRecordingWindow>();
            window.titleContent = new GUIContent("Input Test Recording");
            window.minSize = new Vector2(380.0f, 680.0f);
            window.Show();
        }

        private void OnGUI()
        {
            bool isGUIEnabled = GUI.enabled;

            inputAnimation = (InputAnimationAsset)EditorGUILayout.ObjectField("Input Animation", inputAnimation, typeof(InputAnimationAsset), false);
            if (inputAnimation == null)
            {
                return;
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Input test recording is only available in play mode", MessageType.Warning);

                isGUIEnabled = false;
                GUI.enabled = false;
            }

            string[] recordingModeNames = { "Record Input Animation", "Record Test Values" };
            GUILayout.SelectionGrid((int)mode, recordingModeNames, recordingModeNames.Length);

            switch (mode)
            {
                case RecordingMode.InputAnimation:
                    bool wasRecording = (recorder != null);
                    bool record = GUILayout.Toggle(wasRecording, wasRecording ? "Stop Recording" : "Start Recording", "Button");

                    if (record != wasRecording)
                    {
                        if (record)
                        {
                            StartRecording();
                        }
                        else
                        {
                            StopRecording();
                            EditorUtility.SetDirty(inputAnimation);
                        }
                    }
                    break;

                case RecordingMode.TestValues:
                    break;
            }
        }

        private bool StartRecording()
        {
            IInputSimulationService inputSimService = MixedRealityToolkit.Instance.GetService<IInputSimulationService>();
            if (inputSimService == null)
            {
                return false;
            }

            var profile = inputSimService.InputSimulationProfile;
            if (!profile)
            {
                return false;
            }

            var recorderObject = new GameObject();
            recorderObject.hideFlags = HideFlags.HideAndDontSave;
            recorder = recorderObject.AddComponent<InputAnimationRecorder>();
            recorder.settings = profile.RecordingSettings;

            return true;
        }

        private void StopRecording()
        {
            if (recorder != null)
            {
                inputAnimation.InputAnimation = recorder.InputAnimation;
                Destroy(recorder.gameObject);
            }
        }
    }
}
