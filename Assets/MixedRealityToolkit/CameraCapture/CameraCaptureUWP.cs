using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;

#if WINDOWS_UWP
using System.Runtime.InteropServices;
using UnityEngine.XR.WSA.WebCam;
using Windows.Media.Devices;
#endif

namespace CameraCapture
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

		public bool  IsReady           { get { return isReady; } }
		public bool  IsRequestingImage { get; private set; }
		public float FieldOfView       { get { return fieldOfView; } }
		public int   Exposure { set{ 
				IntPtr unknown = camera.GetUnsafePointerToVideoDeviceController();
				using (VideoDeviceControllerWrapper wrapper = new VideoDeviceControllerWrapper(unknown)) {
					wrapper.SetExposure(value);
				}} 
			}
		public int   Whitebalance { set{ 
				IntPtr unknown = camera.GetUnsafePointerToVideoDeviceController();
				using (VideoDeviceControllerWrapper wrapper = new VideoDeviceControllerWrapper(unknown)) {
					wrapper.SetWhiteBalance(value);
				}} 
			}
		#endregion

		#region Methods
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

		public void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> aOnImageAcquired)
		{
			if (!isReady || IsRequestingImage)
				return;

			GetImage((tex, transform) => {
				if (aOnImageAcquired != null)
					aOnImageAcquired(tex.GetRawTextureData<Color24>(), transform, tex.width, tex.height);
			});
		}
		public void RequestImage(Action<Texture, Matrix4x4> aOnImageAcquired)
		{
			if (!isReady || IsRequestingImage)
				return;

			GetImage((tex, transform) => {
				if (aOnImageAcquired != null)
					aOnImageAcquired(tex, transform);
			});
		}

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