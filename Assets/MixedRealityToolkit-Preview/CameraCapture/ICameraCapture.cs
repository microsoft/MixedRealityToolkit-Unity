using System;
using Unity.Collections;
using UnityEngine;

namespace CameraCapture
{
	public interface ICameraCapture
	{
		bool  IsReady           { get; }
		bool  IsRequestingImage { get; }
		float FieldOfView       { get; }

		void Initialize(bool aPreferGPUTexture, CameraResolution aResolution, Action aOnInitialized);
		void Shutdown();

		void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> aOnImageAcquired);
		void RequestImage(Action<Texture,              Matrix4x4> aOnImageAcquired);
	}
}