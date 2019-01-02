using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Histogram {
	int[] _v;
	float[] _r;
	float[] _g;
	float[] _b;
	float _total = 0;
	int   _buckets;

	public int BucketCount { get { return _v.Length; } }
	public float Max { get {
		int max = _v[0];
		for (int i = 1; i < _v.Length; i++) {
			if (_v[i] > max) max = _v[i];
			//if (_r[i] > max) max = _r[i];
			//if (_g[i] > max) max = _g[i];
			//if (_b[i] > max) max = _b[i];
		}
		return max / _total;
	} }
	public Color GetBucket(int aBucketId) { return new Color(_r[aBucketId]/_total, _g[aBucketId]/_total, _b[aBucketId]/_total, _v[aBucketId]/_total); }
	public int GetBucketId(float aPercent) { return (int)(aPercent * _buckets); }

	public Histogram(int aBuckets = 10) {
		_buckets  = aBuckets-1;

		_v = new int  [aBuckets];
		_r = new float[aBuckets];
		_g = new float[aBuckets];
		_b = new float[aBuckets];
	}

	public void Clear() {
		for (int i = 0; i < _v.Length; i++) {
			_v[i] = 0;
			_r[i] = 0;
			_g[i] = 0;
			_b[i] = 0;
		}
		_total = 0;
	}

	public void Add(Color aColor) {
		float   gray = aColor.r*0.2126f + aColor.g*0.7152f + aColor.b*0.0722f;
		Add(aColor.r, aColor.g, aColor.b, gray);
	}
	public void Add(Color32 aColor) {
		float   r    = aColor.r/255f, g = aColor.g/255f, b = aColor.b/255f;
		float   gray = r*0.2126f + g*0.7152f + b*0.0722f;
		Add(r, g, b, gray);
	}
	public void Add(float r, float g, float b, float gray) {
		int bucket = (int)(gray * _buckets);
		_v[bucket] += 1;
		_r[bucket] += r;
		_g[bucket] += g;
		_b[bucket] += b;
		_total += 1;
	}

	/// <summary>
	/// Finds the location on the histogram where a certain percentage of brightness is below
	/// </summary>
	/// <param name="aPercent"></param>
	/// <returns></returns>
	public float FindPercentage(float aPercent) {
		int curr = 0;
		
		for (int i = 0; i < _v.Length; i++) {
			curr += _v[i];
			float percent = curr/_total;
			if (percent >= aPercent)
				return i / (float)_buckets;
		}
		return 1;
	}

	public Color GetColor(float aRangeMin, float aRangeMax) {
		int minI = (int)(aRangeMin * _buckets);
		int maxI = (int)(aRangeMax * _buckets);

		Vector3 result = Vector3.zero;
		for (int i = minI; i <= maxI; i++) {
			result.x += _r[i];
			result.y += _g[i];
			result.z += _b[i];
		}
		result.Normalize();
		return new Color(result.x, result.y, result.z, 1);
	}

	public override string ToString() {
		string result = "Hist: ";
		for (int i = 0; i < _v.Length; i++) {
			result += (int)((_v[i] / _total) * 100) + " ";
		}
		return result;
	}
}
