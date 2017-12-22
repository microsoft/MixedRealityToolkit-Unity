// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MixedRealityToolkit.Common
{
    /// <summary>
    /// 3D integer class - operates similarly to Unity's Vector3D
    /// </summary>
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Int3 : IEquatable<Int3>, IFormattable
    {
        public static readonly Int3 zero = new Int3(0, 0, 0);
        public static readonly Int3 one = new Int3(1, 1, 1);

        public static readonly Int3 forward = new Int3(0, 0, 1);
        public static readonly Int3 back = new Int3(0, 0, -1);
        public static readonly Int3 up = new Int3(0, 1, 0);
        public static readonly Int3 down = new Int3(0, -1, 0);
        public static readonly Int3 left = new Int3(-1, 0, 0);
        public static readonly Int3 right = new Int3(1, 0, 0);

        public int x;
        public int y;
        public int z;

        public Int3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Int3(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public override int GetHashCode()
        {
            var hashCode = x.GetHashCode();
            hashCode = (hashCode * 397) ^ y.GetHashCode();
            hashCode = (hashCode * 397) ^ z.GetHashCode();
            return hashCode;
        }

        public int this[int index]
        {
            get
            {
                if (index == 0) return x;
                if (index == 1) return y;
                if (index == 2) return z;
                throw new ArgumentOutOfRangeException("Invalid Int3 index!");
            }
            set
            {
                if (index == 0) x = value;
                if (index == 1) y = value;
                if (index == 2) z = value;
                throw new ArgumentOutOfRangeException("Invalid Int3 index!");
            }
        }

        #region conversion
        public static explicit operator Vector3(Int3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static explicit operator Vector2(Int3 v)
        {
            return new Vector2(v.x, v.y);
        }
        #endregion

        #region math operations
        public float magnitude
        {
            get { return Magnitude(this); }
        }

        public int sqrMagnitude
        {
            get { return SqrMagnitude(this); }
        }

        public int dotOne
        {
            get { return DotOne(this); }
        }

        public float Magnitude(Int3 v)
        {
            return Mathf.Sqrt(SqrMagnitude(v));
        }

        public int SqrMagnitude(Int3 v)
        {
            return v.x * v.x + v.y * v.y + v.z * v.z;
        }

        public static Int3 Scale(Int3 l, Int3 r)
        {
            return new Int3(l.x * r.x, l.y * r.y, l.z * r.z);
        }

        public static int Dot(Int3 l, Int3 r)
        {
            return l.x * r.x + l.y * r.y + l.z * r.z;
        }

        public static int DotOne(Int3 v)
        {
            return v.x * v.y * v.z;
        }

        public static Int3 Cross(Int3 l, Int3 r)
        {
            return new Int3(l.y * r.z - l.z * r.y,
                            l.z * r.x - l.x * r.z,
                            l.x * r.y - l.y * r.x);
        }

        public static Int3 Min(Int3 l, Int3 r)
        {
            return new Int3(Mathf.Min(l.x, r.x), Mathf.Min(l.y, r.y), Mathf.Min(l.z, r.z));
        }

        public static Int3 Max(Int3 l, Int3 r)
        {
            return new Int3(Mathf.Max(l.x, r.x), Mathf.Max(l.y, r.y), Mathf.Max(l.z, r.z));
        }

        public static Int3 Clamp(Int3 v, Int3 min, Int3 max)
        {
            return new Int3(Mathf.Clamp(v.x, min.x, max.x),
                            Mathf.Clamp(v.y, min.y, max.y),
                            Mathf.Clamp(v.z, min.z, max.z));
        }

        public static Int3 ClosestPowerOfTwo(Int3 v)
        {
            return new Int3(Mathf.ClosestPowerOfTwo(v.x), Mathf.ClosestPowerOfTwo(v.y), Mathf.ClosestPowerOfTwo(v.z));
        }

        public static int CubicToLinearIndex(Int3 v, Int3 size)
        {
            return (v.x) +
                   (v.y * size.x) +
                   (v.z * size.x * size.y);
        }

        public static Int3 LinearToCubicIndex(int v, Int3 size)
        {
            return new Int3(v % size.x,
                           (v / size.x) % size.y,
                           (v / (size.x * size.y)) % size.z);
        }
        #endregion math operations

        #region math operators
        public static Int3 operator +(Int3 l, Int3 r)
        {
            return new Int3(l.x + r.x, l.y + r.y, l.z + r.z);
        }

        public static Int3 operator -(Int3 l, Int3 r)
        {
            return new Int3(l.x - r.x, l.y - r.y, l.z - r.z);
        }

        public static Int3 operator -(Int3 v)
        {
            return new Int3(-v.x, -v.y, -v.z);
        }

        public static Int3 operator *(Int3 v, int d)
        {
            return new Int3(v.x * d, v.y * d, v.z * d);
        }

        public static Int3 operator *(int d, Int3 v)
        {
            return new Int3(v.x * d, v.y * d, v.z * d);
        }

        public static Int3 operator /(Int3 v, int d)
        {
            return new Int3(v.x / d, v.y / d, v.z / d);
        }
        #endregion math operators

        #region comparison
        public bool Equals(Int3 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public static bool operator ==(Int3 l, Int3 r)
        {
            return l.Equals(r);
        }

        public static bool operator !=(Int3 l, Int3 r)
        {
            return !l.Equals(r);
        }

        public override bool Equals(object value)
        {
            return (value is Int3) ? Equals((Int3)value) : false;
        }
        #endregion

        #region IFormattable
        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2}", x, y, z);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                return ToString(formatProvider);
            }

            return string.Format(formatProvider,
                                 "X:{0} Y:{1} Z:{2}",
                                 x.ToString(format, formatProvider),
                                 y.ToString(format, formatProvider),
                                 z.ToString(format, formatProvider));
        }
        #endregion
    }
}