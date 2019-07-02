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
    class ScreenshotUtility : EditorWindow
    {
        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/Native Resolution")]
        private static void CaptureScreenshot1x()
        {
            CaptureScreenshot(GetScreenshotFileName(), 1);
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/Native Resolution (Transparent Background)")]
        private static void CaptureScreenshot1xAlphaComposite()
        {
            CaptureScreenshotAlphaComposite(Camera.main, true, GetScreenshotFileName(), 1);
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/2x Resolution")]
        private static void CaptureScreenshot2x()
        {
            CaptureScreenshot(GetScreenshotFileName(), 2);
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/2x Resolution (Transparent Background)")]
        private static void CaptureScreenshot2xAlphaComposite()
        {
            CaptureScreenshotAlphaComposite(Camera.main, true, GetScreenshotFileName(), 2);
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/4x Resolution")]
        private static void CaptureScreenshot4x()
        {
            CaptureScreenshot(GetScreenshotFileName(), 4);
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Take Screenshot/4x Resolution (Transparent Background)")]
        private static void CaptureScreenshot4xAlphaComposite()
        {
            CaptureScreenshotAlphaComposite(Camera.main, true, GetScreenshotFileName(), 4);
        }

        /// <summary>
        /// Captures a screenshot with the current main camera's clear color.
        /// </summary>
        /// <param name="path">The path to save the screenshot to.</param>
        /// <param name="superSize">The multiplication factor to apply to the native resolution.</param>
        /// <returns>True on successful screenshot capture, false otherwise.</returns>
        public static bool CaptureScreenshot(string path, int superSize)
        {
            if (string.IsNullOrEmpty(path) || superSize <= 0)
            {
                return false;
            }

            ScreenCapture.CaptureScreenshot(path, superSize);

            Debug.LogFormat("Screenshot captured to: {0}", path);

            return true;
        }

        /// <summary>
        /// Captures a screenshot from a specified camera and optionally a transparent clear color.
        /// </summary>
        /// <param name="clearBackground">True if the captured screenshot should have a transparent background</param>
        /// <param name="camera">The camera to take the screenshot from.</param>
        /// <param name="path">The path to save the screenshot to.</param>
        /// <param name="superSize">The multiplication factor to apply to the native resolution.</param>
        /// <returns>True on successful screenshot capture, false otherwise.</returns>
        public static bool CaptureScreenshotAlphaComposite(Camera camera, bool clearBackground, string path, int superSize)
        {
            if (camera == null || string.IsNullOrEmpty(path) || superSize <= 0)
            {
                return false;
            }

            // Create a camera clone with a transparent clear color.
            var renderCamera = new GameObject().AddComponent<Camera>();
            renderCamera.transform.position = camera.transform.position;
            renderCamera.transform.rotation = camera.transform.rotation;
            renderCamera.clearFlags = clearBackground ? CameraClearFlags.Color : camera.clearFlags;
            renderCamera.backgroundColor = clearBackground ? new Color(0.0f, 0.0f, 0.0f, 0.0f) : camera.backgroundColor;
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
                DestroyImmediate(outputTexture);
                DestroyImmediate(renderCamera.gameObject);
                DestroyImmediate(renderTexture);
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
