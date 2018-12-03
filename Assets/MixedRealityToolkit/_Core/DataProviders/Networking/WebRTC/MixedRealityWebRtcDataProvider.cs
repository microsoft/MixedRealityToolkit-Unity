// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Networking.WebRTC
{
    /// <summary>
    /// The Mixed Reality Toolkit's Web RTC Data provider.
    /// </summary>
    public class MixedRealityWebRtcDataProvider : BaseDataProvider, IMixedRealityNetworkProvider
    {
        /// <inheritdoc />
        public MixedRealityWebRtcDataProvider(string name, uint priority) : base(name, priority) { }

        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName => Name;

        #region IEquality Implementation

        public static bool Equals(IMixedRealityNetworkProvider left, IMixedRealityNetworkProvider right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealityNetworkProvider)obj);
        }

        private bool Equals(IMixedRealityNetworkProvider other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        #endregion
    }
}