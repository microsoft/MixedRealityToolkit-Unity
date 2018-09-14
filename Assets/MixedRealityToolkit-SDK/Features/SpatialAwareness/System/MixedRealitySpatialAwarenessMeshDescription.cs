// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem
{
    /// <summary>
    /// Class poviding the default implementation of the <see cref="IMixedRealitySpatialAwarenessMeshDescription"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessMeshDescription : MixedRealitySpatialAwarenessBaseDescription, IMixedRealitySpatialAwarenessMeshDescription
    {
        /// <inheritdoc />
        public MeshFilter MeshData {get; private set;}

        public MixedRealitySpatialAwarenessMeshDescription(Vector3 position, MeshFilter meshData) : base(position)
        {
            MeshData = meshData;
        }

        #region IEqualityComparer implementation

        public static bool Equals(IMixedRealitySpatialAwarenessMeshDescription left, IMixedRealitySpatialAwarenessMeshDescription right)
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

            return Equals((IMixedRealitySpatialAwarenessMeshDescription)obj);
        }

        private bool Equals(IMixedRealitySpatialAwarenessMeshDescription other)
        {
            if (other == null) { return false; }
            if (!base.Equals(other)) { return false; }

            return MeshData.Equals(other.MeshData);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            string s = $"Position({Position}):MeshHash({MeshData.GetHashCode()}";
            return s.GetHashCode();
        }

        #endregion IEqualityComparer implementation
    }
}
