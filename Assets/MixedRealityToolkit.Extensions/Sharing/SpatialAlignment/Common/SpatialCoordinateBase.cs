// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Numerics;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.Common
{
    /// <summary>
    /// Helper base class for implementations of <see cref="ISpatialCoordinate"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of Id for this coordinate.</typeparam>
    public abstract class SpatialCoordinateBase<TKey> : ISpatialCoordinate
    {
        public event Action StateChanged;

        private readonly string stringId;

        /// <inheritdoc />
        string ISpatialCoordinate.Id => stringId;

        /// <summary>
        /// The Id for this coordinate.
        /// </summary>
        public TKey Id { get; }

        /// <inheritdoc />
        public virtual LocatedState State { get { return LocatedState.Resolved; } }

        public SpatialCoordinateBase(TKey id)
        {
            stringId = id.ToString();
            Id = id;
        }

        protected void OnStateChanged() => StateChanged?.Invoke();

        /// <inheritdoc />
        public abstract Vector3 CoordinateToWorldSpace(Vector3 vector);

        /// <inheritdoc />
        public abstract Quaternion CoordinateToWorldSpace(Quaternion quaternion);

        /// <inheritdoc />
        public abstract Vector3 WorldToCoordinateSpace(Vector3 vector);

        /// <inheritdoc />
        public abstract Quaternion WorldToCoordinateSpace(Quaternion quaternion);
    }
}
