// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if WINDOWS_UWP
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.WebCam;
#else
using UnityEngine.VR.WSA.WebCam;
#endif
#endif

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// Manages the camera capture on the HoloLens
    /// </summary>
    public class CameraCaptureHololens : MonoBehaviour
    {
        // Executed once a frame has succesfully been captured
        public delegate void FrameCapturesDelegate(List<byte> frameData, int width, int height);
        public FrameCapturesDelegate OnFrameCapture;

#if WINDOWS_UWP
        /// <summary>
        /// Manages the camera capture
        /// </summary>
        private PhotoCapture photoCaptureObject;

        /// <summary>
        /// Is the HoloLens capturing photos?
        /// </summary>
        private bool capturing = false;
#endif

        /// <summary>
        /// Width of the photo taken
        /// </summary>
        private int photoWidth;

        /// <summary>
        /// Height of the photo taken
        /// </summary>
        private int photoHeight;

        /// <summary>
        /// Texture to which the photo will be saved to
        /// </summary>
        private Texture2D targetTexture;
        
        /// <summary>
        /// Vertical resolution of the capture camera image
        /// </summary>
        private const int VerticalCameraResolution = 504;
        /// <summary>
        /// Horizontal resolution of the capture camera image
        /// </summary>
        private const int HorizontalCameraResolution = 896;

        /// <summary>
        /// Starts capturing photos
        /// </summary>
        public void StartCapture()
        {
#if WINDOWS_UWP
            if(!capturing)
            {
                PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
                capturing = true;
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
#if WINDOWS_UWP
            if(capturing)
            {
                photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            }
            capturing = false;
#else
            Debug.LogWarning("Capturing isn't supported on this platform");
#endif
        }

#if WINDOWS_UWP

        /// <summary>
        /// Called when a capture object has been created, it configures the camera for the capture process
        /// </summary>
        /// <param name="captureObject">Contains the camera intent to open</param>
        private void OnPhotoCaptureCreated(PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;

            var cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            photoHeight = VerticalCameraResolution;
            photoWidth = HorizontalCameraResolution;
            cameraParameters.cameraResolutionWidth = photoWidth;
            cameraParameters.cameraResolutionHeight = photoHeight;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
            captureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
        }

        /// <summary>
        /// Called when the photo mode starts, if it's successfull then it'll start taking photos
        /// </summary>
        /// <param name="result">Result of the intent of starting the camera</param>
        private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
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

        /// <summary>
        /// Called when a photo has been captured to memory, if successfull,
        /// it'll copy the photo to the target texture
        /// </summary>
        /// <param name="result">Result of the photo process</param>
        /// <param name="photoCaptureFrame">Contains the photo information</param>
        private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            if (result.success)
            {
                if(targetTexture != null)
                {
                    Destroy(targetTexture);
                }

                targetTexture = new Texture2D(HorizontalCameraResolution, VerticalCameraResolution, TextureFormat.RGB24, false);
                // Copy the raw image data into our target texture
                photoCaptureFrame.UploadImageDataToTexture(targetTexture);

                if(OnFrameCapture != null)
                {
                    OnFrameCapture(targetTexture.GetRawTextureData().ToList(), photoWidth, photoHeight);
                }
            }
            else
            {
                Debug.LogError("Failed to capturing image");
            }

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
