// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Unity.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Preview.CameraCapture
{
	public interface ICameraCapture
	{
		/// <summary>
		/// Is the camera completely initialized and ready to begin taking pictures?
		/// </summary>
		bool  IsReady           { get; }
		/// <summary>
		/// Is the camera currently already busy with taking a picture?
		/// </summary>
		bool  IsRequestingImage { get; }
		/// <summary>
		/// Field of View of the camera in degrees. This value is never ready until after 
		/// initialization, and in many cases, isn't accurate until after a picture has
		/// been taken. It's best to check this after each picture if you need it.
		/// </summary>
		float FieldOfView       { get; }

		/// <summary>
		/// Starts up and selects a device's camera, and finds appropriate picture settings
		/// based on the provided resolution! 
		/// </summary>
		/// <param name="preferGPUTexture">Do you prefer GPU textures, or do you prefer a NativeArray of colors? Certain optimizations may be present to take advantage of this preference.</param>
		/// <param name="resolution">Preferred resolution for taking pictures, note that resolutions are not guaranteed! Refer to CameraResolution for details.</param>
		/// <param name="onInitialized">When the camera is initialized, this callback is called! Some cameras may return immediately, others may take a while. Can be null.</param>
		void Initialize(bool preferGPUTexture, CameraResolution resolution, Action onInitialized);
		/// <summary>
		/// Done with the camera, free up resources!
		/// </summary>
		void Shutdown();

		/// <summary>
		/// Request an image from the camera, and provide it as an array of colors on the CPU!
		/// </summary>
		/// <param name="onImageAcquired">This is the function that will be called when the image is ready. Matrix is the transform of the device when the picture was taken, and integers are width and height of the NativeArray.</param>
		void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> onImageAcquired);
		/// <summary>
		/// Request an image from the camera, and provide it as a GPU Texture!
		/// </summary>
		/// <param name="onImageAcquired">This is the function that will be called when the image is ready. Texture is not guaranteed to be a Texture2D, could also be a WebcamTexture. Matrix is the transform of the device when the picture was taken.</param>
		void RequestImage(Action<Texture,              Matrix4x4> onImageAcquired);
	}
}