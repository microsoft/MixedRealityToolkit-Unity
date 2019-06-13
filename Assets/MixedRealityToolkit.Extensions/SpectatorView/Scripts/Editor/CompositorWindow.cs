// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor
{
    [Description("Compositor")]
    internal class CompositorWindow : CompositorWindowBase<CompositorWindow>
    {
        private const float maxFrameOffset = 0.2f;
        private const float statisticsUpdateCooldownTimeSeconds = 0.1f;
        private const int lowQueuedOutputFrameWarningMark = 6;
        private Vector2 scrollPosition;
        private int textureRenderMode;
        private string framerateStatisticsMessage;
        private Color framerateStatisticsColor = Color.green;

        private bool compositorStatsFoldout;
        private bool recordingFoldout;
        private bool hologramSettingsFoldout;

        private float hologramAlpha;

        private float statisticsUpdateTimeSeconds = 0.0f;
        private string appIPAddress;

        private static string holographicCameraIPAddressKey = $"{nameof(CompositorWindow)}.{nameof(holographicCameraIPAddress)}";
        private static string appIPAddressKey = $"{nameof(CompositorWindow)}.{nameof(appIPAddress)}";

        [MenuItem("Spectator View/Compositor", false, 0)]
        public static void ShowCompositorWindow()
        {
            ShowWindow();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            CompositionManager compositionManager = GetCompositionManager();
            if (compositionManager != null)
            {
                hologramAlpha = compositionManager.DefaultAlpha;
            }

            holographicCameraIPAddress = PlayerPrefs.GetString(holographicCameraIPAddressKey, "localhost");
            appIPAddress = PlayerPrefs.GetString(appIPAddressKey, "localhost");
        }

        private void OnDisable()
        {
            PlayerPrefs.SetString(holographicCameraIPAddressKey, holographicCameraIPAddress);
            PlayerPrefs.SetString(appIPAddressKey, appIPAddress);
            PlayerPrefs.Save();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                NetworkConnectionGUI();
                CompositeGUI();
                RecordingGUI();
                HologramSettingsGUI();
                CompositorStatsGUI();
            }
            EditorGUILayout.EndScrollView();
        }

        private void NetworkConnectionGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                DeviceInfoObserver stateSynchronizationDevice = null;
                if (StateSynchronizationObserver.IsInitialized)
                {
                    stateSynchronizationDevice = StateSynchronizationObserver.Instance.GetComponent<DeviceInfoObserver>();
                }
                DeviceInfoObserver holographicCameraDevice = GetHolographicCameraDevice();

                HolographicCameraNetworkConnectionGUI(AppDeviceTypeLabel, stateSynchronizationDevice, GetSpatialCoordinateSystemParticipant(stateSynchronizationDevice), showCalibrationStatus: false, ref appIPAddress);
                HolographicCameraNetworkConnectionGUI(HolographicCameraDeviceTypeLabel, holographicCameraDevice, GetSpatialCoordinateSystemParticipant(holographicCameraDevice), showCalibrationStatus: true, ref holographicCameraIPAddress);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CompositeGUI()
        {
            EditorGUILayout.BeginVertical("Box");
            {
                //Title
                {
                    string title;
                    CompositionManager compositionManager = GetCompositionManager();
                    if (compositionManager != null && compositionManager.IsVideoFrameProviderInitialized)
                    {
                        float framesPerSecond = compositionManager.GetVideoFramerate();
                        title = string.Format("Composite [{0} x {1} @ {2:F2} frames/sec]", renderFrameWidth, renderFrameHeight, framesPerSecond);
                    }
                    else
                    {
                        title = "Composite";
                    }

                    RenderTitle(title, Color.green);
                }
                EditorGUILayout.BeginHorizontal("Box");
                {
                    string[] compositionOptions = new string[] { "Final composited texture", "All intermediate textures" };
                    GUIContent renderingModeLabel = new GUIContent("Display texture", "Choose between displaying the composited video texture or seeing intermediate textures displayed in 4 sections (top right: input video, bottom right: alpha mask, bottom left: opaque hologram, top left: final alpha-blended hologram)");
                    textureRenderMode = EditorGUILayout.Popup(renderingModeLabel, textureRenderMode, compositionOptions);
                    FullScreenCompositorWindow fullscreenWindow = FullScreenCompositorWindow.TryGetWindow();
                    if (fullscreenWindow != null)
                    {
                        fullscreenWindow.TextureRenderMode = textureRenderMode;
                    }

                    if (GUILayout.Button("Fullscreen", GUILayout.Width(120)))
                    {
                        FullScreenCompositorWindow.ShowFullscreen();
                    }
                }
                EditorGUILayout.EndHorizontal();

                // Rendering
                CompositeTextureGUI(textureRenderMode);
            }
            EditorGUILayout.EndVertical();
        }

        private void RecordingGUI()
        {
            recordingFoldout = EditorGUILayout.Foldout(recordingFoldout, "Recording");
            if (recordingFoldout)
            {
                CompositionManager compositionManager = GetCompositionManager();

                EditorGUILayout.BeginVertical("Box");
                {
                    RenderTitle("Recording", Color.green);

                    GUI.enabled = compositionManager != null && compositionManager.TextureManager != null;
                    if (compositionManager == null || !compositionManager.IsRecording())
                    {
                        if (GUILayout.Button("Start Recording"))
                        {
                            compositionManager.StartRecording();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Stop Recording"))
                        {
                            compositionManager.StopRecording();
                        }
                    }
                
                    if (GUILayout.Button("Take Picture"))
                    {
                        compositionManager.TakePicture();
                    }

                    EditorGUILayout.Space();
                    GUI.enabled = true;

                    // Open Folder
                    if (GUILayout.Button("Open Folder"))
                    {
                        Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HologramCapture"));
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void HologramSettingsGUI()
        {
            hologramSettingsFoldout = EditorGUILayout.Foldout(hologramSettingsFoldout, "Hologram Settings");
            if (hologramSettingsFoldout)
            {

                EditorGUILayout.BeginVertical("Box");
                {
                    CompositionManager compositionManager = GetCompositionManager();

                    if (compositionManager != null)
                    {
                        EditorGUILayout.Space();

                        GUI.enabled = compositionManager == null || !compositionManager.IsVideoFrameProviderInitialized;
                        GUIContent label = new GUIContent("Video source", "The video capture card you want to use as input for compositing.");
                        compositionManager.CaptureDevice = (CompositionManager.FrameProviderDeviceType)EditorGUILayout.Popup(label, ((int)compositionManager.CaptureDevice), Enum.GetNames(typeof(CompositionManager.FrameProviderDeviceType)));
                        GUI.enabled = true;

                        EditorGUILayout.Space();
                    }

                    GUIContent alphaLabel = new GUIContent("Alpha", "The alpha value used to blend holographic content with video content. 0 will result in completely transparent holograms, 1 in completely opaque holograms.");
                    float newAlpha = EditorGUILayout.Slider(alphaLabel, this.hologramAlpha, 0, 1);
                    if (newAlpha != hologramAlpha)
                    {
                        hologramAlpha = newAlpha;
                        if (compositionManager != null && compositionManager.TextureManager != null)
                        {
                            compositionManager.TextureManager.SetHologramShaderAlpha(newAlpha);
                        }
                    }

                    EditorGUILayout.Space();

                    if (compositionManager != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            float previousFrameOffset = compositionManager.VideoTimestampToHolographicTimestampOffset;
                            GUIContent frameTimeAdjustmentLabel = new GUIContent("Frame time adjustment", "The time in seconds to offset video timestamps from holographic timestamps. Use this to manually adjust for network latency if holograms appear to lag behind or follow ahead of the video content as you move the camera.");
                            compositionManager.VideoTimestampToHolographicTimestampOffset = EditorGUILayout.Slider(frameTimeAdjustmentLabel, previousFrameOffset, -1 * maxFrameOffset, maxFrameOffset);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void CompositorStatsGUI()
        {
            compositorStatsFoldout = EditorGUILayout.Foldout(compositorStatsFoldout, "Compositor Stats");
            if (compositorStatsFoldout)
            {
                CompositionManager compositionManager = GetCompositionManager();
                if (compositionManager != null && compositionManager.IsVideoFrameProviderInitialized)
                {
                    UpdateStatistics(compositionManager);

                    RenderTitle(framerateStatisticsMessage, framerateStatisticsColor);
                    
                    int queuedFrameCount = compositionManager.GetQueuedOutputFrameCount();
                    Color queuedFrameColor = (queuedFrameCount > lowQueuedOutputFrameWarningMark) ? Color.green : Color.red;
                    RenderTitle($"{queuedFrameCount} Queued output frames", queuedFrameColor);
                }
                else
                {
                    framerateStatisticsMessage = null;
                    framerateStatisticsColor = Color.green;
                    statisticsUpdateTimeSeconds = 0.0f;

                    RenderTitle("Compositor is not running, no statistics available", Color.green);
                }
            }
        }

        private void UpdateStatistics(CompositionManager compositionManager)
        {
            statisticsUpdateTimeSeconds -= Time.deltaTime;
            if (statisticsUpdateTimeSeconds <= 0)
            {
                statisticsUpdateTimeSeconds = statisticsUpdateCooldownTimeSeconds;

                float average;
                framerateStatisticsMessage = GetFramerateStatistics(compositionManager, out average);
                framerateStatisticsColor = (average > compositionManager.GetVideoFramerate()) ? Color.green : Color.red;
            }
        }

        private string GetStatsString(string title, Queue<float> statElements, out float average)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            float total = 0.0f;
            foreach (var v in statElements)
            {
                min = Mathf.Min(v, min);
                max = Mathf.Max(v, max);
                total += v;
            }

            average = total / statElements.Count;
            return string.Format("{0}: Min:{1} Max:{2} Avg:{3:N1}", title, (int)min, (int)max, average);
        }

        private string GetFramerateStatistics(CompositionManager compositionManager, out float average)
        {
            return GetStatsString("Compositor framerate", compositionManager.FramerateStatistics, out average);
        }
    }
}