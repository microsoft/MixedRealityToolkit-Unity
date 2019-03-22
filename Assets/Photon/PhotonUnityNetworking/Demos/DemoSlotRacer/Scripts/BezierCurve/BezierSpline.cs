// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bezier.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Original: http://catlikecoding.com/unity/tutorials/curves-and-splines/
//  Used in SlotRacer Demo
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

using System;

namespace Photon.Pun.Demo.SlotRacer.Utils
{
	public class BezierSpline : MonoBehaviour
	{
		[SerializeField]
		private Vector3[] points;

		[SerializeField]
		private float[] lengths;

		[SerializeField]
		private float[] lengthsTime;

		public float TotalLength;

		[SerializeField]
		private BezierControlPointMode[] modes;

		[SerializeField]
		private bool loop;

		public bool Loop
		{
			get {
				return loop;
			}
			set {
				loop = value;
				if (value == true) {
					modes[modes.Length - 1] = modes[0];
					SetControlPoint(0, points[0]);
				}
			}
		}

		public int ControlPointCount
		{
			get {
				return points.Length;
			}
		}


		void Awake()
		{
			this.ComputeLengths();

		}
		public Vector3 GetControlPoint(int index)
		{
			return points[index];
		}

		public void SetControlPoint(int index, Vector3 point)
		{
			if (index % 3 == 0)
			{
				Vector3 delta = point - points[index];
				if (loop)
				{
					if (index == 0)
					{
						points[1] += delta;
						points[points.Length - 2] += delta;
						points[points.Length - 1] = point;
					}
					else if (index == points.Length - 1)
					{
						points[0] = point;
						points[1] += delta;
						points[index - 1] += delta;
					}
					else
					{
						points[index - 1] += delta;
						points[index + 1] += delta;
					}
				}
				else
				{
					if (index > 0)
					{
						points[index - 1] += delta;
					}
					if (index + 1 < points.Length)
					{
						points[index + 1] += delta;
					}
				}
			}
			points[index] = point;
			EnforceMode(index);
		}

		public BezierControlPointMode GetControlPointMode(int index)
		{
			return modes[(index + 1) / 3];
		}

		public void SetControlPointMode(int index, BezierControlPointMode mode)
		{
			int modeIndex = (index + 1) / 3;
			modes[modeIndex] = mode;
			if (loop)
			{
				if (modeIndex == 0) {
					modes[modes.Length - 1] = mode;
				}
				else if (modeIndex == modes.Length - 1) {
					modes[0] = mode;
				}
			}
			EnforceMode(index);
		}

		private void EnforceMode(int index)
		{
			int modeIndex = (index + 1) / 3;
			BezierControlPointMode mode = modes[modeIndex];
			if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
			{
				return;
			}

			int middleIndex = modeIndex * 3;
			int fixedIndex, enforcedIndex;
			if (index <= middleIndex)
			{
				fixedIndex = middleIndex - 1;
				if (fixedIndex < 0)
				{
					fixedIndex = points.Length - 2;
				}
				enforcedIndex = middleIndex + 1;
				if (enforcedIndex >= points.Length)
				{
					enforcedIndex = 1;
				}
			}else
			{
				fixedIndex = middleIndex + 1;
				if (fixedIndex >= points.Length)
				{
					fixedIndex = 1;
				}
				enforcedIndex = middleIndex - 1;
				if (enforcedIndex < 0)
				{
					enforcedIndex = points.Length - 2;
				}
			}

			Vector3 middle = points[middleIndex];
			Vector3 enforcedTangent = middle - points[fixedIndex];
			if (mode == BezierControlPointMode.Aligned)
			{
				enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
			}
			points[enforcedIndex] = middle + enforcedTangent;
		}

		public int CurveCount
		{
			get {
				return (points.Length - 1) / 3;
			}
		}

		public Vector3 GetPoint(float t)
		{
			int i;
			if (t >= 1f)
			{
				t = 1f;
				i = points.Length - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * CurveCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}
			return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
		}
		
		public Vector3 GetVelocity(float t)
		{
			int i;
			if (t >= 1f)
			{
				t = 1f;
				i = points.Length - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * CurveCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}
			return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
		}
		
		public Vector3 GetDirection(float t)
		{
			return GetVelocity(t).normalized;
		}

		public void AddCurve ()
		{
			Vector3 point = points[points.Length - 1];
			Array.Resize(ref points, points.Length + 3);
			point.x += 1f;
			points[points.Length - 3] = point;
			point.x += 1f;
			points[points.Length - 2] = point;
			point.x += 1f;
			points[points.Length - 1] = point;

			Array.Resize(ref modes, modes.Length + 1);
			modes[modes.Length - 1] = modes[modes.Length - 2];
			EnforceMode(points.Length - 4);

			if (loop)
			{
				points[points.Length - 1] = points[0];
				modes[modes.Length - 1] = modes[0];
				EnforceMode(0);
			}
		}
			
		public void Reset()
		{
			points = new Vector3[] {
				new Vector3(1f, 0f, 0f),
				new Vector3(2f, 0f, 0f),
				new Vector3(3f, 0f, 0f),
				new Vector3(4f, 0f, 0f)
			};
			modes = new BezierControlPointMode[] {
				BezierControlPointMode.Free,
				BezierControlPointMode.Free
			};
		}

		public void ComputeLengths()
		{
			int subDivisions = 100;

			int totalSamples = points.Length * subDivisions;

			// lets create lengths for each control point.
			this.lengths = new float[totalSamples];
			this.lengthsTime = new float[totalSamples];

			float totalDistance = 0;
			float CurrentTime = 0f;
		
			Vector3 pos;
			Vector3 lastPos = this.GetPoint (0f);
			// go from the first, to the second to last
			for (var i = 0; i < totalSamples - 1; i++)
			{
				CurrentTime = (1f * i) / totalSamples;
				pos = this.GetPoint (CurrentTime);

				float _delta = (pos - lastPos).magnitude;

				totalDistance += _delta ;
				this.lengths [i] = totalDistance;

				this.lengthsTime [i] = CurrentTime;
				lastPos = pos;
			}

			this.TotalLength = totalDistance;

		}

		public Vector3 GetPositionAtDistance(float distance,bool reverse = false)
		{
			if (reverse)
			{
				distance = this.TotalLength - distance;
			}

			distance = Mathf.Repeat (distance, this.TotalLength);

			// make sure that we are within the total distance of the points
			if(distance <= 0) return points[0];
			if(distance >= this.TotalLength) return points[points.Length - 1];

			// lets find the first point that is below the distance
			// but, who's next point is above the distance
			var index = 0;
			while (index < lengths.Length -1 && lengths[index] < distance)
				index++;

		//	Debug.Log("Index ="+index);

			// get the percentage of travel from the current length to the next
			// where the distance is.
			//var deltaAmount = Mathf.InverseLerp(lengths[index-1], lengths[index], distance);

			float deltaDistanceRatio =  (distance-lengths[index-1])/(lengths [index] - lengths [index - 1]) ;

			float deltaTime = (lengthsTime [index] - lengthsTime [index - 1]) * deltaDistanceRatio;
			//float splineDistance = (lengths [index - 1] + (lengths [index] - lengths [index - 1]) * amount) / this.TotalLength;


			return GetPoint(this.lengthsTime[index]+deltaTime);
			// we use that, to get the actual point
		//	return Vector3.Lerp(points[index-1], points[index], amount);
		}
	}
}