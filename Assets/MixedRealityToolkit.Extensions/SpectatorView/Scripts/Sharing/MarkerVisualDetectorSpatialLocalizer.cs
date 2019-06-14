// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class MarkerVisualDetectorLocalizationSettings : ISpatialLocalizationSettings
    {
        public MarkerVisualDetectorLocalizationSettings() { }

        public void Serialize(BinaryWriter writer) { }
    }

    internal class MarkerVisualDetectorSpatialLocalizer : SpatialLocalizer<MarkerVisualDetectorLocalizationSettings>
    {
        [Tooltip("The reference to an IMarkerDetector GameObject")]
        [SerializeField]
        private MonoBehaviour MarkerDetector = null;
        private IMarkerDetector markerDetector = null;

        private MarkerDetectorCoordinateService markerDetectorCoordinateService = null;
        private TaskCompletionSource<int> markerAssigned = null;
        private CancellationTokenSource discoveryCTS = null;

        private Dictionary<Guid, Action<Action<BinaryWriter>>> sessionWriteAndSendDictionary = new Dictionary<Guid, Action<Action<BinaryWriter>>>();
        private Dictionary<Guid, string> sessionCoordinateIdDictionary = new Dictionary<Guid, string>();
        private Dictionary<string, ISpatialCoordinate> coordinateDictionary = new Dictionary<string, ISpatialCoordinate>();
        private HashSet<string> neededCoordinates = new HashSet<string>();
        private bool localizing = false;

        public override Guid SpatialLocalizerId => Id;
        public readonly Guid Id = new Guid("2DA7D277-323F-4A0D-B3BB-B2BA6D3EF70E");

#if UNITY_EDITOR
        private void OnValidate()
        {
            FieldHelper.ValidateType<IMarkerDetector>(MarkerDetector);
        }
#endif

        private void Awake()
        {
            if (SpatialCoordinateSystemManager.Instance != null)
            {
                SpatialCoordinateSystemManager.Instance.RegisterSpatialLocalizer(this);
            }
            else
            {
                DebugLog("SpatialCoordinateSystemManager was not found in scene");
            }
        }

        /// <inheritdoc />
        public override ISpatialLocalizationSession CreateLocalizationSession(IPeerConnection peerConnection, MarkerVisualDetectorLocalizationSettings settings)
        {
            markerDetector = (markerDetector == null) ? MarkerDetector as IMarkerDetector : markerDetector;
            return new LocalizationSession(this, settings, peerConnection, debugLogging);
        }

        /// <inheritdoc />
        public override bool TryDeserializeSettings(BinaryReader reader, out MarkerVisualDetectorLocalizationSettings settings)
        {
            settings = new MarkerVisualDetectorLocalizationSettings();
            return true;
        }

        private class LocalizationSession : DisposableBase, ISpatialLocalizationSession
        {
            private readonly MarkerVisualDetectorSpatialLocalizer localizer;
            private readonly MarkerVisualDetectorLocalizationSettings settings;
            private readonly IPeerConnection peerConnection;
            private readonly ISpatialCoordinateService coordinateService;
            private readonly bool debugLogging = false;
            private readonly TaskCompletionSource<string> coordinateAssigned = null;
            private readonly CancellationTokenSource discoveryCTS = null;

            private string coordinateId = string.Empty;

            public LocalizationSession(MarkerVisualDetectorSpatialLocalizer localizer, MarkerVisualDetectorLocalizationSettings settings, IPeerConnection peerConnection, bool debugLogging = false)
            {
                this.localizer = localizer;
                this.settings = settings;
                this.peerConnection = peerConnection;
                this.debugLogging = debugLogging;

                this.coordinateAssigned = new TaskCompletionSource<string>();
                this.coordinateService = new MarkerDetectorCoordinateService(this.localizer.markerDetector, debugLogging);
                this.discoveryCTS = new CancellationTokenSource();
            }

            /// <inheritdoc />
            protected override void OnManagedDispose()
            {
                coordinateService.Dispose();
                discoveryCTS.Dispose();
            }

            /// <inheritdoc />
            public async Task<ISpatialCoordinate> LocalizeAsync(CancellationToken cancellationToken)
            {
                DebugLog("Waiting for marker visual");
                await Task.WhenAny(coordinateAssigned.Task, Task.Delay(-1, cancellationToken));
                if (string.IsNullOrEmpty(coordinateId))
                {
                    DebugLog("Failed to assign coordinate id");
                    return null;
                }

                DebugLog($"Attempting to discover coordinate: {coordinateId}");
                ISpatialCoordinate coordinate = null;
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(discoveryCTS.Token, cancellationToken))
                {
                    if (await coordinateService.TryDiscoverCoordinatesAsync(cts.Token, new string[] { coordinateId.ToString() }))
                    {
                        DebugLog($"Coordinate discovery completed: {coordinateId}");
                        if (!coordinateService.TryGetKnownCoordinate(coordinateId, out coordinate))
                        {
                            DebugLog("Failed to find spatial coordinate although discovery completed.");
                        }
                        else
                        {
                            SendCoordinateFound(coordinate.Id);
                            return coordinate;
                        }
                    }
                }

                return null;
            }

            /// <inheritdoc />
            public void OnDataReceived(BinaryReader reader)
            {
                string command = reader.ReadString();
                switch (command)
                {
                    case MarkerVisualLocalizationSettings.DiscoveryHeader:
                        int maxSupportedMarkerId = reader.Read();
                        string coordinateId = DetermineCoordinateId(maxSupportedMarkerId);
                        SendCoordinateAssigned(coordinateId);
                        break;
                    default:
                        DebugLog($"Sent unknown command: {command}");
                        break;
                }
            }

            private void DebugLog(string message)
            {
                if (debugLogging)
                {
                    Debug.Log($"MarkerVisualDetectorSpatialLocalizer.LocalizationSession: {message}");
                }
            }

            private void SendCoordinateAssigned(string coordinateId)
            {
                DebugLog($"Sending coordinate assignment: {coordinateId}");
                peerConnection.SendData(writer =>
                {
                    writer.Write(MarkerVisualLocalizationSettings.CoordinateAssignedHeader);
                    writer.Write(coordinateId);
                });
            }

            private void SendCoordinateFound(string coordinateId)
            {
                DebugLog($"Sending coordinate found: {coordinateId}");
                peerConnection.SendData(writer =>
                {
                    writer.Write(MarkerVisualLocalizationSettings.CoordinateFoundHeader);
                    writer.Write(coordinateId);
                });
            }

            private string DetermineCoordinateId(int maxSupportedMarkerId)
            {
                DebugLog("GetMarkerId currently only supports marker id: 0, changes are needed to support multiple sessions in parallel.");
                return 0.ToString();
            }
        }
    }
}
