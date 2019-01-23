// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if WINDOWS_UWP
using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;

using System.Runtime.InteropServices;
using UnityEngine.XR.WSA.WebCam;
using Windows.Media.Devices;

namespace Microsoft.MixedReality.Toolkit.Preview.CameraCapture
{
	public class CameraCaptureUWP : ICameraCapture
	{
		#region Fields
		private PhotoCapture     camera      = null;
		private CameraParameters cameraParams;
		private CameraResolution resolution  = null;
		private Texture2D        cacheTex    = null;
		private Texture2D        resizedTex  = null;
		private bool             isReady     = false;
		private float            fieldOfView = 45;

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
		public float FieldOfView
		{
			get
			{
				return fieldOfView;
			}
		}
		public int   Exposure
		{
			set
			{ 
				IntPtr unknown = camera.GetUnsafePointerToVideoDeviceController();
				using (VideoDeviceControllerWrapper wrapper = new VideoDeviceControllerWrapper(unknown))
				{
					wrapper.SetExposure(value);
				}
			} 
		}
		public int   Whitebalance 
		{
			set
			{ 
				IntPtr unknown = camera.GetUnsafePointerToVideoDeviceController();
				using (VideoDeviceControllerWrapper wrapper = new VideoDeviceControllerWrapper(unknown))
				{
					wrapper.SetWhiteBalance(value);
				}
			} 
		}
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
				PhotoCapture.SupportedResolutions.OrderBy((res) =>  res.width * res.height).First() :
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
			
				IntPtr unknown = camera.GetUnsafePointerToVideoDeviceController();
				using (VideoDeviceControllerWrapper wrapper = new VideoDeviceControllerWrapper(unknown))
				{
					wrapper.SetExposure(-7);
					wrapper.SetWhiteBalance(5000);
					wrapper.SetISO(80);
				}

				if (aOnInitialized != null)
				{
					aOnInitialized();
				}

				isReady = true;
			});
		}
		
		private void GetImage(Action<Texture2D, Matrix4x4> aOnFinished)
		{
			IsRequestingImage = true;
		
			if (camera == null)
			{
				Debug.LogError("[CameraCapture] camera hasn't been initialized!");
			}

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
					}
					else
					{
						transform[0,2] = -transform[0,2];
						transform[1,2] = -transform[1,2];
						transform[2,2] = -transform[2,2];
						//transform = transform; //transform * Camera.main.transform.localToWorldMatrix.inverse;
					}
					Matrix4x4 proj;
					if (!frame.TryGetProjectionMatrix(out proj))
					{
						fieldOfView = Mathf.Atan(1.0f / proj[0,0] ) * 2.0f * Mathf.Rad2Deg;
					}

					frame.UploadImageDataToTexture(cacheTex);
					Texture tex = resizedTex;
					resolution.ResizeTexture(cacheTex, ref tex, true);
					resizedTex = (Texture2D)tex;

					if (aOnFinished != null)
					{
						aOnFinished(resizedTex, transform);
					}

					camera.StopPhotoModeAsync((a)=>
					{
						IsRequestingImage = false;
					});
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
			{
				return;
			}

			GetImage((tex, transform) =>
			{
				if (aOnImageAcquired != null)
				{
					aOnImageAcquired(tex.GetRawTextureData<Color24>(), transform, tex.width, tex.height);
				}
			});
		}
		/// <summary>
		/// Request an image from the camera, and provide it as a GPU Texture!
		/// </summary>
		/// <param name="onImageAcquired">This is the function that will be called when the image is ready. Texture is not guaranteed to be a Texture2D, could also be a WebcamTexture. Matrix is the transform of the device when the picture was taken.</param>
		public void RequestImage(Action<Texture, Matrix4x4> aOnImageAcquired)
		{
			if (!isReady || IsRequestingImage)
			{
				return;
			}

			GetImage((tex, transform) =>
			{
				if (aOnImageAcquired != null)
				{
					aOnImageAcquired(tex, transform);
				}
			});
		}

		/// <summary>
		/// Done with the camera, free up resources!
		/// </summary>
		public void Shutdown()
		{
			if (cacheTex   != null)
			{
				GameObject.Destroy(cacheTex);
			}
			if (resizedTex != null)
			{
				GameObject.Destroy(resizedTex);
			}

			if (camera != null)
			{
				camera.Dispose();
			}
			camera = null;
		}
		#endregion
	}

	/// <summary>
	/// A wrapper for setting camera exposure settings in UWP.
	/// </summary>
	public sealed class VideoDeviceControllerWrapper : IDisposable
	{
		private VideoDeviceController controller = null;
		private bool                  disposed   = false;

		public VideoDeviceControllerWrapper(IntPtr unknown)
		{
			controller = (VideoDeviceController)Marshal.GetObjectForIUnknown(unknown);
		}
 
		~VideoDeviceControllerWrapper()
		{
			Dispose(false);
		}
 
		/// <summary>
		/// Manually override the camera's exposure.
		/// </summary>
		/// <param name="exposure">These appear to be imaginary units of some kind. Seems to be integer values around, but not exactly -10 to +10.</param>
		public void SetExposure(int exposure)
		{
			//Debug.LogFormat("({3} : {0}-{1}) +{2}", controller.Exposure.Capabilities.Min, controller.Exposure.Capabilities.Max, controller.Exposure.Capabilities.Step, exposure);
			if (!controller.Exposure.TrySetAuto(false))
			{
				Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set auto exposure off", typeof(VideoDeviceControllerWrapper));
			}

			if (!controller.Exposure.TrySetValue((double)exposure))
			{
				Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set exposure to {1} as requested", typeof(VideoDeviceControllerWrapper), exposure);
			}
		}
		/// <summary>
		/// Manually override the camera's white balance.
		/// </summary>
		/// <param name="kelvin">White balance temperature in kelvin! Also seems a bit arbitrary as to what values it accepts.</param>
		public void SetWhiteBalance(int kelvin)
		{
			if (!controller.WhiteBalance.TrySetAuto(false))
			{
				Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set auto WhiteBalance off", typeof(VideoDeviceControllerWrapper));
			}

			if (!controller.WhiteBalance.TrySetValue((double)kelvin))
			{
				Debug.LogErrorFormat("[{0}] HoloLens locatable camera has failed to set WhiteBalance to {1} as requested", typeof(VideoDeviceControllerWrapper), kelvin);
			}
		}
		/// <summary>
		/// Manually override the camera's ISO.
		/// </summary>
		/// <param name="iso">Camera's sensitivity to light, kinda like gain.</param>
		public void SetISO(int iso)
		{
			var task = controller.IsoSpeedControl.SetValueAsync((uint)iso);
		}
 
		/// <summary>
		/// Dispose of resources!
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
		/// If that's confusing to you too, maybe read this: https://docs.microsoft.com/en-us/dotnet/api/system.idisposable.dispose
		/// </summary>
		public void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (controller != null)
				{
					Marshal.ReleaseComObject(controller);
					controller = null;
				}
				disposed = true;
			}
		}
	}
}
#endif