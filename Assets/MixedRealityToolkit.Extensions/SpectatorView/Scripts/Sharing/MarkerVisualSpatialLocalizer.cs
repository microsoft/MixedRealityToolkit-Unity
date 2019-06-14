// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
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
    internal class MarkerVisualLocalizationSettings : ISpatialLocalizationSettings
    {
        public const string DiscoveryHeader = "MVISUALDISC";
        public const string CoordinateAssignedHeader = "ASSIGNID";
        public const string CoordinateFoundHeader = "COORDFOUND";

        /// <inheritdoc />
        public MarkerVisualLocalizationSettings() { }

        /// <inheritdoc />
        public void Serialize(BinaryWriter writer) { }

    }

    /// <summary>
    /// SpatialLocalizer that shows a marker
    /// </summary>
    internal class MarkerVisualSpatialLocalizer : SpatialLocalizer<MarkerVisualLocalizationSettings>
    {
        [Tooltip("The reference to an IMarkerVisual GameObject.")]
        [SerializeField]
        private MonoBehaviour MarkerVisual = null;
        private IMarkerVisual markerVisual = null;

        [Tooltip("The reference to the camera transform.")]
        [SerializeField]
        private Transform cameraTransform;

        [Tooltip("Marker Visual poosition relative to the device camera.")]
        [SerializeField]
        private Vector3 markerVisualPosition = Vector3.zero;

        [Tooltip("Marker Visual Rotation relative to the device camera.")]
        [SerializeField]
        private Vector3 markerVisualRotation = new Vector3(0, 180, 0);

        public override Guid SpatialLocalizerId => Id;
        public static readonly Guid Id = new Guid("BA5C8EA7-439C-4E1A-9925-218A391EF309");

#if UNITY_EDITOR
        private void OnValidate()
        {
            FieldHelper.ValidateType<IMarkerVisual>(MarkerVisual);
        }
#endif

        /// <inheritdoc />
        public override ISpatialLocalizationSession CreateLocalizationSession(IPeerConnection peerConnection, MarkerVisualLocalizationSettings settings)
        {
            markerVisual = (markerVisual == null) ? MarkerVisual as IMarkerVisual : markerVisual;
            return new LocalizationSession(this, settings, peerConnection, debugLogging);
        }

        /// <inheritdoc />
        public override bool TryDeserializeSettings(BinaryReader reader, out MarkerVisualLocalizationSettings settings)
        {
            settings = new MarkerVisualLocalizationSettings();
            return true;
        }

        private class LocalizationSession : DisposableBase, ISpatialLocalizationSession
        {
            private readonly MarkerVisualSpatialLocalizer localizer;
            private readonly MarkerVisualLocalizationSettings settings;
            private readonly IPeerConnection peerConnection;
            private readonly ISpatialCoordinateService coordinateService;
            private readonly bool debugLogging = false;
            private readonly TaskCompletionSource<string> coordinateAssigned = null;
            private readonly TaskCompletionSource<string> coordinateFound = null;
            private readonly CancellationTokenSource discoveryCTS = null;

            private string coordinateId = string.Empty;

            public LocalizationSession(MarkerVisualSpatialLocalizer localizer, MarkerVisualLocalizationSettings settings, IPeerConnection peerConnection, bool debugLogging = false)
            {
                this.localizer = localizer;
                this.settings = settings;
                this.peerConnection = peerConnection;
                this.debugLogging = debugLogging;

                coordinateAssigned = new TaskCompletionSource<string>();
                coordinateFound = new TaskCompletionSource<string>();
                discoveryCTS = new CancellationTokenSource();

                var markerToCamera = Matrix4x4.TRS(this.localizer.markerVisualPosition, Quaternion.Euler(this.localizer.markerVisualRotation), Vector3.one);
                this.coordinateService = new MarkerVisualCoordinateService(this.localizer.markerVisual, markerToCamera, this.localizer.cameraTransform, this.localizer.debugLogging);
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
                DebugLog("Localizing");
                if (!TrySendMarkerVisualDiscoveryMessage())
                {
                    Debug.LogWarning("Failed to send marker visual discovery message, spatial localization failed.");
                    return null;
                }

                // Receive marker to show
                DebugLog("Waiting to have a coordinate id assigned");
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
                    if (await coordinateService.TryDiscoverCoordinatesAsync(discoveryCTS.Token, new string[] { coordinateId.ToString() }))
                    {
                        DebugLog($"Coordinate discovery completed: {coordinateId}");
                        if (!coordinateService.TryGetKnownCoordinate(coordinateId, out coordinate))
                        {
                            DebugLog("Failed to find spatial coordinate although discovery completed.");
                        }
                    }
                }

                DebugLog($"Waiting for the coordinate to be found: {coordinateId}");
                await Task.WhenAny(coordinateFound.Task, Task.Delay(-1, cancellationToken));

                return coordinate;
            }

            /// <inheritdoc />
            public void OnDataReceived(BinaryReader reader)
            {
                string command = reader.ReadString();
                DebugLog($"Received command: {command}");
                switch (command)
                {
                    case MarkerVisualLocalizationSettings.CoordinateAssignedHeader:
                        coordinateId = reader.ReadString();
                        DebugLog($"Assigned coordinate id: {coordinateId}");
                        coordinateAssigned?.SetResult(coordinateId);
                        break;
                    case MarkerVisualLocalizationSettings.CoordinateFoundHeader:
                        string detectedId = reader.ReadString();
                        if (coordinateId == detectedId)
                        {
                            DebugLog($"Ending discovery: {coordinateId}");
                            discoveryCTS?.Cancel();

                            DebugLog($"Coordinate was found: {coordinateId}");
                            coordinateFound?.SetResult(detectedId);
                        }
                        else
                        {
                            DebugLog($"Unexpected coordinate found, expected: {coordinateId}, detected: {detectedId}");
                        }
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
                    Debug.Log($"MarkerVisualSpatialLocalizer.LocalizationSession: {message}");
                }
            }

            private bool TrySendMarkerVisualDiscoveryMessage()
            {
                if (localizer.markerVisual.TryGetMaxSupportedMarkerId(out var maxId))
                {
                    DebugLog($"Sending maximum id for discovery: {maxId}");
                    peerConnection?.SendData(writer =>
                    {
                        writer.Write(MarkerVisualLocalizationSettings.DiscoveryHeader);
                        writer.Write(maxId);
                    });

                    return true;
                }

                DebugLog("Unable to obtain max id from marker visual");
                return false;
            }
        }
    }
}
