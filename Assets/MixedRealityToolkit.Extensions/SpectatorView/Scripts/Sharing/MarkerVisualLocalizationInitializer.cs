// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class MarkerVisualLocalizationInitializer : MonoBehaviour
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
            DebugLog("Registering ParticipantConnected event.");
            SpatialCoordinateSystemManager.Instance.ParticipantConnected -= ParticipantConnected;
        }

        private void ParticipantConnected(SpatialCoordinateSystemParticipant participant)
        {
            DebugLog($"Participant connected: {participant?.SocketEndpoint?.Address ?? "IPAddress unknown"}");
            SpatialCoordinateSystemManager.Instance.LocalizeAsync(participant.SocketEndpoint, MarkerVisualSpatialLocalizer.Id, new MarkerVisualLocalizationSettings());
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                Debug.Log($"MarkerVisualLocalizationInitializer: {message}");
            }
        }
    }
}
