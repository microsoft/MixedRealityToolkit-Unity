// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// SpatialLocalizer that is based on a marker detector.
    /// </summary>
    internal class MarkerDetectorSpatialLocalizer : SpatialLocalizer<MarkerDetectorLocalizationSettings>
    {
        public static readonly Guid Id = new Guid("698D46CF-2099-4E06-9ADE-2FD0C18992F4");

        [Tooltip("The reference to an IMarkerDetector GameObject")]
        [SerializeField]
        private MonoBehaviour MarkerDetector = null;
        private IMarkerDetector markerDetector = null;

        public override Guid SpatialLocalizerId => Id;

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

        public override bool TryCreateLocalizationSession(IPeerConnection peerConnection, MarkerDetectorLocalizationSettings settings, out ISpatialLocalizationSession session)
        {
            session = new LocalizationSession(this, settings);
            return true;
        }

        private class LocalizationSession : DisposableBase, ISpatialLocalizationSession
        {
            private readonly MarkerDetectorSpatialLocalizer localizer;
            private readonly MarkerDetectorLocalizationSettings settings;
            private readonly MarkerDetectorCoordinateService coordinateService;

            public LocalizationSession(MarkerDetectorSpatialLocalizer localizer, MarkerDetectorLocalizationSettings settings)
            {
                this.localizer = localizer;
                this.settings = settings;

                this.localizer.markerDetector.SetMarkerSize(settings.MarkerSize);
                this.coordinateService = new MarkerDetectorCoordinateService(this.localizer.markerDetector, this.localizer.debugLogging);
            }

            public async Task<ISpatialCoordinate> LocalizeAsync(CancellationToken cancellationToken)
            {
                localizer.DebugLog("Getting host coordinate");

                await coordinateService.TryDiscoverCoordinatesAsync(cancellationToken, new int[] { settings.MarkerID });

                if (!coordinateService.TryGetKnownCoordinate(settings.MarkerID, out ISpatialCoordinate spatialCoordinate))
                {
                    Debug.LogError("Unexpected failure to discover a marker coordinate");
                }

                return spatialCoordinate;
            }

            public void OnDataReceived(BinaryReader reader)
            {
            }

            protected override void OnManagedDispose()
            {
                this.coordinateService.Dispose();
            }
        }
    }
}
