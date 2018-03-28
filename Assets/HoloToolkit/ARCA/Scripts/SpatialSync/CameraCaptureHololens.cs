// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if NETFX_CORE
using UnityEngine.XR.WSA.WebCam;
#endif

namespace HoloToolkit.ARCapture
{
    public class CameraCaptureHololens : MonoBehaviour
    {
		// Executed once a frame has succesfully been captured
        public delegate void FrameCapturesDelegate(List<byte> frameData, int width, int height);
        public FrameCapturesDelegate OnFrameCapture;

#if NETFX_CORE
		private PhotoCapture photoCaptureObject;
#endif
        private int photoWidth;
        private int photoHeight;

#if NETFX_CORE
		private bool capturing = false;
#endif
        private Texture2D targetTexture;

        public void StartCapture()
        {
#if NETFX_CORE
			if(!capturing)
			{
				PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
			}
			capturing = true;
#else
			Debug.LogWarning("Capturing only supported on the HoloLens platform");
#endif
        }

        public void StopCapture()
        {
#if NETFX_CORE
			if(capturing)
			{
				photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
			}
			capturing = false;
#else
			Debug.LogWarning("Capturing only supported on the HoloLens platform");
#endif
        }

#if NETFX_CORE
	private void OnPhotoCaptureCreated(PhotoCapture captureObject)
	{
		photoCaptureObject = captureObject;

		CameraParameters c = new CameraParameters();
		c.hologramOpacity = 0.0f;
		photoHeight = 504;
		photoWidth = 896;
		c.cameraResolutionWidth = photoWidth;
		c.cameraResolutionHeight = photoHeight;
		c.pixelFormat = CapturePixelFormat.BGRA32;
		captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
	}

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

	private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
	{
		if (result.success)
		{
			if(targetTexture != null)
			{
				Destroy(targetTexture);
			}

			targetTexture = new Texture2D(896, 504, TextureFormat.RGB24, false);
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

	private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
	{
		photoCaptureObject.Dispose();
		photoCaptureObject = null;
		capturing = false;
	}

	private void OnDestroy()
	{
		if(capturing)
		{
			photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
		}
	}
#endif
    }
}
