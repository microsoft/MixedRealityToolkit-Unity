// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Unity.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.CameraCapture
{
	public class CameraCaptureWebcam : ICameraCapture
	{
		Transform        poseSource     = null;
		WebCamTexture    webcamTex      = null;
		WebCamDevice     device;
		float            startTime      = 0;
		float            fieldOfView    = 45;
		CameraResolution resolution     = null;
		Texture          resizedTexture = null;

		public bool  IsReady           { get { return webcamTex != null && webcamTex.isPlaying && (Time.time - startTime) > 0.5f; } }
		public bool  IsRequestingImage { get; private set; }
		public float FieldOfView       { get { return fieldOfView; } }

		public CameraCaptureWebcam(Transform aPoseSource, float aFieldOfView)
		{
			poseSource  = aPoseSource;
			fieldOfView = aFieldOfView;
		}

		public void Initialize(bool aPreferGPUTexture, CameraResolution aResolution, Action aOnInitialized)
		{
			if (webcamTex != null)
				throw new Exception("[CameraCapture] Only need to initialize once!");
			// No cameras? Ditch out!
			if (WebCamTexture.devices.Length <= 0)
				return;

			resolution = aResolution;

			// Find a rear facing camera we can use, or use the first one
			WebCamDevice[] devices = WebCamTexture.devices;
			device = devices[0];
			for (int i = 0; i < devices.Length; i++)
			{
				if (!devices[i].isFrontFacing || devices[i].name.ToLower().Contains("rear"))
					device = devices[i];
			}
		
			// Pick a camera resolution
			if (resolution.nativeResolution == NativeResolutionMode.Largest)
				webcamTex = new WebCamTexture(device.name, 10000, 10000, 2);
			else if (resolution.nativeResolution == NativeResolutionMode.Smallest)
				webcamTex = new WebCamTexture(device.name, 1, 1, 2);
			else if (resolution.nativeResolution == NativeResolutionMode.Closest)
				webcamTex = new WebCamTexture(device.name, resolution.size.x, resolution.size.y, 2);
			else
				throw new NotImplementedException(resolution.nativeResolution.ToString());
			
			webcamTex.Play();
		
			if (aOnInitialized != null)
				aOnInitialized();
			startTime = Time.time;
		}

		public void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> aOnImageAcquired)
		{
			resolution.ResizeTexture(webcamTex, ref resizedTexture, true);
			NativeArray<Color24> pixels = ((Texture2D)resizedTexture).GetRawTextureData<Color24>();// _resizedTexture is Texture2D ? ((Texture2D)_resizedTexture).GetPixels32() : ((WebCamTexture)_resizedTexture).GetPixels32();

			if (aOnImageAcquired != null)
				aOnImageAcquired(pixels, poseSource == null ? Matrix4x4.identity : poseSource.localToWorldMatrix, resizedTexture.width, resizedTexture.height);
		}

		public void RequestImage(Action<Texture, Matrix4x4> aOnImageAcquired)
		{
			resolution.ResizeTexture(webcamTex, ref resizedTexture, false);

			if (aOnImageAcquired != null)
				aOnImageAcquired(resizedTexture, poseSource == null ? Matrix4x4.identity : poseSource.localToWorldMatrix);
		}

		public void Shutdown()
		{
			if (webcamTex != null && webcamTex.isPlaying)
				webcamTex.Stop();
			webcamTex = null;
		}
	}
}
