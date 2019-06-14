// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
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
    public class MarkerVisualCoordinateService : SpatialCoordinateServiceBase<int>
    {
        private class SpatialCoordinate : SpatialCoordinateUnityBase<int>
        {
            private readonly IMarkerVisual markerVisual;

            private LocatedState locatedState = LocatedState.Resolved;

            public UnityEngine.Matrix4x4 WorldToCoordinate
            {
                get
                {
                    return worldMatrix;
                }

                set
                {
                    worldMatrix = value;
                }
            }

            /// <inheritdoc/>
            public override LocatedState State => locatedState;

            public SpatialCoordinate(int id, IMarkerVisual markerVisual)
                : base(id) { this.markerVisual = markerVisual; }

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
        private readonly UnityEngine.Matrix4x4 markerToCamera;
        private readonly UnityEngine.Transform cameraTransform;

        protected override bool SupportsDiscovery => false;
        private bool debugLogging = false;

        public MarkerVisualCoordinateService(IMarkerVisual markerVisual, UnityEngine.Matrix4x4 markerToCamera, UnityEngine.Transform cameraTransform, bool debugLogging = false)
        {
            this.markerVisual = markerVisual ?? throw new ArgumentNullException("MarkerVisual was null.");
            this.markerToCamera = markerToCamera;
            this.cameraTransform = cameraTransform;
            this.debugLogging = debugLogging;
            DebugLog("Service Created");
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();
            DebugLog("Service Disposed");
        }

        protected override bool TryParse(string id, out int result) => int.TryParse(id, out result);

        protected override async Task OnDiscoverCoordinatesAsync(CancellationToken cancellationToken, int[] idsToLocate)
        {
            if (idsToLocate == null || idsToLocate.Length < 1)
            {
                throw new ArgumentNullException($"{nameof(MarkerVisualCoordinateService)} depends on ids so that it could visualize them, at least one should be provided.");
            }

            DebugLog($"Creating spatial coordinate {idsToLocate[0]}");
            SpatialCoordinate markerCoordinate = new SpatialCoordinate(idsToLocate[0], markerVisual);
            OnNewCoordinate(markerCoordinate.Id, markerCoordinate);

            DebugLog($"Showing marker");
            markerCoordinate.ShowMarker();

            DebugLog($"Waiting for cancellation token");
            while (cancellationToken.IsCancellationRequested)
            {
                // Continually cache the current marker visual location in the world.
                markerCoordinate.WorldToCoordinate = markerToCamera * cameraTransform.localToWorldMatrix;
                await Task.Delay(1, cancellationToken).IgnoreCancellation(); // Wait a frame, this is how Unity synchronization context will let you wait for next frame
            }

            DebugLog($"Hiding marker");
            markerCoordinate.HideMarker();
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                UnityEngine.Debug.Log($"MarkerVisualCoordinateService: {message}");
            }
        }
    }
}
