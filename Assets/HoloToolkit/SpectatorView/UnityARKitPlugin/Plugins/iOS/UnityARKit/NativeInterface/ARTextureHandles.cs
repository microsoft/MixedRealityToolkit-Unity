using System;

namespace UnityEngine.XR.iOS
{
	public struct ARTextureHandles
	{
		// Native (Metal) texture handles for the device camera buffer
		public IntPtr textureY;
		public IntPtr textureCbCr;
	}
}

