// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem
{
    /// <summary>
    /// Class poviding the default implementation of the <see cref="IMixedRealitySpatialAwarenessBaseDescription"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessBaseDescription : IMixedRealitySpatialAwarenessBaseDescription
    {
        /// <inheritdoc />
        public Vector3 Position { get; private set; }

        public MixedRealitySpatialAwarenessBaseDescription(Vector3 position)
        {
            Position = position;
        }

        #region IEqualityComparer implementation

        public static bool Equals(IMixedRealitySpatialAwarenessBaseDescription left, IMixedRealitySpatialAwarenessBaseDescription right)
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

            return Equals((IMixedRealitySpatialAwarenessBaseDescription)obj);
        }

        private bool Equals(IMixedRealitySpatialAwarenessBaseDescription other)
        {
            if (other == null) { return false; }

            return Position.Equals(other.Position);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            string s = $"Position({Position})";
            return s.GetHashCode();
        }

        #endregion IEqualityComparer implementation
    }
}
