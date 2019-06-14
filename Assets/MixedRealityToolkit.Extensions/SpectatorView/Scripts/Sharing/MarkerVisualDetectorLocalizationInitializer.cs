// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class MarkerVisualDetectorLocalizationInitializer : MonoBehaviour
    {
        [SerializeField]
        private bool debugLogging = false;

        [SerializeField]
        private MarkerVisualDetectorSpatialLocalizer spatialLocalizer;

        private void Start()
        {
            SpatialCoordinateSystemManager.Instance.ParticipantConnected += ParticipantConnected;
        }

        private void OnDestroy()
        {
            SpatialCoordinateSystemManager.Instance.ParticipantConnected -= ParticipantConnected;
        }

        private async void ParticipantConnected(SpatialCoordinateSystemParticipant participant)
        {
            DebugLog($"Participant connected: {participant.SocketEndpoint.Address}");
            await SpatialCoordinateSystemManager.Instance.LocalizeAsync(participant.SocketEndpoint, spatialLocalizer, new MarkerVisualDetectorLocalizationSettings());
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                DebugLog($"MarkerVisualDetectorLocalizationInitializer: {message}");
            }
        }
    }
}
