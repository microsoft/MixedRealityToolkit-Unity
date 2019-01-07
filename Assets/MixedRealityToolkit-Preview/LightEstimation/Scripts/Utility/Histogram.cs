// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.LightEstimation
{
	/// <summary>
	/// A handy tool for working with an image's overall colors!
	/// </summary>
	public class Histogram
	{
		int  [] v;
		float[] r;
		float[] g;
		float[] b;
		float total = 0;
		int   buckets;

		public int   BucketCount { get { return v.Length; } }
		public float Max { get 
		{
			int max = v[0];
			for (int i = 1; i < v.Length; i++)
			{
				if (v[i] > max) max = v[i];
			}
			return max / total;
		} }
		public Color GetBucket(int bucketId) { return new Color(r[bucketId]/total, g[bucketId]/total, b[bucketId]/total, v[bucketId]/total); }
		public int   GetBucketId(float percent) { return (int)(percent * buckets); }

		public Histogram(int aBuckets = 10)
		{
			buckets  = aBuckets-1;

			v = new int  [aBuckets];
			r = new float[aBuckets];
			g = new float[aBuckets];
			b = new float[aBuckets];
		}

		/// <summary>
		/// Clear out allll the data! Fresh histogram, so you can start it again on a new one :)
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < v.Length; i++)
			{
				v[i] = 0;
				r[i] = 0;
				g[i] = 0;
				b[i] = 0;
			}
			total = 0;
		}

		public void Add(Color color)
		{
			float   gray = color.r*0.2126f + color.g*0.7152f + color.b*0.0722f;
			Add(color.r, color.g, color.b, gray);
		}
		public void Add(Color32 color)
		{
			float   r    = color.r/255f, g = color.g/255f, b = color.b/255f;
			float   gray = r*0.2126f + g*0.7152f + b*0.0722f;
			Add(r, g, b, gray);
		}
		public void Add(float r, float g, float b, float gray)
		{
			int bucket = (int)(gray * buckets);
			v[bucket] += 1;
			this.r[bucket] += r;
			this.g[bucket] += g;
			this.b[bucket] += b;
			total += 1;
		}

		/// <summary>
		/// Finds the location on the histogram where a certain percentage of brightness is below
		/// </summary>
		/// <param name="percent"></param>
		/// <returns></returns>
		public float FindPercentage(float percent)
		{
			int curr = 0;
		
			for (int i = 0; i < v.Length; i++)
			{
				curr += v[i];
				float currPercent = curr/total;
				if (currPercent >= percent)
					return i / (float)buckets;
			}
			return 1;
		}

		/// <summary>
		/// Gets the average color of the histogram between the givel lightness range!
		/// </summary>
		public Color GetColor(float rangeMin, float rangeMax)
		{
			int minI = (int)(rangeMin * buckets);
			int maxI = (int)(rangeMax * buckets);

			Vector3 result = Vector3.zero;
			for (int i = minI; i <= maxI; i++)
			{
				result.x += r[i];
				result.y += g[i];
				result.z += b[i];
			}
			result.Normalize();
			return new Color(result.x, result.y, result.z, 1);
		}

		public override string ToString()
		{
			string result = "Hist: ";
			for (int i = 0; i < v.Length; i++)
			{
				result += (int)((v[i] / total) * 100) + " ";
			}
			return result;
		}
	}
}