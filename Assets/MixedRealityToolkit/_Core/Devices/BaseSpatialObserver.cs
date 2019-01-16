// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Devices
{
    public abstract class BaseSpatialObserver : BaseExtensionService, IMixedRealitySpatialAwarenessObserver
    {
        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            return x.Equals(y);
        }

        /// <inheritdoc /> 
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealitySpatialAwarenessObserver)obj);
        }

        private bool Equals(IMixedRealitySpatialAwarenessObserver other)
        {
            return ((other != null) &&
                (SourceId == other.SourceId) &&
                string.Equals(SourceName, other.SourceName));
        }

        /// <inheritdoc />
        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc /> 
        public override int GetHashCode()
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }

        /// <inheritdoc />
        public uint SourceId { get; protected set; }

        /// <inheritdoc />
        public string SourceName { get; protected set; }

        #endregion IMixedRealityEventSource Implementation

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public BaseSpatialObserver(string name, uint priority) : base(name, priority)
        {
            SourceId = MixedRealityToolkit.SpatialAwarenessSystem.GenerateNewSourceId();
            SourceName = name;
        }

        #region IMixedRealitySpatialAwarenessObserver implementation

        /// <inheritdoc />
        public AutoStartBehavior StartupBehavior { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        
        /// <inheritdoc />
        public int DefaultPhysicsLayer { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <inheritdoc />
        public int DefaultPhysicsLayerMask => throw new System.NotImplementedException();

        /// <inheritdoc />
        public bool IsRunning => throw new System.NotImplementedException();

        /// <inheritdoc />
        public bool IsStationaryObserver { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <inheritdoc />
        public Vector3 ObservationExtents { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <inheritdoc />
        public Vector3 ObserverOrigin { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <inheritdoc />
        public float UpdateInterval { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


        /// <inheritdoc />
        public abstract void Resume();

        /// <inheritdoc />
        public abstract void Suspend();

        protected abstract BaseSpatialAwarenessObject CreateSpatialObject();

        /// <summary>
        /// Cleans up objects managed by the observer.
        /// </summary>
        protected abstract void CleanUpSpatialObjectList();

        /// <summary>
        /// Clean up the resources associated with the surface.
        /// </summary>
        protected abstract void CleanUpSpatialObject(BaseSpatialAwarenessObject spatialObject, bool destroyGameObject = true);

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}