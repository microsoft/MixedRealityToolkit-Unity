#if USE_ARFOUNDATION

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

namespace CameraCapture
{
	public class CameraCaptureARFoundation : ICameraCapture
	{
		CameraResolution resolution;
		Texture2D        captureTex;
		bool             ready = false;

		public bool  IsReady           { get { return ready; } }
		public bool  IsRequestingImage { get { return false;  } }
		public float FieldOfView       { get { 
			Matrix4x4 proj = Matrix4x4.identity;
			ARSubsystemManager.cameraSubsystem.TryGetProjectionMatrix(ref proj);
			float     fov  = Mathf.Atan(1.0f / proj[1,1] ) * 2.0f * Mathf.Rad2Deg;
			return fov;
		} }

		public void Initialize(bool aPreferGPUTexture, CameraResolution aResolution, Action aOnInitialized)
		{
			resolution = aResolution;
			
			Action<ARSystemStateChangedEventArgs> handler = null;
			handler = (aState) => {
				// Camera and orientation data aren't ready until ARFoundation is actually tracking!
				if (aState.state == ARSystemState.SessionTracking) {
					ARSubsystemManager.systemStateChanged -= handler;
					ready = true;
					if (aOnInitialized != null)
						aOnInitialized();
				}
			};
			ARSubsystemManager.systemStateChanged += handler;
		}
		
		unsafe void GrabScreen()
		{
			// Grab the latest image from ARFoundation
			CameraImage image;
			if (!ARSubsystemManager.cameraSubsystem.TryGetLatestImage(out image))
			{
				Debug.LogError("[CameraCaptureARFoundation] Could not get latest image!");
				return;
			}
			
			// Set up resizing parameters
			Vector2Int size = resolution.AdjustSize(new Vector2Int( image.width, image.height ));
			var conversionParams = new CameraImageConversionParams {
				inputRect        = new RectInt(0, 0, image.width, image.height),
				outputDimensions = new Vector2Int(size.x, size.y),
				outputFormat     = TextureFormat.RGB24,
				transformation   = CameraImageTransformation.MirrorY
			};
			
			// make sure we have a texture to store the resized image
			if (captureTex == null || captureTex.width != size.x || captureTex.height != size.y)
			{
				if (captureTex != null)
					GameObject.Destroy(captureTex);
				captureTex = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
			}
			
			// And do the resize!
			var rawTextureData = captureTex.GetRawTextureData<byte>();
			image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
			image.Dispose();
			captureTex.Apply();
		}

		Matrix4x4 GetCamTransform()
		{
			Matrix4x4 matrix = Matrix4x4.identity;
			ARSubsystemManager.cameraSubsystem.TryGetDisplayMatrix(ref matrix);
			// This matrix transforms a 2D UV coordinate based on the device's orientation.
			// It will rotate, flip, but maintain values in the 0-1 range. This is technically
			// just a 3x3 matrix stored in a 4x4

			// These are the matrices provided in specific phone orientations:

			// 1 0 0 Landscape Left (upside down)
			// 0 1 0 The source image is upside down as well, so this is identity
			// 0 0 0 
			if (Mathf.RoundToInt(matrix[0,0]) == 1 && Mathf.RoundToInt(matrix[1,1]) == 1)
				matrix = Matrix4x4.Rotate( Quaternion.Euler(0,0,180) );

			//-1 0 1 Landscape Right
			// 0-1 1
			// 0 0 0
			else if (Mathf.RoundToInt(matrix[0,0]) == -1 && Mathf.RoundToInt(matrix[1,1]) == -1)
				matrix = Matrix4x4.identity;

			// 0 1 0 Portrait
			//-1 0 1
			// 0 0 0
			else if (Mathf.RoundToInt(matrix[0,1]) == 1 && Mathf.RoundToInt(matrix[1,0]) == -1)
				matrix = Matrix4x4.Rotate( Quaternion.Euler(0,0,90) );

			// 0-1 1 Portrait (upside down)
			// 1 0 0
			// 0 0 0
			else if (Mathf.RoundToInt(matrix[0,1]) == -1 && Mathf.RoundToInt(matrix[1,0]) == 1)
				matrix = Matrix4x4.Rotate( Quaternion.Euler(0,0,-90) );

			else Debug.LogWarningFormat("Unexpected Matrix provided from ARFoundation!\n{0}", matrix.ToString());
			
			return matrix * Camera.main.transform.localToWorldMatrix;
		}
		
		public void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> aOnImageAcquired)
		{
			GrabScreen();

			if (aOnImageAcquired != null)
				aOnImageAcquired(captureTex.GetRawTextureData<Color24>(), GetCamTransform(), captureTex.width, captureTex.height);
		}
		public void RequestImage(Action<Texture, Matrix4x4> aOnImageAcquired)
		{
			GrabScreen();

			if (aOnImageAcquired != null)
				aOnImageAcquired(captureTex, GetCamTransform());
		}

		public void Shutdown()
		{
			if (captureTex != null)
				GameObject.Destroy(captureTex);
		}
	}
}

#endif