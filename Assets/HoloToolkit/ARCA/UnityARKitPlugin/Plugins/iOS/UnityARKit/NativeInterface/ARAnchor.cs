using System;

namespace UnityEngine.XR.iOS
{
	public struct ARAnchor
	{
		public string identifier;

		/**
 		The transformation matrix that defines the anchor's rotation, translation and scale in world coordinates.
		 */
		public Matrix4x4 transform;
	}
}

