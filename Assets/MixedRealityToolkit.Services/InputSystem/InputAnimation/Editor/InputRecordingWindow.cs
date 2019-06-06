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
    /// Tools for recording input animation.
    /// </summary>
    public class InputRecordingWindow : EditorWindow
    {
        private InputAnimation animation = null;
        
        private string loadedFilePath = "";

        private IMixedRealityInputRecordingService recService = null;
        private IMixedRealityInputRecordingService RecService => recService ?? (recService = MixedRealityToolkit.Instance.GetService<IMixedRealityInputRecordingService>());

        private IInputSimulationService simService = null;
        private IInputSimulationService SimService => simService ?? (simService = MixedRealityToolkit.Instance.GetService<IInputSimulationService>());

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

        private RecordingMode mode = RecordingMode.Recording;
        public RecordingMode Mode
        {
            get { return mode; }
            private set
            {
                mode = value;
            }
        }

        private Texture2D iconPlay = null;
        private Texture2D iconPause = null;
        private Texture2D iconRecord = null;
        private Texture2D iconRecordActive = null;
        private Texture2D iconStop = null;
        private Texture2D iconStepFwd = null;
        private Texture2D iconStepBack = null;
        private Texture2D iconJumpFwd = null;
        private Texture2D iconJumpBack = null;

        bool markersFoldout = true;
        bool testTemplateFoldout = false;
        private Vector2 testTemplateScroll = Vector2.zero;

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
            Mode = (RecordingMode)GUILayout.SelectionGrid((int)Mode, modeStrings, modeStrings.Length);

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
            if (RecService == null)
            {
                EditorGUILayout.HelpBox("No input recording service found", MessageType.Info);
                return;
            }

            using (new GUILayout.HorizontalScope())
            {
                bool newUseTimeLimit = GUILayout.Toggle(RecService.UseBufferTimeLimit, "Use buffer time limit");
                if (newUseTimeLimit != RecService.UseBufferTimeLimit)
                {
                    RecService.UseBufferTimeLimit = newUseTimeLimit;
                }

                using (new GUIEnabledWrapper(RecService.UseBufferTimeLimit))
                {
                    float newTimeLimit = EditorGUILayout.FloatField(RecService.RecordingBufferTimeLimit);
                    if (newTimeLimit != RecService.RecordingBufferTimeLimit)
                    {
                        RecService.RecordingBufferTimeLimit = newTimeLimit;
                    }
                }
            }

            bool wasRecording = RecService.IsRecording;
            bool record = GUILayout.Toggle(wasRecording, wasRecording ? new GUIContent(iconRecordActive, "Stop recording input animation") : new GUIContent(iconRecord, "Record new input animation"), "Button");

            if (record != wasRecording)
            {
                if (record)
                {
                    RecService.StartRecording();
                }
                else
                {
                    RecService.StopRecording();

                    ExportAnimation(true);
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

            EditorGUILayout.Space();

            // bool wasPlaying = (director.state == PlayState.Playing);
            // bool wasPaused = (director.state == PlayState.Paused);

            // EditorGUILayout.BeginHorizontal();
            // bool jumpBack = GUILayout.Button(new GUIContent(iconJumpBack, "Reset input animation"), "Button");
            // bool play = GUILayout.Toggle(wasPlaying, wasPlaying ? new GUIContent(iconPlay, "Stop playing input animation") : new GUIContent(iconPlay, "Play back input animation"), "Button");
            // bool stepFwd = GUILayout.Button(new GUIContent(iconStepFwd, "Step forward one frame"), "Button");
            // EditorGUILayout.EndHorizontal();

            // float time = (float)director.time;
            // float newTime = GUILayout.HorizontalSlider(time, 0.0f, (float)inputAnimation.duration);

            // if (play != wasPlaying)
            // {
            //     if (play)
            //     {
            //         director.Play();
            //     }
            //     else
            //     {
            //         director.Pause();
            //     }
            // }
            // if (jumpBack)
            // {
            //     director.time = 0.0f;
            //     director.Evaluate();
            // }
            // if (stepFwd)
            // {
            //     director.time += Time.deltaTime;
            //     director.Evaluate();
            // }
            // if (newTime != time)
            // {
            //     director.time = newTime;
            //     director.Evaluate();
            // }

            // // Repaint while playing to update the timeline
            // if (director.state == PlayState.Playing)
            // {
            //     Repaint();
            // }
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

        private void ExportAnimation(bool loadAfterExport)
        {
            string outputPath;
            if (loadedFilePath.Length > 0)
            {
                string loadedFilename = Path.GetFileName(loadedFilePath);
                string loadedDirectory = Path.GetDirectoryName(loadedFilePath);
                outputPath = EditorUtility.SaveFilePanel(
                    "Select output path",
                    loadedDirectory,
                    loadedFilename,
                    InputAnimationSerializationUtils.Extension);
            }
            else
            {
                outputPath = EditorUtility.SaveFilePanelInProject(
                    "Select output path",
                    RecService.GenerateOutputFilename(),
                    InputAnimationSerializationUtils.Extension,
                    "Enter filename for exporting input animation");
            }

            if (outputPath.Length > 0)
            {
                string filename = Path.GetFileName(outputPath);
                string directory = Path.GetDirectoryName(outputPath);

                string result = RecService.ExportRecordedInput(filename, directory);

                if (loadAfterExport)
                {
                    LoadAnimation(result);
                }
            }
        }

        private void LoadAnimation(string filepath)
        {
            if (filepath.Length > 0)
            {
                try
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Open))
                    {
                        animation = new InputAnimation();
                        animation.FromStream(fs);
                        loadedFilePath = filepath;
                    }
                }
                catch (IOException ex)
                {
                    Debug.LogError(ex.Message);
                    loadedFilePath = "";
                }
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

//         /// Make sure the necessary scene objects exist based on the mode
//         private void UpdateSceneObjects()
//         {
//             if (Application.isPlaying)
//             {
//                 if (ownerObject == null)
//                 {
//                     ownerObject = new GameObject();
//                     ownerObject.hideFlags = HideFlags.HideAndDontSave;
//                 }

//                 switch (mode)
//                 {
//                     case RecordingMode.Recording:
//                         if (director != null)
//                         {
//                             DestroyImmediate(director);
//                         }
//                         break;

//                     case RecordingMode.Playback:
//                         if (recorder != null)
//                         {
//                             DestroyImmediate(recorder);
//                         }

//                         if (director == null)
//                         {
//                             director = ownerObject.AddComponent<PlayableDirector>();
//                             director.playOnAwake = false;
//                             director.playableAsset = inputAnimation;
//                         }
//                         break;
//                 }
//             }
//             else
//             {
//                 if (ownerObject != null)
//                 {
//                     DestroyImmediate(ownerObject);
//                 }
//                 return;
//             }
//         }

//         public override void OnInspectorGUI()
//         {
//             GUI.enabled = true;
//             bool isGUIEnabled = GUI.enabled;

//             UpdateSceneObjects();

//             string[] modeStrings = { "Record", "Playback" };
//             RecordingMode newMode = (RecordingMode)GUILayout.SelectionGrid((int)mode, modeStrings, modeStrings.Length);
//             if (newMode != mode)
//             {
//                 SetMode(newMode);
//             }

//             switch (mode)
//             {
//                 case RecordingMode.Recording:
//                     DrawRecordingGUI();
//                     break;
//                 case RecordingMode.Playback:
//                     DrawPlaybackGUI();
//                     break;
//             }

//             EditorGUILayout.Space();

// // XXX Reloading the scene is currently not supported,
// // due to the life cycle of the MRTK "instance" object.
// // Enable the button below once scene reloading is supported!
// #if false
//             GUI.enabled = isGUIEnabled && Application.isPlaying;
//             bool reloadScene = GUILayout.Button("Reload Scene");
//             if (reloadScene)
//             {
//                 Scene activeScene = SceneManager.GetActiveScene();
//                 if (activeScene.IsValid())
//                 {
//                     SceneManager.LoadScene(activeScene.name);
//                     return;
//                 }
//             }
//             GUI.enabled = isGUIEnabled;
// #endif

//             EditorGUILayout.Space();

//             DrawMarkersGUI();

//             DrawTestTemplateGUI();

//             EditorGUILayout.Space();

//             GUI.enabled = isGUIEnabled;
//             DrawDefaultInspector();
//         }

//         public void DrawMarkersGUI()
//         {
//             InputAnimation anim = inputAnimation.InputAnimation;

//             markersFoldout = EditorGUILayout.Foldout(markersFoldout, "Markers");
//             if (!markersFoldout)
//             {
//                 return;
//             }

//             EditorGUILayout.BeginVertical();

//             bool addEvent = GUILayout.Button("Add Marker");

//             for (int i = 0; i < anim.markerCount; ++i)
//             {
//                 var marker = anim.GetMarker(i);

//                 EditorGUILayout.BeginHorizontal();
//                 float newMarkerTime = EditorGUILayout.FloatField(marker.time);
//                 string newMarkerName = EditorGUILayout.TextField(marker.name);
//                 bool removeMarker = GUILayout.Button("Remove");
//                 EditorGUILayout.EndHorizontal();

//                 if (removeMarker)
//                 {
//                     anim.RemoveMarker(i);
//                     continue;
//                 }
//                 if (newMarkerTime != marker.time)
//                 {
//                     anim.SetMarkerTime(i, newMarkerTime);
//                 }
//                 if (newMarkerName != marker.name)
//                 {
//                     marker.name = newMarkerName;
//                 }
//             }

//             if (addEvent)
//             {
//                 var marker = new InputAnimationMarker();
//                 marker.time = (float)director.time;
//                 marker.name = "Marker";
//                 anim.AddMarker(marker);
//                 EditorUtility.SetDirty(inputAnimation);
//             }

//             EditorGUILayout.EndVertical();
//         }

//         private void DrawTestTemplateGUI()
//         {
//             testTemplateFoldout = EditorGUILayout.Foldout(testTemplateFoldout, "Test Template");
//             if (!testTemplateFoldout)
//             {
//                 return;
//             }

//             var anim = inputAnimation.InputAnimation;

//             string testName = Regex.Replace(inputAnimation.name, @"[^a-zA-Z0-9_]", "");
//             string sceneName = SceneManager.GetActiveScene().name;

//             string testTemplate = "";

//             // Common setup of scene and loading input animation
//             testTemplate += string.Format(
//                 "/// Test {0}\n" +
//                 "[UnityTest]\n" +
//                 "public IEnumerator Test_{1}()\n" +
//                 "{{\n" +
//                 "    // Load test scene\n" +
//                 "    var loadOp = TestUtilities.LoadTestSceneAsync(\"{2}\");\n" +
//                 "    while (loadOp.MoveNext())\n" +
//                 "    {{\n" +
//                 "        yield return new WaitForFixedUpdate();\n" +
//                 "    }}\n" +
//                 "\n" +
//                 "    // Load input animation\n" +
//                 "    var inputAsset = (InputAnimationAsset)Resources.Load(\"{3}\", typeof(InputAnimationAsset));\n" +
//                 "\n" +
//                 "    // Component to drive animation in the scene\n" +
//                 "    var director = TestUtilities.RunPlayable(inputAsset);\n" +
//                 "\n",
//                 inputAnimation.name,
//                 testName,
//                 sceneName,
//                 inputAnimation.name);

//             // Wait for each of the test markers in the input animation
//             for (int i = 0; i < anim.markerCount; ++i)
//             {
//                 var marker = anim.GetMarker(i);

//                 testTemplate += string.Format(
//                     "    // {0}\n" +
//                     "    {{\n" +
//                     "        InputAnimationMarker marker = inputAsset.InputAnimation.GetMarker({1});\n" +
//                     "        yield return new WaitForPlayableTime(director, marker.time);\n" +
//                     "        // ADD TEST CONDITIONS HERE\n" +
//                     "    }}\n",
//                     marker.name,
//                     i);
//             }

//             testTemplate +=
//                 "\n" +
//                 "    yield return new WaitForPlayableEnded(director);\n" +
//                 "}";

//             testTemplateScroll = EditorGUILayout.BeginScrollView(testTemplateScroll, GUILayout.Height(200));
//             EditorGUILayout.TextArea(testTemplate);
//             EditorGUILayout.EndScrollView();
//         }

//         private bool StartRecording()
//         {
//             IInputSimulationService inputSimService = MixedRealityToolkit.Instance.GetService<IInputSimulationService>();
//             if (inputSimService == null)
//             {
//                 return false;
//             }

//             var profile = inputSimService.InputSimulationProfile;
//             if (!profile)
//             {
//                 return false;
//             }

//             recorder = ownerObject.AddComponent<InputAnimationRecorder>();
//             recorder.settings = profile.RecordingSettings;

//             return true;
//         }

//         private void StopRecording()
//         {
//             if (inputAnimation != null && recorder != null)
//             {
//                 inputAnimation.InputAnimation = recorder.InputAnimation;
//                 Destroy(recorder);
//             }
//         }
    }
}
