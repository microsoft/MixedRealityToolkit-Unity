// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor
{
    public class CompositorWindowBase<TWindow> : EditorWindowBase<TWindow> where TWindow : EditorWindowBase<TWindow>
    {
        protected string holographicCameraIPAddress;

        private CompositionManager cachedCompositionManager;
        private HolographicCameraNetworkManager cachedHolographicCameraNetworkManager;

        protected int renderFrameWidth;
        protected int renderFrameHeight;
        private float uiFrameWidth = 100;
        private float uiFrameHeight = 100;
        private float aspect;

        private const string trackingLostStatusMessage = "Tracking lost";
        private const string trackingStalledStatusMessage = "No tracking update in over a second";
        private const string locatingWorldAnchorStatusMessage = "Locating world anchor...";
        private const string locatedWorldAnchorStatusMessage = "Located";
        private const string calibrationLoadedMessage = "Loaded";
        private const string calibrationNotLoadedMessage = "No camera calibration received";
        private const int horizontalFrameRectangleMargin = 50;
        protected const int textureRenderModeComposite = 0;
        protected const int textureRenderModeSplit = 1;
        private const float quadPadding = 4;

        protected virtual void OnEnable()
        {
            renderFrameWidth = CompositionManager.GetVideoFrameWidth();
            renderFrameHeight = CompositionManager.GetVideoFrameHeight();
            aspect = ((float)renderFrameWidth) / renderFrameHeight;
        }

        protected void HolographicCameraNetworkConnectionGUI()
        {
            CompositionManager compositionManager = GetCompositionManager();
            HolographicCameraNetworkManager cameraNetworkManager = GetHolographicCameraNetworkManager();

            EditorGUILayout.BeginVertical("Box");
            {
                Color titleColor;
                if (cameraNetworkManager != null &&
                    cameraNetworkManager.IsConnected &&
                    cameraNetworkManager.HasTracking &&
                    cameraNetworkManager.IsAnchorLocated &&
                    !cameraNetworkManager.IsTrackingStalled &&
                    compositionManager != null &&
                    compositionManager.IsCalibrationDataLoaded)
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
                        anchorStatusMessage = trackingLostStatusMessage;
                    }
                    else if (cameraNetworkManager.IsTrackingStalled)
                    {
                        anchorStatusMessage = trackingStalledStatusMessage;
                    }
                    else if (!cameraNetworkManager.IsAnchorLocated)
                    {
                        anchorStatusMessage = locatingWorldAnchorStatusMessage;
                    }
                    else
                    {
                        anchorStatusMessage = locatedWorldAnchorStatusMessage;
                    }

                    GUILayout.Label($"Anchor status: {anchorStatusMessage}");

                    string calibrationStatusMessage;
                    if (compositionManager != null && compositionManager.IsCalibrationDataLoaded)
                    {
                        calibrationStatusMessage = calibrationLoadedMessage;
                    }
                    else
                    {
                        calibrationStatusMessage = calibrationNotLoadedMessage;
                    }

                    GUILayout.Label($"Calibration status: {calibrationStatusMessage}");

                    if (GUILayout.Button(new GUIContent("Disconnect", "Disconnects the network connection to the holographic camera.")))
                    {
                        cameraNetworkManager.Disconnect();
                    }

                    if (GUILayout.Button(new GUIContent("Locate Shared Spatial Coordinate", "Detects the shared location used to position objects in the same physical location on multiple devices")))
                    {
                        cameraNetworkManager.SendLocateSharedAnchorCommand();
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

        protected void CompositeTextureGUI(int textureRenderMode)
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

        protected CompositionManager GetCompositionManager()
        {
            if (cachedCompositionManager == null)
            {
                cachedCompositionManager = FindObjectOfType<CompositionManager>();
            }

            return cachedCompositionManager;
        }

        protected HolographicCameraNetworkManager GetHolographicCameraNetworkManager()
        {
            if (cachedHolographicCameraNetworkManager == null)
            {
                cachedHolographicCameraNetworkManager = FindObjectOfType<HolographicCameraNetworkManager>();
            }

            return cachedHolographicCameraNetworkManager;
        }
    }
}