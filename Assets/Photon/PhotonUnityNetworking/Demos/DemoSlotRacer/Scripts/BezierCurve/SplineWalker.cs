// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplineWalker.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Original: http://catlikecoding.com/unity/tutorials/curves-and-splines/
//  Used in SlotRacer Demo
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

namespace Photon.Pun.Demo.SlotRacer.Utils
{
	[ExecuteInEditMode]
	public class SplineWalker : MonoBehaviour {

		public BezierSpline spline;

		public float Speed = 0f;

		public bool lookForward;

		public bool reverse;

		private float progress;

		public float currentDistance =0f;

		public float currentClampedDistance;

		public void SetPositionOnSpline(float position)
		{
			currentDistance = position;
			ExecutePositioning ();
		}

		void Update()
		{
			// update the distance used.
			currentDistance += Speed * Time.deltaTime;
			ExecutePositioning ();
		}

		public void ExecutePositioning()
		{
			if(spline==null)
			{
				return;
			}
			// move the transform to the new point
			transform.position = spline.GetPositionAtDistance(currentDistance,this.reverse);

			// update the distance used.
			currentDistance += Speed * Time.deltaTime;


			if (lookForward) {
				transform.LookAt(spline.GetPositionAtDistance(currentDistance+1,this.reverse));
			}
		}
	}	
}