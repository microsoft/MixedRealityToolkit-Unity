// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing;
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
    public class MarkerVisualizeCoordinateService : SpatialCoordinateServiceBase<int>
    {
        private class SpatialCoordinate : SpatialCoordinateBase<int>
        {
            private UnityEngine.Matrix4x4 worldToCoordinate;
            private UnityEngine.Quaternion worldToCoordinateRotation;
            private UnityEngine.Quaternion coordinateToWorldRotation;

            public new bool IsLocated
            {
                get => base.IsLocated;
                set => base.IsLocated = value;
            }

            public UnityEngine.Matrix4x4 WorldToCoordinate
            {
                set
                {
                    worldToCoordinate = value;
                    worldToCoordinateRotation = UnityEngine.Quaternion.LookRotation(value.GetColumn(2), value.GetColumn(1));
                    coordinateToWorldRotation = UnityEngine.Quaternion.Inverse(worldToCoordinateRotation);
                }
            }

            public SpatialCoordinate(int id)
                : base(id) { }

            public override Vector3 CoordinateToWorldSpace(Vector3 vector) => worldToCoordinate.inverse.MultiplyPoint(vector.AsUnityVector()).AsNumericsVector();

            public override Quaternion CoordinateToWorldSpace(Quaternion quaternion) => (coordinateToWorldRotation * quaternion.AsUnityQuaternion()).AsNumericsQuaternion();

            public override Vector3 WorldToCoordinateSpace(Vector3 vector) => worldToCoordinate.MultiplyPoint(vector.AsUnityVector()).AsNumericsVector();

            public override Quaternion WorldToCoordinateSpace(Quaternion quaternion) => (worldToCoordinateRotation * quaternion.AsUnityQuaternion()).AsNumericsQuaternion();
        }

        private readonly IMarkerVisual markerVisual;
        private readonly UnityEngine.Transform markerInWorldSpace;
        private readonly Func<int> generateMarkerId;

        private SpatialCoordinate markerBeingVisualized;

        public MarkerVisualizeCoordinateService(IMarkerVisual markerVisual, UnityEngine.Transform markerInWorldSpace, Func<int> generateMarkerId)
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

        protected override async Task RunTrackingAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            markerBeingVisualized = new SpatialCoordinate(generateMarkerId()) { IsLocated = true };
            OnNewCoordinate(markerBeingVisualized.Id, markerBeingVisualized);

            markerVisual.ShowMarker(markerBeingVisualized.Id);

            // TODO we can probably get rid of UpdateTick and use a Unity synchronization context here to wait frames
            await Task.Delay(-1, cancellationToken).IgnoreCancellation();

            markerVisual.HideMarker();
            markerBeingVisualized.IsLocated = false;
            markerBeingVisualized = null;
        }

        public void UpdateTick()
        {
            ThrowIfDisposed();

            if (markerBeingVisualized != null)
            {
                markerBeingVisualized.WorldToCoordinate = markerInWorldSpace.worldToLocalMatrix;
            }
        }
    }
}
