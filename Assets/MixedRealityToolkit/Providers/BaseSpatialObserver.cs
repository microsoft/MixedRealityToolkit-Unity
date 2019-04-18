// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public class BaseSpatialObserver : BaseDataProvider, IMixedRealitySpatialAwarenessObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the observer.</param>
        /// <param name="spatialAwarenessSystem">The <see cref="SpatialAwareness.IMixedRealitySpatialAwarenessSystem"/> to which the observer is providing data.</param>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <param name="priority">The registration priority of the data provider.</param>
        /// <param name="profile">The configuration profile for the data provider.</param>
        public BaseSpatialObserver(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority, 
            BaseMixedRealityProfile profile = null) : base(registrar, spatialAwarenessSystem, name, priority, profile)
        {
            if (MixedRealityToolkit.SpatialAwarenessSystem != null)
            {
                SourceId = MixedRealityToolkit.SpatialAwarenessSystem.GenerateNewSourceId();
            }
            else
            {
                Debug.LogError($"A spatial observer is registered in your service providers profile, but the spatial awareness system is turned off. Please either turn on spatial awareness or remove {name}.");
            }

            SourceName = name;
        }

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
        public uint SourceId { get; }

        /// <inheritdoc />
        public string SourceName { get; }

        #endregion IMixedRealityEventSource Implementation

        #region IMixedRealitySpatialAwarenessObserver implementation

        /// <inheritdoc />
        public AutoStartBehavior StartupBehavior { get; set; } = AutoStartBehavior.AutoStart;

        /// <inheritdoc />
        public int DefaultPhysicsLayer { get; } = 31;

        /// <inheritdoc />
        public bool IsRunning { get; protected set; } = false;

        /// <inheritdoc />
        public bool IsStationaryObserver { get; set; } = false;

        /// <inheritdoc />
        public Quaternion ObserverRotation { get; set; } = Quaternion.identity;

        public Vector3 ObserverOrigin { get; set; } = Vector3.zero;

        /// <inheritdoc />
        public VolumeType ObserverVolumeType { get; set; } = VolumeType.AxisAlignedCube;

        /// <inheritdoc />
        public Vector3 ObservationExtents { get; set; } = Vector3.one * 3f; // 3 meter sides / radius

        /// <inheritdoc />
        public float UpdateInterval { get; set; } = 3.5f; // 3.5 seconds

        /// <inheritdoc />
        public virtual void Resume() { }

        /// <inheritdoc />
        public virtual void Suspend() { }

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}
