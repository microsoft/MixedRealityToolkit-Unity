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
		private int  [] v;
		private float[] r;
		private float[] g;
		private float[] b;
		private float total = 0;
		private int   buckets;
		
		/// <param name="aBuckets">How many slots will we store values in? On a normal histogram picture, you can imagine this as "How many vertical bars do I want?". More gives you more granularity, but more than 255 would probably be pointless.</param>
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
		
		/// <summary>
		/// Add a color entry to the histogram!
		/// </summary>
		/// <param name="r">Red 0-1 please!</param>
		/// <param name="g">Green 0-1 please!</param>
		/// <param name="b">Blue 0-1 please!</param>
		/// <param name="gray">The Lightness/Value/Greyscale that corresponds with this color! (r*0.2126f + g*0.7152f + b*0.0722f is a pretty good one)</param>
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
		/// Add a color entry to the histogram!
		/// </summary>
		/// <param name="r">Red 0-255 please!</param>
		/// <param name="g">Green 0-255 please!</param>
		/// <param name="b">Blue 0-255 please!</param>
		/// <param name="gray">The Lightness/Value/Greyscale 0-1 that corresponds with this color! ((r/255)*0.2126f + (g/255)*0.7152f + (b/255)*0.0722f is a pretty good one)</param>
		public void Add(byte r, byte g, byte b, float gray)
		{
			int bucket = (int)(gray * buckets);
			v[bucket] += 1;
			this.r[bucket] += r/255f;
			this.g[bucket] += g/255f;
			this.b[bucket] += b/255f;
			total += 1;
		}

		/// <summary>
		/// Finds the location on the histogram where a certain percentage of brightness is below
		/// </summary>
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