// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Base class for input sources that don't inherit from MonoBehaviour.
    /// </summary>
    public class GenericInputSource : IInputSource
    {
        public GenericInputSource(uint sourceId, string name)
        {
            SourceId = sourceId;
            Name = name;
        }

        public GenericInputSource(uint sourceId, string name, SupportedInputInfo _supportedInputInfo)
        {
            SourceId = sourceId;
            Name = name;
            supportedInputInfo = _supportedInputInfo;
        }

        public uint SourceId { get; private set; }

        public string Name { get; private set; }

        private SupportedInputInfo supportedInputInfo;

        public virtual SupportedInputInfo GetSupportedInputInfo()
        {
            return supportedInputInfo;
        }

        public bool SupportsInputInfo(SupportedInputInfo inputInfo)
        {
            return (GetSupportedInputInfo() & inputInfo) == inputInfo;
        }

        #region IEquality Implementation

        public static bool Equals(IInputSource left, IInputSource right)
        {
            return left.Equals(right);
        }

        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IInputSource)obj);
        }

        private bool Equals(IInputSource other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(Name, other.Name);
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