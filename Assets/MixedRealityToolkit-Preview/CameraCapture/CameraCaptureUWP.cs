// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;

#if WINDOWS_UWP
using System.Runtime.InteropServices;
using UnityEngine.XR.WSA.WebCam;
using Windows.Media.Devices;
#endif

namespace Microsoft.MixedReality.Toolkit.CameraCapture
{
	public class CameraCaptureUWP : ICameraCapture
	{
#if WINDOWS_UWP
		#region Fields
		PhotoCapture     camera      = null;
		CameraParameters cameraParams;
		CameraResolution resolution  = null;
		Texture2D        cacheTex    = null;
		Texture2D        resizedTex  = null;
		bool             isReady     = false;
		float            fieldOfView = 45;

		/// <summary>
		/// Is the camera completely initialized and ready to begin taking pictures?
		/// </summary>
		public bool  IsReady           { get { return isReady; } }
		/// <summary>
		/// Is the camera currently already busy with taking a picture?
		/// </summary>
		public bool  IsRequestingImage { get; private set; }
		/// <summary>
		/// Field of View of the camera in degrees. This value is never ready until after 
		/// initialization, and in many cases, isn't accurate until after a picture has
		/// been taken. It's best to check this after each picture if you need it.
		/// </summary>
		public float FieldOfView       { get { return fieldOfView; } }
		public int   Exposure
		{ set { 
			IntPtr unknown = camera.GetUnsafePointerToVideoDeviceController();
			using (VideoDeviceControllerWrapper wrapper = new VideoDeviceControllerWrapper(unknown))
			{
				wrapper.SetExposure(value);
			}
		} }
		public int   Whitebalance 
		{ set { 
			IntPtr unknown = camera.GetUnsafePointerToVideoDeviceController();
			using (VideoDeviceControllerWrapper wrapper = new VideoDeviceControllerWrapper(unknown))
			{
				wrapper.SetWhiteBalance(value);
			}
		} }
		#endregion

		#region Methods
		/// <summary>
		/// Starts up and selects a device's camera, and finds appropriate picture settings
		/// based on the provided resolution! 
		/// </summary>
		/// <param name="preferGPUTexture">Do you prefer GPU textures, or do you prefer a NativeArray of colors? Certain optimizations may be present to take advantage of this preference.</param>
		/// <param name="resolution">Preferred resolution for taking pictures, note that resolutions are not guaranteed! Refer to CameraResolution for details.</param>
		/// <param name="onInitialized">When the camera is initialized, this callback is called! Some cameras may return immediately, others may take a while. Can be null.</param>
		public void Initialize(bool aPreferGPUTexture, CameraResolution aResolution, Action aOnInitialized)
		{
			resolution = aResolution;
			Resolution cameraResolution = resolution.nativeResolution == NativeResolutionMode.Smallest ?
				PhotoCapture.SupportedResolutions.OrderBy((res) => res.width * res.height).First() :
				PhotoCapture.SupportedResolutions.OrderBy((res) => -res.width * res.height).First();

			cacheTex = new Texture2D(cameraResolution.width, cameraResolution.height);

			// Create a PhotoCapture object
			PhotoCapture.CreateAsync(false, delegate(PhotoCapture captureObject)
			{
				camera = captureObject;
				cameraParams = new CameraParameters();
				cameraParams.hologramOpacity        = 0.0f;
				cameraParams.cameraResolutionWidth  = cameraResolution.width;
				cameraParams.cameraResolutionHeight = cameraResolution.height;
				cameraParams.pixelFormat            = CapturePixelFormat.BGRA32;
			
				if (aOnInitialized != null)
					aOnInitialized();

				isReady = true;
			});
		}

		void GetImage(Action<Texture2D, Matrix4x4> aOnFinished)
		{
			IsRequestingImage = true;
		
			if (camera == null)
				Debug.LogError("[CameraCapture] camera hasn't been initialized!");

			camera.StartPhotoModeAsync(cameraParams, startResult =>
			{
				camera.TakePhotoAsync((photoResult, frame) =>
				{
					// Grab the camera matrix
					Matrix4x4 transform;
					if (!frame.TryGetCameraToWorldMatrix(out transform))
					{
						Debug.Log("[CameraCapture] Can't get camera matrix!!");
						transform = Camera.main.transform.localToWorldMatrix;
					} else {
						transform[0,2] = -transform[0,2];
						transform[1,2] = -transform[1,2];
						transform[2,2] = -transform[2,2];
						//transform = transform; //transform * Camera.main.transform.localToWorldMatrix.inverse;
					}
					Matrix4x4 proj;
					if (!frame.TryGetProjectionMatrix(out proj))
						fieldOfView = Mathf.Atan(1.0f / proj[0,0] ) * 2.0f * Mathf.Rad2Deg;
					
					frame.UploadImageDataToTexture(cacheTex);
					Texture tex = resizedTex;
					resolution.ResizeTexture(cacheTex, ref tex, true);
					resizedTex = (Texture2D)tex;

					if (aOnFinished != null)
						aOnFinished(resizedTex, transform);

					camera.StopPhotoModeAsync((a)=>{ IsRequestingImage = false; });
				});
			});
		}

		/// <summary>
		/// Request an image from the camera, and provide it as an array of colors on the CPU!
		/// </summary>
		/// <param name="onImageAcquired">This is the function that will be called when the image is ready. Matrix is the transform of the device when the picture was taken, and integers are width and height of the NativeArray.</param>
		public void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> aOnImageAcquired)
		{
			if (!isReady || IsRequestingImage)
				return;

			GetImage((tex, transform) => {
				if (aOnImageAcquired != null)
					aOnImageAcquired(tex.GetRawTextureData<Color24>(), transform, tex.width, tex.height);
			});
		}
		/// <summary>
		/// Request an image from the camera, and provide it as a GPU Texture!
		/// </summary>
		/// <param name="onImageAcquired">This is the function that will be called when the image is ready. Texture is not guaranteed to be a Texture2D, could also be a WebcamTexture. Matrix is the transform of the device when the picture was taken.</param>
		public void RequestImage(Action<Texture, Matrix4x4> aOnImageAcquired)
		{
			if (!isReady || IsRequestingImage)
				return;

			GetImage((tex, transform) => {
				if (aOnImageAcquired != null)
					aOnImageAcquired(tex, transform);
			});
		}

		/// <summary>
		/// Done with the camera, free up resources!
		/// </summary>
		public void Shutdown()
		{
			if (cacheTex   != null) GameObject.Destroy(cacheTex);
			if (resizedTex != null) GameObject.Destroy(resizedTex);

			if (camera != null)
				camera.Dispose();
			camera = null;
		}
		#endregion
#else
		public bool  IsReady           { get { throw new PlatformNotSupportedException(); } }
		public bool  IsRequestingImage { get { throw new PlatformNotSupportedException(); } }
		public float FieldOfView       { get { throw new PlatformNotSupportedException(); } }
		public int   Exposure     { set { } }
		public int   Whitebalance { set { } }

		public void Initialize  (bool aPreferGPUTexture, CameraResolution aResolution, Action aOnInitialized) { throw new PlatformNotSupportedException(); }
		public void Shutdown    ()                                                                            { throw new PlatformNotSupportedException(); }
		public void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> aOnImageAcquired)          { throw new PlatformNotSupportedException(); }
		public void RequestImage(Action<Texture,              Matrix4x4> aOnImageAcquired)                    { throw new PlatformNotSupportedException(); }
#endif
	}

#if WINDOWS_UWP
	public sealed class VideoDeviceControllerWrapper : IDisposable
	{
		private VideoDeviceController controller = null;

		public VideoDeviceControllerWrapper(IntPtr unknown)
		{
			controller = (VideoDeviceController)Marshal.GetObjectForIUnknown(unknown);
		}
 
		~VideoDeviceControllerWrapper()
		{
			Dispose();
		}
 
		public void SetExposure(int exposure)
		{
			//Debug.LogFormat("({3} : {0}-{1}) +{2}", controller.Exposure.Capabilities.Min, controller.Exposure.Capabilities.Max, controller.Exposure.Capabilities.Step, exposure);
			if (!controller.Exposure.TrySetAuto(false))
				Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set auto exposure off", typeof(VideoDeviceControllerWrapper));

			if (!controller.Exposure.TrySetValue((double)exposure))
				Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set exposure to {1} as requested", typeof(VideoDeviceControllerWrapper), exposure);
		}
		public void SetWhiteBalance(int kelvin)
		{
			if (!controller.WhiteBalance.TrySetAuto(false))
				Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set auto WhiteBalance off", typeof(VideoDeviceControllerWrapper));

			if (!controller.WhiteBalance.TrySetValue((double)kelvin))
				Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set WhiteBalance to {1} as requested", typeof(VideoDeviceControllerWrapper), kelvin);
		}
		public void SetFocus(int focus)
		{
			if (!controller.Focus.TrySetAuto(false))
				Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set auto Focus off", typeof(VideoDeviceControllerWrapper));

			if (!controller.Focus.TrySetValue((double)focus))
				Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set Focus to {1} as requested", typeof(VideoDeviceControllerWrapper), focus);
		}
		public void SetISO(int iso)
		{
			var task = controller.IsoSpeedControl.SetValueAsync((uint)iso);
		}
 
		public void Dispose()
		{
			if (controller != null)
			{
				Marshal.ReleaseComObject(controller);
				controller = null;
			}
		}
	}
#endif
}