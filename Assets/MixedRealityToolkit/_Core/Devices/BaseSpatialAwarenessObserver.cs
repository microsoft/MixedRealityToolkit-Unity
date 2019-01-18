// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Devices
{
    public abstract class BaseSpatialAwarenessObserver : BaseExtensionService, IMixedRealitySpatialAwarenessObserver
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
        public BaseSpatialAwarenessObserver(string name, uint priority) : base(name, priority)
        {
            SourceId = MixedRealityToolkit.SpatialAwarenessSystem.GenerateNewSourceId();
            SourceName = name;
        }

        #region IMixedRealitySpatialAwarenessObserver implementation

        /// <inheritdoc />
        public AutoStartBehavior StartupBehavior { get; set; }

        /// <inheritdoc />
        public int DefaultPhysicsLayer { get; set; }

        /// <inheritdoc />
        public int DefaultPhysicsLayerMask { get; set; }

        /// <inheritdoc />
        public bool IsRunning { get; set; }

        /// <inheritdoc />
        public bool IsStationaryObserver { get; set; }

        /// <inheritdoc />
        public VolumeType ObserverVolumeType { get; set; }

        /// <inheritdoc />
        public Vector3 ObservationExtents { get; set; }

        /// <inheritdoc />
        public Vector3 ObserverOrigin { get; set; }

        /// <inheritdoc />
        public float ObserverRadius { get; set; }

        /// <inheritdoc />
        public float UpdateInterval { get; set; }

        ///<inheritdoc />
        public bool RecalculateNormals { get; set; }

        /// <inheritdoc />
        public abstract void Resume();

        /// <inheritdoc />
        public abstract void Suspend();

        /// <summary>
        /// Cleans up objects managed by the observer.
        /// </summary>
        protected abstract void CleanUpSpatialObjectList();

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}