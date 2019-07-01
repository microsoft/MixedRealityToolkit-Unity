// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Tools for recording and playing back input animation in the Unity editor.
    /// </summary>
    public class InputRecordingWindow : EditorWindow
    {
        private InputAnimation animation
        {
            get { return PlaybackService?.Animation; }
            set { if (PlaybackService != null) PlaybackService.Animation = value; }
        }

        private string loadedFilePath = "";

        private IMixedRealityInputRecordingService recordingService = null;
        private IMixedRealityInputRecordingService RecordingService => recordingService ?? (recordingService = MixedRealityToolkit.Instance.GetService<IMixedRealityInputRecordingService>());

        private IMixedRealityInputPlaybackService playbackService = null;
        private IMixedRealityInputPlaybackService PlaybackService => playbackService ?? (playbackService = MixedRealityToolkit.Instance.GetService<IMixedRealityInputPlaybackService>());

        private IInputSimulationService simService = null;
        private IInputSimulationService SimService => simService ?? (simService = MixedRealityToolkit.Instance.GetService<IInputSimulationService>());

        public enum ToolMode
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

        private ToolMode mode = ToolMode.Recording;
        public ToolMode Mode
        {
            get { return mode; }
            private set
            {
                mode = value;
            }
        }

        /// Icon textures
        private Texture2D iconPlay = null;
        private Texture2D iconPause = null;
        private Texture2D iconRecord = null;
        private Texture2D iconRecordActive = null;
        private Texture2D iconStop = null;
        private Texture2D iconStepFwd = null;
        private Texture2D iconStepBack = null;
        private Texture2D iconJumpFwd = null;
        private Texture2D iconJumpBack = null;

        [MenuItem("Mixed Reality Toolkit/Utilities/Input Recording")]
        private static void ShowWindow()
        {
            InputRecordingWindow window = GetWindow<InputRecordingWindow>();
            window.titleContent = new GUIContent("Input Recording");
            window.minSize = new Vector2(380.0f, 680.0f);
            window.Show();
        }

        private void OnGUI()
        {
            LoadIcons();

            string[] modeStrings = { "Record", "Playback" };
            Mode = (ToolMode)GUILayout.SelectionGrid((int)Mode, modeStrings, modeStrings.Length);

            switch (mode)
            {
                case ToolMode.Recording:
                    DrawRecordingGUI();
                    break;
                case ToolMode.Playback:
                    DrawPlaybackGUI();
                    break;
            }

            EditorGUILayout.Space();

// XXX Reloading the scene is currently not supported,
// due to the life cycle of the MRTK "instance" object.
// Enable the button below once scene reloading is supported!
#if false
            using (new GUIEnabledWrapper(Application.isPlaying))
            {
                bool reloadScene = GUILayout.Button("Reload Scene");
                if (reloadScene)
                {
                    Scene activeScene = SceneManager.GetActiveScene();
                    if (activeScene.IsValid())
                    {
                        SceneManager.LoadScene(activeScene.name);
                        return;
                    }
                }
            }
#endif
        }

        private void DrawRecordingGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Input test recording is only available in play mode", MessageType.Info);
                return;
            }
            if (RecordingService == null)
            {
                EditorGUILayout.HelpBox("No input recording service found", MessageType.Info);
                return;
            }

            using (new GUILayout.HorizontalScope())
            {
                bool newUseTimeLimit = GUILayout.Toggle(RecordingService.UseBufferTimeLimit, "Use buffer time limit");
                if (newUseTimeLimit != RecordingService.UseBufferTimeLimit)
                {
                    RecordingService.UseBufferTimeLimit = newUseTimeLimit;
                }

                using (new GUIEnabledWrapper(RecordingService.UseBufferTimeLimit))
                {
                    float newTimeLimit = EditorGUILayout.FloatField(RecordingService.RecordingBufferTimeLimit);
                    if (newTimeLimit != RecordingService.RecordingBufferTimeLimit)
                    {
                        RecordingService.RecordingBufferTimeLimit = newTimeLimit;
                    }
                }
            }

            bool wasRecording = RecordingService.IsRecording;
            bool record = GUILayout.Toggle(wasRecording, wasRecording ? new GUIContent(iconRecordActive, "Stop recording input animation") : new GUIContent(iconRecord, "Record new input animation"), "Button");

            if (record != wasRecording)
            {
                if (record)
                {
                    RecordingService.StartRecording();
                }
                else
                {
                    RecordingService.StopRecording();

                    SaveAnimation(true);
                }
            }

            DrawAnimationInfo();
        }

        private void DrawPlaybackGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Input test playback is only available in play mode", MessageType.Info);
                return;
            }
            if (SimService == null)
            {
                EditorGUILayout.HelpBox("No input simulation service found", MessageType.Info);
                return;
            }

            DrawAnimationInfo();

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Load ..."))
                {
                    string filepath = EditorUtility.OpenFilePanel(
                        "Select input animation file",
                        "",
                        InputAnimationSerializationUtils.Extension);

                    LoadAnimation(filepath);
                }
            }

            using (new GUIEnabledWrapper(PlaybackService != null))
            {
                bool wasPlaying = PlaybackService.IsPlaying;

                bool play, stepFwd, jumpBack;
                using (new GUILayout.HorizontalScope())
                {
                    jumpBack = GUILayout.Button(new GUIContent(iconJumpBack, "Reset input animation"), "Button");
                    play = GUILayout.Toggle(wasPlaying, wasPlaying ? new GUIContent(iconPlay, "Stop playing input animation") : new GUIContent(iconPlay, "Play back input animation"), "Button");
                    stepFwd = GUILayout.Button(new GUIContent(iconStepFwd, "Step forward one frame"), "Button");
                }

                float time = PlaybackService.LocalTime;
                float duration = (animation != null ? animation.Duration : 0.0f);
                float newTime = GUILayout.HorizontalSlider(time, 0.0f, duration);

                if (play != wasPlaying)
                {
                    if (play)
                    {
                        PlaybackService.Play();
                    }
                    else
                    {
                        PlaybackService.Pause();
                    }
                }
                if (jumpBack)
                {
                    PlaybackService.LocalTime = 0.0f;
                }
                if (stepFwd)
                {
                    PlaybackService.LocalTime += Time.deltaTime;
                }
                if (newTime != time)
                {
                    PlaybackService.LocalTime = newTime;
                }

                // Repaint while playing to update the timeline
                if (PlaybackService.IsPlaying)
                {
                    Repaint();
                }
            }
        }

        private void DrawAnimationInfo()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Animation Info:", EditorStyles.boldLabel);

                if (animation != null)
                {
                    GUILayout.Label($"File Path: {loadedFilePath}");
                    GUILayout.Label($"Duration: {animation.Duration}");
                }
            }
        }

        private void SaveAnimation(bool loadAfterExport)
        {
            string outputPath;
            if (loadedFilePath.Length > 0)
            {
                string loadedDirectory = Path.GetDirectoryName(loadedFilePath);
                outputPath = EditorUtility.SaveFilePanel(
                    "Select output path",
                    loadedDirectory,
                    InputAnimationSerializationUtils.GetOutputFilename(),
                    InputAnimationSerializationUtils.Extension);
            }
            else
            {
                outputPath = EditorUtility.SaveFilePanelInProject(
                    "Select output path",
                    InputAnimationSerializationUtils.GetOutputFilename(),
                    InputAnimationSerializationUtils.Extension,
                    "Enter filename for exporting input animation");
            }

            if (outputPath.Length > 0)
            {
                string filename = Path.GetFileName(outputPath);
                string directory = Path.GetDirectoryName(outputPath);

                string result = RecordingService.SaveInputAnimation(filename, directory);
                RecordingService.DiscardRecordedInput();

                if (loadAfterExport)
                {
                    LoadAnimation(result);
                }
            }
        }

        private void LoadAnimation(string filepath)
        {
            if (PlaybackService.LoadInputAnimation(filepath))
            {
                loadedFilePath = filepath;
            }
            else
            {
                loadedFilePath = "";
            }
        }

        private void LoadIcons()
        {
            LoadTexture(ref iconPlay, "MRTK_TimelinePlay.png");
            LoadTexture(ref iconPause, "MRTK_TimelinePause.png");
            LoadTexture(ref iconRecord, "MRTK_TimelineRecord.png");
            LoadTexture(ref iconRecordActive, "MRTK_TimelineRecordActive.png");
            LoadTexture(ref iconStop, "MRTK_TimelineStop.png");
            LoadTexture(ref iconStepFwd, "MRTK_TimelineStepFwd.png");
            LoadTexture(ref iconStepBack, "MRTK_TimelineStepBack.png");
            LoadTexture(ref iconJumpFwd, "MRTK_TimelineJumpFwd.png");
            LoadTexture(ref iconJumpBack, "MRTK_TimelineJumpBack.png");
        }

        private static void LoadTexture(ref Texture2D tex, string filename)
        {
            const string assetPath = "StandardAssets/Textures";
            if (tex == null)
            {
                tex = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath(Path.Combine(assetPath, filename)), typeof(Texture2D));
            }
        }
    }
}
