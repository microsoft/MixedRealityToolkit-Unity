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
    public class CompositorWindow : EditorWindowBase<CompositorWindow>
    {
        private const float maxFrameOffset = 0.2f;
        private const float statisticsUpdateCooldownTimeSeconds = 0.1f;
        private const float quadPadding = 4;
        private const int textureRenderModeComposite = 0;
        private const int textureRenderModeSplit = 1;
        private const int horizontalFrameRectangleMargin = 50;
        private const int lowQueuedOutputFrameWarningMark = 6;
        private Vector2 scrollPosition;
        private int renderFrameWidth;
        private int renderFrameHeight;
        private float aspect;
        private int textureRenderMode;
        private string framerateStatisticsMessage;
        private Color framerateStatisticsColor = Color.green;

        private bool compositorStatsFoldout;
        private bool recordingFoldout;
        private bool hologramSettingsFoldout;

        private float hologramAlpha;

        private float uiFrameWidth = 100;
        private float uiFrameHeight = 100;
        private float statisticsUpdateTimeSeconds = 0.0f;

        private CompositionManager cachedCompositionManager;
        private HolographicCameraNetworkManager cachedHolographicCameraNetworkManager;

        private string holographicCameraIPAddress;

        [MenuItem("Spectator View/Compositor", false, 0)]
        public static void ShowCompositorWindow()
        {
            ShowWindow();
        }

        private void OnEnable()
        {
            renderFrameWidth = CompositionManager.GetVideoFrameWidth();
            renderFrameHeight = CompositionManager.GetVideoFrameHeight();
            aspect = ((float)renderFrameWidth) / renderFrameHeight;

            CompositionManager compositionManager = GetCompositionManager();
            if (compositionManager != null)
            {
                hologramAlpha = compositionManager.DefaultAlpha;
            }

            holographicCameraIPAddress = PlayerPrefs.GetString(nameof(holographicCameraIPAddress), "localhost");
        }

        private void OnDisable()
        {
            PlayerPrefs.SetString(nameof(holographicCameraIPAddress), holographicCameraIPAddress);
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
            HolographicCameraNetworkManager cameraNetworkManager = GetHolographicCameraNetworkManager();

            EditorGUILayout.BeginVertical("Box");
            {
                Color titleColor;
                if (cameraNetworkManager != null &&
                    cameraNetworkManager.IsConnected &&
                    cameraNetworkManager.HasTracking &&
                    cameraNetworkManager.IsAnchorLocated &&
                    !cameraNetworkManager.IsTrackingStalled)
                {
                    titleColor = Color.green;
                }
                else
                {
                    titleColor = Color.yellow;
                }
                RenderTitle("Holographic Camera", titleColor);

                if (cameraNetworkManager != null && cameraNetworkManager.IsConnected)
                {
                    if (cameraNetworkManager.ConnectedIPAddress == cameraNetworkManager.HoloLensIPAddress)
                    {
                        GUILayout.Label($"Connected to {cameraNetworkManager.HoloLensName} ({cameraNetworkManager.HoloLensIPAddress})");
                    }
                    else
                    {
                        GUILayout.Label($"Connected to {cameraNetworkManager.HoloLensName} ({cameraNetworkManager.ConnectedIPAddress} -> {cameraNetworkManager.HoloLensIPAddress})");
                    }

                    string anchorStatusMessage;
                    if (!cameraNetworkManager.HasTracking)
                    {
                        anchorStatusMessage = "Tracking lost";
                    }
                    else if (cameraNetworkManager.IsTrackingStalled)
                    {
                        anchorStatusMessage = "No tracking update in over a second";
                    }
                    else if (!cameraNetworkManager.IsAnchorLocated)
                    {
                        anchorStatusMessage = "Locating world anchor...";
                    }
                    else
                    {
                        anchorStatusMessage = "Located";
                    }

                    GUILayout.Label($"Anchor status: {anchorStatusMessage}");

                    if (GUILayout.Button(new GUIContent("Disconnect", "Disconnects the network connection to the holographic camera.")))
                    {
                        cameraNetworkManager.Disconnect();
                    }
                }
                else
                {
                    GUILayout.Label("Not connected.");

                    GUILayout.BeginHorizontal();
                    {
                        holographicCameraIPAddress = EditorGUILayout.TextField(holographicCameraIPAddress);
                        ConnectButtonGUI(holographicCameraIPAddress, cameraNetworkManager);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        protected void ConnectButtonGUI(string targetIpString, HolographicCameraNetworkManager remoteDevice)
        {
            string tooltip = string.Empty;
            IPAddress targetIp;
            bool validAddress = ParseAddress(targetIpString, out targetIp);

            if (remoteDevice == null)
            {
                tooltip = $"{nameof(HolographicCameraNetworkManager)} is missing from the scene.";
            }
            else if (!Application.isPlaying)
            {
                tooltip = "The scene must be in play mode to connect.";
            }
            else if (!validAddress)
            {
                tooltip = "The IP address for the remote device is not valid.";
            }

            GUI.enabled = validAddress && Application.isPlaying && remoteDevice != null;
            string label = remoteDevice != null && remoteDevice.IsConnecting ? "Disconnect" : "Connect";
            if (GUILayout.Button(new GUIContent(label, tooltip), GUILayout.Width(90)) && remoteDevice != null)
            {
                if (remoteDevice.IsConnecting)
                {
                    remoteDevice.Disconnect();
                }
                else
                {
                    remoteDevice.ConnectTo(targetIpString);
                }
            }
            GUI.enabled = true;
        }

        private bool ParseAddress(string targetIpString, out IPAddress targetIp)
        {
            if (targetIpString == "localhost")
            {
                targetIp = IPAddress.Loopback;
                return true;
            }
            else
            {
                return IPAddress.TryParse(targetIpString, out targetIp);
            }
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
                }
                EditorGUILayout.EndHorizontal();

                // Rendering
                CompositeTextureGUI();
            }
            EditorGUILayout.EndVertical();
        }

        private void CompositeTextureGUI()
        {
            UpdateFrameDimensions();

            Rect framesRect = GUILayoutUtility.GetRect(uiFrameWidth, uiFrameHeight);

            if (Event.current != null && Event.current.type == EventType.Repaint)
            {
                CompositionManager compositionManager = GetCompositionManager();
                if (compositionManager != null && compositionManager.TextureManager != null)
                {
                    if (textureRenderMode == textureRenderModeSplit)
                    {
                        Rect[] quadrantRects = CalculateVideoQuadrants(framesRect);
                        if (compositionManager.TextureManager.compositeTexture != null)
                        {
                            Graphics.DrawTexture(quadrantRects[0], compositionManager.TextureManager.compositeTexture);
                        }

                        if (compositionManager.TextureManager.colorRGBTexture != null)
                        {
                            Graphics.DrawTexture(quadrantRects[1], compositionManager.TextureManager.colorRGBTexture, compositionManager.TextureManager.IgnoreAlphaMaterial);
                        }

                        if (compositionManager.TextureManager.renderTexture != null)
                        {
                            Graphics.DrawTexture(quadrantRects[2], compositionManager.TextureManager.renderTexture, compositionManager.TextureManager.IgnoreAlphaMaterial);
                        }

                        if (compositionManager.TextureManager.alphaTexture != null)
                        {
                            Graphics.DrawTexture(quadrantRects[3], compositionManager.TextureManager.alphaTexture, compositionManager.TextureManager.IgnoreAlphaMaterial);
                        }
                    }
                    else
                    {
                        Graphics.DrawTexture(framesRect, compositionManager.TextureManager.compositeTexture);
                    }
                }
            }
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

        private void UpdateFrameDimensions()
        {
            uiFrameWidth = position.width;
            uiFrameHeight = position.height;

            if (uiFrameWidth <= uiFrameHeight * aspect)
            {
                uiFrameHeight = uiFrameWidth / aspect;
            }
            else
            {
                uiFrameWidth = uiFrameHeight * aspect;
            }

            uiFrameWidth -= horizontalFrameRectangleMargin;
        }

        private Rect[] CalculateVideoQuadrants(Rect videoRect)
        {
            float quadWidth = videoRect.width / 2 - quadPadding / 2;
            float quadHeight = videoRect.height / 2 - quadPadding / 2;
            Rect[] rects = new Rect[4];
            rects[0] = new Rect(videoRect.x, videoRect.y, quadWidth, quadHeight);
            rects[1] = new Rect(videoRect.x + quadWidth + quadPadding, videoRect.y, quadWidth, quadHeight);
            rects[2] = new Rect(videoRect.x, videoRect.y + quadHeight + quadPadding, quadWidth, quadHeight);
            rects[3] = new Rect(videoRect.x + quadWidth + quadPadding, videoRect.y + quadHeight + quadPadding, quadWidth, quadHeight);
            return rects;
        }

        private CompositionManager GetCompositionManager()
        {
            if (cachedCompositionManager == null)
            {
                cachedCompositionManager = FindObjectOfType<CompositionManager>();
            }

            return cachedCompositionManager;
        }

        private HolographicCameraNetworkManager GetHolographicCameraNetworkManager()
        {
            if (cachedHolographicCameraNetworkManager == null)
            {
                cachedHolographicCameraNetworkManager = FindObjectOfType<HolographicCameraNetworkManager>();
            }

            return cachedHolographicCameraNetworkManager;
        }
    }
}