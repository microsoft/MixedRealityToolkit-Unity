// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using NumericsVector3 = System.Numerics.Vector3;
using NumericsQuaternion = System.Numerics.Quaternion;
using UnityEngine.XR.WSA;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.Common
{
    /// <summary>
    /// Helper base class for implementations of <see cref="ISpatialCoordinate"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of Id for this coordinate.</typeparam>
    public abstract class TransformSpatialCoordinate<TKey> : DisposableBase, ISpatialCoordinate<TKey>
    {
        public event Action StateChanged;

        protected GameObject gameObject;

        /// <summary>
        /// The Id for this coordinate.
        /// </summary>
        public TKey Id { get; }

        /// <inheritdoc />
        public virtual LocatedState State => LocatedState.Resolved;

        public TransformSpatialCoordinate(TKey id)
        {
            gameObject = new GameObject($"Marker Detector Spatial Anchor - {id}");

            Id = id;
        }

        protected void OnStateChanged() => StateChanged?.Invoke();

        /// <inheritdoc />
        public virtual NumericsVector3 CoordinateToWorldSpace(NumericsVector3 vector) => gameObject.transform.TransformPoint(vector.AsUnityVector()).AsNumericsVector();

        /// <inheritdoc />
        public virtual NumericsQuaternion CoordinateToWorldSpace(NumericsQuaternion quaternion) => gameObject.transform.rotation.AsNumericsQuaternion() * quaternion;

        /// <inheritdoc />
        public virtual NumericsVector3 WorldToCoordinateSpace(NumericsVector3 vector) => gameObject.transform.InverseTransformPoint(vector.AsUnityVector()).AsNumericsVector();

        /// <inheritdoc />
        public virtual NumericsQuaternion WorldToCoordinateSpace(NumericsQuaternion quaternion) => Quaternion.Inverse(gameObject.transform.rotation).AsNumericsQuaternion() * quaternion;
    }
}
