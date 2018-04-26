using System;

namespace UnityEngine.XR.iOS
{
	public struct ARHitTestResult
	{
		/**
		 The type of the hit-test result.
		 */
		public ARHitTestResultType type;

		/**
 		The distance from the camera to the intersection in meters.
		*/
		public double distance;

		/**
 		The transformation matrix that defines the intersection's rotation, translation and scale
 		relative to the anchor or nearest feature point.
 		*/
		public Matrix4x4 localTransform;

		/**
 		The transformation matrix that defines the intersection's rotation, translation and scale
 		relative to the world.
 		*/
		public Matrix4x4 worldTransform;

		/**
 		The anchor that the hit-test intersected.
 		*/
		public string anchorIdentifier;

		/**
		True if the test represents a valid hit test. Data is undefined otherwise.
		*/
		public bool isValid;
	}
}

