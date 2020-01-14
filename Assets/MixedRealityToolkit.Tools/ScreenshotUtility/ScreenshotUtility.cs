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
            CaptureScreenshot(GetScreenshotPath(), 1);
            EditorUtility.RevealInFinder(GetScreenshotDirectory());
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/Native Resolution (Transparent Background)")]
        private static void CaptureScreenshot1xAlphaComposite()
        {
            CaptureScreenshot(GetScreenshotPath(), 1, true);
            EditorUtility.RevealInFinder(GetScreenshotDirectory());
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/2x Resolution")]
        private static void CaptureScreenshot2x()
        {
            CaptureScreenshot(GetScreenshotPath(), 2);
            EditorUtility.RevealInFinder(GetScreenshotDirectory());
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/2x Resolution (Transparent Background)")]
        private static void CaptureScreenshot2xAlphaComposite()
        {
            CaptureScreenshot(GetScreenshotPath(), 2, true);
            EditorUtility.RevealInFinder(GetScreenshotDirectory());
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/4x Resolution")]
        private static void CaptureScreenshot4x()
        {
            CaptureScreenshot(GetScreenshotPath(), 4);
            EditorUtility.RevealInFinder(GetScreenshotDirectory());
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/4x Resolution (Transparent Background)")]
        private static void CaptureScreenshot4xAlphaComposite()
        {
            CaptureScreenshot(GetScreenshotPath(), 4, true);
            EditorUtility.RevealInFinder(GetScreenshotDirectory());
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
            if (!transparentClearColor && (camera == null || camera == CameraCache.Main))
            {
                ScreenCapture.CaptureScreenshot(path, superSize);

                Debug.LogFormat("Screenshot captured to: {0}", path);

                return true;
            }

            // Make sure we have a valid camera to render from.
            if (camera == null)
            {
                camera = CameraCache.Main;

                if (camera == null)
                {
                    Debug.Log("Failed to acquire a valid camera to capture a screenshot from.");

                    return false;
                }
            }

            // Create a camera clone with a transparent clear color.
            var renderCamera = new GameObject().AddComponent<Camera>();
            renderCamera.orthographic = camera.orthographic;
            renderCamera.transform.position = camera.transform.position;
            renderCamera.transform.rotation = camera.transform.rotation;
            renderCamera.clearFlags = transparentClearColor ? CameraClearFlags.Color : camera.clearFlags;
            renderCamera.backgroundColor = transparentClearColor ? new Color(0.0f, 0.0f, 0.0f, 0.0f) : camera.backgroundColor;
            renderCamera.nearClipPlane = camera.nearClipPlane;
            renderCamera.farClipPlane = camera.farClipPlane;

            if (renderCamera.orthographic)
            {
                renderCamera.orthographicSize = camera.orthographicSize;
            }
            else
            {
                renderCamera.fieldOfView = camera.fieldOfView;
            }

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
        /// Gets a directory which is safe for saving screenshots.
        /// </summary>
        /// <returns>A directory safe for saving screenshots.</returns>
        public static string GetScreenshotDirectory()
        {
            return Application.temporaryCachePath;
        }

        /// <summary>
        /// Gets a unique screenshot path with a file name based on date and time.
        /// </summary>
        /// <returns>A unique screenshot path.</returns>
        public static string GetScreenshotPath()
        {
            return Path.Combine(GetScreenshotDirectory(), string.Format("Screenshot_{0:yyyy-MM-dd_hh-mm-ss-tt}_{1}.png", DateTime.Now, GUID.Generate()));
        }
    }
}
