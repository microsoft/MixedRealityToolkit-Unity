// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Base class for input sources that inherit from MonoBehaviour.
    /// </summary>
    public abstract class BaseInputSource : MonoBehaviour, IInputSource
    {
        public uint SourceId { get; protected set; }

        public string Name { get; protected set; }

        public abstract SupportedInputInfo GetSupportedInputInfo();

        public bool SupportsInputInfo(SupportedInputInfo inputInfo)
        {
            return (GetSupportedInputInfo() & inputInfo) == inputInfo;
        }

        #region IEquality Implementation

        public static bool operator ==(BaseInputSource left, BaseInputSource right)
        {
            if (((object)left) == null || ((object)right) == null)
            {
                return Equals(left, right);
            }

            return left.Equals(right);
        }

        public static bool operator !=(BaseInputSource left, BaseInputSource right)
        {
            return !(left == right);
        }

        private bool Equals(IInputSource other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IInputSource)obj);
        }

        public static bool Equals(IInputSource left, IInputSource right)
        {
            return left.SourceId == right.SourceId;
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            var left = (IInputSource)x;
            var right = (IInputSource)y;
            if (left != null && right != null)
            {
                return Equals(left, right);
            }

            return false;
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation
    }
}
