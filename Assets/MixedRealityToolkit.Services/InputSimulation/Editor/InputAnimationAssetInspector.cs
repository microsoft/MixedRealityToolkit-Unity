// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEngine.Playables;
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
    [CustomEditor(typeof(InputAnimationAsset))]
    public class InputAnimationAssetEditor : UnityEditor.Editor
    {
        public enum RecordingMode
        {
            /// <summary>
            /// Record input animation and store in the asset.
            /// </summary>
            Recording,
            /// <summary>
            /// Play back input animation as simulated input.
            /// </summary>
            Playback,
        }

        private RecordingMode mode;
        public RecordingMode Mode => mode;

        private InputAnimationAsset inputAnimation => target as InputAnimationAsset;

        private GameObject ownerObject = null;
        private PlayableDirector director = null;
        private InputAnimationRecorder recorder = null;

        private Texture2D iconPlay = null;
        private Texture2D iconPause = null;
        private Texture2D iconRecord = null;
        private Texture2D iconRecordActive = null;
        private Texture2D iconStop = null;
        private Texture2D iconStepFwd = null;
        private Texture2D iconStepBack = null;
        private Texture2D iconJumpFwd = null;
        private Texture2D iconJumpBack = null;

        public void Awake()
        {
            string assetPath = "StandardAssets/Textures";

            if (iconPlay == null)
            {
                iconPlay = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_TimelinePlay.png"), typeof(Texture2D));
            }
            if (iconPause == null)
            {
                iconPause = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_TimelinePause.png"), typeof(Texture2D));
            }
            if (iconRecord == null)
            {
                iconRecord = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_TimelineRecord.png"), typeof(Texture2D));
            }
            if (iconRecordActive == null)
            {
                iconRecordActive = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_TimelineRecordActive.png"), typeof(Texture2D));
            }
            if (iconStop == null)
            {
                iconStop = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_TimelineStop.png"), typeof(Texture2D));
            }
            if (iconStepFwd == null)
            {
                iconStepFwd = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_TimelineStepFwd.png"), typeof(Texture2D));
            }
            if (iconStepBack == null)
            {
                iconStepBack = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_TimelineStepBack.png"), typeof(Texture2D));
            }
            if (iconJumpFwd == null)
            {
                iconJumpFwd = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_TimelineJumpFwd.png"), typeof(Texture2D));
            }
            if (iconJumpBack == null)
            {
                iconJumpBack = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_TimelineJumpBack.png"), typeof(Texture2D));
            }

            SetMode(RecordingMode.Recording);
        }

        public void OnDestroy()
        {
            if (ownerObject != null)
            {
                DestroyImmediate(ownerObject);
            }
        }

        public void SetMode(RecordingMode _mode)
        {
            if (_mode != mode)
            {
                mode = _mode;
                UpdateSceneObjects();
            }
        }

        /// Make sure the necessary scene objects exist based on the mode
        private void UpdateSceneObjects()
        {
            if (Application.isPlaying)
            {
                if (ownerObject == null)
                {
                    ownerObject = new GameObject();
                    ownerObject.hideFlags = HideFlags.HideAndDontSave;
                }

                switch (mode)
                {
                    case RecordingMode.Recording:
                        if (director != null)
                        {
                            DestroyImmediate(director);
                        }
                        break;

                    case RecordingMode.Playback:
                        if (recorder != null)
                        {
                            DestroyImmediate(recorder);
                        }

                        if (director == null)
                        {
                            director = ownerObject.AddComponent<PlayableDirector>();
                            director.playOnAwake = false;
                            director.playableAsset = inputAnimation;
                        }
                        break;
                }
            }
            else
            {
                if (ownerObject != null)
                {
                    DestroyImmediate(ownerObject);
                }
                return;
            }
        }

        public override void OnInspectorGUI()
        {
            bool isGUIEnabled = GUI.enabled;

            UpdateSceneObjects();

            string[] modeStrings = { "Record", "Playback" };
            RecordingMode newMode = (RecordingMode)GUILayout.SelectionGrid((int)mode, modeStrings, modeStrings.Length);
            if (newMode != mode)
            {
                SetMode(newMode);
            }

            switch (mode)
            {
                case RecordingMode.Recording:
                    DrawRecordingGUI();
                    break;
                case RecordingMode.Playback:
                    DrawPlaybackGUI();
                    break;
            }

            EditorGUILayout.Space();

            DrawEventsGUI();

            EditorGUILayout.Space();

            GUI.enabled = isGUIEnabled;
            DrawDefaultInspector();
        }

        private void DrawRecordingGUI()
        {
            bool isGUIEnabled = GUI.enabled;
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Input test recording is only available in play mode", MessageType.Info);
                return;
            }

            bool wasRecording = (recorder != null);
            bool record = GUILayout.Toggle(wasRecording, wasRecording ? new GUIContent(iconRecordActive, "Stop recording input animation") : new GUIContent(iconRecord, "Record new input animation"), "Button");

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
        }

        private void DrawPlaybackGUI()
        {
            Debug.Assert(director != null);

            bool isGUIEnabled = GUI.enabled;
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Input test playback is only available in play mode", MessageType.Info);
                return;
            }

            bool wasPlaying = (director.state == PlayState.Playing);
            bool wasPaused = (director.state == PlayState.Paused);

            EditorGUILayout.BeginHorizontal();
            bool jumpBack = GUILayout.Button(new GUIContent(iconJumpBack, "Reset input animation"), "Button");
            bool play = GUILayout.Toggle(wasPlaying, wasPlaying ? new GUIContent(iconPlay, "Stop playing input animation") : new GUIContent(iconPlay, "Play back input animation"), "Button");
            bool stepFwd = GUILayout.Button(new GUIContent(iconStepFwd, "Step forward one frame"), "Button");
            EditorGUILayout.EndHorizontal();

            float time = (float)director.time;
            float newTime = GUILayout.HorizontalSlider(time, 0.0f, (float)inputAnimation.duration);

            if (play != wasPlaying)
            {
                if (play)
                {
                    director.Play();
                }
                else
                {
                    director.Pause();
                }
            }
            if (jumpBack)
            {
                director.time = 0.0f;
                director.Evaluate();
            }
            if (stepFwd)
            {
                director.time += Time.deltaTime;
                director.Evaluate();
            }
            if (newTime != time)
            {
                director.time = newTime;
                director.Evaluate();
            }

            // Repaint while playing to update the timeline
            if (director.state == PlayState.Playing)
            {
                Repaint();
            }
        }

        public void DrawEventsGUI()
        {
            InputAnimation anim = inputAnimation.InputAnimation;

            bool addEvent = GUILayout.Button("Add Event");

            for (int i = 0; i < anim.markerCount; ++i)
            {
                var marker = anim.GetMarker(i);

                EditorGUILayout.BeginHorizontal();
                float newMarkerTime = EditorGUILayout.FloatField(marker.time);
                string newMarkerName = EditorGUILayout.TextField(marker.name);
                bool removeMarker = GUILayout.Button("Remove");
                EditorGUILayout.EndHorizontal();

                if (removeMarker)
                {
                    anim.RemoveMarker(i);
                    continue;
                }
                if (newMarkerTime != marker.time)
                {
                    anim.SetMarkerTime(i, newMarkerTime);
                }
                if (newMarkerName != marker.name)
                {
                    marker.name = newMarkerName;
                }
            }

            if (addEvent)
            {
                var marker = new InputAnimationMarker();
                marker.time = (float)director.time;
                marker.name = "Marker";
                anim.AddMarker(marker);
                EditorUtility.SetDirty(inputAnimation);
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

            recorder = ownerObject.AddComponent<InputAnimationRecorder>();
            recorder.settings = profile.RecordingSettings;

            return true;
        }

        private void StopRecording()
        {
            if (inputAnimation != null && recorder != null)
            {
                inputAnimation.InputAnimation = recorder.InputAnimation;
                Destroy(recorder);
            }
        }
    }
}
