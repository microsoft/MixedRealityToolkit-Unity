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
    public abstract class TransformSpatialCoordinate<TKey> : DisposableBase, ISpatialCoordinate<TKey>, IPersistableSpatialCoordinate
    {
        public event Action StateChanged;

        protected GameObject gameObject;

        /// <summary>
        /// The Id for this coordinate.
        /// </summary>
        public TKey Id { get; }

        protected ISpatialCoordinatePersistenceStore<TKey> PersistenceStore { get; }

        /// <inheritdoc />
        public virtual LocatedState State
        {
            get
            {
                if (IsPersisted)
                {
                    WorldAnchor worldAnchor = gameObject.GetComponent<WorldAnchor>();
                    if (worldAnchor.isLocated)
                    {
                        return LocatedState.Tracking;
                    }
                    else
                    {
                        return LocatedState.Resolved;
                    }
                }
                else
                {
                    return LocatedState.Resolved;
                }
            }
        }

        public TransformSpatialCoordinate(ISpatialCoordinatePersistenceStore<TKey> persistenceStore, TKey id)
        {
            gameObject = new GameObject($"Marker Detector Spatial Anchor - {id}");

            PersistenceStore = persistenceStore;
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

        public virtual bool IsPersisted => PersistenceStore.IsPersisted(Id);

        public virtual void PersistCoordinate()
        {
            PersistenceStore.AddPersistedTransform(gameObject, Id);
        }

        public virtual void DepersistCoordinate()
        {
            PersistenceStore.RemovePersistedTransform(gameObject, Id);
        }
    }
}
