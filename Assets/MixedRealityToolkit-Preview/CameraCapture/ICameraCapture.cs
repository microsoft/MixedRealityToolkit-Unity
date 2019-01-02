// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Unity.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.CameraCapture
{
	public interface ICameraCapture
	{
		bool  IsReady           { get; }
		bool  IsRequestingImage { get; }
		float FieldOfView       { get; }

		void Initialize(bool preferGPUTexture, CameraResolution resolution, Action onInitialized);
		void Shutdown();

		void RequestImage(Action<NativeArray<Color24>, Matrix4x4, int, int> onImageAcquired);
		void RequestImage(Action<Texture,              Matrix4x4> onImageAcquired);
	}
}