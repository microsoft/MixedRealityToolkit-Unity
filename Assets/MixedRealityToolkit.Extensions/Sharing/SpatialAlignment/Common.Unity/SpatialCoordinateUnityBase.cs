// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Numerics;

namespace Microsoft.MixedReality.SpatialAlignment.Common
{
    /// <summary>
    /// Helper base class for implementations of <see cref="ISpatialCoordinate"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of Id for this coordinate.</typeparam>
    public abstract class SpatialCoordinateUnityBase<TKey> : SpatialCoordinateBase<TKey>
    {
        public SpatialCoordinateUnityBase(TKey id) : base(id) { }

        public sealed override Vector3 CoordinateToWorldSpace(Vector3 vector) => CoordinateToWorldSpace(vector.AsUnityVector()).AsNumericsVector();


        public sealed override Quaternion CoordinateToWorldSpace(Quaternion quaternion) => CoordinateToWorldSpace(quaternion.AsUnityQuaternion()).AsNumericsQuaternion();

        public sealed override Vector3 WorldToCoordinateSpace(Vector3 vector) => CoordinateToWorldSpace(vector.AsUnityVector()).AsNumericsVector();

        public sealed override Quaternion WorldToCoordinateSpace(Quaternion quaternion) => CoordinateToWorldSpace(quaternion.AsUnityQuaternion()).AsNumericsQuaternion();

        /// <inheritdoc />
        protected abstract UnityEngine.Vector3 CoordinateToWorldSpace(UnityEngine.Vector3 vector);

        /// <inheritdoc />
        protected abstract UnityEngine.Quaternion CoordinateToWorldSpace(UnityEngine.Quaternion quaternion);

        /// <inheritdoc />
        protected abstract UnityEngine.Vector3 WorldToCoordinateSpace(UnityEngine.Vector3 vector);

        /// <inheritdoc />
        protected abstract UnityEngine.Quaternion WorldToCoordinateSpace(UnityEngine.Quaternion quaternion);
    }
}
