using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Positions a transform representing a world origin such that a connected peer device's world origin (relative to a
    /// shared spatial coordinate) is used as the effective local world origin (as determined
    /// by the shared spatial coordinate).
    /// </summary>
    public class SpatialCoordinateTransformer : Singleton<SpatialCoordinateTransformer>
    {
        [SerializeField]
        private bool debugLogging = false;

        [Tooltip("The transform that should be translated to the position of the world origin of the peer device")]
        [SerializeField]
        private Transform sharedCoordinateOrigin = null;

        public Transform SharedCoordinateOrigin => sharedCoordinateOrigin;

        private SpatialCoordinateSystemParticipant currentParticipant;

        private void Start()
        {
            DebugLog("Registering ParticipantConnected and ParticipantDisconnected events.");
            SpatialCoordinateSystemManager.Instance.ParticipantConnected += OnParticipantConnected;
            SpatialCoordinateSystemManager.Instance.ParticipantDisconnected += OnParticipantDisconnected;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            DebugLog("Unregistering ParticipantConnected and ParticipantDisconnected events.");
            SpatialCoordinateSystemManager.Instance.ParticipantConnected -= OnParticipantConnected;
            SpatialCoordinateSystemManager.Instance.ParticipantDisconnected -= OnParticipantDisconnected;
        }

        private void Update()
        {
            if (currentParticipant != null && sharedCoordinateOrigin != null && currentParticipant.Coordinate != null && currentParticipant.PeerSpatialCoordinateIsLocated)
            {
                // Obtain a position and rotation that transforms this application's local world origin to the shared spatial coordinate space.
                var localWorldToCoordinatePosition = currentParticipant.Coordinate.WorldToCoordinateSpace(Vector3.zero);
                var localWorldToCoordinateRotation = currentParticipant.Coordinate.WorldToCoordinateSpace(Quaternion.identity);

                // Obtain a position and rotation that transforms the peer's shared spatial coordinate to its local world space.
                var peerCoordinateToWorldPosition = currentParticipant.PeerSpatialCoordinateWorldPosition;
                var peerCoordinateToWorldRotation = currentParticipant.PeerSpatialCoordinateWorldRotation;

                // Create a transform that converts the local world space to the peer world space (peer coordinate to peer world * local world to local shared coordinate).
                sharedCoordinateOrigin.position = peerCoordinateToWorldPosition + localWorldToCoordinatePosition;
                sharedCoordinateOrigin.rotation = peerCoordinateToWorldRotation * localWorldToCoordinateRotation;
                DebugLog($"Updated transform, Position: {sharedCoordinateOrigin.position.ToString("G4")}, Rotation: {sharedCoordinateOrigin.rotation.ToString("G4")}");
            }
        }

        private void OnParticipantDisconnected(SpatialCoordinateSystemParticipant participant)
        {
            DebugLog($"Participant disconnected: {participant?.SocketEndpoint?.Address ?? "IPAddress unknown"}");
            if (currentParticipant == null)
            {
                Debug.LogError("Unexpected that no participant was registered when a participant disconnected");
            }
            currentParticipant = null;
        }

        private void OnParticipantConnected(SpatialCoordinateSystemParticipant participant)
        {
            DebugLog($"Participant connected: {participant?.SocketEndpoint?.Address ?? "IPAddress unknown"}");
            if (currentParticipant != null)
            {
                Debug.LogError("Unexpected existing participant when another participant connected");
            }
            currentParticipant = participant;
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                Debug.Log($"SpatialCoordinateWorldOriginTransformer: {message}");
            }
        }
    }
}