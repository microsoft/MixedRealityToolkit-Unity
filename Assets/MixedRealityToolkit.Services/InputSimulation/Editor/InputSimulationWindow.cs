// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Tools for simulating and recording input as well as playing back input animation in the Unity editor.
    /// </summary>
    public class InputSimulationWindow : EditorWindow
    {
        private InputAnimation animation
        {
            get { return PlaybackService?.Animation; }
            set { if (PlaybackService != null) PlaybackService.Animation = value; }
        }

        private string loadedFilePath = "";

        private IInputSimulationService simulationService = null;
        private IInputSimulationService SimulationService
        {
            get
            {
                if (simulationService == null)
                {
                    simulationService = (CoreServices.InputSystem as IMixedRealityDataProviderAccess).GetDataProvider<IInputSimulationService>();
                }

                return simulationService;
            }
        }

        private IMixedRealityInputRecordingService recordingService = null;
        private IMixedRealityInputRecordingService RecordingService
        {
            get
            {
                if (recordingService == null)
                {
                    recordingService = (CoreServices.InputSystem as IMixedRealityDataProviderAccess).GetDataProvider<IMixedRealityInputRecordingService>();
                }

                return recordingService;
            }
        }

        private IMixedRealityInputPlaybackService playbackService = null;
        private IMixedRealityInputPlaybackService PlaybackService
        {
            get
            {
                if (playbackService == null)
                {
                    playbackService = (CoreServices.InputSystem as IMixedRealityDataProviderAccess).GetDataProvider<IMixedRealityInputPlaybackService>();
                }

                return playbackService;
            }
        }

        public enum ToolMode
        {
            /// <summary>
            /// Record input animation and store in the asset.
            /// </summary>
            Record,
            /// <summary>
            /// Play back input animation as simulated input.
            /// </summary>
            Playback,
        }

        private ToolMode mode = ToolMode.Record;
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
        private Texture2D iconRecord = null;
        private Texture2D iconRecordActive = null;
        private Texture2D iconStepFwd = null;
        private Texture2D iconJumpBack = null;
        private Texture2D iconJumpFwd = null;

        [MenuItem("Mixed Reality Toolkit/Utilities/Input Simulation")]
        private static void ShowWindow()
        {
            InputSimulationWindow window = GetWindow<InputSimulationWindow>();
            window.titleContent = new GUIContent("Input Simulation");
            window.minSize = new Vector2(380.0f, 680.0f);
            window.Show();
        }

        private void OnGUI()
        {
            LoadIcons();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Input simulation is only available in play mode", MessageType.Info);
                return;
            }

            DrawSimulationGUI();

            EditorGUILayout.Separator();

            string[] modeStrings = Enum.GetNames(typeof(ToolMode));
            Mode = (ToolMode)GUILayout.SelectionGrid((int)Mode, modeStrings, modeStrings.Length);

            switch (mode)
            {
                case ToolMode.Record:
                    DrawRecordingGUI();
                    break;
                case ToolMode.Playback:
                    DrawPlaybackGUI();
                    break;
            }

            EditorGUILayout.Space();

// XXX Reloading the scene is currently not supported,
// due to the life cycle of the MRTK "instance" object (see see #4530).
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

        private void DrawSimulationGUI()
        {
            if (SimulationService == null)
            {
                EditorGUILayout.HelpBox("No input simulation service found", MessageType.Info);
                return;
            }

            DrawHeadGUI();
            DrawHandsGUI();
        }

        private void DrawHeadGUI()
        {
            if (!CameraCache.Main)
            {
                return;
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label($"Head:");

                Transform headTransform = CameraCache.Main.transform;
                Vector3 newPosition = EditorGUILayout.Vector3Field("Position", headTransform.position);
                Vector3 newRotation = DrawRotationGUI("Rotation", headTransform.rotation.eulerAngles);
                bool resetHand = GUILayout.Button("Reset");

                if (newPosition != headTransform.position)
                {
                    headTransform.position = newPosition;
                }
                if (newRotation != headTransform.rotation.eulerAngles)
                {
                    headTransform.rotation = Quaternion.Euler(newRotation);
                }
                if (resetHand)
                {
                    headTransform.position = Vector3.zero;
                    headTransform.rotation = Quaternion.identity;
                }
            }
        }

        private void DrawHandsGUI()
        {
            HandSimulationMode newHandSimMode = (HandSimulationMode)EditorGUILayout.EnumPopup("Hand Simulation Mode", SimulationService.HandSimulationMode);

            if (newHandSimMode != SimulationService.HandSimulationMode)
            {
                SimulationService.HandSimulationMode = newHandSimMode;
            }

            using (new GUILayout.HorizontalScope())
            {
                DrawHandGUI(
                    "Left",
                    SimulationService.IsAlwaysVisibleHandLeft, v => SimulationService.IsAlwaysVisibleHandLeft = v,
                    SimulationService.HandPositionLeft, v => SimulationService.HandPositionLeft = v,
                    SimulationService.HandRotationLeft, v => SimulationService.HandRotationLeft = v,
                    SimulationService.ResetHandLeft);

                DrawHandGUI(
                    "Right",
                    SimulationService.IsAlwaysVisibleHandRight, v => SimulationService.IsAlwaysVisibleHandRight = v,
                    SimulationService.HandPositionRight, v => SimulationService.HandPositionRight = v,
                    SimulationService.HandRotationRight, v => SimulationService.HandRotationRight = v,
                    SimulationService.ResetHandRight);
            }
        }

        private void DrawHandGUI(string name,
            bool isAlwaysVisible, Action<bool> setAlwaysVisible,
            Vector3 position, Action<Vector3> setPosition,
            Vector3 rotation, Action<Vector3> setRotation,
            Action reset)
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label($"{name} Hand:");

                bool newIsAlwaysVisible = EditorGUILayout.Toggle("Always Visible", isAlwaysVisible);
                Vector3 newPosition = EditorGUILayout.Vector3Field("Position", position);
                Vector3 newRotation = DrawRotationGUI("Rotation", rotation);
                bool resetHand = GUILayout.Button("Reset");

                if (newIsAlwaysVisible != isAlwaysVisible)
                {
                    setAlwaysVisible(newIsAlwaysVisible);
                }
                if (newPosition != position)
                {
                    setPosition(newPosition);
                }
                if (newRotation != rotation)
                {
                    setRotation(newRotation);
                }
                if (resetHand)
                {
                    reset();
                }
            }
        }

        private void DrawRecordingGUI()
        {
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
            var recordButtonContent = wasRecording
                ? new GUIContent(iconRecordActive, "Stop recording input animation")
                : new GUIContent(iconRecord, "Record new input animation");
            bool record = GUILayout.Toggle(wasRecording, recordButtonContent, "Button");

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

                bool play, stepFwd, jumpBack, jumpFwd;
                using (new GUILayout.HorizontalScope())
                {
                    jumpBack = GUILayout.Button(new GUIContent(iconJumpBack, "Jump to the start of the input animation"), "Button");
                    var playButtonContent = wasPlaying
                        ? new GUIContent(iconPlay, "Stop playing input animation")
                        : new GUIContent(iconPlay, "Play back input animation");
                    play = GUILayout.Toggle(wasPlaying, playButtonContent, "Button");
                    stepFwd = GUILayout.Button(new GUIContent(iconStepFwd, "Step forward one frame"), "Button");
                    jumpFwd = GUILayout.Button(new GUIContent(iconJumpFwd, "Jump to the end of the input animation"), "Button");
                }

                float time = PlaybackService.LocalTime;
                float duration = (animation != null ? animation.Duration : 0.0f);
                float newTimeField = EditorGUILayout.FloatField("Current time", time);
                float newTimeSlider = GUILayout.HorizontalSlider(time, 0.0f, duration);

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
                if (jumpFwd)
                {
                    PlaybackService.LocalTime = duration;
                }
                if (stepFwd)
                {
                    PlaybackService.LocalTime += Time.deltaTime;
                }
                if (newTimeField != time)
                {
                    PlaybackService.LocalTime = newTimeField;
                }
                if (newTimeSlider != time)
                {
                    PlaybackService.LocalTime = newTimeSlider;
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
                    GUILayout.Label($"Duration: {animation.Duration} seconds");
                }
                else
                {
                    GUILayout.Label("No animation loaded");
                }
            }
        }

        private Vector3 DrawRotationGUI(string label, Vector3 rotation)
        {
            Vector3 newRotation = EditorGUILayout.Vector3Field(label, rotation);

            return newRotation;
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
            LoadTexture(ref iconRecord, "MRTK_TimelineRecord.png");
            LoadTexture(ref iconRecordActive, "MRTK_TimelineRecordActive.png");
            LoadTexture(ref iconStepFwd, "MRTK_TimelineStepFwd.png");
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
