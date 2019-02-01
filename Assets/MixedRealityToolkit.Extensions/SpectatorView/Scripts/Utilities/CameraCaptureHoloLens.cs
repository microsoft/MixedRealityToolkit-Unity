// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.WebCam;
#else
using UnityEngine.VR.WSA.WebCam;
#endif
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities
{
    public delegate void FrameCaptureHandler(byte[] frameData, int width, int height, Matrix4x4 projectionMatrix, Matrix4x4 cameraToWorldMatrix);

    /// <summary>
    /// Manages the camera capture on the HoloLens
    /// </summary>
    public class CameraCaptureHoloLens : MonoBehaviour
    {
        public event FrameCaptureHandler OnFrameCapture;

#if UNITY_WSA
        /// <summary>
        /// Manages the camera capture
        /// </summary>
        private PhotoCapture photoCaptureObject;

        /// <summary>
        /// Is the HoloLens capturing photos?
        /// </summary>
        private bool capturing = false;
#endif

        private int photoWidth;
        private int photoHeight;

        /// <summary>
        /// Texture to which the photo will be saved to
        /// </summary>
        private Texture2D targetTexture;

        /// <summary>
        /// Starts capturing photos
        /// </summary>
        public void StartCapture()
        {
#if UNITY_WSA
            if(!capturing)
            {
                try
                {
                    PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
                    capturing = true;
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception thrown creating PhotoCapture: " + e.ToString());
                }
            }
#else
            Debug.LogWarning("Capturing isn't supported on this platform");
#endif
        }

        /// <summary>
        /// Stops capturing photos
        /// </summary>
        public void StopCapture()
        {
#if UNITY_WSA
            if(capturing)
            {
                photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            }
            capturing = false;
#else
            Debug.LogWarning("Capturing isn't supported on this platform");
#endif
        }

#if UNITY_WSA

        /// <summary>
        /// Called when a capture object has been created, it configures the camera for the capture process
        /// </summary>
        /// <param name="captureObject">Contains the camera intent to open</param>
        private void OnPhotoCaptureCreated(PhotoCapture captureObject)
        {
            if (captureObject == null)
            {
                Debug.LogError("Failed to create capture object");
                return;
            }

            photoCaptureObject = captureObject;

            var cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            photoWidth = cameraResolution.width;
            photoHeight = cameraResolution.height;

            cameraParameters.cameraResolutionWidth = photoWidth;
            cameraParameters.cameraResolutionHeight = photoHeight;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            Debug.Log("Attempting to start photo mode: " + cameraParameters.cameraResolutionWidth + "x" + cameraParameters.cameraResolutionHeight);
            captureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
        }

        /// <summary>
        /// Called when the photo mode starts, if it's successfull then it'll start taking photos
        /// </summary>
        /// <param name="result">Result of the intent of starting the camera</param>
        private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
        {
            try
            {
                if (result.success)
                {
                    photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
                }
                else
                {
                    Debug.LogError("Unable to start photo mode!");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Encountered errors starting photo mode: " + e.ToString());
            }
        }

        /// <summary>
        /// Called when a photo has been captured to memory, if successfull,
        /// it'll copy the photo to the target texture
        /// </summary>
        /// <param name="result">Result of the photo process</param>
        /// <param name="photoCaptureFrame">Contains the photo information</param>
        private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            Debug.Log("Photo captured from HoloLens");
            if (result.success)
            {
                if(targetTexture != null)
                {
                    Destroy(targetTexture);
                }

                targetTexture = new Texture2D(photoWidth, photoHeight, TextureFormat.RGB24, false);
                // Copy the raw image data into our target texture
                photoCaptureFrame.UploadImageDataToTexture(targetTexture);

                if(OnFrameCapture != null)
                {
                    Matrix4x4 projectionMatrix;
                    if(!photoCaptureFrame.TryGetProjectionMatrix(out projectionMatrix))
                    {
                        Debug.LogWarning("Unable to obtain projection matrix");
                    }

                    Matrix4x4 cameraToWorldMatrix;
                    if(!photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix))
                    {
                        Debug.LogWarning("Unable to obtain camera to world matrix");
                    }

                    OnFrameCapture?.Invoke(targetTexture.GetRawTextureData().ToArray(), photoWidth, photoHeight, projectionMatrix, cameraToWorldMatrix);
                }
            }
            else
            {
                Debug.LogError("Failed to capture image");
            }

            photoCaptureFrame.Dispose();
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }

        /// <summary>
        /// Called when the photo mode stops
        /// </summary>
        /// <param name="result">Result of the intent of stopping the photo mode</param>
        private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
        {
            photoCaptureObject.Dispose();
            photoCaptureObject = null;
            capturing = false;
        }

        private void OnDestroy()
        {
            StopCapture();
        }
#endif
    }
}