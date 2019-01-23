// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Unity.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Preview.CameraCapture
{
	public class CameraCaptureScreen : ICameraCapture
	{
		private Camera           sourceCamera;
		private Texture2D        captureTex  = null;
		private bool             initialized = false;
		private CameraResolution resolution  = null;
		private int              renderMask  = ~(1 << 31);

		/// <summary>
		/// Is the camera completely initialized and ready to begin taking pictures?
		/// </summary>
		public bool  IsReady
		{
			get
			{
				return initialized;
			}
		}
		/// <summary>
		/// Is the camera currently already busy with taking a picture?
		/// </summary>
		public bool  IsRequestingImage
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Field of View of the camera in degrees. This value is never ready until after 
		/// initialization, and in many cases, isn't accurate until after a picture has
		/// been taken. It's best to check this after each picture if you need it.
		/// </summary>
		public float FieldOfView
		{
			get
			{
				return Camera.main.fieldOfView;
			}
		}

		public CameraCaptureScreen(Camera aSourceCamera, int aRenderMask = ~(1 << 31))
		{
			sourceCamera = aSourceCamera;
			renderMask   = aRenderMask;
		}

		/// <summary>
		/// Starts up and selects a device's camera, and finds appropriate picture settings
		/// based on the provided resolution! 
		/// </summary>
		/// <param name="preferGPUTexture">Do you prefer GPU textures, or do you prefer a NativeArray of colors? Certain optimizations may be present to take advantage of this preference.</param>
		/// <param name="resolution">Preferred resolution for taking pictures, note that resolutions are not guaranteed! Refer to CameraResolution for details.</param>
		/// <param name="onInitialized">When the camera is initialized, this callback is called! Some cameras may return immediately, others may take a while. Can be null.</param>
		public void Initialize(bool aPreferGPUTexture, CameraResolution aResolution, Action aOnInitialized)
		{
			resolution  = aResolution;
			initialized = true;

			if (aOnInitialized != null)
			{
				aOnInitialized();
			}
		}

		private void GrabScreen(Vector2Int aSize)
		{
			if (captureTex == null || captureTex.width != aSize.x || captureTex.height != aSize.y)
			{
				if (captureTex != null)
				{
					GameObject.Destroy(captureTex);
				}
				captureTex = new Texture2D(aSize.x, aSize.y, TextureFormat.RGB24, false);
			}
			RenderTexture rt  = RenderTexture.GetTemporary(aSize.x, aSize.y, 24);
			int oldMask = sourceCamera.cullingMask;
			sourceCamera.targetTexture = rt;
			sourceCamera.cullingMask   = renderMask;
			sourceCamera.Render();

			RenderTexture.active = rt;
			captureTex.ReadPixels(sourceCamera.pixelRect, 0, 0, false);
			captureTex.Apply();
			sourceCamera.targetTexture = null;
			sourceCamera.cullingMask   = oldMask;
			RenderTexture.active = null;

			RenderTexture.ReleaseTemporary(rt);
		}

		/// <summary>
		/// Request an image from the camera, and provide it as an array of colors on the CPU!
		/// </summary>
		/// <param name="onImageAcquired">This is the function that will be called when the image is ready. Matrix is the transform of the device when the picture was taken, and integers are width and height of the NativeArray.</param>
		public void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> aOnImageAcquired)
		{
			Vector2Int size = resolution.AdjustSize(new Vector2Int(sourceCamera.pixelWidth, sourceCamera.pixelHeight));
			GrabScreen(size);

			if (aOnImageAcquired != null)
			{
				aOnImageAcquired(captureTex.GetRawTextureData<Color24>(), sourceCamera.transform.localToWorldMatrix, size.x, size.y);
			}
		}

		/// <summary>
		/// Request an image from the camera, and provide it as a GPU Texture!
		/// </summary>
		/// <param name="onImageAcquired">This is the function that will be called when the image is ready. Texture is not guaranteed to be a Texture2D, could also be a WebcamTexture. Matrix is the transform of the device when the picture was taken.</param>
		public void RequestImage(Action<Texture, Matrix4x4> aOnImageAcquired)
		{
			Vector2Int size = resolution.AdjustSize(new Vector2Int(sourceCamera.pixelWidth, sourceCamera.pixelHeight));
			GrabScreen(size);

			if (aOnImageAcquired != null)
			{
				aOnImageAcquired(captureTex, sourceCamera.transform.localToWorldMatrix);
			}
		}

		/// <summary>
		/// Done with the camera, free up resources!
		/// </summary>
		public void Shutdown()
		{
			if (captureTex != null)
			{
				GameObject.Destroy(captureTex);
			}
		}
	}
}