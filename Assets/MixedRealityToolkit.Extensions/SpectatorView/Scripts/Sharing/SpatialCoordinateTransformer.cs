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
        [Tooltip("The transform that should be translated to the position of the world origin of the peer device")]
        [SerializeField]
        private Transform sharedCoordinateOrigin = null;

        public Transform SharedCoordinateOrigin => sharedCoordinateOrigin;

        private SpatialCoordinateSystemParticipant currentParticipant;

        private void Start()
        {
            SpatialCoordinateSystemManager.Instance.ParticipantConnected += OnParticipantConnected;
            SpatialCoordinateSystemManager.Instance.ParticipantDisconnected += OnParticipantDisconnected;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SpatialCoordinateSystemManager.Instance.ParticipantConnected -= OnParticipantConnected;
            SpatialCoordinateSystemManager.Instance.ParticipantDisconnected -= OnParticipantDisconnected;
        }

        private void Update()
        {
            if (currentParticipant != null && sharedCoordinateOrigin != null && currentParticipant.Coordinate != null && currentParticipant.PeerSpatialCoordinateIsLocated)
            {
                // We need to transform the remote peer's WorldOrigin into a location in this system's world coordinate system.
                Vector3 positionOfPeerWorldOriginInCoordinateSpace = -currentParticipant.PeerSpatialCoordinateWorldPosition;
                Quaternion rotationOfPeerWorldOriginInCoordinateSpace = Quaternion.Inverse(currentParticipant.PeerSpatialCoordinateWorldRotation);

                Vector3 positionOfPeerWorldOriginInLocalWorldSpace = currentParticipant.Coordinate.CoordinateToWorldSpace(positionOfPeerWorldOriginInCoordinateSpace);
                Quaternion rotationOfPeerWorldOriginInLocalWorldSpace = currentParticipant.Coordinate.CoordinateToWorldSpace(rotationOfPeerWorldOriginInCoordinateSpace);

                sharedCoordinateOrigin.position = positionOfPeerWorldOriginInLocalWorldSpace;
                sharedCoordinateOrigin.rotation = rotationOfPeerWorldOriginInLocalWorldSpace;
            }
        }

        private void OnParticipantDisconnected(SpatialCoordinateSystemParticipant participant)
        {
            if (currentParticipant == null)
            {
                Debug.LogError("Unexpected that no participant was registered when a participant disconnected");
            }
            currentParticipant = null;
        }

        private void OnParticipantConnected(SpatialCoordinateSystemParticipant participant)
        {
            if (currentParticipant != null)
            {
                Debug.LogError("Unexpected existing participant when another participant connected");
            }
            currentParticipant = participant;
        }
    }
}