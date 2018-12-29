// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers
{
    /// <summary>
    /// Base <see cref="IMixedRealitySpatialObserverDataProvider"/> implementation
    /// </summary>
    public abstract class BaseMixedRealitySpatialObserverDataProvider : BaseDataProvider, IMixedRealitySpatialObserverDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        protected BaseMixedRealitySpatialObserverDataProvider(string name, uint priority, BaseMixedRealitySpatialObserverProfile profile) : base(name, priority)
        {
            if (profile == null)
            {
                throw new ArgumentNullException($"Missing profile for {name}");
            }

            if (MixedRealityToolkit.SpatialAwarenessSystem == null)
            {
                throw new Exception("Missing Spatial Awareness System!");
            }

            SourceId = MixedRealityToolkit.SpatialAwarenessSystem.GenerateNewObserverId();
            StartupBehavior = profile.StartupBehavior;
            UpdateInterval = profile.UpdateInterval;
            PhysicsLayer = profile.PhysicsLayer;
        }

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            MixedRealityToolkit.SpatialAwarenessSystem?.RaiseSpatialAwarenessObserverDetected(this);

            if (StartupBehavior == AutoStartBehavior.AutoStart)
            {
                StartObserving();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            StopObserving();

            MixedRealityToolkit.SpatialAwarenessSystem?.RaiseSpatialAwarenessObserverLost(this);
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealitySpatialObserverDataProvider Implementation

        /// <inheritdoc />
        public AutoStartBehavior StartupBehavior { get; }

        /// <inheritdoc />
        public float UpdateInterval { get; set; }

        /// <inheritdoc />
        public virtual int PhysicsLayer { get; }

        /// <inheritdoc />
        public bool IsRunning { get; protected set; }

        /// <inheritdoc />
        public virtual void StartObserving()
        {
            IsRunning = true;
        }

        /// <inheritdoc />
        public virtual void StopObserving()
        {
            IsRunning = false;
        }

        #endregion IMixedRealitySpatialObserverDataProvider Implementation

        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        public string SourceName => Name;

        /// <inheritdoc />
        public uint SourceId { get; }

        #endregion IMixedRealityEventSource Implementation

        #region IEquality Implementation

        /// <summary>
        /// Determines if the specified objects are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Equals(IMixedRealitySpatialObserverDataProvider left, IMixedRealitySpatialObserverDataProvider right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealitySpatialObserverDataProvider)obj);
        }

        private bool Equals(IMixedRealitySpatialObserverDataProvider other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation
    }
}