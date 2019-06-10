// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// SpatialLocalizer that is based on a marker detector.
    /// </summary>
    internal class MarkerDetectorSpatialLocalizer : SpatialLocalizer<MarkerDetectorLocalizationSettings, int>
    {
        public static readonly Guid ID = new Guid("698D46CF-2099-4E06-9ADE-2FD0C18992F4");

        [Tooltip("The reference to an IMarkerDetector GameObject")]
        [SerializeField]
        private MonoBehaviour MarkerDetector = null;
        private IMarkerDetector markerDetector = null;

        public override Guid SpatialLocalizerID => ID;

#if UNITY_EDITOR
        private void OnValidate()
        {
            FieldHelper.ValidateType<IMarkerDetector>(MarkerDetector);
        }
#endif

        private void Awake()
        {
            DebugLog("Awake");
            markerDetector = MarkerDetector as IMarkerDetector;
            if (markerDetector == null)
            {
                Debug.LogWarning("Marker detector not appropriately set for MarkerDetectorSpatialLocalizer");
            }
        }

        public override bool TryDeserializeSettings(BinaryReader reader, out MarkerDetectorLocalizationSettings settings)
        {
            return MarkerDetectorLocalizationSettings.TryDeserialize(reader, out settings);
        }

        public override ISpatialLocalizationSession CreateLocalizationSession(MarkerDetectorLocalizationSettings settings)
        {
            return new LocalizationSession(this, settings);
        }

        private class SpatialCoordinate : TransformSpatialCoordinate<int>
        {
            private Marker marker;

            public Marker Marker
            {
                get => marker;
                set
                {
                    if (value != null && value.Id != Id)
                    {
                        throw new InvalidOperationException("Setting a marker with the wrong id.");
                    }

                    if (marker != value)
                    {
                        marker = value;
                        gameObject.transform.position = marker.Position;
                        gameObject.transform.rotation = marker.Rotation;
                    }
                }
            }

            internal GameObject GameObject => gameObject;

            public override LocatedState State => marker == null ? base.State : LocatedState.Tracking;

            public SpatialCoordinate(int id)
                : base(id)
            {
            }

            public SpatialCoordinate(Marker marker)
                : this(marker?.Id ?? throw new ArgumentNullException(nameof(marker)))
            {
                Marker = marker;
            }
        }

        private class LocalizationSession : SpatialLocalizationSession<MarkerDetectorLocalizationSettings>
        {
            private readonly MarkerDetectorSpatialLocalizer localizer;

            public LocalizationSession(MarkerDetectorSpatialLocalizer localizer, MarkerDetectorLocalizationSettings settings)
                : base(settings)
            {
                this.localizer = localizer;
            }

            public override async Task<ISpatialCoordinate> LocalizeAsync(CancellationToken cancellationToken)
            {
                localizer.DebugLog("Getting host coordinate");
                localizer.markerDetector.SetMarkerSize(Settings.MarkerSize);

                TaskCompletionSource<ISpatialCoordinate> coordinateTCS = new TaskCompletionSource<ISpatialCoordinate>();
                void markerDiscovered(Dictionary<int, Marker> markers)
                {
                    if (markers.TryGetValue(Settings.MarkerID, out Marker marker))
                    {
                        localizer.DebugLog("Coordinate found");
                        coordinateTCS.SetResult(new SpatialCoordinate(marker));
                    }
                }

                localizer.markerDetector.MarkersUpdated += markerDiscovered;

                try
                {
                    localizer.DebugLog("Starting to look for coordinates");
                    localizer.markerDetector.StartDetecting();

                    localizer.DebugLog("Awaiting found coordiante");
                    // Don't necessarily need to await here
                    return await coordinateTCS.Task;
                }
                finally
                {
                    localizer.DebugLog("Stopped looking for coordinates");
                    localizer.markerDetector.StopDetecting();

                    localizer.DebugLog("Unsubscribing from coordinate discovered");
                    localizer.markerDetector.MarkersUpdated -= markerDiscovered;
                }
            }
        }
    }
}
