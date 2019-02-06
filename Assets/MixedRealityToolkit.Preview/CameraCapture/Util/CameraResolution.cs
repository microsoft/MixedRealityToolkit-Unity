// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Preview.CameraCapture
{
	public enum NativeResolutionMode
	{
		/// <summary>
		/// Choose the smallest available resolution.
		/// </summary>
		Smallest,
		/// <summary>
		/// Pick the largest available resolution.
		/// </summary>
		Largest,
		/// <summary>
		/// Pick whatever resolution is closes to our provided size. Uses number of pixels, width*height to determine closeness.
		/// </summary>
		Closest
	}
	public enum ResizeWhen
	{
		/// <summary>
		/// Resize when the native image is larger than desired.
		/// </summary>
		IsLarger,
		/// <summary>
		/// Resize when the native image is smaller than desired.
		/// </summary>
		IsSmaller,
		/// <summary>
		/// Resize whenever sizes don't match desired.
		/// </summary>
		Always,
		/// <summary>
		/// Resize things? What? Madness!
		/// </summary>
		Never
	}
	public enum PreserveAspectPriority
	{
		/// <summary>
		/// Preserve the desired width when resizing and maintaining native aspect ratio.
		/// </summary>
		Width,
		/// <summary>
		/// Preserve the desired height when resizing and maintaining native aspect ratio.
		/// </summary>
		Height
	}
	/// <summary>
	/// A utility for dealing with a variety of different types of camera image inputs!
	/// Also can be used to resize images if they aren't up to your expectations.
	/// </summary>
	public class CameraResolution
	{
		/// <summary>
		/// When picking a picture mode for the camera, what's your preference?
		/// </summary>
		public NativeResolutionMode   nativeResolution       = NativeResolutionMode.Smallest;
		/// <summary>
		/// When should we resize the native picture to our specific chosen resolution?
		/// </summary>
		public ResizeWhen             resize                 = ResizeWhen.IsLarger;
		/// <summary>
		/// What's our preferred resolution?
		/// </summary>
		public Vector2Int             size                   = new Vector2Int(-1,200);
		/// <summary>
		/// Should we preserve the aspect ratio of the native image when resizing to our chosen resolution?
		/// </summary>
		public bool                   preserveAspect         = true;
		/// <summary>
		/// If we are preserving aspect ratio, do we prioritize keeping the height, or the width the same?
		/// </summary>
		public PreserveAspectPriority preserveAspectPriority = PreserveAspectPriority.Height;
		
		/// <summary> Calculate the difference between your size and our target resolution. </summary>
		/// <param name="aSize">Your resolution size (pixels).</param>
		/// <returns>The number of pixels different between the resolution sizes.</returns>
		private int       SizeDifference(Vector2Int aSize)
		{
			return Mathf.Abs(aSize.x*aSize.y - size.x*size.y);
		}
		/// <summary>
		/// Given an image size, what would these settings output?
		/// </summary>
		/// <param name="aSourceSize">Your image size</param>
		/// <returns>Resulting image size</returns>
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
					{
						result = new Vector2Int((int)(size.y * (aSourceSize.x/(float)aSourceSize.y)), size.y);
					}
					else if (preserveAspectPriority == PreserveAspectPriority.Width)
					{
						result = new Vector2Int(size.x, (int)(size.x * (aSourceSize.y/(float)aSourceSize.x)));
					}
					else
					{
						throw new System.NotImplementedException();
					}
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
		/// <summary>
		/// Take a texture, and resize it to our specifications. If no resizing occurs, you may get the exact same Texture object you sent in.
		/// </summary>
		/// <param name="aSourceTexture">What texture are we working with?</param>
		/// <param name="aDestTexture">And where shall we put it when we're done?</param>
		/// <param name="aEnsureRGB24">If it's not RGB24, should we convert it anyhow? This will cause an incorrect format texture to 'resize' even if it's the correct size already.</param>
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
				{
					GameObject.Destroy(aDestTexture);
				}
				
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