// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class MarkerVisualDetectorLocalizationInitializer : MonoBehaviour
    {
        [SerializeField]
        private bool debugLogging = false;

        private void Awake()
        {
            DebugLog("Registering ParticipantConnected event.");
            SpatialCoordinateSystemManager.Instance.ParticipantConnected += ParticipantConnected;
        }

        private void OnDestroy()
        {
            DebugLog("Unregistering ParticipantConnected event.");
            SpatialCoordinateSystemManager.Instance.ParticipantConnected -= ParticipantConnected;
        }

        private void ParticipantConnected(SpatialCoordinateSystemParticipant participant)
        {
            DebugLog($"Participant connected: {participant.SocketEndpoint.Address}");
            SpatialCoordinateSystemManager.Instance.LocalizeAsync(participant.SocketEndpoint, MarkerVisualDetectorSpatialLocalizer.Id, new MarkerVisualDetectorLocalizationSettings());
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                Debug.Log($"MarkerVisualDetectorLocalizationInitializer: {message}");
            }
        }
    }
}
