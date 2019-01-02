// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.CameraCapture
{
	public class CameraCaptureScreen : ICameraCapture
	{
		Camera           sourceCamera;
		Texture2D        captureTex  = null;
		bool             initialized = false;
		CameraResolution resolution  = null;
		int              renderMask  = ~(1 << 31);

		public bool  IsReady           { get { return initialized; } }
		public bool  IsRequestingImage { get { return false;  } }
		public float FieldOfView       { get { return Camera.main.fieldOfView; } }

		public CameraCaptureScreen(Camera aSourceCamera, int aRenderMask = ~(1 << 31))
		{
			sourceCamera = aSourceCamera;
			renderMask   = aRenderMask;
		}

		public void Initialize(bool aPreferGPUTexture, CameraResolution aResolution, Action aOnInitialized)
		{
			resolution  = aResolution;
			initialized = true;

			if (aOnInitialized != null)
				aOnInitialized();
		}

		void GrabScreen(Vector2Int aSize)
		{
			if (captureTex == null || captureTex.width != aSize.x || captureTex.height != aSize.y)
			{
				if (captureTex != null) GameObject.Destroy(captureTex);
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

		public void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> aOnImageAcquired)
		{
			Vector2Int size = resolution.AdjustSize(new Vector2Int(sourceCamera.pixelWidth, sourceCamera.pixelHeight));
			GrabScreen(size);

			if (aOnImageAcquired != null)
				aOnImageAcquired(captureTex.GetRawTextureData<Color24>(), sourceCamera.transform.localToWorldMatrix, size.x, size.y);
		}

		public void RequestImage(Action<Texture, Matrix4x4> aOnImageAcquired)
		{
			Vector2Int size = resolution.AdjustSize(new Vector2Int(sourceCamera.pixelWidth, sourceCamera.pixelHeight));
			GrabScreen(size);

			if (aOnImageAcquired != null)
				aOnImageAcquired(captureTex, sourceCamera.transform.localToWorldMatrix);
		}

		public void Shutdown()
		{
			if (captureTex != null) GameObject.Destroy(captureTex);
		}
	}
}