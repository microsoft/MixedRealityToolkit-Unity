// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Numerics;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.Common
{
    /// <summary>
    /// Helper base class for implementations of <see cref="ISpatialCoordinate"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of Id for this coordinate.</typeparam>
    public abstract class SpatialCoordinateUnityBase<TKey> : SpatialCoordinateBase<TKey>
    {
        protected UnityEngine.Matrix4x4 worldMatrix = UnityEngine.Matrix4x4.identity;

        public SpatialCoordinateUnityBase(TKey id) : base(id) { }

        public sealed override Vector3 CoordinateToWorldSpace(Vector3 vector) => CoordinateToWorldSpace(vector.AsUnityVector()).AsNumericsVector();


        public sealed override Quaternion CoordinateToWorldSpace(Quaternion quaternion) => CoordinateToWorldSpace(quaternion.AsUnityQuaternion()).AsNumericsQuaternion();

        public sealed override Vector3 WorldToCoordinateSpace(Vector3 vector) => WorldToCoordinateSpace(vector.AsUnityVector()).AsNumericsVector();

        public sealed override Quaternion WorldToCoordinateSpace(Quaternion quaternion) => WorldToCoordinateSpace(quaternion.AsUnityQuaternion()).AsNumericsQuaternion();

        protected void SetCoordinateWorldTransform(UnityEngine.Vector3 worldPosition, UnityEngine.Quaternion worldRotation)
        {
            worldMatrix = UnityEngine.Matrix4x4.TRS(worldPosition, worldRotation, UnityEngine.Vector3.one);
        }

        /// <inheritdoc />
        protected virtual UnityEngine.Vector3 CoordinateToWorldSpace(UnityEngine.Vector3 vector) => worldMatrix.MultiplyPoint(vector);

        /// <inheritdoc />
        protected virtual UnityEngine.Quaternion CoordinateToWorldSpace(UnityEngine.Quaternion quaternion) => worldMatrix.rotation * quaternion;

        /// <inheritdoc />
        protected virtual UnityEngine.Vector3 WorldToCoordinateSpace(UnityEngine.Vector3 vector) => worldMatrix.inverse.MultiplyPoint(vector);

        /// <inheritdoc />
        protected virtual UnityEngine.Quaternion WorldToCoordinateSpace(UnityEngine.Quaternion quaternion) => worldMatrix.inverse.rotation * quaternion;
    }
}
