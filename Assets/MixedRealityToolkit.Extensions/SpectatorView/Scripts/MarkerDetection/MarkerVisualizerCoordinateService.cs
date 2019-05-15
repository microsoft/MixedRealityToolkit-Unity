// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// A variant of marker based <see cref="ISpatialCoordinateService"/> implementation. This one tracks coordinates displayed on the screen of current mobile device.
    /// The logic is that every time you start tracking a new coordinate is created and shown on the screen, after you stop tracking that coordinates location is no longer updated with the device.
    /// </summary>
    public class MarkerVisualizerCoordinateService : SpatialCoordinateServiceBase<int>
    {
        private class SpatialCoordinate : SpatialCoordinateBase<int>
        {
            private readonly IMarkerVisual markerVisual;

            private LocatedState locatedState = LocatedState.Resolved;
            private UnityEngine.Matrix4x4 worldToCoordinate;
            private UnityEngine.Quaternion worldToCoordinateRotation;
            private UnityEngine.Quaternion coordinateToWorldRotation;

            public UnityEngine.Matrix4x4 WorldToCoordinate
            {
                set
                {
                    worldToCoordinate = value;
                    worldToCoordinateRotation = UnityEngine.Quaternion.LookRotation(value.GetColumn(2), value.GetColumn(1));
                    coordinateToWorldRotation = UnityEngine.Quaternion.Inverse(worldToCoordinateRotation);
                }
            }

            /// <inheritdoc/>
            public override LocatedState State => locatedState;

            public SpatialCoordinate(int id, IMarkerVisual markerVisual)
                : base(id) { this.markerVisual = markerVisual; }

            public override Vector3 CoordinateToWorldSpace(Vector3 vector) => worldToCoordinate.inverse.MultiplyPoint(vector.AsUnityVector()).AsNumericsVector();

            public override Quaternion CoordinateToWorldSpace(Quaternion quaternion) => (coordinateToWorldRotation * quaternion.AsUnityQuaternion()).AsNumericsQuaternion();

            public override Vector3 WorldToCoordinateSpace(Vector3 vector) => worldToCoordinate.MultiplyPoint(vector.AsUnityVector()).AsNumericsVector();

            public override Quaternion WorldToCoordinateSpace(Quaternion quaternion) => (worldToCoordinateRotation * quaternion.AsUnityQuaternion()).AsNumericsQuaternion();

            public void ShowMarker()
            {
                markerVisual.ShowMarker(Id);
                locatedState = LocatedState.Tracking;
            }

            public void HideMarker()
            {
                locatedState = LocatedState.Resolved;
                markerVisual.HideMarker();
            }
        }

        private readonly IMarkerVisual markerVisual;
        private readonly UnityEngine.Transform markerInWorldSpace;
        private readonly Func<int> generateMarkerId;

        private SpatialCoordinate markerCoordinate;

        public MarkerVisualizerCoordinateService(IMarkerVisual markerVisual, UnityEngine.Transform markerInWorldSpace, Func<int> generateMarkerId)
        {
            this.markerVisual = markerVisual ?? throw new ArgumentNullException(nameof(generateMarkerId));
            this.markerInWorldSpace = markerInWorldSpace;
            this.generateMarkerId = generateMarkerId ?? throw new ArgumentNullException(nameof(generateMarkerId));
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            IsTracking = false;
        }

        protected override async Task OnDiscoverCoordinatesAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            markerCoordinate = new SpatialCoordinate(generateMarkerId(), markerVisual);
            OnNewCoordinate(markerCoordinate.Id, markerCoordinate);

            markerCoordinate.ShowMarker();

            // TODO we can probably get rid of UpdateTick and use a Unity synchronization context here to wait frames
            await Task.Delay(-1, cancellationToken).IgnoreCancellation();

            markerCoordinate.HideMarker();
            markerCoordinate = null;
        }

        /// <summary>
        /// Call this method each frame to process the marker position/rotation update.
        /// </summary>
        public void UpdateTick()
        {
            ThrowIfDisposed();

            if (markerCoordinate != null)
            {
                markerCoordinate.WorldToCoordinate = markerInWorldSpace.worldToLocalMatrix;
            }
        }
    }
}
