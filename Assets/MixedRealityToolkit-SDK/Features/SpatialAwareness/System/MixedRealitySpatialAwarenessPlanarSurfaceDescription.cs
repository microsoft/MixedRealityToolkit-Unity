// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem
{
    /// <summary>
    /// Class poviding the default implementation of the <see cref="IMixedRealitySpatialAwarenessPlanarSurfaceDescription"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessPlanarSurfaceDescription : MixedRealitySpatialAwarenessBaseDescription, IMixedRealitySpatialAwarenessPlanarSurfaceDescription
    {
        /// <inheritdoc />
        public Bounds Bounds { get; private set; }

        /// <inheritdoc />
        public Vector3 Normal { get; private set; }

        /// <inheritdoc />
        public SpatialAwarenessSurfaceTypes SurfaceType { get; private set; }

        public MixedRealitySpatialAwarenessPlanarSurfaceDescription(
            Vector3 position,
            Bounds bounds,
            Vector3 normal,
            SpatialAwarenessSurfaceTypes surfaceType) : base(position)
        {
            Bounds = bounds;
            Normal = normal;
            SurfaceType = surfaceType;
        }

        #region IEqualityComparer implementation

        public override string ToString()
        {
            return $"{base.ToString()}:{SurfaceType.ToString()}:Bounds({Bounds.min},{Bounds.max}):Normal({Normal.x},{Normal.y},{Normal.z}";
        }

        public static bool Equals(IMixedRealitySpatialAwarenessPlanarSurfaceDescription left, IMixedRealitySpatialAwarenessPlanarSurfaceDescription right)
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

            return Equals((IMixedRealitySpatialAwarenessPlanarSurfaceDescription)obj);
        }

        private bool Equals(IMixedRealitySpatialAwarenessPlanarSurfaceDescription other)
        {
            if (other == null) { return false; }
            if (!base.Equals(other)) { return false; }

            if (!Bounds.Equals(other.Bounds)) { return false; }
            if (!Normal.Equals(other.Normal)) { return false; }
            if (SurfaceType != other.SurfaceType) { return false; }

            return true;
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            string s = $"Position({Position}):{SurfaceType.ToString()}:Bounds({Bounds.min},{Bounds.max}):Normal({Normal.x},{Normal.y},{Normal.z}";
            return s.GetHashCode();
        }

        #endregion IEqualityComparer implementation
    }
}