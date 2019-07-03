// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Utility class to aide in taking screenshots via menu items and public APIs. Screenshots can 
    /// be capture at various resolutions and with the current camera's clear color or a transparent 
    /// clear color for use in easy post compositing of images.
    /// </summary>
    public class ScreenshotUtility
    {
        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/Native Resolution")]
        private static void CaptureScreenshot1x()
        {
            CaptureScreenshot(GetScreenshotFileName(), 1);
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/Native Resolution (Transparent Background)")]
        private static void CaptureScreenshot1xAlphaComposite()
        {
            CaptureScreenshot(GetScreenshotFileName(), 1, true);
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/2x Resolution")]
        private static void CaptureScreenshot2x()
        {
            CaptureScreenshot(GetScreenshotFileName(), 2);
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/2x Resolution (Transparent Background)")]
        private static void CaptureScreenshot2xAlphaComposite()
        {
            CaptureScreenshot(GetScreenshotFileName(), 2, true);
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/4x Resolution")]
        private static void CaptureScreenshot4x()
        {
            CaptureScreenshot(GetScreenshotFileName(), 4);
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/4x Resolution (Transparent Background)")]
        private static void CaptureScreenshot4xAlphaComposite()
        {
            CaptureScreenshot(GetScreenshotFileName(), 4, true);
        }

        /// <summary>
        /// Captures a screenshot with the current main camera's clear color.
        /// </summary>
        /// <param name="path">The path to save the screenshot to.</param>
        /// <param name="superSize">The multiplication factor to apply to the native resolution.</param>
        /// <param name="transparentClearColor">True if the captured screenshot should have a transparent clear color. Which can be used for screenshot overlays.</param>
        /// <param name="camera">The optional camera to take the screenshot from.</param>
        /// <returns>True on successful screenshot capture, false otherwise.</returns>
        public static bool CaptureScreenshot(string path, int superSize = 1, bool transparentClearColor = false, Camera camera = null)
        {
            if (string.IsNullOrEmpty(path) || superSize <= 0)
            {
                return false;
            }

            // If a transparent clear color isn't needed and we are capturing from the default camera, use Unity's screenshot API.
            if (!transparentClearColor && (camera == null || camera == Camera.main))
            {
                ScreenCapture.CaptureScreenshot(path, superSize);

                Debug.LogFormat("Screenshot captured to: {0}", path);

                return true;
            }

            // Make sure we have a valid camera to render from.
            if (camera == null)
            {
                camera = Camera.main;

                if (camera == null)
                {
                    Debug.Log("Failed to acquire a valid camera to capture a screenshot from.");

                    return false;
                }
            }

            // Create a camera clone with a transparent clear color.
            var renderCamera = new GameObject().AddComponent<Camera>();
            renderCamera.transform.position = camera.transform.position;
            renderCamera.transform.rotation = camera.transform.rotation;
            renderCamera.clearFlags = transparentClearColor ? CameraClearFlags.Color : camera.clearFlags;
            renderCamera.backgroundColor = transparentClearColor ? new Color(0.0f, 0.0f, 0.0f, 0.0f) : camera.backgroundColor;
            renderCamera.fieldOfView = camera.fieldOfView;
            renderCamera.nearClipPlane = camera.nearClipPlane;
            renderCamera.farClipPlane = camera.farClipPlane;

            // Create a render texture for the camera clone to render into.
            var width = Screen.width * superSize;
            var height = Screen.height * superSize;
            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            renderTexture.antiAliasing = 8;
            renderCamera.targetTexture = renderTexture;

            // Render from the camera clone.
            renderCamera.Render();

            // Copy the render from the camera and save it to disk.
            var outputTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            RenderTexture previousRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            outputTexture.ReadPixels(new Rect(0.0f, 0.0f, width, height), 0, 0);
            outputTexture.Apply();
            RenderTexture.active = previousRenderTexture;

            try
            {
                File.WriteAllBytes(path, outputTexture.EncodeToPNG());
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                return false;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(outputTexture);
                UnityEngine.Object.DestroyImmediate(renderCamera.gameObject);
                UnityEngine.Object.DestroyImmediate(renderTexture);
            }

            Debug.LogFormat("Screenshot captured to: {0}", path);

            return true;
        }

        /// <summary>
        /// Gets a screenshot file name based on time and located on the current user's desktop.
        /// </summary>
        /// <returns>The filename of the screenshot.</returns>
        public static string GetScreenshotFileName()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            return Path.Combine(path, string.Format("Screenshot_{0:yyyy-MM-dd_hh-mm-ss-tt}.png", DateTime.Now));
        }
    }
}
