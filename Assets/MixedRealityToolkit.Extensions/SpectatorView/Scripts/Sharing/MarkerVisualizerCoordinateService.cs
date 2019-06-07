//// Copyright (c) Microsoft Corporation. All rights reserved.
//// Licensed under the MIT License. See LICENSE in the project root for license information.

//using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
//using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
//using System;
//using System.Numerics;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
//{
//    /// <summary>
//    /// A variant of marker based <see cref="ISpatialCoordinateService"/> implementation. This one tracks coordinates displayed on the screen of current mobile device.
//    /// The logic is that every time you start tracking a new coordinate is created and shown on the screen, after you stop tracking that coordinates location is no longer updated with the device.
//    /// </summary>
//    public class MarkerVisualizerCoordinateService : SpatialCoordinateServiceBase<int>
//    {
//        private class SpatialCoordinate : TransformSpatialCoordinate<int>
//        {
//            private readonly IMarkerVisual markerVisual;

//            private LocatedState locatedState = LocatedState.Resolved;
//            private UnityEngine.Matrix4x4 worldToCoordinate;
//            private UnityEngine.Quaternion worldToCoordinateRotation;
//            private UnityEngine.Quaternion coordinateToWorldRotation;

//            public UnityEngine.Matrix4x4 WorldToCoordinate
//            {
//                set
//                {
//                    worldToCoordinate = value;
//                    worldToCoordinateRotation = UnityEngine.Quaternion.LookRotation(value.GetColumn(2), value.GetColumn(1));
//                    coordinateToWorldRotation = UnityEngine.Quaternion.Inverse(worldToCoordinateRotation);
//                }
//            }

//            /// <inheritdoc/>
//            public override LocatedState State => locatedState;

//            public SpatialCoordinate(int id, IMarkerVisual markerVisual)
//                : base(id) { this.markerVisual = markerVisual; }

//            public void ShowMarker()
//            {
//                markerVisual.ShowMarker(Id);
//                locatedState = LocatedState.Tracking;
//            }

//            public void HideMarker()
//            {
//                locatedState = LocatedState.Resolved;
//                markerVisual.HideMarker();
//            }
//        }

//        private readonly IMarkerVisual markerVisual;
//        private readonly UnityEngine.Transform markerInWorldSpace;

//        private SpatialCoordinate markerCoordinate;

//        protected override bool SupportsDiscovery => false;

//        public MarkerVisualizerCoordinateService(IMarkerVisual markerVisual, UnityEngine.Transform markerInWorldSpace, Func<int> generateMarkerId)
//        {
//            this.markerVisual = markerVisual ?? throw new ArgumentNullException(nameof(generateMarkerId));
//            this.markerInWorldSpace = markerInWorldSpace;
//        }

//        protected override void OnManagedDispose()
//        {
//            base.OnManagedDispose();
//        }

//        protected override bool TryParse(string id, out int result) => int.TryParse(id, out result);

//        protected override async Task OnDiscoverCoordinatesAsync(CancellationToken cancellationToken, int[] idsToLocate)
//        {
//            if (idsToLocate == null || idsToLocate.Length < 1)
//            {
//                throw new ArgumentNullException($"{nameof(MarkerVisualizerCoordinateService)} depends on ids so that it could visualize them, at least one should be provided.");
//            }

//            markerCoordinate = new SpatialCoordinate(idsToLocate[0], markerVisual);
//            OnNewCoordinate(markerCoordinate.Id, markerCoordinate);

//            markerCoordinate.ShowMarker();

//            while (cancellationToken.IsCancellationRequested)
//            {
//                markerCoordinate.WorldToCoordinate = markerInWorldSpace.worldToLocalMatrix;
//                await Task.Delay(1, cancellationToken).IgnoreCancellation(); // Wait a frame, this is how Unity synchronization context will let you wait for next frame
//            }

//            markerCoordinate.HideMarker();
//            markerCoordinate = null;
//        }
//    }
//}
