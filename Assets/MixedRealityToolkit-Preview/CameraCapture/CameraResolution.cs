// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.CameraCapture
{
	public enum NativeResolutionMode
	{
		Smallest,
		Largest,
		Closest
	}
	public enum ResizeWhen
	{
		IsLarger,
		IsSmaller,
		Always,
		Never
	}
	public enum PreserveAspectPriority
	{
		Width,
		Height
	}
	public class CameraResolution
	{
		public NativeResolutionMode   nativeResolution       = NativeResolutionMode.Smallest;
		public ResizeWhen             resize                 = ResizeWhen.IsLarger;
		public Vector2Int             size                   = new Vector2Int(-1,200);
		public bool                   preserveAspect         = true;
		public PreserveAspectPriority preserveAspectPriority = PreserveAspectPriority.Height;
		
		int               SizeDifference(Vector2Int aSize)
		{
			return Mathf.Abs(aSize.x*aSize.y - size.x*size.y);
		}
		public Vector2Int AdjustSize    (Vector2Int aSourceSize)
		{
			Vector2Int result = aSourceSize;
			if (preserveAspect)
			{
				if ((resize == ResizeWhen.Always) ||
					(aSourceSize.y > size.y && resize == ResizeWhen.IsLarger) ||
					(aSourceSize.y < size.y && resize == ResizeWhen.IsSmaller))
				{
					if (preserveAspectPriority == PreserveAspectPriority.Height)
						result = new Vector2Int((int)(size.y * (aSourceSize.x/(float)aSourceSize.y)), size.y);
					else if (preserveAspectPriority == PreserveAspectPriority.Width)
						result = new Vector2Int(size.x, (int)(size.x * (aSourceSize.y/(float)aSourceSize.x)));
					else
						throw new System.NotImplementedException();
				}
			}
			else
			{
				if ((resize == ResizeWhen.Always) ||
					((aSourceSize.x > size.x || aSourceSize.y > size.y) && resize == ResizeWhen.IsLarger) ||
					((aSourceSize.x < size.x || aSourceSize.y < size.y) && resize == ResizeWhen.IsSmaller))
				{
					result = new Vector2Int(size.x, size.y);
				}
			}
			return result;
		}
		public void       ResizeTexture (Texture    aSourceTexture, ref Texture aDestTexture, bool aEnsureRGB24)
		{
			Vector2Int sourceSize = new Vector2Int(aSourceTexture.width, aSourceTexture.height);
			Vector2Int destSize   = AdjustSize(sourceSize);

			// if source is already the right size and format, just pass it right back!
			bool isRGB24 = aSourceTexture is Texture2D && ((Texture2D)aSourceTexture).format == TextureFormat.RGB24;
			if (sourceSize == destSize && (!aEnsureRGB24 || isRGB24))
			{
				aDestTexture = aSourceTexture;
				return;
			}

			// Set up the size of the result texture if it needs it
			if (aDestTexture == null || !(aDestTexture is Texture2D) || aDestTexture.width != destSize.x || aDestTexture.height != destSize.y)
			{
				if (aDestTexture != null && aDestTexture is Texture2D)
					GameObject.Destroy(aDestTexture);
				
				aDestTexture = new Texture2D(destSize.x, destSize.y, TextureFormat.RGB24, false);
			}

			// Resize the image!
			RenderTexture rt = RenderTexture.GetTemporary(destSize.x, destSize.y, 0);
			Graphics.Blit(aSourceTexture, rt);
			RenderTexture.active = rt;
			((Texture2D)aDestTexture).ReadPixels(new Rect(0,0,aDestTexture.width,aDestTexture.height), 0, 0, false);
			((Texture2D)aDestTexture).Apply();
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(rt);
		}
	}
}