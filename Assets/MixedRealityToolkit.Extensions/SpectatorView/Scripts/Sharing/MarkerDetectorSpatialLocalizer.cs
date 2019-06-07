// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using System;
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

        private MarkerDetectorCoordinateService spatialCoordinateService;

        protected override ISpatialCoordinateService<int> SpatialCoordinateService => spatialCoordinateService;

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

            spatialCoordinateService = new MarkerDetectorCoordinateService(markerDetector, debugLogging);
        }

        protected override void Start()
        {
            base.Start();

            spatialCoordinateService.RestorePersistedCoordinates();
        }

        public override bool TryGetKnownCoordinate(MarkerDetectorLocalizationSettings settings, out ISpatialCoordinate coordinate)
        {
            return SpatialCoordinateService.TryGetKnownCoordinate(settings.MarkerID, out coordinate);
        }

        public override bool TryDeserializeSettings(BinaryReader reader, out MarkerDetectorLocalizationSettings settings)
        {
            return MarkerDetectorLocalizationSettings.TryDeserialize(reader, out settings);
        }

        public override ISpatialLocalizationSession CreateLocalizationSession(MarkerDetectorLocalizationSettings settings)
        {
            return new LocalizationSession(this, settings);
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

                if (localizer.TryGetKnownCoordinate(Settings, out ISpatialCoordinate coordinate))
                {
                    if (coordinate is IPersistableSpatialCoordinate persistableCoordinte)
                    {
                        persistableCoordinte.DepersistCoordinate();
                    }
                }

                using (CancellationTokenSource cts = new CancellationTokenSource())
                {
                    TaskCompletionSource<ISpatialCoordinate> coordinateTCS = new TaskCompletionSource<ISpatialCoordinate>();
                    void coordinateDiscovered(ISpatialCoordinate coord)
                    {
                        localizer.DebugLog("Coordinate found");

                        if (Settings.ShouldPersistCoordinate && coord is IPersistableSpatialCoordinate persistableCoordinate)
                        {
                            persistableCoordinate.PersistCoordinate();
                        }

                        coordinateTCS.SetResult(coord);
                        cts.Cancel();
                    }

                    localizer.SpatialCoordinateService.CoordinatedDiscovered += coordinateDiscovered;
                    try
                    {
                        localizer.DebugLog("Starting to look for coordinates");
                        await localizer.SpatialCoordinateService.TryDiscoverCoordinatesAsync(cts.Token, Settings.MarkerID);
                        localizer.DebugLog("Stopped looking for coordinates");


                        localizer.DebugLog("Awaiting found coordiante");
                        // Don't necessarily need to await here
                        return await coordinateTCS.Task;
                    }
                    finally
                    {
                        localizer.DebugLog("Unsubscribing from coordinate discovered");
                        localizer.SpatialCoordinateService.CoordinatedDiscovered -= coordinateDiscovered;
                    }
                }
            }
        }
    }
}
